using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{

    [Transaction(TransactionMode.Manual)]
    class Lingua : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            //using (var t = new Transaction(doc, "CambiaLingua"))
            //{
            //    t.Start();
            CambiaLingua(App.UICapp);
          
            //    t.Commit();
            //}
            return Result.Succeeded;
        }


        static void CambiaLingua(UIControlledApplication a)
        {
            ResourceSet resourceSet_arrivo;

            App.Lingua lingua_attuale = App.lingua_plugin;
            App.Lingua lingua_arrivo;// ((int)lingua_attuale +1)%1; ci deve essere un modo intelligente per passare da int a enum
            if (lingua_attuale == App.Lingua.ITA)
            {
                lingua_arrivo = App.Lingua.ENG;
                
            }
            else
            {
                lingua_arrivo = App.Lingua.ITA;
                
            }

            foreach (RibbonPanel rp in a.GetRibbonPanels(App.tabName))
            {

                try
                {
                    
                    rp.Title = App.res_valore(rp.Name, lingua_arrivo);

                    foreach (RibbonItem bottone in rp.GetItems())
                    { 
                        try
                        {
                            if (bottone.ItemType == RibbonItemType.SplitButton)
                            {
                                foreach(RibbonItem sbBottone in (bottone as SplitButton).GetItems())
                                {
                                    sbBottone.ItemText =  App.res_valore(sbBottone.Name, lingua_arrivo);
                                    sbBottone.ToolTip = App.res_valore(sbBottone.Name + "_tt", lingua_arrivo);
                                }

                            }
                            bottone.ItemText = App.res_valore(bottone.Name, lingua_arrivo);
                            bottone.ToolTip = App.res_valore(bottone.Name + "_tt", lingua_arrivo);
                           
                        }
                        catch
                        {

                        }
                    }

                }
                catch
                {

                }
            }


            App.lingua_plugin = lingua_arrivo;
            if (lingua_attuale == App.Lingua.ITA)
            {
                Supporto.CambiaSplitButton(App.sb1, 0);
            }
            else
            {
                Supporto.CambiaSplitButton(App.sb1, 1);
            }        
        }

    }
}

