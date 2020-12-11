using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.Form
{
    public partial class Form_Quantità : System.Windows.Forms.Form
    {
        private Document m_doc;
        private Application m_app;
        private UIDocument m_uidoc;

        public Form_Quantità(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            m_uidoc= uiApp.ActiveUIDocument;
            m_doc = m_uidoc.Document;
            m_app = uiApp.Application;
            using (var t = new Transaction(m_doc, "Proj_Info_Scrivi_Parametri"))
            {
                t.Start();
                Migra_AreaIsolamento.Controlla_Parametri(m_doc, m_app);
                if (Migra_AreaIsolamento.parametri_presenti == true)
                {
                    Migra_AreaIsolamento.MigraParaetriIsolamento(m_doc);
                }
                t.Commit();
            }
            
            LeggoAbacoQuantità();
            InitializeComponent();
            //mi serve il peso specifico per ottenere i Kg??
            
        }

        private void Form_Quantità_Load(object sender, EventArgs e)
        {

        }

        private void AbacoQuantità_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void LeggoAbacoQuantità()
        {
            ViewSchedule viewSchedule = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name == "P3 - Duct Insulation Schedule - DYNAMO") as ViewSchedule;

            TableData table = viewSchedule.GetTableData();
            TableSectionData section = table.GetSectionData(SectionType.Body);
            int nRows = section.NumberOfRows;
            int nColumns = section.NumberOfColumns;
            //mi conviene farlo con i dizionari?? key duplicate
            //Dictionary<string, string> MaterialiPesi = new Dictionary<string, string>();
            List<MaterialeIsolante> list = new List<MaterialeIsolante>();
            string tipomateriale = "";


            List<List<string>> rowData = new List<List<string>>();


            List<List<string>> scheduleData = new List<List<string>>();
            for (int i_r = 0; i_r < nRows; i_r++)
            { 
                 

                for (int j = 0; j < nColumns; j++)
                {
                 
                    rowData.Add(new viewSchedule.GetCellText(SectionType.Body, i_r, j));
                    
                }
                scheduleData.Add(rowData);
            }

           int iTipo = scheduleData[0].FindIndex(x => x.Contains("Type"));
           int iPesoSchiuma = scheduleData[0].FindIndex(x => x.Contains("Peso schiuma totale"));
           int iPesoPannelli =  scheduleData[0].FindIndex(x => x.Contains("Peso pannello totale")); 
           int iPesoMatRiciclato = scheduleData[0].FindIndex(x => x.Contains("Peso materiale riciclato"));

            for (int i_r = 2; i_r < scheduleData.Count; i_r++)
            {
                List<string> riga = scheduleData[i_r];

               if( riga[iTipo] != tipomateriale);
                {
                    list.Add(new MaterialeIsolante() {Name = riga[iTipo], Peso = $"{riga[iPesoMatRiciclato]} {riga[iPesoPannelli]} {riga[iPesoMatRiciclato]}" });
                }
                
            }
           
               // MaterialiPesi.Add(riga[iTipo], $"{riga[iPesoMatRiciclato]} {riga[iPesoPannelli]} {riga[iPesoMatRiciclato]}");
            
        }

        private void butt_DettagliQuantità_Click(object sender, EventArgs e)
        {
            ViewSchedule viewSchedule = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name == "P3 - Duct Insulation Schedule - DYNAMO") as ViewSchedule;
            this.Close();
            m_uidoc.ActiveView = viewSchedule;

        }
    }

}
