using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows;
using System.Windows.Controls;

using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.GUI
{

    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class Wpf_impo : Window
    {
        private Document m_doc;
        private Application m_app;
        public Wpf_impo(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            m_app = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_doc = uiDoc.Document;

            InitializeComponent();
            CheckUpgrade.IsChecked = Properties.Settings.Default.updaterAttivo;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.updaterAttivo == false)
            {
                //App.UpdaterAccendi();
                UpdaterRegistry.EnableUpdater(App.updater.GetUpdaterId());
            }
            Properties.Settings.Default.updaterAttivo = true;
            Properties.Settings.Default.Save();
        }

        private void CheckUpgrade_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdaterRegistry.DisableUpdater(App.updater.GetUpdaterId());
            }
            catch 
            { 
            }
            Properties.Settings.Default.updaterAttivo = false;
            Properties.Settings.Default.Save();
        }
    }
}
