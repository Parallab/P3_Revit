using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{
	[Transaction(TransactionMode.Manual)]
	class MigraAreaIsolamento : IExternalCommand
	{
		public static bool parPresenti = false;
		public static bool isolamentipresenti = false;

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;
			Application app = uiApp.Application;

			Supporto.AggiornaDoc(doc);

			using (var t = new Transaction(doc, "Proj_Info_Scrivi_Parametri"))
			{

				ControllaParametriSeEsistenti(doc, app);

				if (parPresenti)
				{
					t.Start();
					MigraParaetriIsolamento(doc);
					t.Commit();
				}
			}

			return Result.Succeeded;
		}


		static public bool ImportaParametriIsolamento(Document _doc, Application _app)
		{
			bool output = false;


			CategorySet categorySet = _app.Create.NewCategorySet();
			categorySet.Insert(_doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctInsulations));

			string originalFile = _app.SharedParametersFilename;

			//RISOLVERE E TROVARE IL MODO PER PRENDERSELO DA VISUAL STUDIO\
			string tempfie = @"C:\Users\Simone Maioli\Desktop\17017_ParamCondivisi.txt";

			try
			{
				_app.SharedParametersFilename = tempfie;
				DefinitionFile SharedParameterFile = _app.OpenSharedParameterFile();
				foreach (DefinitionGroup dg in SharedParameterFile.Groups)
				{
					if (dg.Name == "Materiale riciclato")
					{
						ExternalDefinition externalDefinitionS_App = dg.Definitions.get_Item("P3_Sup_S.app_dyn") as ExternalDefinition;

						using (Transaction t = new Transaction(_doc, "Aggiungi parametro P3"))
						{
							t.Start();
							InstanceBinding newIB = _app.Create.NewInstanceBinding(categorySet);
						// c'è dal 2022...forse dovrei fare Rel_21 da sola? o solo Rel_25 da sola che in futuro ci saranno altri cambiamenti?
						//https://www.revitapidocs.com/2022/a2fe7c6f-e5e2-bafe-23c8-819ba6a6c9b9.htm

							//_doc.ParameterBindings.Insert(externalDefinitionS_App, newIB, BuiltInParameterGroup.PG_AREA); //non va piu bene in r2025
							//Supporto.AggiungiParametroProgetto(_doc, externalDefinitionS_App, newIB, BuiltInParameterGroup.PG_AREA);
							Supporto.AggiungiParametroProgetto(_doc, externalDefinitionS_App, newIB, Supporto.BuiltInParameterGroup_OR_GroupTypeId.AREA);
							t.Commit();
						}
						output = true;
					}
				}
			}
			catch (Exception ex)
			{
				DebugUtils.PrintExceptionInfo(ex);
				output = false;
			}
			finally
			{
				//reset alla fine il fileoriginale
				_app.SharedParametersFilename = originalFile;
			}
			return output;
		}

		static public void MigraParaetriIsolamento(Document _doc)
		{
			Category ductCat = _doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves);
			Category fittCat = _doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting);

			int i_f = 0;
			int i_c = 0;
			int i = 0;

			//lista di isolanti contententi nome P3 
			IList<Element> coll = new FilteredElementCollector(_doc).OfClass(typeof(DuctInsulation)).ToElements().Where(x => x.Name.Contains("P3")).ToList();
			//List<Element> P3Insulation = (List<Element>)new FilteredElementCollector(_doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElements().Where(x => x.Name.Contains("P3"));

			foreach (Element el in coll)
			{
				try
				{
					InsulationLiningBase insul = el as InsulationLiningBase;
					Element host = _doc.GetElement(insul.HostElementId);
					i++;


					if (host.Category.Id == fittCat.Id)
					{
						double fittArea = host.LookupParameter("P3_Sup_S.app").AsDouble();
						insul.LookupParameter("P3_Sup_S.app_dyn").Set(fittArea);
						i_f++;
					}

					if (host.Category.Id == ductCat.Id)
					{
						Parameter ductArea = insul.get_Parameter(BuiltInParameter.RBS_CURVE_SURFACE_AREA);
						insul.LookupParameter("P3_Sup_S.app_dyn").Set(ductArea.AsDouble());
						i_c++;
					}
				}
				catch (Exception ex)
				{
					DebugUtils.PrintExceptionInfo(ex);
				}
			}

			//System.Windows.MessageBox.Show("È stato migrato il parametro di area a " + i_c + " isolamenti di condotti" + System.Environment.NewLine + "e " + i_f + " isolamenti di raccordi", "numero di isolamenti di raccordo e condotti");
		}

		static public void ControllaParametriSeEsistenti(Document doc, Application app)
		{

			// come verificare se il parametro è dentro il binding...definition
			// doc.ParameterBindings.Contains();
			IList<Element> dicoll = new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation)).ToElements();
			if (dicoll.Count > 0)
			{
				isolamentipresenti = true;
				Element di = dicoll[0];
				Parameter sup_dyn = di.LookupParameter("P3_Sup_S.app_dyn");
				if (sup_dyn == null)
				{
					TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
					td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdParametrononEsistente;
					td.MainContent = P3Ribbon.Resources.Lang.lang.taskdParametroIsolanteInserire;
					td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;

					TaskDialogResult result = td.Show();
					if (result == TaskDialogResult.Yes)
					{
						parPresenti = ImportaParametriIsolamento(doc, app);
					}
				}
				else
				{
					parPresenti = true;
				}
			}
			else
			{
				TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
				td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdIsolMancanti;
				td.MainContent = P3Ribbon.Resources.Lang.lang.taskdNonCiSonoIsol;
				TaskDialogResult result = td.Show();
			}
		}

	}

}

