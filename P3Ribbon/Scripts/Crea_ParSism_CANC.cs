using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class Crea_ParSism : IExternalCommand
    {

        static public void CreaParametriCondivisi(Document doc, Application app)
        {   //prendo la categoria di infromaizone di progetto
            Category category = doc.Settings.Categories.get_Item(BuiltInCategory.OST_ProjectInformation);
            CategorySet categorySet = app.Create.NewCategorySet();
            categorySet.Insert(category);

            string originalFile = app.SharedParametersFilename;
            //RISOLVERE E TROVARE IL MODO PER PRENDERSELO DA VISUAL STUDIO\
            string tempfie = @"C:\Users\Simone Maioli\Desktop\17017_ParamCondivisi.txt";

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

            }
            catch
            {

            }
            finally
            {
                //reset alla fine il fileoriginale
                app.SharedParametersFilename = originalFile;
            }
        }



        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            CreaParametriCondivisi(doc, app);


            return Result.Succeeded;
        }
    }

}
