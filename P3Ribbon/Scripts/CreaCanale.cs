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

                ScriviParametro(doc);
                // SBAGLIATO FARLO ANDARE SEMPRE:
                CreaCanale.temp_inizializza_isolante(doc);

                //CreaCondotto(uiApp, app);

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
            }
        }

        public static void ScriviParametro(Document doc)
        {

            Element el = doc.GetElement(new ElementId(2564396));
            el.LookupParameter("Commenti").Set("Test");
            TaskDialog.Show("Debug", "test");

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

