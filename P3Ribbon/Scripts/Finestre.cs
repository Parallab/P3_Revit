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

            Scripts.GUI.Wpf_InfoP3 wpf = new Scripts.GUI.Wpf_InfoP3();
            using (wpf)
            {


                wpf.ShowDialog();

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

            Scripts.GUI.Form_Impostazioni frm = new Scripts.GUI.Form_Impostazioni(commandData);
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

            MigraAreaIsolamento.ControllaParametriSeEsistenti(doc, app);
            if (MigraAreaIsolamento.isolamentipresenti == true)
            {
                if (MigraAreaIsolamento.parPresenti == true)
                {
                    Scripts.GUI.Form_Quantità frm = new Scripts.GUI.Form_Quantità(commandData);
                    using (frm)
                    {
                        frm.ShowDialog();

                    }
                }
                return Result.Succeeded;
            }
            else
                return Result.Failed;
        }

    }

    [Transaction(TransactionMode.Manual)]
    class FinestraImpostazioni : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            Scripts.GUI.Wpf_impo wpf = new GUI.Wpf_impo(commandData);

            wpf.ShowDialog();

            return Result.Succeeded;
        }

    }

    [Transaction(TransactionMode.Manual)]
    class FinestraLibreria : IExternalCommand
    {
        UIDocument uiDoc;
        Document doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            uiDoc = uiApp.ActiveUIDocument;
            doc = uiDoc.Document;
            Application app = uiApp.Application;

            GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
            using (wpf)
            {

                //using (var t = new Transaction(doc, "FinestraInfo"))
                //{
                //t.Start();
                wpf.ShowDialog();
                //    t.Commit();
                //}


                Supporto.ChiudiFinestraCorrente(uiDoc);
            }
            return Result.Succeeded;
        }


    }
}
