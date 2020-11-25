using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class FinestraLibreria : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            Scripts.Form.Form_Libreria frm = new Form.Form_Libreria();
            using (frm)
            {
                using (var t = new Transaction(doc, "FinestraInfo"))
                {
                    t.Start();
                    frm.ShowDialog();
                    t.Commit();
                }
            }
            return Result.Succeeded;
        }

    }
}

