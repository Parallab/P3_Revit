using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.Reflection;
using System.Resources;
using System.Threading;
namespace P3Ribbon.Scripts
{
    class Supporto
    {
        public static List<List<double>> ValoriTabella;
        public static Document doc;
        public static Application app;
        public static AddInId ActiveAddInId;

        public static void AggiornaDoc(Document _doc)
        {
            doc = _doc;
            app = _doc.Application;
        }
      
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
#if RELASE2021

            string PercorsoRisorsa = PathAssembly.Replace("V2021\\P3Ribbon.dll", "P3_InstallerResources\\" + NomeFile);
#else
            string PercorsoRisorsa = PathAssembly.Replace("V2020\\P3Ribbon.dll", "P3_InstallerResources\\" + NomeFile);

#endif
            return PercorsoRisorsa;
        }

        public static bool ControllaSePresentiParamSismici()
        {
            Element projInfo = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).FirstElement();


            bool parametri_presenti = false;

            Parameter Cu = projInfo.LookupParameter("P3_InfoProg_ClasseUso");
            Parameter Vn = projInfo.LookupParameter("P3_InfoProg_VitaNominale");
            Parameter Zs = projInfo.LookupParameter("P3_InfoProg_ZonaSismica");


            if (Cu == null || Vn == null || Zs == null)
            {
                TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
                //parametri sismici non inseriti nel proggetto
                td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdParametriNonInseriti;
                td.MainContent = P3Ribbon.Resources.Lang.lang.taskdParamInserirli;
                td.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.No;

                TaskDialogResult result = td.Show();

                if (result == TaskDialogResult.Ok)
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


            string tempfile = Supporto.TrovaPercorsoRisorsa("P3_ParamCondivisi.txt");

            try
            {

                app.SharedParametersFilename = tempfile;
                DefinitionFile SharedParameterFile = app.OpenSharedParameterFile();
                // potrebbe non essere impostato un file di parametri condivisi nell'istanza di revit. proviamo ad impostarlo noi?
                //if (SharedParameterFile == null)
                //{
                //    string temptempfie = Supporto.TrovaPercorsoRisorsa("SharedParametersFIle_TEMP.txt");
                //    app.SharedParametersFilename = temptempfie;
                //    SharedParameterFile = app.OpenSharedParameterFile();
                //}

                foreach (DefinitionGroup dg in SharedParameterFile.Groups)
                {
                    if (dg.Name == "InfoProgetto")
                    {
                        ExternalDefinition externalDefinitionCU = dg.Definitions.get_Item("P3_InfoProg_ClasseUso") as ExternalDefinition;
                        ExternalDefinition externalDefinitionVN = dg.Definitions.get_Item("P3_InfoProg_VitaNominale") as ExternalDefinition;
                        ExternalDefinition externalDefinitionZS = dg.Definitions.get_Item("P3_InfoProg_ZonaSismica") as ExternalDefinition;


                        using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
                        {
                            t.Start();
                            InstanceBinding newIB = app.Create.NewInstanceBinding(categorySet);

                            doc.ParameterBindings.Insert(externalDefinitionCU, newIB, BuiltInParameterGroup.INVALID);
                            doc.ParameterBindings.Insert(externalDefinitionVN, newIB, BuiltInParameterGroup.INVALID);
                            doc.ParameterBindings.Insert(externalDefinitionZS, newIB, BuiltInParameterGroup.INVALID);

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
                //Legge i parametri nascosti 
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
            using (Transaction t = new Transaction(doc, "CreaParamCondivisi"))
            {
                t.Start();
                doc.Regenerate();
                Autodesk.Revit.DB.View CurrView = doc.ActiveView;
                ViewType viewType = CurrView.ViewType;

                IList<UIView> UlViews = uiDoc.GetOpenUIViews();
                if (UlViews.Count > 1)
                {
                    if (viewType == ViewType.Schedule)
                    {
                    foreach (UIView pView in UlViews)
                    { 
                        
                        if (pView.ViewId.IntegerValue == CurrView.Id.IntegerValue)                            
                            pView.Close();
                    }
                    }
                }
                t.Commit();
            }

        }
        public static List<List<double>> LeggiTabella(Document doc)
        {
            Element proj_info = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).ToElements().FirstOrDefault();

            int ClasseUso = proj_info.LookupParameter("P3_InfoProg_ClasseUso").AsInteger();
            int ZonaSismica = proj_info.LookupParameter("P3_InfoProg_ZonaSismica").AsInteger();
            if (ClasseUso < 2) { ClasseUso = 2; }
            List<List<double>> tabella_leggera = new List<List<double>>();
            var lines = System.IO.File.ReadAllLines(Supporto.TrovaPercorsoRisorsa("P3_TabelleDiPredimensionamento.txt"));
            for (int i_r = 0; i_r < lines.Length; i_r++)
            {
                List<double> sottoLista = new List<double>();

                var fields = lines[i_r].Split(';');
                if (fields[1] == ClasseUso.ToString() && fields[3] == ZonaSismica.ToString())
                {
                    for (int i = 1; i < fields.Count(); i++)
                    {
                        string field = fields[i];

                        sottoLista.Add(double.Parse(field));
                    }
                    tabella_leggera.Add(sottoLista);
                }
            }
            return tabella_leggera;
        }

        public static void CambiaLingua(UIControlledApplication a)
        {
            ResourceSet resourceSet_arrivo;

            App.Lingua lingua_attuale = App.lingua_plugin;

            //if (lingua_attuale != App.lingua_arrivo)
            //{

                if (lingua_attuale == App.Lingua.ITA)
                {
                    App.lingua_arrivo = App.Lingua.ENG;
                    //var langCode = Properties.Settings.Default.languageCode = "en-US";
                    //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);

                }
                else
                {
                    App.lingua_arrivo = App.Lingua.ITA;
                    //var langCode = Properties.Settings.Default.languageCode = "it-IT";
                    //Properties.Settings.Default.Save();
                    //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
                }

                foreach (RibbonPanel rp in a.GetRibbonPanels(App.tabName))
                {

                    try
                    {

                        rp.Title = App.Res_ValoreLingua(rp.Name, App.lingua_arrivo);

                        foreach (RibbonItem bottone in rp.GetItems())
                        {
                            try
                            {
                                if (bottone.ItemType == RibbonItemType.SplitButton)
                                {
                                    foreach (RibbonItem sbBottone in (bottone as SplitButton).GetItems())
                                    {
                                        sbBottone.ItemText = App.Res_ValoreLingua(sbBottone.Name, App.lingua_arrivo);
                                        sbBottone.ToolTip = App.Res_ValoreLingua(sbBottone.Name + "_tt", App.lingua_arrivo);
                                    }

                                }
                                bottone.ItemText = App.Res_ValoreLingua(bottone.Name, App.lingua_arrivo);
                                bottone.ToolTip = App.Res_ValoreLingua(bottone.Name + "_tt", App.lingua_arrivo);

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

                App.lingua_plugin = App.lingua_arrivo;
            //}
        }
       



    }


}

