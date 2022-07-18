using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{

    [Transaction(TransactionMode.Manual)]
        class LinguaInglese : IExternalCommand
        {
            public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
            {
                App.lingua_arrivo = App.Lingua.ENG;
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Application app = uiApp.Application;

                Supporto.CambiaLingua(App.UICapp);

                return Result.Succeeded;

            }        
        }

        [Transaction(TransactionMode.Manual)]
        class LinguaItaliano : IExternalCommand
        {
            public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
            {
                App.lingua_arrivo = App.Lingua.ITA;
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Application app = uiApp.Application;

                Supporto.CambiaLingua(App.UICapp);

                return Result.Succeeded;

            }
        }

}

