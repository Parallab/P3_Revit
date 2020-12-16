using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using P3Ribbon.Scripts;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.Text.RegularExpressions;

namespace P3Ribbon
{

    [Transaction(TransactionMode.Manual)]
    class ParSismici : IExternalCommand
    {
        public static int classe;
        public static bool eng;
        public static int vita;
        public static int zona;

        //FORM//
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;
            IList<Element> projInfos = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).ToElements();
            Element projInfo = projInfos[0];

            Scripts.Form_Def_Acc frm = new Scripts.Form_Def_Acc();
            using (frm)
            {
                using (var t = new Transaction(doc, "Form parametri sismici"))
                {
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
                        }
                    }
                    else
                    {
                        parametri_presenti = true;
                    }

                    if (parametri_presenti)
                    {
                        t.Start();
                        Migra_Parametri_Presenti(doc);
                        frm.ShowDialog();
                        //t.Commit();
                        if (Form_Def_Acc.ok_premuto == true)
                        {
                            ProjInfoImpostaParametri(classe, eng, vita, zona, doc);
                        }
                        else
                        {
                            return Result.Cancelled;
                        }
                        t.Commit();
                        return Result.Succeeded;
                    }

                    else
                    {
                        return Result.Cancelled;
                    }

                }

            }


        }

        public static void ProjInfoImpostaParametri(int _classe, bool _eng, int _vita, int _zona, Document _doc)
        {
            if (Form_Def_Acc.ok_premuto == true)
            {
         
                IList<Element> proj_infos = new FilteredElementCollector(_doc).OfClass(typeof(ProjectInfo)).ToElements();
                Element proj_info = proj_infos[0];

                proj_info.LookupParameter("P3_InfoProg_ClasseUso").Set(_classe);
                //proj_info.LookupParameter("P3_InfoProg_Eng").Set(_eng);
                proj_info.LookupParameter("P3_InfoProg_VitaNominale").Set(_vita);
                proj_info.LookupParameter("P3_InfoProg_ZonaSismica").Set(_zona);
          
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

        public static void Migra_Parametri_Presenti(Document doc)
        {
            IList<Element> proj_infos = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).ToElements();
            Element proj_info = proj_infos[0];
            Parameter Cu = proj_info.LookupParameter("P3_InfoProg_ClasseUso");
            Parameter Zs = proj_info.LookupParameter("P3_InfoProg_ZonaSismica");

            Form_Def_Acc.zona_form = Zs.AsInteger();
            Form_Def_Acc.classe_form = Cu.AsInteger();
        }

    }
}
