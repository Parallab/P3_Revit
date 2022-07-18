using System;
using System.Collections.Generic;
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
    /// Logica di interazione per Wpf_ZonaSismica.xaml
    /// </summary>
    public partial class Wpf_ZonaSismica : UserControl
    {
        public static Button z1 = new Button();
        public static Button z2 = new Button();
        public static Button z3 = new Button();
        public static Button z4 = new Button();
        public Wpf_ZonaSismica()
        {
            InitializeComponent();
            z1 = wpfBottZona1;
            z2 = wpfBottZona2;
            z3 = wpfBottZona3;
            z4 = wpfBottZona4;

            Button bz = TrovaBottoneZona(Wpf_ParamSismici.zona_wpf);
            Wpf_Accelerazione.ColoraBottoneZona(bz, Wpf_ParamSismici.zona_wpf);

        }

        private  Button TrovaBottoneZona(int z)
        {
            Button rtn = new Button();
            if (z == 1) rtn = wpfBottZona1;
            else if (z == 2) rtn = wpfBottZona2;
            else if (z == 3) rtn = wpfBottZona3;
            else if (z == 4) rtn = wpfBottZona4;
            return rtn;
        }
        public void cancellaTextBoxAcc()
        {
            Wpf_Accelerazione.wpftbAcc.Clear();
        }

        private void wpfBottZona1_Click(object sender, RoutedEventArgs e){ Wpf_Accelerazione.ColoraBottoneZona(wpfBottZona1, 1); cancellaTextBoxAcc(); }
        private void wpfBottZona2_Click(object sender, RoutedEventArgs e){ Wpf_Accelerazione.ColoraBottoneZona(wpfBottZona2, 2); cancellaTextBoxAcc(); }
        private void wpfBottZona3_Click(object sender, RoutedEventArgs e){ Wpf_Accelerazione.ColoraBottoneZona(wpfBottZona3, 3); cancellaTextBoxAcc(); }
        private void wpfBottZona4_Click(object sender, RoutedEventArgs e){ Wpf_Accelerazione.ColoraBottoneZona(wpfBottZona4, 4); cancellaTextBoxAcc(); }
    }

}
