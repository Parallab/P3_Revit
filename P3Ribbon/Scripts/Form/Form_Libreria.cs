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

namespace P3Ribbon.Scripts.Form
{
    public partial class Form_Libreria : System.Windows.Forms.Form
    {
        List<MaterialeIsolante> list = new List<MaterialeIsolante>();
        public static System.Windows.Forms.ComboBox _cboMateriali;
        private Document m_doc;
        private Application m_app;
        public Form_Libreria(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            m_app = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_doc = uiDoc.Document;

            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void B_Continua(object sender, EventArgs e)
        {
            TrasferisciStandard.TrasferisciTipiDoc(m_app, m_doc);

        }

        private void Form_Libreria_Load(object sender, EventArgs e)
        {
            list = Materiale.PreAggiorna(m_doc);
            cboMateriali.DataSource = list;
            cboMateriali.DisplayMember = "name";
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

     
    }

}
