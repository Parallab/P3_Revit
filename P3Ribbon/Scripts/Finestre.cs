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
    class FinestraInfo : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            Scripts.Form_InfoP3 frm = new Scripts.Form_InfoP3();
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

    [Transaction(TransactionMode.Manual)]
    class FinestraImpo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Scripts.Form.Form_Impostazioni frm = new Scripts.Form.Form_Impostazioni(commandData);
            using (frm)
            {
                using (var t = new Transaction(doc, "FinestraImpostazini"))
                {
                    t.Start();
                    frm.ShowDialog();
                    t.Commit();
                }
            }
            return Result.Succeeded;
        }

    }

    [Transaction(TransactionMode.Manual)]
    class FinestraQuantità : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;


            Scripts.Form.Form_Quantità frm = new Scripts.Form.Form_Quantità(commandData);
            using (frm)
            {
                //using (var t = new Transaction(doc, "FinestraImpostazini"))
               // {
                   // t.Start();
                    frm.ShowDialog();
                //    t.Commit();
               // }
            }
            return Result.Succeeded;
        }

    }

    [Transaction(TransactionMode.Manual)]
    class WPF : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            Scripts.Form.Wpf_info wps = new Form.Wpf_info(commandData);
            wps.ShowDialog();

            return Result.Succeeded;
        }

    }
}
