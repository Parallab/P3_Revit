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
    /// Logica di interazione per WpfScegliZonaAcc.xaml
    /// </summary>
    public partial class WpfScegliZonaAcc : UserControl
    {
        bool finestraAcc = false;
        public WpfScegliZonaAcc()
        {
            InitializeComponent();
        }

        private void Zona_Sismica_Click(object sender, RoutedEventArgs e)
        {
            finestraAcc = false;
            Wpf_ParamSismici wpfSism = new Wpf_ParamSismici();
            DataContext = wpfSism;
            wpfSism.CambiaFinestra(finestraAcc);
            
        }

        private void Accellerazione_Click(object sender, RoutedEventArgs e)
        {
            finestraAcc = true;
            Wpf_ParamSismici wpfSism = new Wpf_ParamSismici();
            wpfSism.CambiaFinestra(finestraAcc);
        }
    }
}
