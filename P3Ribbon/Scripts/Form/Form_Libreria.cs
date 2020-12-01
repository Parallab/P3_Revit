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
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.Form
{
    public partial class Form_Libreria : System.Windows.Forms.Form
    {
        public Form_Libreria()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //TrasferisciStandard.TrasferisciTipiDoc(app, doc);
           // CreaCanale.temp_inizializza_isolante(doc);
        }

        private void Form_Libreria_Load(object sender, EventArgs e)
        {

            // sistemare porcoeido
            List<Materiale> list = new List<Materiale>();
            list.Add(new Materiale() { ID = "01", Name = "P3 - 150L31PLUS - 30 mm " });
            list.Add(new Materiale() { ID = "02", Name = "P3 - 15HL21PLUS - 20 mm" });
            list.Add(new Materiale() { ID = "03", Name = "P3 - 15HN21PLUS - 20 mm" });
            list.Add(new Materiale() { ID = "04", Name = "P3 - 150L31PLUS - 30 mm" });
            list.Add(new Materiale() { ID = "05", Name = "P3 - 15HR31PLUS - 30 mm" });

            cboMateriali.DataSource = list;
            cboMateriali.ValueMember = "ID";
            cboMateriali.DisplayMember = "Name";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    class Materiale
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class SupportoDoc 
    {
        public static Document doc;
        public static Application app;
    }
    
}
