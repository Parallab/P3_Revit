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

namespace P3Ribbon.Scripts.Form
{
    public partial class Form_Quantità : System.Windows.Forms.Form
    {
        private Document m_doc;
        private Application m_app;
        public Form_Quantità(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_doc = uiDoc.Document;

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

            List<List<string>> scheduleData = new List<List<string>>();
            for (int i = 0; i < nRows; i++)
            {
                List<string> rowData = new List<string>();

                for (int j = 0; j < nColumns; j++)
                {
                    rowData.Add(viewSchedule.GetCellText(SectionType.Body, i, j));
                }
                scheduleData.Add(rowData);
            }
        }


    }
}
