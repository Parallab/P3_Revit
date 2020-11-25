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

            return Result.Succeeded;
        }
        public void TrasferisciTipiDoc(Application app, CommandEventArgs commevent)
        {
            Document docSouce = app.OpenDocumentFile(Par_Sismici.TrovaPercorsoRisorsa("P3 - Duct system template.rte"));
            Document docActive = commevent.ActiveDocument;
        }
    }
}
