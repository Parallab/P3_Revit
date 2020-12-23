using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace P3Ribbon.Scripts
{
       
    [Transaction(TransactionMode.Manual)]
    class ElencoPezzi : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (Supporto.ControllaAbachiP3Presenti("P3 - Duct Fitting Schedule - All"))
            {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Element nlist = null;

                ViewSchedule vistaAbacoP3All = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name == "P3 - Duct Fitting Schedule - All") as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3All;

             return Result.Succeeded;
            }
            else
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = " Abachi non inseriti nel progetto";
                td.MainContent = "Questo abaco non è presente nel progetto, caricare prima la libreria";
                TaskDialogResult result = td.Show();
            }
            return Result.Cancelled;

            #region TEST CREO UN NUOVO ABACO
            //TEST CREO UN NUOVO ABACO///
            //Transaction trans = new Transaction(doc, "ExComm");
            //trans.Start();

            //ViewSchedule vs = ViewSchedule.CreateSchedule(doc, new ElementId(BuiltInCategory.INVALID));
            //vs.Name = "ElencoPezzi";

            //doc.Regenerate();
            #endregion


            #region TEST LEGGO ABACO ABACO   
            //ViewSchedule viewSchedule = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault() as ViewSchedule;

            //TableData table = viewSchedule.GetTableData();
            //TableSectionData section = table.GetSectionData(SectionType.Body);
            //int nRows = section.NumberOfRows;
            //int nColumns = section.NumberOfColumns;

            //List<List<string>> scheduleData = new List<List<string>>();
            //for (int i = 0; i < nRows; i++)
            //{
            //    List<string> rowData = new List<string>();

            //    for (int j = 0; j < nColumns; j++)
            //    {
            //        rowData.Add(viewSchedule.GetCellText(SectionType.Body, i, j));
            //    }
            //    scheduleData.Add(rowData);
            //}
            #endregion

        }
    }

    [Transaction(TransactionMode.Manual)]
    class ElencoPunti : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (Supporto.ControllaAbachiP3Presenti("P3 - Duct Hangers"))
            {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;


                ViewSchedule vistaAbacoP3pts = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name == "P3 - Duct Hangers") as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3pts;

                return Result.Succeeded;
            }
            else
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = " Abachi non inseriti nel progetto";
                td.MainContent = "Questo abaco non è presente nel progetto, caricare prima la libreria";
                TaskDialogResult result = td.Show();
            }
            return Result.Cancelled;

        }
    }

    [Transaction(TransactionMode.Manual)]
    class ElencoMateriali : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (Supporto.ControllaAbachiP3Presenti("P3 - Duct Insulation Schedule - DYNAMO"))
            {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Application app = uiApp.Application;

                using (var t = new Transaction(doc, "Migra Parametro Area Isolamento"))
                {
                    t.Start();
                    MigraAreaIsolamento.ControllaParametriSeEsistenti(doc, app);
                    if (MigraAreaIsolamento.parPresenti == true)
                    {
                        MigraAreaIsolamento.MigraParaetriIsolamento(doc);
                    }
                    t.Commit();
                }


                ViewSchedule vistaAbacoP3insul = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name == "P3 - Duct Insulation Schedule - DYNAMO") as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3insul;

                return Result.Succeeded;
            }
            else
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = " Abachi non inseriti nel progetto";
                td.MainContent = "Questo abaco non è presente nel progetto, caricare prima la libreria";
                TaskDialogResult result = td.Show();
            }
            return Result.Cancelled;

        }
    }

    [Transaction(TransactionMode.Manual)]
    class ElencoStaffaggio : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (Supporto.ControllaAbachiP3Presenti("P3 - Duct Hangers - Components - ITA"))
            {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Application app = uiApp.Application;


                //if (App.lingua_plugin == App.Lingua.ITA)
                //{
                ViewSchedule vistaAbacoP3staff = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name == "P3 - Duct Hangers - Components - ITA") as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3staff;
                //}
                //else 
                //{
                //    viewSchedule = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.Name == "P3 - Duct Hangers - Components - ENG") as ViewSchedule;
                //    uiDoc.ActiveView = viewSchedule;
                //}

                return Result.Succeeded;
            }
            else
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = " Abachi non inseriti nel progetto";
                td.MainContent = "Questo abaco non è presente nel progetto, caricare prima la libreria";
                TaskDialogResult result = td.Show();
            }
            return Result.Cancelled;

        }
    }

}
