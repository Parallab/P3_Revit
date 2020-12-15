using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using P3Ribbon.Scripts;
using Autodesk.Revit.UI.Events;

namespace P3Ribbon.Scripts.Form
{
    public partial class Form_Libreria : System.Windows.Forms.Form
    {
        List<MaterialeIsolante> list = new List<MaterialeIsolante>();
        public static System.Windows.Forms.ComboBox _cboMateriali;
        private Document m_doc;
        private Application m_app;
        private UIDocument m_UiDoc;
        UIApplication m_uiApp;
        public Form_Libreria(ExternalCommandData commandData)
        {
            m_uiApp = commandData.Application;
            m_app = m_uiApp.Application;
            m_UiDoc = m_uiApp.ActiveUIDocument;
            m_doc = m_UiDoc.Document;


            InitializeComponent();
            if (cboMateriali.Items.Count == 1) // piuttosto da vedere se
            {
                cboMateriali.SelectedIndex = 0;
                //cboMateriali. italicoooooo
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void B_Continua(object sender, EventArgs e)
        {

            m_app.DocumentOpened -= new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);

            // m_uiApp.DialogBoxShowing += new EventHandler(dismissFloorQuestion);
            TrasferisciStandard.TrasferisciTipiDoc(m_app, m_doc);
            Materiale.PreAggiorna(m_doc);


            Form_Libreria_Combobox_Aggiorna();
            //TEMP DA SISTEARE CON BOOLEANI
            try
            {
                App.comboMat.AddItems(Materiale.comboBoxMemberDatas);
            }
            catch
            { }
           // this.Close();

            m_doc.Regenerate();
            

        }
        private static void dismissFloorQuestion(object o, DialogBoxShowingEventArgs e)
        {

            TaskDialogShowingEventArgs t = e as TaskDialogShowingEventArgs;


            e.OverrideResult((int)TaskDialogResult.Ok);

        }


        private void Form_Libreria_Combobox_Aggiorna()
        {
            list = Materiale.PreAggiorna(m_doc);
            cboMateriali.DataSource = list;
            cboMateriali.DisplayMember = "name";
        }

        private void Form_Libreria_Load(object sender, EventArgs e)
        {
            Form_Libreria_Combobox_Aggiorna();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            // SISTEMARE
            try
            {
                //App.comboMat.Current =  //cerca quello con l id uguale a quello appena selezionato tra i combobox presenti
            }
            catch
            {

            }
        }

        private void impostazioni_Click(object sender, EventArgs e)
        {


        }

        private void ScegliMat_Click(object sender, EventArgs e)
        {
            MaterialeIsolante obj = cboMateriali.SelectedItem as MaterialeIsolante;
            Materiale.IdInsulTipoPreferito = obj.ID;
            Materiale.SpessoreIsolante = obj.Spessore;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

}
