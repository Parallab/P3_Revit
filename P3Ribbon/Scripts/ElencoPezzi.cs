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

            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Element nlist = null;

            ViewSchedule viewSchedule = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).ToElements().Where(x => x.Name.Contains("P3 - Duct Fitting Schedule - All")) as ViewSchedule;
            uiDoc.ActiveView = viewSchedule;

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

            return Result.Succeeded;
        }
    }
}
