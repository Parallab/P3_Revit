using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{
	[Transaction(TransactionMode.Manual)]
	class CreaCanaleDinamico : IExternalCommand
	{
		//qua si potrebbe creare una funzione di base con diversi input ivnece di copiare e ripetere tutto...

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;
			Application app = uiApp.Application;
			Supporto.AggiornaDoc(doc);

			//non era aggiornato? ma andava nel video? forse problema git..ah no il p3 nome era rimasto uguale!
			if (Supporto.ControllaTipiP3Presenti("P3 - Preinsulated panels system - Dynamic -  ε 0.03mm") || Supporto.ControllaTipiP3Presenti("P3ductal - Dynamic -  ε 0.03mm"))
			{
				string nome = App.rbbCboMateriali.Current.Name;
				Materiale.AggiornaTendinaRibbon(nome);
				try
				{
					//nome era sbagliato? 1/3/24 si non era stato aggiornato il p3 nome..
					DuctType ductDynamicType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.LookupParameter("P3_Nome").AsString() == "P3ductal - Dynamic -  ε 0.03mm") as DuctType;
					//provo il nome vecchio:
					if (ductDynamicType is null)
						ductDynamicType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.LookupParameter("P3_Nome").AsString() == "P3 - Preinsulated panels system - Dynamic -  ε 0.03mm") as DuctType;

					//richiedo che sia il prossimo tipo di default

					//devo guardare se c è il canale 500micron..
					//1/3/24
					if (Materiale.IdInsulTipoPreferito != null)
					{
						Element tipoPreferito = doc.GetElement(Materiale.IdInsulTipoPreferito);
						if (tipoPreferito != null)
						{
							if (tipoPreferito.LookupParameter("P3_Nome").AsString().Contains("19HV")) //se no leggere Alluminio esterno
							{
								ductDynamicType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.LookupParameter("P3_Nome").AsString() == "P3ductal - Dynamic -  500µm") as DuctType;
							}
						}
					}
					uiDoc.PostRequestForElementTypePlacement(ductDynamicType);
				}
				catch (Exception ex) { DebugUtils.PrintExceptionInfo(ex); }

				using (Transaction t = new Transaction(doc, "Crea un canale di tipo dinamico"))
				{

					t.Start();

					RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
					uiApp.PostCommand(cmdId);

					t.Commit();
				}
				return Result.Succeeded;
			}
			else
			{
				TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
				td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdTipiNonPresenti;
				td.MainContent = P3Ribbon.Resources.Lang.lang.taskdTipiCanaleCaricare;
				TaskDialogResult result = td.Show();

				GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
				using (wpf)
				{
					wpf.ShowDialog();
					Supporto.ChiudiFinestraCorrente(uiDoc);
				}
			}
			return Result.Cancelled;
		}


	}

	[Transaction(TransactionMode.Manual)]
	class CreaCanaleScarpette : IExternalCommand
	{

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;
			Application app = uiApp.Application;
			Supporto.AggiornaDoc(doc);

			//non era aggiornato? ma andava nel video? forse problema git..ah no il p3 nome era rimasto uguale!
			if (Supporto.ControllaTipiP3Presenti("P3 - Preinsulated panels system - Tap -  ε 0.03mm") || Supporto.ControllaTipiP3Presenti("P3ductal - Dynamic -  ε 0.03mm"))
			{
				string nome = App.rbbCboMateriali.Current.Name;
				Materiale.AggiornaTendinaRibbon(nome);
				try
				{
					DuctType ductTapType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.LookupParameter("P3_Nome").AsString() == "P3ductal - Tap -  ε 0.03mm") as DuctType;
					//provo nome vecchio
					if (ductTapType is null)
						ductTapType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.LookupParameter("P3_Nome").AsString() == "P3 - Preinsulated panels system - Tap -  ε 0.03mm") as DuctType;

					//devo guardare se c è il canale 500micron..
					//1/3/24
					if (Materiale.IdInsulTipoPreferito != null)
					{
						Element tipoPreferito = doc.GetElement(Materiale.IdInsulTipoPreferito);
						if (tipoPreferito != null)
						{
							if (tipoPreferito.LookupParameter("P3_Nome").AsString().Contains("19HV")) //se no leggere Alluminio esterno
							{
								ductTapType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.LookupParameter("P3_Nome").AsString() == "P3ductal - Tap -  500µm") as DuctType;
							}
						}
					}



					uiDoc.PostRequestForElementTypePlacement(ductTapType);
				}
				catch (Exception ex) { DebugUtils.PrintExceptionInfo(ex); }
				//richiedo che sia il prossimo tipo di default

				using (Transaction t = new Transaction(doc, "Crea un canale di tipo scarpetta"))
				{
					t.Start();

					RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
					uiApp.PostCommand(cmdId);


					t.Commit();
				}
				return Result.Succeeded;
			}
			else
			{
				TaskDialog td = new TaskDialog("Errore");
				td.MainInstruction = "Tipi di canale non inseriti nel progetto";
				td.MainContent = "Canali P3 non inseriti nel progetto, caricare prima la libreria";
				TaskDialogResult result = td.Show();

				GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
				using (wpf)
				{
					wpf.ShowDialog();
					Supporto.ChiudiFinestraCorrente(uiDoc);
				}

			}
			return Result.Cancelled;
		}


	}
}





