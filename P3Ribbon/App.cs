using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Globalization;
using System.Resources;
using Autodesk.Revit.ApplicationServices;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media.Imaging;
using System.Collections;
using Autodesk.Revit.UI.Events;

namespace P3Ribbon
{

    class App : IExternalApplication
    {

        public static UIControlledApplication UICapp;
        public static ControlledApplication Capp;


        public static ComboBox comboMat;
        public enum Lingua
        {
            ITA = 0,
            ENG = 1
        }
        public static Lingua lingua_plugin = Lingua.ITA; // LEGGERE LINGUA REVIT!!! ocio se c è lingua tipo francese


        public static string tabName = "P3ductBIM";
        public static ResourceSet res_ita = Resources.str_IT.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        public static ResourceSet res_eng = Resources.str_EN.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        private object commandData;

        public static void LeggiLingua(ControlledApplication Capp)
        {
            //leggo la lingua di partenza
            LanguageType linguapartenza = Capp.Language;
            if (linguapartenza == LanguageType.Italian)
            {
                lingua_plugin = Lingua.ITA;
            }
            else if (linguapartenza == LanguageType.English_GB || linguapartenza == LanguageType.English_GB)
            {
                lingua_plugin = Lingua.ENG;
            }
        }

        public static object AddRibbonPanel(UIControlledApplication a)
        {

            // Create a custom ribbon tab
            a.CreateRibbonTab(tabName);
            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;


            RibbonPanel ribbonPanelModellazione = a.CreateRibbonPanel(tabName, res_valore("Modellazione"));


            // MODELLAZIONE
            #region bottone: lingua WIP
            AggiungiSplitButtonLingua(ribbonPanelModellazione, thisAssemblyPath);

            #endregion
            #region bottone: carica libreria WIP
            PushButtonData b9Data = new PushButtonData("cmdlibreria", "Carica" + System.Environment.NewLine + "Libreria", thisAssemblyPath, "P3Ribbon.Scripts.FinestraLibreria");
            PushButton pb9 = ribbonPanelModellazione.AddItem(b9Data) as PushButton;
            pb9.ToolTip = "Carica le famiglie e i materiali P3";
            BitmapImage pb9Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Libreria_2_logoP3+ductbim.png"));
            pb9.LargeImage = pb9Image;
            #endregion
            #region bottone: canale WIP
            AggiungiSplitButtonCanale(ribbonPanelModellazione,thisAssemblyPath);
            //PushButtonData b10Data = new PushButtonData("cmdcanale", "Canale", thisAssemblyPath, "P3Ribbon.Scripts.CreaCanale");
            //PushButton pb10 = ribbonPanelModellazione.AddItem(b10Data) as PushButton;
            //pb10.ToolTip = "Posiziona i canali preisolati P3";
            //BitmapImage pb10Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale.png"));
            //pb10.LargeImage = pb10Image;
            #endregion
            #region Panello cambia materiale più ribbonbox
            ComboBoxData cbData = new ComboBoxData("Combo1");
            PushButtonData b11Data = new PushButtonData("cmdmateriale", "Cambia" + System.Environment.NewLine + "Materiale", thisAssemblyPath,
"P3Ribbon.Scripts.CambiaMateriale");
            PushButtonData b15Data = new PushButtonData("cmdcaricamaterial", "Carica Materiale", thisAssemblyPath,
"P3Ribbon.Scripts.PopolaComboBox_temp");

            IList<RibbonItem> MaerialeItems = ribbonPanelModellazione.AddStackedItems(b11Data, cbData, b15Data);

            PushButton pb11 = MaerialeItems[0] as PushButton;
            BitmapImage pb11LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale_Materiale.png"));
            pb11.LargeImage = pb11LargeImage;
            BitmapImage pb11Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale_Materiale16.png"));
            pb11.Image = pb11Image;
            pb11.ToolTip = "Cambia il materiale dei canali P3";


            comboMat = MaerialeItems[1] as ComboBox;
            comboMat.AddItems(Scripts.Materiale.comboBoxMemberDatas);
            comboMat.CurrentChanged += new EventHandler<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs>(comboBx_CurrentChanged);


