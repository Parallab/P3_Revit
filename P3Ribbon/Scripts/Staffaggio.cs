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
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace P3Ribbon.Scripts
{
	[Transaction(TransactionMode.Manual)]
	class Staffaggio : IExternalCommand
    {
		IList<Element> dclist = new List<Element>();
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;


			P3Ribbon.Scripts.Parametrigeometrici IParametri = new P3Ribbon.Scripts.Parametrigeometrici();
			IParametri.CalcolaPerimetro(doc, uiDoc);


			return Result.Succeeded;
		}


		
		public static IList<Element> Seleziona_condotti(Document doc, UIDocument uiDoc)
		{

			TaskDialog td =  new TaskDialog("P3 staffaggio canali");
			td.MainInstruction = "Selezionare la modalità di input";
			string a1 = "Seleziona tutti i condotti all'interno del progetto Revit corrente";
			string b1 = "Selezione manuale da schermo";
			td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, a1);
			td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, b1);
			TaskDialogResult result = td.Show();

			//Seleziono tutti i condotti nel progetto corrente
			if (result == TaskDialogResult.CommandLink1)
			{
				int i = 0;
				IList<Element> dc_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).WhereElementIsNotElementType().ToElements();
				//Creo una lista vuota
				List<Element> dclist1 = new List<Element>();
				foreach (Element el in dc_coll)
				{
					i++;
					dclist1.Add(el);
				}
				System.Windows.MessageBox.Show("Nel progetto corrente ci sono " + i + " condotti");
				return dclist1;
                
			}
			//Seleziono i condotti da finestra 
			else
			{
				IList<Element> dclist2 = SelSoloCondottiDaFinestra(uiDoc);
				System.Windows.MessageBox.Show("Hai selezionato " + dclist2.Count() +" condotti");
				return dclist2;
			}
			
		}
		#region  Funzione che seleziona solo i condotti da finestra con un selection filter     
		public static IList<Element> SelSoloCondottiDaFinestra(UIDocument uidoc)
		{
			ISelectionFilter selFilterdc = new FiltraCondotti();

			IList<Element> dc_coll = uidoc.Selection.PickElementsByRectangle(selFilterdc, "seleziona solo i condotti");
			return dc_coll;
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
        #endregion


		//filtro elementi verticali e lugni < di 250 mm
        public static IList<Element> FiltraCondottiCortiVert(Document doc, UIDocument uiDoc)
        {
			IList<Element> dclist = new List<Element>();
			dclist = (List<Element>)Seleziona_condotti(doc, uiDoc);
			int i = 0;
			XYZ dir = XYZ.Zero;
			foreach (Element dc in dclist.ToList())
			{
				
				//Ricavo il parametro di lunghezza per ogni condotto
				double dc_lungh_IM = dc.LookupParameter("Lunghezza").AsDouble();
				double dc_lungh = UnitUtils.ConvertFromInternalUnits(dc_lungh_IM, DisplayUnitType.DUT_MILLIMETERS);
				//Leggo le coordinate dei punti inizali e finali del condotto e calcolo la direzione
				LocationCurve Lp = dc.Location as LocationCurve;
				Curve c = Lp.Curve;
				XYZ pt1 = c.GetEndPoint(0);
				XYZ pt2 = c.GetEndPoint(1);
				dir = pt2.Subtract(pt1).Normalize();

				//condizione se verticale o minore di 250 mm
				if (dir.Z == 1 || dc_lungh < 250)
				{
					dclist.Remove(dc);
					i++;
				}
			}
			TaskDialog td = new TaskDialog("P3 staffaggio canali");
			td.MainInstruction = "sono stati indivituati " + i + " canali verticali o troppo corti";
			TaskDialogResult result = td.Show();
			return dclist;
		}

		//Classe per leggere parametri geometrici e calcolare il perimetro 
		
	}
	public class Parametrigeometrici
	{
		public void CalcolaPerimetro(Document doc, UIDocument uiDoc)
		{
			IList<Element> dclist = Staffaggio.FiltraCondottiCortiVert(doc, uiDoc);
			List<double> dc_spiso_list = new List<double>();
			List<double> dc_largh_list = new List<double>();
			List<double> dc_alt_list = new List<double>();
			List<double> dc_per_list = new List<double>();


			foreach (Element dc in dclist)
			{
				double per;

				double dc_spiso = CalcolaSpessoreIsolamento(dc);
				dc_spiso_list.Add(dc_spiso);
				double dc_largh = CalcolaLarghezza(dc);
				dc_largh_list.Add(dc_largh);
				double dc_alt = CalcolaAltezza(dc);
				dc_alt_list.Add(dc_alt);

				per = (dc_largh + dc_alt);
				dc_per_list.Add(per);
			}

		}

		#region funzioni che mi calcolano i parametri nel sistema metrico 
		public double CalcolaSpessoreIsolamento(Element dc)
		{
			double dc_spiso_IM = dc.LookupParameter("Spessore isolamento").AsDouble() * 2;
			double dc_spiso = UnitUtils.ConvertFromInternalUnits(dc_spiso_IM, DisplayUnitType.DUT_MILLIMETERS);
			return dc_spiso;
		}
		public double CalcolaLarghezza(Element dc)
		{
			double dc_largh_IM = dc.LookupParameter("Larghezza").AsDouble() * 2;
			double dc_largh = UnitUtils.ConvertFromInternalUnits(dc_largh_IM, DisplayUnitType.DUT_MILLIMETERS);
			return dc_largh;
		}
		public double CalcolaAltezza(Element dc)
		{
			double dc_alt_IM = dc.LookupParameter("Altezza").AsDouble() * 2;
			double dc_alt = UnitUtils.ConvertFromInternalUnits(dc_alt_IM, DisplayUnitType.DUT_MILLIMETERS);
			return dc_alt;
		}

		#endregion

	}
}
