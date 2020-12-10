using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
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
        private List<ElementId> P3InsulationTypeIds;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;
            Element Insualtion;
            ElementId CondottoId;
            List<ElementId> InsulationTypeIds;
            double spiso = Materiale.SpessoreIsolante;

        ISelectionFilter selFilterdc = new FiltraCondotti();
            IList<ElementId> DuctsId = Seleziona(uiDoc, msg, selFilterdc);

            InsulationTypeIds = (List<ElementId>)new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds();
            //IList<Element> Dcinsul = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctInsulations).WhereElementIsNotElementType().ToElements();
            IList<Element> Dcinsul = new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation)).WhereElementIsElementType().ToElements();
            //ICollection<ElementId> DcinsulId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctInsulations).WhereElementIsNotElementType().ToElementIds();


            using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
            {

                t.Start();
                //potrei fare il contrario andandomi a prendere l'host  ma non riesco ad otterene la FamilyInstance dell'isol
                foreach ( var insId in InsulationTypeIds)
                {
                    CondottoId = (doc.GetElement(insId) as DuctInsulation).HostElementId;
                    foreach(var Dcid in DuctsId)
                    {
                        if (CondottoId.IntegerValue == Dcid.IntegerValue)
                        {
           
                            try
                            {
                                doc.GetElement(insId).get_Parameter(BuiltInParameter.RBS_INSULATION_THICKNESS_FOR_DUCT).Set(spiso);
                                doc.GetElement(insId).ChangeTypeId(Scripts.Materiale.IdInsulTipoPreferito);
                            }
                            catch(System.Exception ex)
                            {
                                TaskDialog.Show("Exception", ex.ToString());
                            }

                        }

                    }
                }
                  
                    t.Commit();
                    return Result.Succeeded;
                
            }
        }

        public ICollection<Element> CollectFamilyInstances(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            ElementCategoryFilter fi = new ElementCategoryFilter(BuiltInCategory.OST_DuctInsulations, true);

            ICollection<Element> collection = collector.OfClass(typeof(FamilyInstance)).WherePasses(fi).ToElements();

            return collection;
        }


        public class FiltraCondotti : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {

                if (element.Category.Name == "Condotto" || element.Category.Name == "Raccordo")
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