            PushButton pb15 = MaerialeItems[2] as PushButton;
            BitmapImage pb15LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_FrecciaGiù.png"));
            pb15.LargeImage = pb15LargeImage;
            BitmapImage pb15Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_FrecciaGiù16.png"));
            pb15.Image = pb15Image;
            pb15.ToolTip = "Carica i materiali nel ribbonbox";




            #endregion



            // QUANTITÀ
            RibbonPanel ribbonPanelQuantità = a.CreateRibbonPanel(tabName, "Quantità");
            #region bottone: elenco prezzi WIP
            PushButtonData b5Data = new PushButtonData("cmdelencopezzi", "Elenco" + System.Environment.NewLine + "Prezzi", thisAssemblyPath, "P3Ribbon.Scripts.ElencoPrezzi");
            PushButton pb5 = ribbonPanelQuantità.AddItem(b5Data) as PushButton;
            pb5.ToolTip = "Elenca i canali e i pezzi speciali P3";
            BitmapImage pb5Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoPezzi.png"));
            pb5.LargeImage = pb5Image;
            #endregion
            #region bottone: elenco materiali (ex area)
            PushButtonData b3Data = new PushButtonData("cmdAreaisolamento", "Elenco" + System.Environment.NewLine + "Materiali", thisAssemblyPath, "P3Ribbon.Scripts.ElencoMateriali");
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
            PushButtonData b6Data = new PushButtonData("cmdelencostaffaggi", "Elenco" + System.Environment.NewLine + "Staffaggio", thisAssemblyPath, "P3Ribbon.Scripts.ElencoStaffaggio");
            PushButton pb6 = ribbonPanelSisma.AddItem(b6Data) as PushButton;
            pb6.ToolTip = "Elenca gli staffaggi utilizzati e la relativa componentistica";
            BitmapImage pb6Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoStaffaggio_2.png"));
            pb6.LargeImage = pb6Image;
            #endregion
            #region bottone: elenco componenti WIP
            PushButtonData b13Data = new PushButtonData("cmdelencocomp", "Elenco" + System.Environment.NewLine + "Componenti", thisAssemblyPath, "P3Ribbon.Scripts.ElencoComponenti");
            PushButton pb13 = ribbonPanelSisma.AddItem(b13Data) as PushButton;
            pb13.ToolTip = "Elenca i componenti P3";
            BitmapImage pb13Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoComponenti.png"));
            pb13.LargeImage = pb13Image;
            #endregion


            // MATERIALE RICICLATO
            RibbonPanel ribbonPanelMaterialeRiciclato = a.CreateRibbonPanel(tabName, "Materiale Riciclato");
            #region bottone: quantità WIP
            PushButtonData b7Data = new PushButtonData("cmdquantità", "Quantità", thisAssemblyPath, "P3Ribbon.Scripts.FinestraQuantità");
            PushButton pb7 = ribbonPanelMaterialeRiciclato.AddItem(b7Data) as PushButton;
            pb7.ToolTip = "Elenca la quantità di materiale riciclato nei canali P3";
            BitmapImage pb7Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Quantità.png"));
            pb7.LargeImage = pb7Image;
            #endregion

            // INFO
            #region
            RibbonPanel ribbonPanelInfo = a.CreateRibbonPanel(tabName, "Info");
            //inizializzo i due bottoni
            PushButtonData b4Data = new PushButtonData("cmdinfo", "Contatti", thisAssemblyPath, "P3Ribbon.Scripts.FinestraInfo");
            PushButtonData b12Data = new PushButtonData("cmdimpo", "Impostazioni", thisAssemblyPath, "P3Ribbon.Scripts.FinestraImpo");
            //
            IList<RibbonItem> InfoItems = ribbonPanelInfo.AddStackedItems(b4Data, b12Data);

            #region bottone: info

            PushButton pb4 = InfoItems[0] as PushButton;
            pb4.ToolTip = "Informazioni e contatti P3";
            BitmapImage pb4LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Info.png"));
            pb4.LargeImage = pb4LargeImage;
            BitmapImage pb4Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Info16.png"));
            pb4.Image = pb4Image;
            #endregion
            #region impostazioni

