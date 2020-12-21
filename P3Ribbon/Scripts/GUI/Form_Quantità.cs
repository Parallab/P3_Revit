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

namespace P3Ribbon.Scripts.GUI
{
    public partial class Form_Quantità : System.Windows.Forms.Form
    {
        private Document m_doc;
        private Application m_app;
        private UIDocument m_uidoc;

        List<Materiale> listaQuantità = new List<Materiale>();

        public Form_Quantità(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            m_uidoc= uiApp.ActiveUIDocument;
            m_doc = m_uidoc.Document;
            m_app = uiApp.Application;
            using (var t = new Transaction(m_doc, "Calcolo area"))
            {
                t.Start();
                MigraAreaIsolamento.ControllaParametriSeEsistenti(m_doc, m_app);
                if (MigraAreaIsolamento.parPresenti == true)
                {
                    MigraAreaIsolamento.MigraParaetriIsolamento(m_doc);
                }
                t.Commit();
            }
            
            LeggoAbacoQuantità();
            InitializeComponent();
            //mi serve il peso specifico per ottenere i Kg??
            
        }
     
        private void LeggoAbacoQuantità()
        {
            ViewSchedule AbacoQuantità = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name  == "P3 - Duct Insulation Schedule - DYNAMO") as ViewSchedule;

            TableData table = AbacoQuantità.GetTableData();
            TableSectionData section = table.GetSectionData(SectionType.Body);
            int nRows = section.NumberOfRows;
            int nColumns = section.NumberOfColumns;

            ////mi conviene farlo con i dizionari?? key duplicate
            //Dictionary<string, string> MaterialiPesi = new Dictionary<string, string>();
           
            int ir = 0;
            int cr = nColumns;

            List<List<string>> scheduleData = new List<List<string>>();
            for (int i = 0; i < nRows; i++)
            {
                List<string> rowDatatemp = new List<string>();
                List<string> rowData = new List<string>();
              
                for (int j = 0; j < nColumns; j++)
                {

                    rowData.Add(AbacoQuantità.GetCellText(SectionType.Body, i, j));

                }
                    scheduleData.Add(rowData);

            }

           int iTipo = scheduleData[0].FindIndex(x => x == "Type");
           //int iPesoSchiuma = scheduleData[0].FindIndex(x => x == "Peso schiuma totale");
           //int iPesoPannelli =  scheduleData[0].FindIndex(x => x == "Peso pannello totale"); 
           //int iPesoMatRiciclato = scheduleData[0].FindIndex(x => x == "Peso materiale riciclato"); perchè mi da indice -1??

            int iPesoMatRiciclato = 0;
            for (int i = 0; i < scheduleData[0].Count ; i++)
            {
                if (scheduleData[0][i] == "Peso materiale riciclato")
                {
                    iPesoMatRiciclato = i;
                    break;
                }
            }
          
            
            
            for (int i_r =3; i_r < scheduleData.Count; i_r++)
            {
                List<string> riga = scheduleData[i_r];

                listaQuantità.Add(new Materiale() {name = riga[iTipo], peso = $"{riga[iPesoMatRiciclato]}"});
            }

            foreach (var item in listaQuantità)
            {

            }
        }

        private void Form_Quantità_Load(object sender, EventArgs e)
        {
            string nomeprecedente = "";
            int n = 0;
            foreach (var Materiale in listaQuantità)
            {
                if (int.TryParse(Materiale.name, out n))
                {
                    AbacoQuantità.Rows.Add(nomeprecedente, Materiale.peso);
                }
                nomeprecedente = Materiale.name;

            }
               

        }

        private void butt_DettagliQuantità_Click(object sender, EventArgs e)
        {
            this.Close();

            ViewSchedule viewSchedule = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault
                (x => x.Name == "P3 - Duct Insulation Schedule - DYNAMO") as ViewSchedule;
           
            m_uidoc.ActiveView = viewSchedule;
            

        }
    }

}
