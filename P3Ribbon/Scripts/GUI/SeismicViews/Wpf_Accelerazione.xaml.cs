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

namespace P3Ribbon.Scripts.GUI.SeismicViews
{
    /// <summary>
    /// Logica di interazione per Wpf_Accelerazione.xaml
    /// </summary>
    public partial class Wpf_Accelerazione : UserControl
    {
        public static TextBox wpftbAcc = new TextBox();

        public Wpf_Accelerazione()
        {
            InitializeComponent();
            wpftbAcc = this.wpfTexBoxAccell;
            wpfTexBoxAccell.Text = Wpf_ParamSismici.accValue.ToString();
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

        public void AccToZonaSism()
        {

            try
            {
                //double acc;
                //var texbox = double.TryParse(wpfTexBoxAccell.Text, out acc);
                double acc;
                bool b = double.TryParse(wpfTexBoxAccell.Text, NumberStyles.Any, new CultureInfo("en-US"), out acc);
                if (acc > 0.35)
                {
                    ColoraBottoneZona(Wpf_ZonaSismica.z1, 1);
                }
                else if (acc > 0.25 && acc <= 0.35)
                {
                    ColoraBottoneZona(Wpf_ZonaSismica.z1, 1);
                }
                else if ((acc > 0.15 && acc <= 0.25))
                {
                    ColoraBottoneZona(Wpf_ZonaSismica.z2, 2);
                }
                else if ((acc > 0.05 && acc <= 0.15))
                {
                    ColoraBottoneZona(Wpf_ZonaSismica.z3, 3);
                }
                else if ((acc < 0.05))
                {
                    ColoraBottoneZona(Wpf_ZonaSismica.z4, 4);
                }
            }
            catch
            {}
        }
        public static void ColoraBottoneZona(Button bottone, int z)
        {
            //colora i bottoni non selezionati del loro colore originale
            BrushConverter bc = new BrushConverter();

            Wpf_ZonaSismica.z1.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            Wpf_ZonaSismica.z2.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            Wpf_ZonaSismica.z3.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
            Wpf_ZonaSismica.z4.Background = (Brush)bc.ConvertFrom("#FF81B2BF");

            //colora solo il bottone selezionato
            bottone.Background = (Brush)bc.ConvertFrom("#FF31788B");
            Wpf_ParamSismici.zona_wpf = z;
        }

        private void wpfTexBoxAccell_TextChanged(object sender, TextChangedEventArgs e)
       {
            AccToZonaSism();
            Wpf_ParamSismici.accValue = this.wpfTexBoxAccell.Text;
        }
    }
}
