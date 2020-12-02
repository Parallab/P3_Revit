using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class DynamicModelUpdater : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application; ;


            WatcherUpdater.temp_inizializza_isolante(doc);

            using (Transaction t = new Transaction(doc, "ModelUpdater"))
            {
                t.Start();
                WatcherUpdater updater = new WatcherUpdater(app.ActiveAddInId);

                UpdaterRegistry.RegisterUpdater(updater);

                //ElementCategoryFilter f = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
                LogicalOrFilter f = Supporto.CatFilterDuctAndFitting;
                UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), f, Element.GetChangeTypeElementAddition());

                return Result.Succeeded;
                t.Commit();
            }

        }
        // INTANTO HARD CODE, ma poi dipenderà dal meùa  tendina
    }



    public class WatcherUpdater : IUpdater
    {
        static AddInId _appId;
        static UpdaterId _updaterId;

        // INTANTO HARD CODE, ma poi dipenderà dal meùa  tendina
        public static double insul_spessore_Scelto;
        public static ElementId insul_tipo_scelto;

        public WatcherUpdater(AddInId id)
        {
            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("604b1052-f742-4951-8576-c261d1993108"));
        }

        public static void temp_inizializza_isolante(Document doc)
        {
            insul_spessore_Scelto = 10 / 308.8; //1=304.8mm
            insul_tipo_scelto = new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds().First();
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            Application app = doc.Application;
            foreach (ElementId id in data.GetAddedElementIds())
            {
                try
                {
                    Element el = doc.GetElement(id);
                    string nome = "";
                    if (el.GetType() == typeof(Duct))
                    {
                        nome = doc.GetElement(el.GetTypeId()).Name;
                    }
                    else
                    {
                        nome = (doc.GetElement(el.GetTypeId()) as ElementType).FamilyName;
                    }



                    if (nome.Contains("P3")) // IN FUTURO USARE UN PARAMETO (NASCOSTO?)
                    {
                        DuctInsulation.Create(doc, id, insul_tipo_scelto, insul_spessore_Scelto);
                    }
                }
                catch (System.Exception ex)
                {
                    //TaskDialog.Show("Exception", ex.ToString());
                }
            }

        }

        #region metodi dell updater
        public string GetAdditionalInformation()
        {
            return "P3 - Applicazione materiale isolante";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        public string GetUpdaterName()
        {
            return "P3 - Applicazione materiale isolante";
        }
        #endregion
    }
}
