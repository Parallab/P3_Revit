using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

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
           

            string tempfie = Supporto.TrovaPercorsoRisorsa("P3_ParamCondivisi.txt");

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
                //reset alla fine il file   originale
                app.SharedParametersFilename = originalFile;
            }
            return output;
        }
        public static bool ControllaTipiP3Presenti(string nometipo)
        {
            List<string> IsolatiECondottiP3Presenti = new List<string>();
            bool tipiCondottiCaricati = false;
            FilteredElementCollector collTipiPresenti = new FilteredElementCollector(doc).WherePasses(Supporto.CatFilterDuctAndInsul).WhereElementIsElementType();


            //guardo tutti i tipi che mi interessamno presenti nel mio doc
            foreach (Element type in collTipiPresenti)
            {
                //Leggo il parametro nascosto che corrisponde all'attuale nome del tipo
                try
                {
                    string nome = type.LookupParameter("P3_Nome").AsString();

                    if (nome.StartsWith("P3"))
                    {
                        IsolatiECondottiP3Presenti.Add(nome);
                    }
                }
                catch
                {

                }
            }
            if (IsolatiECondottiP3Presenti.Contains(nometipo))
            {
                tipiCondottiCaricati = true;
            }
            else
            {
                tipiCondottiCaricati = false;
            }
            return tipiCondottiCaricati;

        }


        public static bool ControllaAbachiP3Presenti(string AbacoNome)
        {
            List<string> AbachiP3Presenti = new List<string>();
            bool AbachiP3Caricati = false;
            IList<Element> collAbachiPresenti = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().ToElements();

            foreach (Element el in collAbachiPresenti)
            {
                try
                {
                    //leggo il parametro nascosto
                    string nome = el.LookupParameter("P3_Nome_i").AsString();

                    if (nome.StartsWith("P3"))
                    {
                        AbachiP3Presenti.Add(nome);
                    }
                }
                catch
                {

                }
            }
            if (AbachiP3Presenti.Contains(AbacoNome))
            {
                AbachiP3Caricati = true;
            }
            else
            {
                AbachiP3Caricati = false;
            }
            return AbachiP3Caricati;
        }
        public static bool ControllaStaffaPresente()
        {
            FilteredElementCollector collStaffe = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).WhereElementIsElementType();
            bool StaffaP3Caricata = false;

            foreach (var type in collStaffe)
            {
                //da usare poi parametri nascosti, FATTO

				string typeName = type.LookupParameter("P3_Nome").AsString();
                if (typeName == "P3_DuctHanger")
                {
                    StaffaP3Caricata = true;
                }

            }
            return StaffaP3Caricata;

        }
        public static void ChiudiFinestraCorrente(UIDocument uiDoc)
        {
            Autodesk.Revit.DB.View CurrView = doc.ActiveView;
            IList<UIView> UlViews = uiDoc.GetOpenUIViews();
            if (UlViews.Count > 1)
            {
                foreach (UIView pView in UlViews)
                {
                    if (pView.ViewId.IntegerValue == CurrView.Id.IntegerValue)
                        pView.Close();
                }
            }
        }

    }


}

