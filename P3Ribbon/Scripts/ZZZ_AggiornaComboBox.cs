using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Autodesk.Revit.ApplicationServices.Application;

// DA CANC

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class AggiornaComboBox : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            try
            {
                App.comboMat.AddItems(Scripts.Materiale.comboBoxMemberDatas);
            }
            catch
            {

            }


            return Result.Succeeded;
        }
    }
}
