using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.GUI
{
    /// <summary>
    /// Logica di interazione per UserControl1.xa
    /// </summary>
    public partial class Wpf_Libreria : Window, IDisposable
    {
        List<Materiale> list = new List<Materiale>();
        ObservableCollection<Materiale> wpfcboItems = new ObservableCollection<Materiale>();
        private Document _doc;
        private Application _app;
        private UIDocument _UiDoc;
        UIApplication _uiApp;

        public Wpf_Libreria(ExternalCommandData commandData)
        {
            _uiApp = commandData.Application;
            _app = _uiApp.Application;
            _UiDoc = _uiApp.ActiveUIDocument;
            _doc = _UiDoc.Document;

            InitializeComponent();


            //controllare se qualche oaran globale è gia compilato
            WpfAggiornaLibreria();
            if (wpfCboMateriali.Items.Count == 0)
            {
                string CaricaMat = P3Ribbon.Resources.Lang.lang.wpfPrbpCaricareMat;
                wpfCboMateriali.Items.Add(CaricaMat);
                wpfCboMateriali.SelectedIndex = 0;
            }
        }

        private void WpfBottCaricaLibreria_Click(object sender, RoutedEventArgs e)
        {

            _app.DocumentOpened -= new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);


            // if non è stato già fatto, cioè fare un metodoche controlla se nei tipi del mio documento c è QUEL PARAMETRO NASCOSTO CHE USIAMO X IDENTIFICARE IL TUTTO...
            //{
            using (var t = new Transaction(Supporto.doc, "Carica libreria"))
            {
                t.Start();
                TrasferisciStandard.TrasferisciTipiDoc(_app, _doc);
                //Materiale.PreAggiorna(m_doc); //lo faccio gia andare in form_libreriac_combobox_aggiorna:  
                WpfAggiornaLibreria();
                //TEMP DA SISTEARE CON BOOLEANI
                try
                {
                    App.ribbCboMembers = App.rbbCboMateriali.AddItems(Materiale.comboBoxMemberDatas);
                }
                catch
                { }
                // this.Close();
                //}
                _doc.Regenerate();
                t.Commit();
            }

            _app.DocumentOpened += new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);
        }


        public void WpfAggiornaLibreria()
        {
            wpfcboItems = Materiale.PreAggiorna(_doc);
            if (wpfcboItems != null)
            {
                if (wpfcboItems.Count > 0)
                {
                    //items.clear lo si utilizza quando si aggiunge un item con .add
                    try
                    {
                        wpfCboMateriali.Items.Clear();
                    }
                    catch
                    {

                    }
                    wpfCboMateriali.ItemsSource = null;

                    wpfCboMateriali.ItemsSource = wpfcboItems;
                    wpfCboMateriali.DisplayMemberPath = "name";

                    // combobox ribbon -> combobox wpf
                    if (Materiale.IdInsulTipoPreferito == null)
                    {
                        wpfCboMateriali.SelectedIndex = 0;
                    }
                    else
                    {
                        wpfCboMateriali.SelectedIndex = App.ribbCboMembers.IndexOf(App.rbbCboMateriali.Current);

                    }



                }
            }
        }


        private void cboMateriali_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void wpfBottScegliMateriale_Click(object sender, RoutedEventArgs e)
        {
            ImpostaMateriale();
            //combobox wpf -> combobox ribbon
            if (App.ribbCboMembers != null) //la prima volta che pocarico la libreria parte uesto ma non ho ancora scritto comboboxMembers_ribbon quindi giusto che salti
            {
                foreach (ComboBoxMember cbm in App.ribbCboMembers)
                {
                    string cbm_nome_totale = cbm.Name; // nel name del combo box member abbiamo concatenato l id e lo spessore (ma perche c è lo spessore nel name? non leggevamo lo spessore dal parametro di tipo dell isoalnte?)
                    int indice_ = cbm_nome_totale.IndexOf("_");
                    int cbm_id = Int32.Parse(cbm_nome_totale.Substring(0, indice_));
                    if (cbm_id == Materiale.IdInsulTipoPreferito.IntegerValue)
                    {
                        App.rbbCboMateriali.Current = cbm;
                    }
                }
            }
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (wpfCboMateriali.Items.Count > 1) // da sistemare, perche appena all inizio aggiungo la stringa "caricare la libreria prima di scegliere il materiae" il comcbobox si aggiorna e parte sto metodo, ma se non ho ancora le famiglie non voglio che mi imposti il materiale!
            //                                  // serve qualcosa di piu intelligente , il etodo ceh serve anche altrove che controlla se ho caricato, magari anceh solo con un booleano? o controlare velocemente tra i tip se c è QUEL PARAMETRO NASCOSTO CHE DOBBIAMO CREARE
            //{
            //    ImpostaMateriale();
            //}
        }

        private void ImpostaMateriale()
        {
            Materiale obj = wpfCboMateriali.SelectedItem as Materiale;
            Materiale.IdInsulTipoPreferito = obj.ID;
            Materiale.SpessoreIsolante = obj.spessore;
        }

        public void Dispose()
        {
            //throw new NotImplementedException(); //secondo me va tolto, penso che il dispose serva per cancellare dalla memroai in modo piu o meno sicuro le variabili che non stiamo piu usando, nello specifico noi usimo "using" per "bloccare" le variabili dentr forse anche temporalmente visto che la finestra si chiude da sola...BHOHHHH
        }
    }
}


