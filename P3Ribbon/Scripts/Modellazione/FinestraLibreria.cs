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
    //[Transaction(TransactionMode.Manual)]
    //class FinestraLibreria : IExternalCommand
    //{
    //    UIDocument uiDoc;
    //    Document doc;

    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        UIApplication uiApp = commandData.Application;
    //        uiDoc = uiApp.ActiveUIDocument;
    //        doc = uiDoc.Document;
    //        Application app = uiApp.Application;

    //        Form.Wpf_Libreria wpf = new Form.Wpf_Libreria(commandData);
    //        using (wpf)
    //        {

    //        //using (var t = new Transaction(doc, "FinestraInfo"))
    //        //{
    //        //t.Start();
    //        wpf.ShowDialog(); 
    //        //    t.Commit();
    //        //}

    //        // la finestra si chiude da sola e nona rriva a questo punto quindi il comando non termina. potrei inserire chiudi feinstra dentro al bottone ma cmq nona rrivo a return result succeeded
    //        ChiudiFinestraCorrente();
    //        }
    //        return Result.Succeeded;
    //    }


    //    private void ChiudiFinestraCorrente()
    //    {
    //        Autodesk.Revit.DB.View CurrView = doc.ActiveView;
    //        IList<UIView> UlViews = uiDoc.GetOpenUIViews();
    //        if (UlViews.Count > 1)
    //        {
    //            foreach (UIView pView in UlViews)
    //            {
    //                if (pView.ViewId.IntegerValue == CurrView.Id.IntegerValue)
    //                    pView.Close();
    //            }
    //        }
    //    }
    //}
}


