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
        string msg = P3Ribbon.Resources.Lang.lang.selSelezionaIsolanti;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {


            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;
            ElementId ductId;
            List<ElementId> InsulationTypeIds;
            double spiso = Materiale.SpessoreIsolante;

            Supporto.AggiornaDoc(doc);

            ISelectionFilter selFilterdc = new FiltraCondotti();
            IList<ElementId> selDuctids = Seleziona(uiDoc, msg, selFilterdc);

            InsulationTypeIds = (List<ElementId>)new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds();

           

            using (Transaction t = new Transaction(doc, "Cambia spessore isolante"))
            {

                t.Start();
                //potrei fare il contrario andandomi a prendere l'host  ma non riesco ad otterene la FamilyInstance dell'isol
                //prendo tutti gli isolanti presenti e il loro rispettivo host(condotto) se il condotto coincide con la selezione 
                foreach ( var insId in InsulationTypeIds)
                {
                    ductId = (doc.GetElement(insId) as DuctInsulation).HostElementId;
                    foreach(var selId in selDuctids)
                    {
                        if (ductId.IntegerValue == selId.IntegerValue)
                        {
           
                            try
                            {
                                doc.GetElement(insId).get_Parameter(BuiltInParameter.RBS_INSULATION_THICKNESS_FOR_DUCT).Set(spiso);
                                doc.GetElement(insId).ChangeTypeId(Materiale.IdInsulTipoPreferito);
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
                double catId = element.Category.Id.IntegerValue;
                //if (element.Category.Name == "Condotto" || element.Category.Name == "Raccordo") // DA SISTEMARE
                if (catId == Supporto.doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting).Id.IntegerValue 
                    || catId == Supporto.doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves).Id.IntegerValue)
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
                try
                {
                    sel = uiDoc.Selection.PickElementsByRectangle(_filtro, msg).Select(e => e.Id).ToList();
                }
                catch
                {

                }
            }
            return sel;
        }
    }
}