            PushButton pb12 = InfoItems[1] as PushButton;
            pb12.ToolTip = "Impostazioni sull'applicativo P3";
            BitmapImage pb12LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_p3_inkscape_icona_Setting.png"));
            pb12.LargeImage = pb12LargeImage;
            BitmapImage pb12Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_p3_inkscape_icona_Setting16.png"));
            pb12.Image = pb12Image;

            #endregion
            #endregion


            MigraRibbonPanelName2Titolo(a);
            return Result.Succeeded;
        }

        private static void comboBx_CurrentChanged(object sender, ComboBoxCurrentChangedEventArgs e)
        {
            //var debug =  comboMat.Current;
            string nome = comboMat.Current.Name;
            int indice_ = nome.IndexOf("_");
            Scripts.Materiale.AggiornaTendinaRibbon(nome);

        }

        static void MigraRibbonPanelName2Titolo(UIControlledApplication a)
        {
            List<RibbonPanel> rPanels = a.GetRibbonPanels(App.tabName);
            foreach (RibbonPanel rp in rPanels)
            {
                rp.Title = rp.Name;
            }
        }


        public Result OnStartup(UIControlledApplication application)
        {


            AddRibbonPanel(application);
            UICapp = application;


            //attiva i registri all'avvio di revit
            Scripts.DynamicModelUpdater updater = new Scripts.DynamicModelUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);
            LogicalOrFilter f = Scripts.Supporto.CatFilterDuctAndFitting;
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), f, Element.GetChangeTypeElementAddition());

            return Result.Succeeded;

        }
        public Result OnShutdown(UIControlledApplication application)
        {
            Scripts.DynamicModelUpdater updater = new Scripts.DynamicModelUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }

        public static string res_valore(string Var, Lingua l)
        {

            ResourceSet rs = null;
            if (l == Lingua.ITA)
            {
                rs = res_ita;
            }
            else if (l == Lingua.ENG)
            {
                rs = res_eng;
            }

            return rs.GetObject(Var).ToString();
        }

        public static string res_valore(string Var)
        {
            //ricordarsi di modificare nel caso di altra lingua
            //all'accensione!
            ResourceSet rs = Resources.str_IT.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            return rs.GetObject(Var).ToString();

        }

        private static void AggiungiSplitButtonLingua(RibbonPanel rp, string Assemblypath)
        {
            PushButtonData sb1BOne = new PushButtonData("cmdLinguaIT", "Italiano", Assemblypath, "P3Ribbon.Scripts.CambaLingua");
            sb1BOne.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_LinguaItaliana.png"));

            PushButtonData sb1BTwo = new PushButtonData("cmdLinguaENG", "Inglese", Assemblypath, "P3Ribbon.Scripts.CambaLingua");
            sb1BTwo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_LinguaInglese.png"));

            SplitButtonData sbd1 = new SplitButtonData("splitButton1", "Split");
            SplitButton sb1 = rp.AddItem(sbd1) as SplitButton;
            sb1.AddPushButton(sb1BOne);
            sb1.AddPushButton(sb1BTwo);

        }
        private static void AggiungiSplitButtonCanale(RibbonPanel rp, string Assemblypath)
        {
            PushButtonData sb2BOne = new PushButtonData("cmdcanaledinamico", "Canale" + System.Environment.NewLine + "Dinamico", Assemblypath, "P3Ribbon.Scripts.CreaCanaleDinamico");
            sb2BOne.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale.png"));
            sb2BOne.ToolTip = "Crea un condotto di tipo: Dinamico";

            PushButtonData sb2BTwo = new PushButtonData("cmdcanalescarpette", "Canale" + System.Environment.NewLine + "Scarpette", Assemblypath, "P3Ribbon.Scripts.CreaCanaleScarpette");
            sb2BTwo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale.png"));
            sb2BTwo.ToolTip = "Crea un condotto di tipo: Scarpette";

            SplitButtonData sbd2 = new SplitButtonData("splitButton2", "Split");
            SplitButton sb2 = rp.AddItem(sbd2) as SplitButton;
            sb2.AddPushButton(sb2BOne);
            sb2.AddPushButton(sb2BTwo);

        }

    }
}
