using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media.Imaging;

namespace P3Ribbon
{
    class App : IExternalApplication
    {
        static void AddRibbonPanel(UIControlledApplication a)
        {
            // Create a custom ribbon tab
            string tabName = "P3ductBIM";
            a.CreateRibbonTab(tabName);
			// Get dll assembly path
			string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

			// MODELLAZIONE
			RibbonPanel ribbonPanelModellazione = a.CreateRibbonPanel(tabName, "Modellazione");
			#region bottone: lingua WIP
			PushButtonData b8Data = new PushButtonData("cmdlingua", "Lingua", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
			PushButton pb8 = ribbonPanelModellazione.AddItem(b8Data) as PushButton;
			pb8.ToolTip = "Seleziona tra inglese e italiano";
			BitmapImage pb8Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Lingua.png"));
			pb8.LargeImage = pb8Image;
			#endregion
			#region bottone: carica libreria WIP
			PushButtonData b9Data = new PushButtonData("cmdlibreria", "Carica" + System.Environment.NewLine + "Libreria", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
			PushButton pb9 = ribbonPanelModellazione.AddItem(b9Data) as PushButton;
			pb9.ToolTip = "Carica le famiglie e i materiali P3";
			BitmapImage pb9Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Libreria_2_logoP3+ductbim.png"));
			pb9.LargeImage = pb9Image;
			#endregion
			#region bottone: canale WIP
			PushButtonData b10Data = new PushButtonData("cmdcanale", "Canale", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
			PushButton pb10 = ribbonPanelModellazione.AddItem(b10Data) as PushButton;
			pb10.ToolTip = "Posiziona i canali preisolati P3";
			BitmapImage pb10Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale.png"));
			pb10.LargeImage = pb10Image;
			#endregion
			#region bottone: cambia materiale WIP
			PushButtonData b11Data = new PushButtonData("cmdmateriale", "Cambia" + System.Environment.NewLine + "Materiale", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
			PushButton pb11 = ribbonPanelModellazione.AddItem(b11Data) as PushButton;
			pb11.ToolTip = "Cambia il materiale dei canali P3";
			BitmapImage pb11Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale_Materiale.png"));
			pb11.LargeImage = pb11Image;
			#endregion

			// QUANTITÀ
			RibbonPanel ribbonPanelQuantità = a.CreateRibbonPanel(tabName, "Quantità");
			#region bottone: elenco pezzi WIP
			PushButtonData b5Data = new PushButtonData("cmdelencopezzi", "Elenco" + System.Environment.NewLine + "Pezzi", thisAssemblyPath, "P3Ribbon.Scripts.Migra_AreaIsolamento");
			PushButton pb5 = ribbonPanelQuantità.AddItem(b5Data) as PushButton;
			pb5.ToolTip = "Elenca i canali e i pezzi speciali P3";
			BitmapImage pb5Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoPezzi.png"));
			pb5.LargeImage = pb5Image;
			#endregion
			#region bottone: elenco materiali (ex area)
			PushButtonData b3Data = new PushButtonData("cmdAreaisolamento", "Elenco" + System.Environment.NewLine + "Materiali", thisAssemblyPath, "P3Ribbon.Scripts.Migra_AreaIsolamento");
			PushButton pb3 = ribbonPanelQuantità.AddItem(b3Data) as PushButton;
			pb3.ToolTip = "Elenca i materiali utilizzati nei canali P3 e le relative superfici"; // DA SISTEMARE
			BitmapImage pb3Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoMateriali.png"));
			pb3.LargeImage = pb3Image;
			#endregion

			// SISMA
			RibbonPanel ribbonPanelSisma = a.CreateRibbonPanel(tabName, "Sisma");
			#region bottone: parametri sismici
			PushButtonData b1Data = new PushButtonData("cmdParsism", "Parametri" + System.Environment.NewLine + "  Sisimici  ", thisAssemblyPath, "P3Ribbon.Par_Sismici");
			PushButton pb1 = ribbonPanelSisma.AddItem(b1Data) as PushButton;
			pb1.ToolTip = "Compilazione dei parametri sismici per dimensionare le staffe dei condotti";
			BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/config_sism.png"));
			pb1.LargeImage = pb1Image;
			#endregion
			#region bottone: posizionamento staffaggio
			PushButtonData b2Data = new PushButtonData("cmdstaff", "Posizionamento" + System.Environment.NewLine + "Staffaggio", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
			PushButton pb2 = ribbonPanelSisma.AddItem(b2Data) as PushButton;
			pb2.ToolTip = "Posizionamento e dimensionamento automatico dei canali alle strutture";
			BitmapImage pb2Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/staff.png"));
			pb2.LargeImage = pb2Image;
			#endregion
			#region bottone:elenco staffaggi WIP
			PushButtonData b6Data = new PushButtonData("cmdelencostaffaggi", "Elenco" + System.Environment.NewLine + "Staffaggio", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
			PushButton pb6 = ribbonPanelSisma.AddItem(b6Data) as PushButton;
			pb6.ToolTip = "Elenca gli staffaggi utilizzati e la relativa componentistica";
			BitmapImage pb6Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoStaffaggio_2.png"));
			pb6.LargeImage = pb6Image;
			#endregion

			// MATERIALE RICICLATO
			RibbonPanel ribbonPanelMaterialeRiciclato = a.CreateRibbonPanel(tabName, "Materiale Riciclato");
			#region bottone: quantità WIP
			PushButtonData b7Data = new PushButtonData("cmdquantità", "Quantità", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
			PushButton pb7 = ribbonPanelMaterialeRiciclato.AddItem(b7Data) as PushButton;
			pb7.ToolTip = "Elenca la quantità di materiale riciclato nei canali P3";
			BitmapImage pb7Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Quantità.png"));
			pb7.LargeImage = pb7Image;
			#endregion

			// INFO
            RibbonPanel ribbonPanelInfo = a.CreateRibbonPanel(tabName, "Info");
			#region bottone: info
			PushButtonData b4Data = new PushButtonData("cmdinfo", "contatti", thisAssemblyPath, "P3Ribbon.Scripts.FinestraInfo");
			PushButton pb4 = ribbonPanelInfo.AddItem(b4Data) as PushButton;
			pb4.ToolTip = "Informazioni e contatti P3";
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
