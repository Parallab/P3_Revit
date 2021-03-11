
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Globalization;
using System.Resources;
using Autodesk.Revit.ApplicationServices;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI.Events;
using P3Ribbon.Scripts;
using Autodesk.Revit.DB.Events;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace P3Ribbon
{
    class App : IExternalApplication
    {

        public static UIControlledApplication UICapp;
        public static ControlledApplication Capp;


        public static ComboBox rbbCboMateriali;
        public static SplitButton sb1;
        public static IList<ComboBoxMember> ribbCboMembers;
        public enum Lingua
        {
            ITA = 0,
            ENG = 1
        }
        public static Lingua lingua_plugin = Lingua.ITA;
        public static Lingua lingua_arrivo;

        public static string tabName = "P3duct𝗯𝗶𝗺";

        public static ResourceSet res_ita = Resources.Lang.rp_ITA.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        public static ResourceSet res_eng = Resources.Lang.rp_ENG.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        private object commandData;

        public static DynamicModelUpdater updater;

        public static void LeggiLingua(ControlledApplication Capp)
        {
            //leggo la lingua di partenza
            LanguageType linguapartenza = Capp.Language;
            if (linguapartenza == LanguageType.Italian)
            {
                lingua_plugin = Lingua.ITA;
            }
            else
            //else if (linguapartenza == LanguageType.English_GB || linguapartenza == LanguageType.English_GB)
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


            RibbonPanel ribbonPanelModellazione = a.CreateRibbonPanel(tabName, Res_ValoreLingua("Modellazione"));

            // MODELLAZIONE
            #region bottone: lingua WIP
            AggiungiSplitButtonLingua(ribbonPanelModellazione, thisAssemblyPath);
            #endregion

            #region bottone: carica libreria WIP
            //PushButtonData b9Data = new PushButtonData("cmdlibreria", "Carica" + System.Environment.NewLine + "Libreria", thisAssemblyPath, "P3Ribbon.Scripts.FinestraLibreria");
            //PushButton pb9 = ribbonPanelModellazione.AddItem(b9Data) as PushButton;
            //pb9.ToolTip = "Carica le famiglie e i materiali P3";
            //BitmapImage pb9Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Libreria_2_logoP3+ductbim.png"));
            //pb9.LargeImage = pb9Image;
            #endregion

            #region bottone: canale WIP
            AggiungiSplitButtonCanale(ribbonPanelModellazione, thisAssemblyPath);
            #endregion

            #region Panello cambia materiale più ribbonbox
            ComboBoxData cbData = new ComboBoxData("Combo1");
            PushButtonData b11Data = new PushButtonData("cmdmateriale", "Cambia" + System.Environment.NewLine + "Materiale", thisAssemblyPath, "P3Ribbon.Scripts.CambiaMateriale");


            IList<RibbonItem> ItemsMateriale = ribbonPanelModellazione.AddStackedItems(b11Data, cbData);

            PushButton pb11 = ItemsMateriale[0] as PushButton;
            BitmapImage pb11LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale_Materiale.png"));
            pb11.LargeImage = pb11LargeImage;
            BitmapImage pb11Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale_Materiale16.png"));
            pb11.Image = pb11Image;
            pb11.ToolTip = "Cambia il materiale dei canali P3";

            rbbCboMateriali = ItemsMateriale[1] as ComboBox;
            //comboboxMembers_ribbon = rbCboMat.AddItems(Materiale.comboBoxMemberDatas);
            rbbCboMateriali.CurrentChanged += new EventHandler<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs>(comboBx_CurrentChanged);
            #endregion



            // QUANTITÀ
            RibbonPanel ribbonPanelQuantità = a.CreateRibbonPanel(tabName, "Quantità");
            #region bottone: elenco pezzi WIP
            PushButtonData b5Data = new PushButtonData("cmdelencopezzi", "Elenco" + System.Environment.NewLine + "Canali", thisAssemblyPath, "P3Ribbon.Scripts.ElencoPezzi");
            PushButton pb5 = ribbonPanelQuantità.AddItem(b5Data) as PushButton;
            pb5.ToolTip = "Elenca i canali e i pezzi speciali P3";
            BitmapImage pb5Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoPezzi.png"));
            pb5.LargeImage = pb5Image;
            #endregion
            #region bottone: elenco materiali (ex area)
            PushButtonData b3Data = new PushButtonData("cmdlistamateriali", "Elenco" + System.Environment.NewLine + "Materiali", thisAssemblyPath, "P3Ribbon.Scripts.ElencoMateriali");
            PushButton pb3 = ribbonPanelQuantità.AddItem(b3Data) as PushButton;
            pb3.ToolTip = "Elenca i materiali utilizzati nei canali P3 e le relative superfici"; // DA SISTEMARE
            BitmapImage pb3Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale_Materiale.png"));
            pb3.LargeImage = pb3Image;
            #endregion


            // SISMA
            RibbonPanel ribbonPanelSisma = a.CreateRibbonPanel(tabName, "Sisma");
            #region bottone: parametri sismici
            PushButtonData b1Data = new PushButtonData("cmdParsism", "Parametri" + System.Environment.NewLine + "  Sisimici  ", thisAssemblyPath, "P3Ribbon.ParSismici");
            PushButton pb1 = ribbonPanelSisma.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Compilazione dei parametri sismici per dimensionare le staffe dei condotti";
            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_SismaRosso.png"));
            pb1.LargeImage = pb1Image;
            #endregion

            #region bottone: posizionamento staffaggio
            PushButtonData b2Data = new PushButtonData("cmdstaff", "Posizionamento" + System.Environment.NewLine + "Staffaggio", thisAssemblyPath, "P3Ribbon.Scripts.Staffaggio");
            PushButton pb2 = ribbonPanelSisma.AddItem(b2Data) as PushButton;
            pb2.ToolTip = "Posizionamento e dimensionamento automatico dei canali alle strutture";
            BitmapImage pb2Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_CanaleStaffatoRosso.png"));
            pb2.LargeImage = pb2Image;
            #endregion


            #region Split button Elenco staffaggi
            AggiungiSplitButtonElencostaff(ribbonPanelSisma, thisAssemblyPath);

            #region bottone:elenco staffaggi WIP
            //PushButtonData b6Data = new PushButtonData("cmdelencostaff", "Elenco" + System.Environment.NewLine + "Punti", thisAssemblyPath, "P3Ribbon.Scripts.ElencoPunti");
            //PushButton pb6 = ribbonPanelSisma.AddItem(b6Data) as PushButton;
            //pb6.ToolTip = "Elenca gli staffaggi utilizzati e la relativa componentistica";
            //BitmapImage pb6Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoStaffaggio_2.png"));
            //pb6.LargeImage = pb6Image;
            #endregion
            #region bottone: elenco componenti WIP
            //PushButtonData b13Data = new PushButtonData("cmdelencoco", "Elenco" + System.Environment.NewLine + "Componenti", thisAssemblyPath, "P3Ribbon.Scripts.ElencoComponenti");
            //PushButton pb13 = ribbonPanelSisma.AddItem(b13Data) as PushButton;
            //pb13.ToolTip = "Elenca i componenti P3";
            //BitmapImage pb13Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoComponenti.png"));
            //pb13.LargeImage = pb13Image;
            #endregion

            #endregion


            // MATERIALE RICICLATO
            RibbonPanel ribbonPanelMaterialeRiciclato = a.CreateRibbonPanel(tabName, "Materiale Riciclato");
            #region bottone: quantità WIP
            PushButtonData b7Data = new PushButtonData("cmdquantità", "Quantità", thisAssemblyPath, "P3Ribbon.Scripts.FinestraQuantità");
            PushButton pb7 = ribbonPanelMaterialeRiciclato.AddItem(b7Data) as PushButton;
            pb7.ToolTip = "Elenca la quantità di materiale riciclato nei canali P3";
            BitmapImage pb7Image = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_MatRIciclatoVerde.png"));
            pb7.LargeImage = pb7Image;
            #endregion

            // INFO
            #region
            RibbonPanel ribbonPanelInfo = a.CreateRibbonPanel(tabName, "Info");
            //inizializzo i due bottoni
            PushButtonData b4Data = new PushButtonData("cmdinfo", "Contatti", thisAssemblyPath, "P3Ribbon.Scripts.FinestraInfo");
            PushButtonData b12Data = new PushButtonData("cmdimpo", "Impostazioni", thisAssemblyPath, "P3Ribbon.Scripts.FinestraImpostazioni");
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
            //var debug =  rbCboMat.Current;
            Supporto.AggiornaDoc(e.Application.ActiveUIDocument.Document);
            string nome = rbbCboMateriali.Current.Name;
            Materiale.AggiornaTendinaRibbon(nome);
        }

        static void MigraRibbonPanelName2Titolo(UIControlledApplication a)
        {
            List<RibbonPanel> rPanels = a.GetRibbonPanels(App.tabName);
            foreach (RibbonPanel rp in rPanels)
            {
                rp.Title = rp.Name;
            }
        }

        
        public Result OnStartup(UIControlledApplication UiCapplication)
        {
            Properties.Settings.Default.updaterAttivo = true;
            Properties.Settings.Default.Save();

            try
            {   //creo degli eventhandler all'aeprtura di un documento e alla creazione di uno nuovo
                UiCapplication.ControlledApplication.DocumentOpened += new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(Application_DocumentOpened);
                UiCapplication.ControlledApplication.DocumentCreated += new EventHandler<Autodesk.Revit.DB.Events.DocumentCreatedEventArgs>(Application_DocumentCreated);
                //application.ControlledApplication.DocumentChanged += new EventHandler<Autodesk.Revit.DB.Events.DocumentChangedEventArgs>(Application_DocumentChanged);
                UiCapplication.ViewActivated += new EventHandler<ViewActivatedEventArgs>(OnViewActivated);
            }
            catch
            {
                return Result.Failed;
            }
            var langCode = Properties.Settings.Default.languageCode;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
            AddRibbonPanel(UiCapplication);
            UICapp = UiCapplication;

            Supporto.ActiveAddInId = UiCapplication.ActiveAddInId;
    
            UpdaterAccendi();
            ResourceManager rm = new ResourceManager("items", Assembly.GetExecutingAssembly());
            return Result.Succeeded;
        }

        public static void UpdaterAccendi()
        {
            App.updater = new DynamicModelUpdater(Supporto.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            LogicalOrFilter f = Supporto.CatFilterDuctAndFitting;
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), f, Element.GetChangeTypeElementAddition());
        }


        public Result OnShutdown(UIControlledApplication application)
        {
            //DynamicModelUpdater updater = new DynamicModelUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());

            application.ControlledApplication.DocumentOpened -= new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);
            return Result.Succeeded;
        }


        public static void Application_DocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs args)
        {
            Document doc = args.Document;
            Supporto.AggiornaDoc(doc);            
            //Application app = doc.Application;
            //Supporto.doc = doc; // SPERIAMO CHE CI RISOLVA TUTTI I PROBLEMI DEL MONDO
            //Supporto.app = app;
            try
            {
                Materiale.PreAggiorna(doc);
                if (rbbCboMateriali.GetItems().Count == 0)
                {
                    ribbCboMembers = rbbCboMateriali.AddItems(Materiale.comboBoxMemberDatas);
                }
            }
            catch (Exception ex)
            {
                //throw;
            }


        }


        private void Application_DocumentCreated(object sender, DocumentCreatedEventArgs args)
        {
            
            Document doc = args.Document;
            Supporto.AggiornaDoc(doc);
            //Application app = doc.Application;
            //Supporto.doc = doc; // SPERIAMO CHE CI RISOLVA TUTTI I PROBLEMI DEL MONDO
            //Supporto.app = app;
            //lascio cmq quanto segue perche anche se è un fil enuovo magari uso come template qualcosa ocn già i materiali P3 e quindi devo controllarli e compilare i combobox vari:
            try
            {
                Materiale.PreAggiorna(doc);
                if (rbbCboMateriali.GetItems().Count == 0)
                { 
                    ribbCboMembers = rbbCboMateriali.AddItems(Materiale.comboBoxMemberDatas); 
                }
            }
            catch (Exception ex)
            {
                //throw;
            }

        }

        private void Application_DocumentChanged(object sender, DocumentChangedEventArgs args)
        { 
           //Supporto.AggiornaDoc(args.GetDocument());
           //Supporto.doc = args.GetDocument();
        }
   
        void OnViewActivated(
          object sender,
          ViewActivatedEventArgs e)
        {
            View vPrevious = e.PreviousActiveView;
            View vCurrent = e.CurrentActiveView;

            Document doc = vCurrent.Document;
            Supporto.AggiornaDoc(doc);
        }

        public static string Res_ValoreLingua(string Var, Lingua l)
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

        public static string Res_ValoreLingua(string Var)
        {
            //ricordarsi di modificare nel caso di altra lingua
            //all'accensione!
            ResourceSet rs = Resources.Lang.rp_ITA.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            return rs.GetObject(Var).ToString();

        }



        private static void AggiungiSplitButtonLingua(RibbonPanel rp, string Assemblypath)
        {
            PushButtonData sb1One = new PushButtonData("cmdCaricaLibreria", "Carica Libreria", Assemblypath, "P3Ribbon.Scripts.FinestraLibreria");
            sb1One.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Libreria_3_logoP3.png"));
            sb1One.ToolTip = "Carica le famiglie e i materiali P3";

            PushButtonData sb1Two = new PushButtonData("cmdLinguaIT", "Italiano", Assemblypath, "P3Ribbon.Scripts.LinguaItaliano");
            sb1Two.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_LinguaItaliana.png"));

            PushButtonData sb1Three = new PushButtonData("cmdLinguaENG", "Inglese", Assemblypath, "P3Ribbon.Scripts.LinguaInglese");
            sb1Three.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_LinguaInglese.png"));

            SplitButtonData sbd1 = new SplitButtonData("splitButtonLingua", "Split");
            
            sb1 = rp.AddItem(sbd1) as SplitButton;
            sb1.IsSynchronizedWithCurrentItem = false;
            sb1.AddPushButton(sb1One);
            sb1.AddPushButton(sb1Two);
            sb1.AddPushButton(sb1Three);

          
        }
        private static void AggiungiSplitButtonCanale(RibbonPanel rp, string Assemblypath)
        {
            PushButtonData sb2BOne = new PushButtonData("cmdcanaledinamico", "Canale", Assemblypath, "P3Ribbon.Scripts.CreaCanaleDinamico");
            sb2BOne.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale.png"));
            sb2BOne.ToolTip = "Crea un condotto di tipo: Dinamico";

            PushButtonData sb2BTwo = new PushButtonData("cmdcanalescarpette", "Stacco a" + System.Environment.NewLine + "Scarpetta", Assemblypath, "P3Ribbon.Scripts.CreaCanaleScarpette");
            sb2BTwo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_Canale.png"));
            sb2BTwo.ToolTip = "Crea un condotto di tipo: Scarpette";

            SplitButtonData sbd2 = new SplitButtonData("splitButtonCanale", "Split");
            SplitButton sb2 = rp.AddItem(sbd2) as SplitButton;
            sb2.AddPushButton(sb2BOne);
            sb2.AddPushButton(sb2BTwo);

        }
        private static void AggiungiSplitButtonElencostaff(RibbonPanel rp, string Assemblypath)
        {
            PushButtonData sb3BOne = new PushButtonData("cmdelencostaffaggi", "Elenco" + System.Environment.NewLine + "Staffaggio", Assemblypath, "P3Ribbon.Scripts.ElencoStaffaggio");
            sb3BOne.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoStaffaggio_dettaglio_bianco.png"));
            sb3BOne.ToolTip = "Elenca gli staffaggi utilizzati e la relativa componentistica";

            PushButtonData sb3BTwo = new PushButtonData("cmdelencopunti", "Elenco" + System.Environment.NewLine + "Punti", Assemblypath, "P3Ribbon.Scripts.ElencoPunti");
            sb3BTwo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/P3Ribbon;component/Resources/Icons/20041_P3_Inkscape_Icona_ElencoStaffaggio_dettaglio_bianco.png"));
            sb3BTwo.ToolTip = "Elenca i componenti P3";

            SplitButtonData sbd3 = new SplitButtonData("splitButtonElencoStaffaggio", "Split");
            SplitButton sb3 = rp.AddItem(sbd3) as SplitButton;
            sb3.AddPushButton(sb3BOne);
            sb3.AddPushButton(sb3BTwo);

        }

    }
}
