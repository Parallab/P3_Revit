using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class CambiaMateriale : IExternalCommand
    {
		string msg = "Seleziona i canali a cui cambiare isolante";
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;
			Application app = uiApp.Application;

			ISelectionFilter selFilterdc = new FiltraCondotti();
			using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
			{

				t.Start();

				foreach (ElementId id in Seleziona(uiDoc, msg, selFilterdc))
				{

					doc.GetElement(id).get_Parameter(BuiltInParameter.INSULATION_WIDTH).Set(Scripts.Materiale.SpessoreIsolante);
					doc.GetElement(id).ChangeTypeId(Scripts.Materiale.IdInsulTipoPreferito);

				}

				t.Commit();
			}


			return Result.Succeeded;
		}

		public class FiltraCondotti : ISelectionFilter
		{
			public bool AllowElement(Element element)
			{

				if (element.Category.Name == "Condotto")
				{
					return true;
				}
				return false;
			}

			public bool AllowReference(Reference refer, XYZ point)
			{
				return false;
			}
		}


		public static IList<ElementId> Seleziona(UIDocument uiDoc, string msg, ISelectionFilter _filtro)
		{
			//Preparo una lista vuota
			IList<ElementId> sel = new List<ElementId>();
			//Prima guarda se ho qualcosa di già selezionato
			ICollection<ElementId> sel_c = new List<ElementId>();
			sel_c = uiDoc.Selection.GetElementIds();
			if (sel_c.Any())
			{
				sel = sel_c.ToList();
			}
			//altrimenti richiedo di selezionare da schermo
			else
			{
				sel = uiDoc.Selection.PickElementsByRectangle(_filtro, msg).Select(e => e.Id).ToList();
			}
			return sel;
		}
	}
}
