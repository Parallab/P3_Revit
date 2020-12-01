using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Mechanical;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB.Structure;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class CreaCanale : IExternalCommand
    {

        public static Document doc;
        public static Application app;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            doc = uiDoc.Document;
            app = uiApp.Application;

            using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
            {
                t.Start(); //serve?

                // SBAGLIATO FARLO ANDARE SEMPRE:
                CreaCanale.temp_inizializza_isolante(doc);
                ////
                CreaCondotto(uiApp, app);
                CreaMuro(doc);

                t.Commit();
            }

            return Result.Succeeded;
        }


        //public static List<ElementId> P3InsulationTypeIds = new List<ElementId>();

        // INTANTO HARD CODE, ma poi dipenderà dal meùa  tendina
        public static double insul_spessore_Scelto;
        public static ElementId insul_tipo_scelto;

        public static void temp_inizializza_isolante(Document doc)
        {
            insul_spessore_Scelto = 10 / 308.8; //1=304.8mm
            insul_tipo_scelto = new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds().First();
        }

        public static void OnDocumentChanged(object sender, DocumentChangedEventArgs e)

        {
            ICollection<ElementId> nuovi_id = e.GetAddedElementIds(Supporto.CatFilterDuctAndFitting);
            //
            string debug = "";
            foreach (ElementId id in nuovi_id)
            {
                debug += id.IntegerValue.ToString();
                debug += "; ";
                // non funziona se metto l isoalnte qui, forse perche entra in un loop??
            }
            //
            if (nuovi_id.Count > 0)
            {
                TaskDialog.Show("Debug", debug);
                //temp(nuovi_id);
                CreaMuro(doc);
            }
        }
        public static void CreaMuro(Document document)
        {
            var level_id = new ElementId(1526);
            XYZ point_a = new XYZ(-10, 0, 0);
            XYZ point_b = new XYZ(10, 10, 10);
            Line line = Line.CreateBound(point_a, point_b);

       
                Wall wall = Wall.Create(doc, line, level_id, false);
                var position = new XYZ(0, 0, 0);
                WallType wType = new FilteredElementCollector(doc).OfClass(typeof(WallType)).Cast<WallType>().FirstOrDefault();
                var symbol = document.GetElement(wType.Id) as FamilySymbol;
                var level = (Level)document.GetElement(wall.LevelId);
                document.Create.NewFamilyInstance(position, symbol, wall, level, StructuralType.NonStructural);
            

        }

        public static void temp(ICollection<ElementId> ids)
        {
            app.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
            foreach (ElementId id in ids)
            {
                DuctInsulation.Create(doc, id, ElementId.InvalidElementId, insul_spessore_Scelto);
            }
            app.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
        }
        public static void CreaCondotto(UIApplication uiApp, Application app)
        {
            app.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
            //DuctInsulation.Create(doc, new ElementId(2518792), insul_tipo_scelto, insul_spessore_Scelto); 
            //RevitCommandId cmdid = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
            //uiApp.PostCommand(cmdid);

            //app.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
        }


    }

    }

