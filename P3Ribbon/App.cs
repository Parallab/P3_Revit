using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace P3Ribbon
{
    class App : IExternalApplication
    {
        static void AddRibbonPanel(UIControlledApplication a)
        {
            // Create a custom ribbon tab
            string tabName = "P3Ductal";
            a.CreateRibbonTab(tabName);
            // Add a new ribbon panel
            RibbonPanel ribbonPanelGetsisci = a.CreateRibbonPanel(tabName, "Gestisci");
            RibbonPanel ribbonPanelInfo = a.CreateRibbonPanel(tabName, "Info");

            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            //PARAMETRI SISMICI//
            #region 
            // Creo il bottone e lo collego alla classe
            PushButtonData b1Data = new PushButtonData("cmdParsism", "Parametri" + System.Environment.NewLine + "  Sisimici  ", thisAssemblyPath, "P3Ribbon.Par_Sismici");
            //carico l'icona
            PushButton pb1 = ribbonPanelGetsisci.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Compilazione dei parametri sismici per dimensionare le staffe dei condotti";
            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/config_sism.png"));
            pb1.LargeImage = pb1Image;
            #endregion

            //STAFFAGGIO//
            #region 
            // Creo il bottone e lo collego alla classe
            PushButtonData b2Data = new PushButtonData("cmdstaff", "Dimensionamento" + System.Environment.NewLine + "  staffe  ", thisAssemblyPath, "P3Ribbon.Staffaggio");
            //carico l'icona
            PushButton pb2 = ribbonPanelGetsisci.AddItem(b2Data) as PushButton;
            pb2.ToolTip = "dimensionamento della staffatura dei condotti";
            BitmapImage pb2Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/staff.png"));
            pb2.LargeImage = pb2Image;
            #endregion

            //AREA ISOLAMENTO//
            #region 
            // Creo il bottone e lo collego alla classe
            PushButtonData b3Data = new PushButtonData("cmdAreaisolamento", "Area " + System.Environment.NewLine + "  Isolamento  ", thisAssemblyPath, "P3Ribbon.Scripts.Migra_AreaIsolamento");
            //carico l'icona
            PushButton pb3 = ribbonPanelGetsisci.AddItem(b3Data) as PushButton;
            pb3.ToolTip = "Migra l'area del isolamento";
            BitmapImage pb3Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Area_Iso.png"));
            pb3.LargeImage = pb3Image;
            #endregion

            //INFO//
            #region 
            // Creo il bottone e lo collego alla classe
            PushButtonData b4Data = new PushButtonData("cmdinfo", "contatti", thisAssemblyPath, "P3Ribbon.Scripts.FinestraInfo");
            //carico l'icona
            PushButton pb4 = ribbonPanelInfo.AddItem(b4Data) as PushButton;
            pb4.ToolTip = "info e contatti";
            BitmapImage pb4Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/info32.png"));
            pb4.LargeImage = pb4Image;
            #endregion
        }


        public Result OnStartup(UIControlledApplication application)
        {
            AddRibbonPanel(application);
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}
