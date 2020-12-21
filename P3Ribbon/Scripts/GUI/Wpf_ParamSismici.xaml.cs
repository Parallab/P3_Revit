using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P3Ribbon.Scripts.GUI
{
    /// <summary>
    /// Logica di interazione per Wpf_ParamSismici.xaml
    /// </summary>
    public partial class Wpf_ParamSismici : Window, IDisposable
    {
        //assegno valore temporaneo alla classe uso e zona sismica
        public static int zona_wpf = 666;
        public static int classe_wpf = 666;
        public static bool ok_premuto = false;

        public Wpf_ParamSismici()
        {
            InitializeComponent();
            DaPojInfoaWpf();
            ColoraBottoniSeParametriCompilati();
        }


        private Button TrovaBottoneClasseUso(int c)
        {
            System.Windows.Controls.Button rtn = new Button();
            if (c == 1) rtn = wpfBottCUsoI;
            else if (c == 2) rtn = wpfBottCUsoII;
            else if (c == 3) rtn = wpfBottCUsoIII;
            else if (c == 4) rtn = wpfBottCUsoIV;

            return rtn;
        }

        private Button TrovaBottoneZona(int z)
        {
            System.Windows.Controls.Button rtn = new Button();
            if (z == 1) rtn = wpfBottZona1;
            else if (z == 2) rtn = wpfBottZona2;
            else if (z == 3) rtn = wpfBottZona3;
            else if (z == 4) rtn = wpfBottZona4;

            return rtn;
        }

        private void DecimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            bool approvedDecimalPoint = false;

            if (e.Text == ".")
            {
                if (!((TextBox)sender).Text.Contains("."))
                    approvedDecimalPoint = true;
            }

            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
                e.Handled = true;
        }

        public void AccToClasseUso()
        {
        
            try
            {
                //double acc;
                //var texbox = double.TryParse(wpfTexBoxAccell.Text, out acc);
                double acc;
                bool b = double.TryParse(wpfTexBoxAccell.Text, NumberStyles.Any, new CultureInfo("en-US"), out acc);

                if (acc > 0.35)
                {
                    ColoraBottoneZona(wpfBottZona1, 1);
                    wpfTexBoxAccell.Background = Brushes.IndianRed;
                }
                else if (acc > 0.25 && acc <= 0.35)
                {
                    ColoraBottoneZona(wpfBottZona1, 1);
                    wpfTexBoxAccell.Background = Brushes.White;
                }
                else if ((acc > 0.15 && acc <= 0.25))
                {
                    ColoraBottoneZona(wpfBottZona2, 2);
                    wpfTexBoxAccell.Background = Brushes.White;
                }
                else if ((acc > 0.05 && acc <= 0.15))
                {
                    ColoraBottoneZona(wpfBottZona3, 3);
                    wpfTexBoxAccell.Background = Brushes.White;
                }
                else if ((acc < 0.05))
                {
                    ColoraBottoneZona(wpfBottZona4, 4);
                    wpfTexBoxAccell.Background = Brushes.White;
                }
            }
            catch
            {

            }
            
         
        }

        private void ColoraBottoniSeParametriCompilati()
        {
            if (classe_wpf != 666)
            {
                Button bc = TrovaBottoneClasseUso(classe_wpf);
                ColoraBottoniClasse(bc, classe_wpf);
            }
            if (zona_wpf != 666)
            {
                Button bz = TrovaBottoneZona(zona_wpf);
                ColoraBottoneZona(bz, zona_wpf);
            }
        }

        private void ColoraBottoneZona(System.Windows.Controls.Button bottone, int z)
        {
            //colora i bottoni non selezionati del loro colore originale
            BrushConverter bc = new BrushConverter();
            wpfBottZona1.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            wpfBottZona2.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            wpfBottZona3.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            wpfBottZona4.Background = (Brush)bc.ConvertFrom("#FF81B2BF");

            //colora solo il bottone selezionato
            bottone.Background = (Brush)bc.ConvertFrom("#FF31788B");
            zona_wpf = z;
        }
        private void ColoraBottoniClasse(System.Windows.Controls.Button bottone, int c)
        {  
            //colora i bottoni non selezionati del loro colore originale
            BrushConverter bc = new BrushConverter();
            wpfBottCUsoI.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            wpfBottCUsoII.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            wpfBottCUsoIII .Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            wpfBottCUsoIV.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            
            //colora solo il bottone selezionato
            bottone.Background = (Brush)bc.ConvertFrom("#FF31788B");
            classe_wpf = c;
        }


        #region Eventi al click dei bottoni ClasseUso e Zona
        private void WpfbottZona1_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona1, 1);}
        private void WpfbottZona2_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona2, 2);}
        private void WpfbottZona3_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona3, 3);}
        private void WpfbottZona4_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona4, 4);}
        private void WpfbottCUsoI_Click(object sender, RoutedEventArgs e){ColoraBottoniClasse(wpfBottCUsoI, 1);}
        private void wpfBottCUsoII_Click(object sender, RoutedEventArgs e){ ColoraBottoniClasse(wpfBottCUsoII, 2);}
        private void WpfbottCUsoIII_Click(object sender, RoutedEventArgs e){ ColoraBottoniClasse(wpfBottCUsoIII, 3);}
        private void WpfbottCUsoIV_Click(object sender, RoutedEventArgs e) { ColoraBottoniClasse(wpfBottCUsoIV, 4); }
        #endregion

        private void wpfTextBoxAccell_TextChanged(object sender, TextChangedEventArgs e)
        {
          AccToClasseUso();
        }
        private void wpfBottOk_Click(object sender, RoutedEventArgs e)
        {
            ok_premuto = true;
            ParSismici.classe = classe_wpf;
            ParSismici.zona = zona_wpf;
            this.Close();
            //ParSismici.eng = eng_form;
            //ParSismici.vita = vita_form;
        }

        private void wpfBottAnnulla_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
        public void Dispose()
        {
            
        }
        public void DaPojInfoaWpf()
        {
            Element proj_info = new FilteredElementCollector(Supporto.doc).OfClass(typeof(ProjectInfo)).FirstElement();

           classe_wpf = proj_info.LookupParameter("P3_InfoProg_ClasseUso").AsInteger();
            //proj_info.LookupParameter("P3_InfoProg_Eng").Set(_eng);
            //proj_info.LookupParameter("P3_InfoProg_VitaNominale").Set(_vita);
            zona_wpf = proj_info.LookupParameter("P3_InfoProg_ZonaSismica").AsInteger();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
