using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{
	[Transaction(TransactionMode.Manual)]
	class TrasferisciStandard : IExternalCommand
	{

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;
			Application app = uiApp.Application;

			Supporto.AggiornaDoc(doc);

			TrasferisciTipiDoc(app, doc);

			return Result.Succeeded;
		}
		public static void TrasferisciTipiDoc(Application app, Document doc)
		{
			// guardo i tipi nel documento template
			ICollection<ElementId> IdTipiDaCopiare = new Collection<ElementId>();
			Document docSource = null;
			//string path_rte = "P3 - Duct system template19.rte";
			// 2024/02/06
			string nome_rte ="P3 - Duct system template" + app.VersionNumber.Substring(2, 2) + ".rte";
			string path_rte = Supporto.TrovaPercorsoRisorsaInstaller(nome_rte);
			//vorrei già provare ad aprire..
			if (!File.Exists(path_rte))
			{
				TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
				//file non trovati
				//td.MainInstruction = Resources.Lang.lang.taskdParametriNonInseriti;
				td.MainContent = Resources.Lang.lang.taskdErroreFileMancante+ " " + path_rte;
				//td.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.No;

				TaskDialogResult result = td.Show();
				return;
			}


			//try
			//{
				docSource = app.OpenDocumentFile(Supporto.TrovaPercorsoRisorsaInstaller(path_rte));
			//}
			//catch
			//{
			//	//finestra di avviso?
			//	try
			//	{
			//		//se non trovo vorrei provare ad aprire l'ultima versione?
			//	}
			//	catch
			//	{
			//		//altro avviso
			//	}
			//}



			//IMPORTO INSUL E TIPI DI CONDOTTI 
			List<string> nomiTipiPresenti = new List<string>();
			FilteredElementCollector collTipiPresenti = new FilteredElementCollector(doc).WherePasses(Supporto.CatFilterDuctAndInsul).WhereElementIsElementType();

			//guardo tutti i tipi che mi interessamno presenti nel mio doc
			foreach (ElementType type in collTipiPresenti)
			{
				try
				{
					string nome = type.LookupParameter("P3_Nome").AsString();
					if (nome.StartsWith("P3"))
					{
						nomiTipiPresenti.Add(nome);
					}
				}
				catch
				{

				}
			}

			//insul e condotti presenti nella risorsa
			FilteredElementCollector collTipiRisorsa = new FilteredElementCollector(docSource).WherePasses(Supporto.CatFilterDuctAndInsul).WhereElementIsElementType();
			CopyPasteOptions option = new CopyPasteOptions();
			option.SetDuplicateTypeNamesHandler(new HideAndAcceptDuplicateTypeNamesHandler());

			foreach (ElementType type in collTipiRisorsa)
			{
				try
				{
					string nome = type.LookupParameter("P3_Nome").AsString();
					if (nome.StartsWith("P3"))
					{
						// contollRE SE ESISTE NEL DOC
						if (!(nomiTipiPresenti.Contains(nome)))
						{
							IdTipiDaCopiare.Add(type.Id);
						}

					}
				}
				catch
				{

				}
			}

			//COLLETTORE STAFFE 
			//staffe presenti nel mio  documento
			FilteredElementCollector collStaffeDoc = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).WhereElementIsElementType();
			foreach (var type in collStaffeDoc)
			{
				try
				{
					if (type.LookupParameter("P3_Nome").AsString() == "P3_DuctHanger")
					{
						nomiTipiPresenti.Add(type.Name);
					}
				}
				catch
				{
				}

			}

			//staffe presenti nella risorsa
			FilteredElementCollector collStaffeRisorsa = new FilteredElementCollector(docSource).OfCategory(BuiltInCategory.OST_SpecialityEquipment).WhereElementIsElementType();
			foreach (var type in collStaffeRisorsa)
			{

				//string typeName = type.Name;
				string typeName = type.LookupParameter("P3_Nome").AsString();
				if (typeName == "P3_DuctHanger")
				{
					if (!(nomiTipiPresenti.Contains(typeName)))
					{
						IdTipiDaCopiare.Add(type.Id);

					}
				}

			}


			//IMPORTO ABACHI
			//abachi presenti nel documento corrente
			IList<Element> collAbachiPresenti = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().ToElements();
			List<string> nomiAbachiPresenti = new List<string>();

			foreach (Element schedule in collAbachiPresenti)
			{
				try
				{
					string nome = schedule.LookupParameter("P3_Nome_i").AsString();

					if (nome.StartsWith("P3"))
					{
						nomiAbachiPresenti.Add(nome);

					}
				}
				catch
				{

				}
			}

			//abachi presenti nel documento risorsa
			ICollection<ElementId> collAbachiRisorsa = new Collection<ElementId>();
			IList<Element> AbachiRisorsa = new FilteredElementCollector(docSource).OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().ToElements();

			foreach (Element abaco in AbachiRisorsa)
			{
				string nome = abaco.Name; //non voglio più il nome,

				//Parameter param = abaco.LookupParameter("P3_Nome_i");
				//if (param != null)
				//{
				//string nome = param.AsString();
				if (nome.StartsWith("P3"))
				{
					// contollRE SE ESISTE NEL DOC
					if (!(nomiAbachiPresenti.Contains(nome)))
					{
						collAbachiRisorsa.Add(abaco.Id);
					}

				}
				//}
			}
			try
			{
				ICollection<ElementId> ids = ICollectionIds_Estendi(IdTipiDaCopiare, collAbachiRisorsa);
				ElementTransformUtils.CopyElements(docSource, ids, doc, Transform.Identity, option);
				ids.Clear();
			}
			catch (Exception ex)
			{

			}
			collAbachiRisorsa.Clear();
			IdTipiDaCopiare.Clear();
			docSource.Close(false);
		}

		//evito i taskdialog per accettare i materiali duplicati
		class HideAndAcceptDuplicateTypeNamesHandler : IDuplicateTypeNamesHandler
		{
			public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
			{
				return DuplicateTypeAction.UseDestinationTypes;
				//e se volessi modificare il nome??
			}
		}


		private static ICollection<ElementId> ICollectionIds_Estendi(ICollection<ElementId> coll1, ICollection<ElementId> coll2)
		{
			ICollection<ElementId> unione = coll1;
			foreach (ElementId id in coll2)
			{
				unione.Add(id);
			}

			return unione;
		}
	}
}
