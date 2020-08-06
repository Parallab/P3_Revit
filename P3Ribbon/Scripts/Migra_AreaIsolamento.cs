using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Mechanical;
using System.Windows;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.Xml;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class Migra_AreaIsolamento : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            //CONTROLLARE PRIMA SE SON PRESENTI, SE NON SON PRESENTI AGGIUNGERLI
            bool parametri_presenti = CreaParametriIsolamento(doc, app);

            if (parametri_presenti)
            {
                Category c_condotti = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves);
                Category c_raccordi = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting);

                using (var t = new Transaction(doc, "Proj_Info_Scrivi_Parametri"))
                {

                    t.Start();
                    IList<Element> coll = new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation)).ToElements();

                    int i_f = 0;
                    int i_c = 0;
                    int i = 0;
                    foreach (Element el in coll)
                    {
                        try
                        {
                            InsulationLiningBase insul = el as InsulationLiningBase;
                            Element host = doc.GetElement(insul.HostElementId);
                            i++;



                            if (host.Category.Id == c_raccordi.Id)
                            {
                                double area_rac = host.LookupParameter("P3_Sup_S.app").AsDouble();
                                insul.LookupParameter("P3_Sup_S.app_dyn").Set(area_rac);
                                i_f++;
                            }


                            if (host.Category.Id == c_condotti.Id)
                            {
                                Parameter area_condotto = insul.get_Parameter(BuiltInParameter.RBS_CURVE_SURFACE_AREA);
                                insul.LookupParameter("P3_Sup_S.app_dyn").Set(area_condotto.AsDouble());
                                i_c++;
                            }
                        }
                        catch
                        {

                        }
                    }
                   //MessageBox.Show(i + " isolamenti trovati", "numero di isolamento condotti");
                    System.Windows.MessageBox.Show("È stato migrato il parametro di area a " + i_c + " condotti isolati "  + System.Environment.NewLine + "e "+ i_f + " raccordi isolati"  , "numero di condotti e raccordi isolati" );

                    t.Commit();
                }
            }
            return Result.Succeeded;
        }


        static public bool CreaParametriIsolamento(Document doc, Application app)
        {
            bool output = false;
            //prendo la categoria di condotti
            //Category c_co = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves);
            //Category c_ra = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting);

            CategorySet categorySet = app.Create.NewCategorySet();
            //categorySet.Insert(c_co);
            //categorySet.Insert(c_ra);
            categorySet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctInsulations));

            string originalFile = app.SharedParametersFilename;

            //RISOLVERE E TROVARE IL MODO PER PRENDERSELO DA VISUAL STUDIO\
            string tempfie = @"C:\Users\Simone Maioli\Desktop\17017_ParamCondivisi.txt";

            try
            {
                app.SharedParametersFilename = tempfie;
                DefinitionFile SharedParameterFile = app.OpenSharedParameterFile();
                foreach (DefinitionGroup dg in SharedParameterFile.Groups)
                {
                    if (dg.Name == "Materiale riciclato")
                    {
                        ExternalDefinition externalDefinitionS_App = dg.Definitions.get_Item("P3_Sup_S.app_dyn") as ExternalDefinition;

                        using (Transaction t = new Transaction(doc, "Aggiungi parametro P3"))
                        {
                            t.Start();
                            InstanceBinding newIB = app.Create.NewInstanceBinding(categorySet);
                            doc.ParameterBindings.Insert(externalDefinitionS_App, newIB, BuiltInParameterGroup.PG_AREA);
                            t.Commit();
                        }
                        output = true;
                    }
                }
            }
            catch
            {
                output = false;
            }
            finally
            {
                //reset alla fine il fileoriginale
                app.SharedParametersFilename = originalFile;
            }
            return output;
        }

    }
}
