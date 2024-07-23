using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Application = Autodesk.Revit.ApplicationServices.Application;

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

		public static string TrovaPercorsoRisorsaInstaller(string NomeFile)
		{
			Assembly a = Assembly.GetExecutingAssembly();
			string PathAssembly = Assembly.GetExecutingAssembly().Location; //percorso dll
																			// Get the directory of the assembly
			string DirectoryAssembly = System.IO.Path.GetDirectoryName(PathAssembly);
			string PercorsoRisorsa = System.IO.Path.Combine(DirectoryAssembly, "P3_InstallerResources\\", NomeFile);

			//fa schifo sto if, sarebbe da fare in modo migliore con variabili v2021 da qualche parte:
			//#if Rel_21_24

			//			//string PercorsoRisorsa = PathAssembly.Replace("V2021\\P3Ribbon.dll", "P3_InstallerResources\\" + NomeFile);
			//			string PercorsoRisorsa = PathAssembly.Replace("P3Ribbon_2021-24.dll", "P3_InstallerResources\\" + NomeFile);
			//#elif DEBUG
			//			//questo funziona male se debuggo
			//			//TEMP 25/1/24 debug
			//			string PercorsoRisorsa = PathAssembly.Replace("P3Ribbon.dll", "P3_InstallerResources\\" + NomeFile);
			//#else
			//			//string PercorsoRisorsa = PathAssembly.Replace("V2020\\P3Ribbon.dll", "P3_InstallerResources\\" + NomeFile);
			//			string PercorsoRisorsa = PathAssembly.Replace("P3Ribbon_2019-20.dll", "P3_InstallerResources\\" + NomeFile);

			//#endif

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


			string tempfile = Supporto.TrovaPercorsoRisorsaInstaller("P3_ParamCondivisi.txt");

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

							// c'è dal 2022...forse dovrei fare Rel_21 da sola? o solo Rel_25 da sola che in futuro ci saranno altri cambiamenti?
							//
							//https://www.revitapidocs.com/2022/a2fe7c6f-e5e2-bafe-23c8-819ba6a6c9b9.htm
							//dovrebbe andare già dal 2024... :
							//https://www.revitapidocs.com/2024/a2fe7c6f-e5e2-bafe-23c8-819ba6a6c9b9.htm
							//https://www.revitapidocs.com/2024/c3bed87a-956f-47c3-060c-0294c7ef43e7.htm
							//https://forums.autodesk.com/t5/revit-api-forum/revit-2024-grouptypeid-missing-parametergroup-other-invalid/td-p/12288651
							//https://forums.autodesk.com/t5/revit-api-forum/revit-2024-other-parameter-group/td-p/12086226

							//doc.ParameterBindings.Insert(externalDefinitionCU, newIB, BuiltInParameterGroup.INVALID);//non va piu bene in r2025
							//doc.ParameterBindings.Insert(externalDefinitionVN, newIB, BuiltInParameterGroup.INVALID);//non va piu bene in r2025
							//doc.ParameterBindings.Insert(externalDefinitionZS, newIB, BuiltInParameterGroup.INVALID);//non va piu bene in r2025
							AggiungiParametroProgetto(doc, externalDefinitionCU, newIB, Supporto.BuiltInParameterGroup_OR_GroupTypeId.ALTRO);
							AggiungiParametroProgetto(doc, externalDefinitionVN, newIB, Supporto.BuiltInParameterGroup_OR_GroupTypeId.ALTRO);
							AggiungiParametroProgetto(doc, externalDefinitionZS, newIB, Supporto.BuiltInParameterGroup_OR_GroupTypeId.ALTRO);

							t.Commit();
						}
					}
				}
				output = true;
			}
			catch (Exception ex)
			{
				DebugUtils.PrintExceptionInfo(ex);
				output = false;
			}
			finally
			{
				//reset alla fine il file   originale
				app.SharedParametersFilename = originalFile;
			}
			return output;
		}

		public enum BuiltInParameterGroup_OR_GroupTypeId
		{//FACCIO A MANO PER GESTIRE PASSAGGIO DA 
			AREA,
			ALTRO
		}

		//		public object ParseParameterGroupOrGroupTypeId(string input)
		//		{
		//#if (Rel_25)
		//			return ParseStaticType(typeof(GroupTypeId), input);
		//#else
		//        return ParseStaticType(typeof(BuiltInParameterGroup), input);
		//#endif
		//		}

		//		private object ParseStaticType(Type type, string input)
		//		{
		//			// Get all public static fields or properties from the type
		//			var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static)
		//				.Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
		//				.ToArray();

		//			foreach (var member in members)
		//			{
		//				if (member.Name.Equals(input, StringComparison.OrdinalIgnoreCase))
		//				{
		//					if (member.MemberType == MemberTypes.Field)
		//					{
		//						return ((FieldInfo)member).GetValue(null);
		//					}
		//					else if (member.MemberType == MemberTypes.Property)
		//					{
		//						return ((PropertyInfo)member).GetValue(null);
		//					}
		//				}
		//			}

		//			throw new ArgumentException($"Input string does not correspond to a valid {type.Name}.", nameof(input));
		//		}

		public static object BuiltInParameterGroup_OR_GroupTypeId_Converti(BuiltInParameterGroup_OR_GroupTypeId nome)
		{
#if (Rel_25 || Debug_25)
			switch (nome)
			{
				case BuiltInParameterGroup_OR_GroupTypeId.AREA:
					return GroupTypeId.Area;
				case BuiltInParameterGroup_OR_GroupTypeId.ALTRO:
					return new ForgeTypeId(string.Empty);
				default:
					return new ForgeTypeId(string.Empty);
			}
#else
			switch (nome)
			{
				case BuiltInParameterGroup_OR_GroupTypeId.AREA:
					return BuiltInParameterGroup.PG_AREA;
				case BuiltInParameterGroup_OR_GroupTypeId.ALTRO:
					return BuiltInParameterGroup.INVALID;
				default:
					return BuiltInParameterGroup.INVALID;
			}
#endif
		}


		public static void AggiungiParametroProgetto(Document _doc, ExternalDefinition _extdef, InstanceBinding _newIB, BuiltInParameterGroup_OR_GroupTypeId gruppo) //<T> T gruppo 
		{
			//ho messo _doc perchè lo stesso metodo è usato su migra area isolamento, da testare se si può allinear etutto a supporto.doc
#if (Rel_25 || Debug_25)
			ForgeTypeId gruppo_forge = (ForgeTypeId)BuiltInParameterGroup_OR_GroupTypeId_Converti(gruppo);
			_doc.ParameterBindings.Insert(_extdef, _newIB, gruppo_forge);
#else
			BuiltInParameterGroup gruppo_builtin = (BuiltInParameterGroup)BuiltInParameterGroup_OR_GroupTypeId_Converti(gruppo);
			_doc.ParameterBindings.Insert(_extdef, _newIB, gruppo_builtin);//non va piu bene in r2025
#endif
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

					string nome = type.LookupParameter("P3_Nome")?.AsString();
					if (nome == null)
						continue;

					if (nome.StartsWith("P3"))
					{
						IsolatiECondottiP3Presenti.Add(nome);
					}
				}
				catch (Exception ex)
				{
					DebugUtils.PrintExceptionInfo(ex);
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
					string nome = el.LookupParameter("P3_Nome_i")?.AsString();
					if (nome == null)
						continue;
					if (nome.StartsWith("P3"))
					{
						AbachiP3Presenti.Add(nome);
					}
				}
				catch (Exception ex)
				{
					DebugUtils.PrintExceptionInfo(ex);
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

							if (Supporto.exIntegerValue(pView.ViewId) == Supporto.exIntegerValue(CurrView.Id))
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
			var lines = System.IO.File.ReadAllLines(Supporto.TrovaPercorsoRisorsaInstaller("P3_TabelleDiPredimensionamento.txt"));
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
		public static void AggiornaLinguaRibbon(UIControlledApplication a)
		{
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
						catch (Exception ex)
						{
							DebugUtils.PrintExceptionInfo(ex);
						}
					}

				}
				catch (Exception ex)
				{
					DebugUtils.PrintExceptionInfo(ex);
				}
			}
		}
		public static void CambiaLingua(UIControlledApplication a)
		{
			//ResourceSet resourceSet_arrivo;

			//App.Lingua lingua_attuale = App.lingua_plugin;

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo cultureInfo = CultureInfo.DefaultThreadCurrentCulture;
			if (App.lingua_arrivo == App.Lingua.ENG)
			{
				var langCode = Properties.Settings.Default.languageCode = "en-US";
				//Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
				//cultureInfo = new CultureInfo("en");
				cultureInfo = new CultureInfo(langCode);

			}
			// Set the current thread's culture to the selected culture
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			// Manually reload the resource file for the selected culture
			Resources.Lang.lang.Culture = cultureInfo;

			AggiornaLinguaRibbon(a);

			App.lingua_plugin = App.lingua_arrivo;
		}
		//public static void CambiaLingua_BKP(UIControlledApplication a)
		//{
		//	//ResourceSet resourceSet_arrivo;

		//	App.Lingua lingua_attuale = App.lingua_plugin;

		//	//if (lingua_attuale != App.lingua_arrivo)
		//	//{

		//	CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		//	CultureInfo cultureInfo = CultureInfo.DefaultThreadCurrentCulture;			
		//	if (lingua_attuale == App.Lingua.ITA)
		//	{
		//		App.lingua_arrivo = App.Lingua.ENG;
		//		//var langCode = Properties.Settings.Default.languageCode = "en-US";
		//		//Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
		//		cultureInfo = new CultureInfo("en");

		//	}
		//	else
		//	{
		//		App.lingua_arrivo = App.Lingua.ITA;
		//		//var langCode = Properties.Settings.Default.languageCode = "it-IT";
		//		//Properties.Settings.Default.Save();
		//		//Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
		//	}
		//	// Set the current thread's culture to the selected culture
		//	Thread.CurrentThread.CurrentCulture = cultureInfo;
		//	Thread.CurrentThread.CurrentUICulture = cultureInfo;
		//	// Manually reload the resource file for the selected culture
		//	Resources.Lang.lang.Culture = cultureInfo;

		//	AggiornaLinguaRibbon(a);

		//	App.lingua_plugin = App.lingua_arrivo;
		//	//}
		//}

		public static double ConvertiInterne2cm(double interne)
		{
			double cm = 0;
#if (Rel_25 || Rel_21_24 || DEBUG || Debug_25)

			cm = UnitUtils.ConvertFromInternalUnits(interne, UnitTypeId.Centimeters);
#else
			cm = UnitUtils.ConvertFromInternalUnits(interne, DisplayUnitType.DUT_CENTIMETERS); //distanzaControff
#endif
			return cm;
		}


		public static string ConvertiXYZ2cm_stringa(XYZ _p)
		{
			string x_cm = ConvertiInterne2cm(_p.X).ToString("F2");
			string y_cm = ConvertiInterne2cm(_p.Y).ToString("F2");
			string z_cm = ConvertiInterne2cm(_p.Z).ToString("F2");
			return $"{x_cm}, {y_cm}, {z_cm}";
		}


		public static int exIntegerValue(ElementId id)
		{
#if (Rel_19_20 || Rel_21_24)
			return id.IntegerValue;//anche x 24?
#else
			return (int)id.Value; //sarebbe un long...lo converto?
#endif
		}

		public static ElementId exElementId(int id_int)
		{
#if (Rel_19_20 || Rel_21_24)
			return new ElementId(id_int);//anche x 24?
#else
			return new ElementId((long)id_int); //sarebbe un long...lo converto?
#endif
		}


	}

	public static class DebugUtils
	{
		public static void PrintExceptionInfo(Exception ex)
		{
			// Get the method that called this function
			MethodBase callingMethod = new StackTrace().GetFrame(1).GetMethod();

			Debug.WriteLine($"Errore dentro {callingMethod.Name}: {ex.Message}");
			Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
		}
	}
}

