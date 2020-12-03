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

namespace P3Ribbon.Scripts.Form
{
    public partial class Form_Libreria : System.Windows.Forms.Form
    {
        public static IList<ComboBoxMemberData> comboBoxMemberDatas = new List<ComboBoxMemberData>();
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

        public void Materiale_ComboBox_Aggiorna()
        {
            List<MaterialeIsolante> list = new List<MaterialeIsolante>();
            List<ElementId> P3InsulationTypeIds = new List<ElementId>();


            P3InsulationTypeIds = (List<ElementId>)new FilteredElementCollector(m_doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds();
            if (P3InsulationTypeIds.Count != 0)
            {
                double _spessore;
                int i = 0;

                foreach (ElementId id in P3InsulationTypeIds)
                {
                    Element el = m_doc.GetElement(id);
                    if (el.Name.Contains("P3"))
                    {
                        //i++; // "Combo item " + i
                        try
                        {
                            ComboBoxMemberData cmbInsualtionData = new ComboBoxMemberData(el.Id.ToString(), "" + el.Name);
                            comboBoxMemberDatas.Add(cmbInsualtionData);
                            _spessore = el.LookupParameter("P3_Insulation_Thickness").AsDouble();
                            list.Add(new MaterialeIsolante() { ID = el.Id, Name = el.Name, Spessore = _spessore });
                        }
                        catch
                        {

                        }
                    }
                }

                cboMateriali.DataSource = list;
            }
            else
            {
                cboMateriali.Items.Add("Libreria P3 non caricata");
            }
        }

        private void Form_Libreria_Load(object sender, EventArgs e)
        {
            Materiale_ComboBox_Aggiorna();
            cboMateriali.DisplayMember = "name";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MaterialeIsolante obj = cboMateriali.SelectedItem as MaterialeIsolante;

            WatcherUpdater.IdInsulTipoPreferito = obj.ID;
            WatcherUpdater.SpessoreIsolante = obj.Spessore;
            //App.comboMat.
            App.comboMat.AddItems(Scripts.Form.Form_Libreria.comboBoxMemberDatas); // SISTEMARE
            try
            {
                //App.comboMat.Current =  //cerca quello con l id uguale a quello appena selezionato tra i combobox presenti
            }
            catch
            {

            }
        }

        class MaterialeIsolante
        {
            public ElementId ID { get; set; }
            public string Name { get; set; }
            public double Spessore { get; set; }

        }
    }

}
