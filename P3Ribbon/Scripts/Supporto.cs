using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
    
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.Reflection;

namespace P3Ribbon.Scripts
{
    class Supporto
    {
        public static List<List<double>> ValoriTabella;
        public static Document doc;
        public static Application app;

        public static LogicalOrFilter CatFilter(bool insul_or_racc)
        {
            IList<ElementFilter> catfilters = new List<ElementFilter>();
            catfilters.Add(new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves));
            if (insul_or_racc)
            {
                catfilters.Add(new ElementCategoryFilter(BuiltInCategory.OST_DuctInsulations));
            }
            else
            {
                catfilters.Add(new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting));
            }

            LogicalOrFilter filter = new LogicalOrFilter(catfilters);
            return filter;
        }

        public static LogicalOrFilter CatFilterDuctAndInsul = CatFilter(true);
        public static LogicalOrFilter CatFilterDuctAndFitting = CatFilter(false);

        public static string TrovaPercorsoRisorsa(string NomeFile)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string PathAssembly = Assembly.GetExecutingAssembly().Location;
            string PercorsoRisorsa = PathAssembly.Replace("P3Ribbon.dll", "P3_Resources\\" + NomeFile);
            return PercorsoRisorsa;
        }

        public static void CambiaSplitButton(SplitButton sb, int i)
        {
            IList<PushButton> spBottoni = sb.GetItems();
            sb.CurrentButton = spBottoni[i];
        }
        public static bool ControllaSePresentiParamSismici()
        {
            Element projInfo = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).FirstElement();
           

            bool parametri_presenti = false;

            Parameter Cu = projInfo.LookupParameter("P3_InfoProg_ClasseUso");
            Parameter En = projInfo.LookupParameter("P3_InfoProg_Eng");
            Parameter Vn = projInfo.LookupParameter("P3_InfoProg_VitaNominale");
            Parameter Zs = projInfo.LookupParameter("P3_InfoProg_ZonaSismica");


            if (Cu == null || En == null || Vn == null || Zs == null)
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = "Parametri sismici non inseriti nel progetto";
                td.MainContent = "Parametri sismici non inseriti nel progetto, inserire i parametri sismici?";
                td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;

                TaskDialogResult result = td.Show();

                if (result == TaskDialogResult.Yes)
                {
                    parametri_presenti = CreaParametriCondivisi(doc, app);
                    return parametri_presenti = true;
                }
                else
                {
                    return parametri_presenti = false;
                }

            }
            else
            {
               return parametri_presenti = true;
            }
        }

        static public bool CreaParametriCondivisi(Document doc, Application app)
        {
            bool output = false;
            //prendo la categoria di informaizone di progetto
            Category category = doc.Settings.Categories.get_Item(BuiltInCategory.OST_ProjectInformation);
            CategorySet categorySet = app.Create.NewCategorySet();
            categorySet.Insert(category);

            string originalFile = app.SharedParametersFilename;
            //RISOLVERE E TROVARE IL MODO PER PRENDERSELO DA VISUAL STUDIO\

            string tempfie = Supporto.TrovaPercorsoRisorsa("17017_ParamCondivisi.txt");

            try
            {

                app.SharedParametersFilename = tempfie;

                DefinitionFile SharedParameterFile = app.OpenSharedParameterFile();

                foreach (DefinitionGroup dg in SharedParameterFile.Groups)
                {
                    if (dg.Name == "InfoProgetto")
                    {
                        ExternalDefinition externalDefinitionCU = dg.Definitions.get_Item("P3_InfoProg_ClasseUso") as ExternalDefinition;
                        ExternalDefinition externalDefinitionVN = dg.Definitions.get_Item("P3_InfoProg_VitaNominale") as ExternalDefinition;
                        ExternalDefinition externalDefinitionZS = dg.Definitions.get_Item("P3_InfoProg_ZonaSismica") as ExternalDefinition;
                        ExternalDefinition externalDefinitionEN = dg.Definitions.get_Item("P3_InfoProg_Eng") as ExternalDefinition;

                        using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
                        {
                            t.Start();
                            InstanceBinding newIB = app.Create.NewInstanceBinding(categorySet);

                            doc.ParameterBindings.Insert(externalDefinitionCU, newIB, BuiltInParameterGroup.INVALID);
                            doc.ParameterBindings.Insert(externalDefinitionVN, newIB, BuiltInParameterGroup.INVALID);
                            doc.ParameterBindings.Insert(externalDefinitionZS, newIB, BuiltInParameterGroup.INVALID);
                            doc.ParameterBindings.Insert(externalDefinitionEN, newIB, BuiltInParameterGroup.INVALID);
                            t.Commit();
                        }

                    }
                }
                output = true;
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
