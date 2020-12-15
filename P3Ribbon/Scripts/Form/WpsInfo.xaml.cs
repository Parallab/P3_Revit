using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows;
using System.Windows.Controls;

using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.Form
{

    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class WpsInfo : Window
    {
        private Document m_doc;
        private Application m_app;
        bool checkBoxState = false;
        public WpsInfo(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            m_app = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_doc = uiDoc.Document;
            InitializeComponent();
         
        }
     

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Scripts.DynamicModelUpdater updater = new Scripts.DynamicModelUpdater(m_app.ActiveAddInId);
            
            if (CheckUpgrade.IsChecked == true)
            {
                Properties.Settings.Default.checkUpdateState = true;
                
                
                UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            }
            if (CheckUpgrade.IsChecked == false)
            {
                UpdaterRegistry.RegisterUpdater(updater);
                Properties.Settings.Default.checkUpdateState = false;
            }

            Properties.Settings.Default.Save();
        }
    }
}
