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
        UIDocument uiDoc;
        Document doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            uiDoc = uiApp.ActiveUIDocument;
            doc = uiDoc.Document;
            Application app = uiApp.Application;


            
            Form.Wpf_Libreria frm = new Form.Wpf_Libreria(commandData);
            using (frm)
            {

            //using (var t = new Transaction(doc, "FinestraInfo"))
            //{
            //t.Start();
            frm.ShowDialog(); //showdialog aspetta return, show continua subito. ma qui con showdialog è come se non andasse avanti!
                              //    t.Commit();
                              //}







                //qui va avanti solo quando chiudo la finestra, vvero quando mi dà un return. se tolgo la funzioen vera e proprioadi trsferimentos tandard la finestra rimane aperta, altrimenti si chiude da sola MA SENZA CONTINUARE QUESTO CODICE QUI
                var test = 1;
            // la finestra si chiude da sola e nona rriva a questo punto quindi il comando non termina. potrei inserire chiudi feinstra dentro al bottone ma cmq nona rrivo a return result succeeded
            ChiudiFinestraCorrente();
            }
            return Result.Succeeded;
        }


        private void ChiudiFinestraCorrente()
        {
            // sarebbe nteressnte inserire tutto questo in un metodo solo per avere ordine qui
            Autodesk.Revit.DB.View CurrView = doc.ActiveView;
            //m_UiDoc.RequestViewChange(CurrView);
            IList<UIView> UlViews = uiDoc.GetOpenUIViews();
            if (UlViews.Count > 1)
            {
                foreach (UIView pView in UlViews)
                {
                    if (pView.ViewId.IntegerValue == CurrView.Id.IntegerValue)
                        //if ((m_doc.GetElement(View.ViewId) as View). )
                        pView.Close();
                }
            }
        }
    }
}


