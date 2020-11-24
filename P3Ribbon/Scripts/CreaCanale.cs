using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class CreaCanale : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;
            using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
            {
                t.Start();
                EseguiBottoneCanale(uiApp);
                t.Commit();
            }

            return Result.Succeeded;
        }
        public static void EseguiBottoneCanale(UIApplication uiApp)
        {
            RevitCommandId cmdid = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
            uiApp.PostCommand(cmdid);
        }

    }
}
