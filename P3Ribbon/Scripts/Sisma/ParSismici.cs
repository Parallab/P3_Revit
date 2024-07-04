using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using P3Ribbon.Scripts;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon
{

	[Transaction(TransactionMode.Manual)]
	class ParSismici : IExternalCommand
	{
		public static int classe;
		public static int vita;
		public static int zona;

		//FORM//
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;
			Application app = uiApp.Application;

			Scripts.GUI.Wpf_ParamSismici frm = new Scripts.GUI.Wpf_ParamSismici();
			using (frm)
			{
				using (var t = new Transaction(doc, "GUI parametri sismici"))
				{

					if (Supporto.ControllaSePresentiParamSismici())
					{
						t.Start();
						if (!Supporto.ControllaSePresentiParamSismici())
						{
							Migra_Parametri_Presenti(doc);
						}
						frm.ShowDialog();
						//t.Commit();
						if (Scripts.GUI.Wpf_ParamSismici.ok_premuto == true)
						{
							ProjInfoImpostaParametri(classe, vita, zona, doc);
						}
						else
						{
							return Result.Cancelled;
						}
						t.Commit();
						return Result.Succeeded;
					}

					else
					{
						return Result.Cancelled;
					}

				}

			}


		}

		public static void ProjInfoImpostaParametri(int _classe, int _vita, int _zona, Document _doc)
		{
			if (Scripts.GUI.Wpf_ParamSismici.ok_premuto == true)
			{

				Element proj_info = new FilteredElementCollector(_doc).OfClass(typeof(ProjectInfo)).FirstElement();

				proj_info.LookupParameter("P3_InfoProg_ClasseUso").Set(_classe);
				//proj_info.LookupParameter("P3_InfoProg_Eng").Set(_eng);
				//proj_info.LookupParameter("P3_InfoProg_VitaNominale").Set(_vita);
				proj_info.LookupParameter("P3_InfoProg_ZonaSismica").Set(_zona);

			}

		}






		public static void Migra_Parametri_Presenti(Document doc)
		{

			Element proj_info = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).FirstElement();
			Parameter Cu = proj_info.LookupParameter("P3_InfoProg_ClasseUso");
			Parameter Zs = proj_info.LookupParameter("P3_InfoProg_ZonaSismica");

			Scripts.GUI.Wpf_ParamSismici.zona_wpf = Zs.AsInteger();
			Scripts.GUI.Wpf_ParamSismici.classe_wpf = Cu.AsInteger();

		}

	}
}
