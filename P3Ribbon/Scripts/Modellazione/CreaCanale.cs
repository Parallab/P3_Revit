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

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            if (Supporto.ControllaTipiP3Presenti("P3 - Preinsulated panels system - Dynamic -  ε 0.03mm"))
            {
               
                DuctType ductDynamicType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.Name.Contains("P3 - Preinsulated panels system - Dynamic")) as DuctType;

                //richiedo che sia il prossimo tipo di default
                uiDoc.PostRequestForElementTypePlacement(ductDynamicType);
                using (Transaction t = new Transaction(doc, "Crea un canale di tipo dinamico"))
                {

                    t.Start();

                    RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
                    uiApp.PostCommand(cmdId);

                    t.Commit();
                }
                return Result.Succeeded;
            }
            else
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = "Tipi di canale non inseriti nel progetto";
                td.MainContent = "Canali P3 non inseriti nel progetto, caricare prima la libreria";
                TaskDialogResult result = td.Show();

            }
                return Result.Cancelled;

        }


    }

    [Transaction(TransactionMode.Manual)]
    class CreaCanaleScarpette : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;


            if (Supporto.ControllaTipiP3Presenti("P3 - Preinsulated panels system - Tap -  ε 0.03mm"))
            {

                DuctType ductTapType = new FilteredElementCollector(doc).OfClass(typeof(DuctType)).FirstOrDefault(x => x.Name.Contains("P3 - Preinsulated panels system - Tap")) as DuctType;

                //richiedo che sia il prossimo tipo di default
                uiDoc.PostRequestForElementTypePlacement(ductTapType);

                using (Transaction t = new Transaction(doc, "Crea un canale di tipo scarpetta"))
                {

                    t.Start();

                    RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.Duct);
                    uiApp.PostCommand(cmdId);


                    t.Commit();
                }
                return Result.Succeeded;
            }
            else
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = "Tipi di canale non inseriti nel progetto";
                td.MainContent = "Canali P3 non inseriti nel progetto, caricare prima la libreria";
                TaskDialogResult result = td.Show();
            }
            return Result.Cancelled;
        }


    }
}





