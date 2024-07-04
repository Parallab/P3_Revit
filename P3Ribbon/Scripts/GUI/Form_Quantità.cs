using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
			m_uidoc = uiApp.ActiveUIDocument;
			m_doc = m_uidoc.Document;
			m_app = uiApp.Application;
			using (var t = new Transaction(m_doc, "Calcolo area"))
			{
				t.Start();

				MigraAreaIsolamento.MigraParaetriIsolamento(m_doc);

				t.Commit();
			}

			LeggoAbacoQuantità();
			InitializeComponent();
		}

		private void LeggoAbacoQuantità()
		{
			string nome_abaco = "P3 - Duct Insulation Recycled Schedule - PLUGIN - ITA";
			//if (App.lingua_plugin == App.Lingua.ITA)
			//{
			//    nome_abaco += " - ITA";
			//}
			//else
			//{
			//    nome_abaco += " - ENG";
			//}
			//

			ViewSchedule AbacoQuantità = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.LookupParameter("P3_Nome_i").AsString() == nome_abaco) as ViewSchedule;

			//filtro per nome dell'abaco e non per il parametro nascosto
			//ViewSchedule AbacoQuantità = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name  == "P3 - Duct Insulation Schedule - PLUGIN - ITA") as ViewSchedule;

			TableData table = AbacoQuantità.GetTableData();
			TableSectionData section = table.GetSectionData(SectionType.Body);
			int nRows = section.NumberOfRows;
			int nColumns = section.NumberOfColumns;

			//int ir = 0;
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

			int iTipo = scheduleData[0].FindIndex(x => x == "Materiale");

			int iPesoMatRiciclato = 0;
			for (int i = 0; i < scheduleData[0].Count; i++)
			{
				if (scheduleData[0][i] == "Peso materiale riciclato")
				{
					iPesoMatRiciclato = i;
					break;
				}
			}

			for (int i_r = 3; i_r < scheduleData.Count; i_r++)
			{
				List<string> riga = scheduleData[i_r];

				listaQuantità.Add(new Materiale() { name = riga[iTipo], peso = $"{riga[iPesoMatRiciclato]}" });
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

			//ViewSchedule viewSchedule = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault
			//    //filtro per nome dell'abaco e non per il parametro nascosto
			//    (x => x.Name == "P3 - Duct Insulation Schedule - PLUGIN") as ViewSchedule;

			string nome_abaco = "P3 - Duct Insulation Recycled Schedule - PLUGIN";
			if (App.lingua_plugin == App.Lingua.ITA)
			{
				nome_abaco += " - ITA";
			}
			else
			{
				nome_abaco += " - ENG";
			}
			ViewSchedule viewSchedule = new FilteredElementCollector(m_doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.LookupParameter("P3_Nome_i").AsString() == nome_abaco) as ViewSchedule;


			m_uidoc.ActiveView = viewSchedule;


		}

	}

}
