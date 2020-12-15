using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.Form
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class Wpf_Libreria : Window, IDisposable
    {
        List<Materiale> list = new List<Materiale>();
        ObservableCollection<Materiale> cmbContent = new ObservableCollection<Materiale>();
        private Document m_doc;
        private Application m_app;
        private UIDocument m_UiDoc;
        UIApplication m_uiApp;

        public Wpf_Libreria(ExternalCommandData commandData)
        {
            m_uiApp = commandData.Application;
            m_app = m_uiApp.Application;
            m_UiDoc = m_uiApp.ActiveUIDocument;
            m_doc = m_UiDoc.Document;

            InitializeComponent();


            //controllare se qualche oaran globale è gia compilato

            Form_Libreria_Combobox_Aggiorna();
            if (cboMateriali.Items.Count == 0)
            {
                cboMateriali.Items.Add("Caricare la libreria prima di scegliere il materiale");
                cboMateriali.SelectedIndex = 0;
            }
        }

        private void buttCaricLibreria_Click(object sender, RoutedEventArgs e)
        {
            var test = 1;
            //Form.UserControl1 frm = new Form.UserControl1();
            //frm.ShowDialog();
            m_app.DocumentOpened -= new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);

            // m_uiApp.DialogBoxShowing += new EventHandler(dismissFloorQuestion);

            // if non è stato già fatto, cioè fare un metodoche controlla se nei tipi del mio documento c è QUEL PARAMETRO NASCOSTO CHE USIAMO X IDENTIFICARE IL TUTTO...
            //{
            using (var t = new Transaction(Supporto.doc, "Carica libreria"))
            {
                t.Start();
                TrasferisciStandard.TrasferisciTipiDoc(m_app, m_doc);
                //Materiale.PreAggiorna(m_doc); //lo faccio gia andare in form_libreriac_combobox_aggiorna:  
                Form_Libreria_Combobox_Aggiorna();
                //TEMP DA SISTEARE CON BOOLEANI
                try
                {
                    App.comboboxMembers_ribbon = App.comboMat.AddItems(Materiale.comboBoxMemberDatas);
                }
                catch
                { }
                // this.Close();
                //}
                m_doc.Regenerate();
                t.Commit();
            }

            m_app.DocumentOpened += new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);
        }


        public void Form_Libreria_Combobox_Aggiorna()
        {
            cmbContent = Materiale.PreAggiorna(m_doc);
            if (cmbContent != null)
            {
                if (cmbContent.Count > 0)
                {
                    cboMateriali.Items.Clear();
                    cboMateriali.ItemsSource = cmbContent;
                    cboMateriali.DisplayMemberPath = "Name";

                    // combobox ribbon -> combobox wpf
                    if (Materiale.IdInsulTipoPreferito == null)
                    {
                        cboMateriali.SelectedIndex = 0;
                    }
                    else
                    {
                        //foreach (var item in cboMateriali.Items) //combobox wpf App.
                        int i = 0;
                        foreach (ComboBoxMember cbm in App.comboboxMembers_ribbon)
                        {
                            //cmbContent observaclletion che alimenta il combobox

                            //if ( item.ID.IntegerValue == Materiale.IdInsulTipoPreferito.IntegerValue)

                            string cbm_nome_totale = cbm.Name; // nel name del combo box member abbiamo concatenato l id e lo spessore (ma perche c è lo spessore nel name? non leggevamo lo spessore dal parametro di tipo dell isoalnte?)
                            int indice_ = cbm_nome_totale.IndexOf("_");
                            int cbm_id = Int32.Parse(cbm_nome_totale.Substring(0, indice_));
                            if (cbm_id == Materiale.IdInsulTipoPreferito.IntegerValue)
                            {
                                //sono state create due classi diverse, bisogna collegarle
                                cboMateriali.SelectedIndex = i;
                            }

                            i++;

                        }



                    }
                }
            }
        }

        private void cboMateriali_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void buttScegliMateriale_Click(object sender, RoutedEventArgs e)
        {
            ImpostaMateriale();
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMateriali.Items.Count > 1) // da sistemare, perche appena all inizio aggiungo la stringa "caricare la libreria prima di scegliere il materiae" il comcbobox si aggiorna e parte sto metodo, ma se non ho ancora le famiglie non voglio che mi imposti il materiale!
                                              // serve qualcosa di piu intelligente , il etodo ceh serve anche altrove che controlla se ho caricato, magari anceh solo con un booleano? o controlare velocemente tra i tip se c è QUEL PARAMETRO NASCOSTO CHE DOBBIAMO CREARE
            {
                ImpostaMateriale();
            }
        }

        private void ImpostaMateriale()
        {
            Materiale obj = cboMateriali.SelectedItem as Materiale;
            Materiale.IdInsulTipoPreferito = obj.ID;
            Materiale.SpessoreIsolante = obj.Spessore;

            //combobox wpf -> combobox ribbon
            if (App.comboboxMembers_ribbon != null) //la prima volta che pocarico la libreria parte uesto ma non ho ancora scritto comboboxMembers_ribbon quindi giusto che salti
            {
                foreach (ComboBoxMember cbm in App.comboboxMembers_ribbon)
                {
                    string cbm_nome_totale = cbm.Name; // nel name del combo box member abbiamo concatenato l id e lo spessore (ma perche c è lo spessore nel name? non leggevamo lo spessore dal parametro di tipo dell isoalnte?)
                    int indice_ = cbm_nome_totale.IndexOf("_");
                    int cbm_id = Int32.Parse(cbm_nome_totale.Substring(0, indice_));
                    if (cbm_id == Materiale.IdInsulTipoPreferito.IntegerValue)
                    {
                        App.comboMat.Current = cbm;
                    }
                }
            }

        }

        public void Dispose()
        {
            //throw new NotImplementedException(); //secondo me va tolto, penso che il dispose serva per cancellare dalla memroai in modo piu o meno sicuro le variabili che non stiamo piu usando, nello specifico noi usimo "using" per "bloccare" le variabili dentr forse anche temporalmente visto che la finestra si chiude da sola...BHOHHHH
        }
    }
}
