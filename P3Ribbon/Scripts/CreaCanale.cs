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

        // INTANTO HARD CODE, ma poi dipenderà dal meùa  tendina
        public static double insul_spessore_Scelto;
        public static ElementId insul_tipo_scelto;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            doc = uiDoc.Document;
            app = uiApp.Application;

            using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
            {

                t.Start(); //serve?

                CreaCondotto(uiApp, app);

                t.Commit();
            }
            return Result.Succeeded;
        
        }
        public static void CreaCondotto(UIApplication uiApp, Application app)
        {
       
            RevitCommandId cmdid = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
            uiApp.PostCommand(cmdid);
  
        }

        //public static void temp_inizializza_isolante(Document doc)
        //{
        //    insul_spessore_Scelto = 10 / 308.8; //1=304.8mm
        //    insul_tipo_scelto = new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds().First();
        //}


    }

  

}

