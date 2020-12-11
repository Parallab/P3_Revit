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
    class CreaCanaleDinamico : IExternalCommand
    {

        public static Document doc;
        public static Application app;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;
            
            DuctType DuctDynamicType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.Name.Contains( "P3 - Preinsulated panels system - Dynamic")) as DuctType;
            uiDoc.PostRequestForElementTypePlacement(DuctDynamicType);

            using (Transaction t = new Transaction(doc, "CanaleDinamico"))
            {

                t.Start(); //serve..sì
              
                RevitCommandId cmdid = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
                uiApp.PostCommand(cmdid);


                t.Commit();
            }
            return Result.Succeeded;

        }


    }

    [Transaction(TransactionMode.Manual)]
    class CreaCanaleScarpette : IExternalCommand
    {

        public static Document doc;
        public static Application app;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            DuctType DuctDynamicType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.Name.Contains("P3 - Preinsulated panels system - Tap")) as DuctType;
            uiDoc.PostRequestForElementTypePlacement(DuctDynamicType);

            using (Transaction t = new Transaction(doc, "CanaleScrpetta"))
            {

                t.Start(); 
                
                RevitCommandId cmdid = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
                uiApp.PostCommand(cmdid);


                t.Commit();
            }
            return Result.Succeeded;

        }


    }
}

  



