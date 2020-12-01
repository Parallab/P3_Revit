using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using System.Collections.ObjectModel;
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

            TrasferisciTipiDoc(app,doc);

            return Result.Succeeded;
        }
        public static void TrasferisciTipiDoc(Application app, Document doc)
        {
            using (var t = new Transaction(doc, "TransferType"))
            {
                List<string> TipiPresenti_nomi = new List<string>();

                FilteredElementCollector CTypes_presenti = new FilteredElementCollector(doc).WherePasses(Supporto.CatFilterDuctAndInsul).WhereElementIsElementType();

                // guardo tutti i tipi che mi interessamno presenti nel mio doc
                foreach (ElementType type in CTypes_presenti)
                {
                    string nome = type.Name;
                    if (nome.StartsWith("P3"))
                    {
                        TipiPresenti_nomi.Add(nome);
                     }
                }


                // guardo i tipi nel documento template
                ICollection<ElementId> copytypeids = new Collection<ElementId>();           

                Document docSource = app.OpenDocumentFile(Par_Sismici.TrovaPercorsoRisorsa("P3 - Duct system template2020.rte"));
                FilteredElementCollector CSourceTypes = new FilteredElementCollector(docSource).WherePasses(Supporto.CatFilterDuctAndInsul).WhereElementIsElementType();
                CopyPasteOptions option = new CopyPasteOptions();
               
                foreach (ElementType type in CSourceTypes)
                {
                    string nome = type.Name;
                    if (nome.StartsWith("P3"))
                    {
                        // contollRE SE ESISTE NEL DOC
                        if (!(TipiPresenti_nomi.Contains(nome))) //perchè non va?
                        { 
                            copytypeids.Add(type.Id);
                        }

                    }  
                }

                //importare gli abachi

                //abachi presenti nel doc source
                IList<Element> sc_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().ToElements();
                List<string> AbachiPresenti_nomi = new List<string>();
                


                foreach (Element schedule in sc_coll)
                {
                    string nome = schedule.Name;
                    if (nome.StartsWith("P3"))
                    {
                        AbachiPresenti_nomi.Add(nome);
                      
                    }
                }

                //leggo gli abachi nella risorsa
                ICollection<ElementId> copyscheduleids = new Collection<ElementId>();
                IList<Element> sc_sources = new FilteredElementCollector(docSource).OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().ToElements();
                
                foreach (Element schedule in sc_sources)
                {
                    string nome = schedule.Name;
                    if (nome.StartsWith("P3"))
                    {
                        // contollRE SE ESISTE NEL DOC
                        if (!(AbachiPresenti_nomi.Contains(nome))) //perchè non va?
                        {
                            copyscheduleids.Add(schedule.Id);
                        }

                    }
                }
                t.Start();
                ElementTransformUtils.CopyElements(docSource, copytypeids, doc, Transform.Identity, option); 
                ElementTransformUtils.CopyElements(docSource,copyscheduleids,doc, Transform.Identity, option);
                copyscheduleids.Clear();
                copytypeids.Clear();
                t.Commit();
            }
        }
    }
}
