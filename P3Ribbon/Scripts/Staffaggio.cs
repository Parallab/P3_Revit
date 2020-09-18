using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace P3Ribbon.Scripts
{
	[Transaction(TransactionMode.Manual)]
	class Staffaggio : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;

			Seleziona_condotti(doc, uiDoc);
			

			return Result.Succeeded;
		}

		public void Seleziona_condotti(Document doc, UIDocument uiDoc)
		{

			TaskDialog td = new TaskDialog("P3 staffaggio canali");
			td.MainInstruction = "Selezionare la modalità di input";
			string a1 = "Seleziona tutti i condotti all'interno del progetto Revit corrente";
			string b1 = "Selezione manuale da schermo";
			td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, a1);
			td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, b1);
			TaskDialogResult result = td.Show();


			if (result == TaskDialogResult.CommandLink1)
			{
				IList<Element> du_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).ToElements();
				//lista vuota
				List<ElementId> newsel = new List<ElementId>();
				foreach (Element el in du_coll)
				{
					newsel.Append(el.Id);
				}
			}
			else
			{

				IList<Element> dulist = SelSoloCondotriDaFinestra(uiDoc);
				System.Windows.MessageBox.Show("Hai selezionato " + dulist.Count() +" condotti");
			}

		}
		public static IList<Element> SelSoloCondotriDaFinestra(UIDocument uidoc)
		{
			ISelectionFilter selFilterdc = new FiltraCondotti();

			IList<Element> du_coll = uidoc.Selection.PickElementsByRectangle(selFilterdc, "seleziona solo i condotti") as IList<Element>;
			return du_coll;
			int i = du_coll.Count();
		}

		public class FiltraCondotti : ISelectionFilter
		{
			 
			public bool  AllowElement(Element element)
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


	}
}
