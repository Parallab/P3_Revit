using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P3Ribbon.Scripts
{

	[Transaction(TransactionMode.Manual)]

	class Staffaggio : IExternalCommand
	{

		public static FamilySymbol fs;
		//4/3/24 questi non dovrebbero essere static?
		//public static List<Element> dclist = new List<Element>();
		//public static List<Condotto> condotti = new List<Condotto>();
		public bool Parametri_presenti = false;
		StringBuilder sb_log = new StringBuilder(); //FORSE COVNIENE INSERIRLO DENTRO ALLA CLASSE CONDOTTO E POI LOGGARE UNO ALLA VOLTA? ALTRIMENTI HO SFASATO TUTTO..

		public static double offset_iniz_cm { get; set; } = 10;



		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiApp = commandData.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiDoc.Document;

			//Controlla se paresenti parametri sismici, in caso contrario chiedi se caricarli
			if (!Supporto.ControllaSePresentiParamSismici())
			{
				//dove è finito il taskdialog? ah è già dentro il metodo
				return Result.Cancelled;
			}
			//Controlla se la famiglia delle staffe è caricata, in caso contrario apri la finestra della libreria
			if (!Supporto.ControllaStaffaPresente())
			{
				TaskDialog td = new TaskDialog(Resources.Lang.lang.taskdErrore);
				td.MainInstruction = Resources.Lang.lang.taskdStaffeNonInserite;
				td.MainContent = Resources.Lang.lang.taskdStaffeCaricarle;
				TaskDialogResult result = td.Show();
				GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
				using (wpf)
				{
					wpf.ShowDialog();
				}
				return Result.Succeeded;
			}




			//Leggi i valori di predimensionamento 
			Supporto.ValoriTabella = Supporto.LeggiTabella(doc);

			//Instanzio la classe condotto creando una lista e di questi filtro quelli verticali
			List<Condotto> Condotti = FiltraCondottiCortiVert(doc, uiDoc); //qua mi fa "return condotti"..
																		   //c'è un po' di confusione tra "condotti" e "Condotti" che dovrebbero essere uguali...




			//vorrei mettere questo sopra ma dovrei sistemare "condotti" vs "Condotti"
			if (Condotti.Count == 0) //era condotti minuscolo
			{
				sb_log.AppendLine(Resources.Lang.lang.taskdErrore_NoCanali);
				//da sistemare con lingua
				TaskDialog td = new TaskDialog(Resources.Lang.lang.taskdErrore);
				td.MainInstruction = Resources.Lang.lang.taskdErrore_NoCanali;
				TaskDialogResult result = td.Show();

				return Result.Cancelled;
			}



			sb_log = new StringBuilder();
			int num_staffe_posizionate = 0;
			using (var t = new Transaction(doc, Resources.Lang.lang.transaction_PosizionaStaffaggio))
			{
				t.Start();
				if (!AttivaFamiglia(doc)) //spostata 1/3/24 perchè serve transizione, non ricordo se l'avevo spostata fuori
				{
					//da sistemare con lingua
					TaskDialog td_FamAttiva = new TaskDialog(Resources.Lang.lang.taskdErrore);
					td_FamAttiva.MainInstruction = Resources.Lang.lang.taskdErrore_AttivazioneFam;
					//td.MainContent = P3Ribbon.Resources.Lang.lang.taskdStaffeCaricarle;
					TaskDialogResult result_FamAttiva = td_FamAttiva.Show();
					t.Dispose(); //serve?
					return Result.Cancelled;
				}
				View3D view3d = TrovaVista3D(doc); //2024/02/06
				if (view3d != null) //2024/04/09 potrei fare if return ma voglio tenere il log alla fine..se no dovrei isnerirlo in altro metodo x far pulizia..
				{


					foreach (Condotto c in Condotti)
					{
						//servirebbe una sorta di log...? //2024/02/21
						sb_log.AppendLine("--------------------------------");
						sb_log.AppendLine($"{Resources.Lang.lang.log_Condotto} : {c.el.Name}");
						sb_log.AppendLine($"{c.largh}x{c.alt}cm");
						sb_log.AppendLine($"ID : {c.el.Id.IntegerValue.ToString()}");
						c.DimensionaDaTabella(doc, sb_log);//classe diversa.. devo "iniettare" sb_log
						c.CalcolaPuntiStaffe(sb_log);//classe diversa.. devo "iniettare" sb_log
						c.TrovaPavimento(doc, view3d, sb_log);//classe diversa.. devo "iniettare" sb_log
						c.PosizionaStaffe(doc, fs, sb_log); //qua non mi interessa loggare, forse solo l id?
						num_staffe_posizionate += c.staffaggi.Count(item => item != null); //non devo contare i null!!!!
					}
					t.Commit();

					//TEMP 2024/02/21
					if (num_staffe_posizionate == 0)
					{
						//da sistemare con lingua
						TaskDialog td_0staffe = new TaskDialog(Resources.Lang.lang.taskdErrore);
						//td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdStaffeNonInserite;
						td_0staffe.MainInstruction = Resources.Lang.lang.taskdErrore_NonPosizionati1 +
							Resources.Lang.lang.taskdErrore_NonPosizionati2;//QUALE VISTA??
																			//td.MainContent = P3Ribbon.Resources.Lang.lang.taskdStaffeCaricarle;
																			//td.MainContent = P3Ribbon.Resources.Lang.lang.taskdStaffeCaricarle;
						TaskDialogResult result_0staffe = td_0staffe.Show();
					}
				}

				//--------------FINE?

				TaskDialog td_fine = new TaskDialog(Resources.Lang.lang.taskdP3StaffCan)
				{
					MainInstruction = $"{Resources.Lang.lang.log_Staffaggio_Output1} : {num_staffe_posizionate} {Environment.NewLine} " +
	 $"{Environment.NewLine}{Resources.Lang.lang.log_Staffaggio_Output2}"
				};

				//td_fine.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,Resources.Lang.lang.log_Staffaggio_Output);
				td_fine.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
				td_fine.DefaultButton = TaskDialogResult.No;
				TaskDialogResult result_fine = td_fine.Show();
				//if (result_fine == TaskDialogResult.CommandLink1)

				if (result_fine == TaskDialogResult.Yes)
				{
					System.Windows.Forms.Form mainForm = new System.Windows.Forms.Form();
					mainForm.Text = "P3 Log";
					// Create a TextBox control for logging
					System.Windows.Forms.TextBox logTextBox = new System.Windows.Forms.TextBox();
					logTextBox.Multiline = true;
					logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
					logTextBox.Location = new System.Drawing.Point(0, 0);
					logTextBox.Size = new System.Drawing.Size(500, 500);
					logTextBox.Dock = System.Windows.Forms.DockStyle.Fill; 
					logTextBox.ReadOnly = true;
					logTextBox.Text = sb_log.ToString();
					mainForm.Controls.Add(logTextBox);
					mainForm.Show();


					//// Create a temporary file to hold the log content
					//string tempFilePath = System.IO.Path.GetTempFileName();
					//System.IO.File.WriteAllText(tempFilePath, sb_log.ToString());

					//try
					//{
					//	// Launch Notepad and open the temporary file
					//	System.Diagnostics.Process.Start("notepad.exe", tempFilePath);
					//}
					//finally
					//{
					//	// Delete the temporary file after Notepad is closed
					//	//System.IO.File.Delete(tempFilePath);
					//}
				}

			}

			Condotti.Clear(); //era minuscolo
			return Result.Succeeded;

		}
		public static IList<Element> Seleziona_condotti(Document doc, UIDocument uiDoc)
		{
			string a1 = P3Ribbon.Resources.Lang.lang.taskdSelezTuttiCondotti;
			string b1 = P3Ribbon.Resources.Lang.lang.taskdSelezCondottiManual;
			TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdP3StaffCan)
			{
				MainInstruction = P3Ribbon.Resources.Lang.lang.taskdSelezionareMod
			};

			td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, a1);
			td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, b1);
			td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, b1);
			TaskDialogResult result = td.Show();

			//Seleziono tutti i condotti nel progetto corrente
			if (result == TaskDialogResult.CommandLink1)
			{
				//int i = 0;
				IList<Element> dc_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).WhereElementIsNotElementType().ToElements();

				////Lista vuota di elementi da riempire
				//List<Element> dclist1 = new List<Element>();

				////in caso usare lo stesso filtro usato per la selezione manuale? forse non si può...e pulire la lista dopo??
				//foreach (Element el in dc_coll)
				//{
				//	ConnectorSet connettors = (el as Duct).ConnectorManager.Connectors;
				//	foreach (Connector c in connettors)
				//	{   //se il connettore ha un profilo Rettangolare allora aggiungi l'elemento alla lista 
				//		if (c.Shape == ConnectorProfileType.Rectangular)
				//		{
				//			i++;
				//			dclist1.Add(el);
				//			break;
				//		}
				//	}
				//}
				#region  Taskdialog quatnità di canali
				//TaskDialog td1 = new TaskDialog("P3 staffaggio canali");
				//td1.MainInstruction = "Nel proggetto corrente ci sono " + dclist1.Count + " condotti";
				//TaskDialogResult result2 = td1.Show();
				#endregion
				//return dclist1;
				return dc_coll;

			}

			//Seleziono i condotti da finestra 
			else if (result == TaskDialogResult.CommandLink2)
			{
				IList<Element> dclist2 = SelSoloCondottiDaFinestra(uiDoc);
				#region taskdialog quantità canali selezionati
				//TaskDialog td2 = new TaskDialog("P3 staffaggio canali");
				//td2.MainInstruction = "hai selezionato " + dclist2.Count + " condotti";
				//TaskDialogResult result2 = td2.Show();
				#endregion
				return dclist2;
			}
			else
			{
				List<Element> ListaVuota = new List<Element>();
				return ListaVuota;
			}

		}
		#region  Funzione che seleziona solo i condotti da finestra con un selection filter     
		public static IList<Element> SelSoloCondottiDaFinestra(UIDocument uidoc)
		{
			ISelectionFilter selFilterdc = new FiltraCondotti();

			IList<Element> dc_coll = uidoc.Selection.PickElementsByRectangle(selFilterdc, Resources.Lang.lang.selezione_condotti);
			return dc_coll;
		}

		public class FiltraCondotti : ISelectionFilter
		{
			public bool AllowElement(Element element)
			{
				//if (element.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM) is null)
				//	return false;
				if (element.Category.Id.IntegerValue == Supporto.doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves).Id.IntegerValue)
					return true;
				//return false; //ho rigirato
				//controllare anche cosa succede per quando si selezionano tutti i canali, e provare ad uniformare, lì avevamo usato questo:
				//oppure "ripulire la lista" dopo la selezione?
				//if ((element as Duct).ConnectorManager is null)
				//	return false;
				//foreach (Connector c in (element as Duct).ConnectorManager.Connectors)
				//{
				//	if (c.Shape != ConnectorProfileType.Rectangular)
				//		return false;
				//}
				return false;
			}

			public bool AllowReference(Reference refer, XYZ point)
			{
				return false;
			}
		}
		#endregion

		//filtro elementi verticali e lunghi < di 250 mm
		public static List<Condotto> FiltraCondottiCortiVert(Document doc, UIDocument uiDoc)
		{

			List<Element> condotti_input = (List<Element>)Seleziona_condotti(doc, uiDoc);
			List<Condotto> condotti_filtrati = new List<Condotto>();
			//int i = 0;
			if (condotti_input.Count == 0)
			{
				return condotti_filtrati; //lista vuota
			}

			bool ignora = false; //nested loops..
			foreach (Element dc in condotti_input)
			{
				//preferisco pulire qui una sola volta vs avere 2 metodi per filtrare tutti i canali o solo quelli selezionati
				if (dc.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM) is null)
					continue;
				if ((dc as Duct).ConnectorManager is null)
					continue;
				//questo avrei potuto farlo come funzione per evitare il loop nidificato.. :
				foreach (Connector c in (dc as Duct).ConnectorManager.Connectors)
				{   //se il connettore NON  ha un profilo Rettangolare allora PASSA AL PROSSIMO. 
					//se ho un canale rett lungo con tanti canali circolari collegati questi si collegano con raccordo rettangoarle in teoria..
					if (c.Shape != ConnectorProfileType.Rectangular)
					{
						ignora = true;
						break;
					}
				}
				if (ignora)
					continue;

				//istanzio il singolo condotto
				Condotto condotto = new Condotto(doc, dc);
				if (Math.Abs(condotto.dir.Z) > 0.9 || condotto.lungh < 25) //4/3/24
				{
					continue;
					//i++;//cosa serve i?per contare quanti ignorati? o per non fare l'if al contrario?
				}
				condotti_filtrati.Add(condotto);
			}
			return condotti_filtrati;



		}
		public static List<XYZ> coordinate = new List<XYZ>();


		public bool AttivaFamiglia(Document doc)
		{
			fs = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).FirstOrDefault(x => x.Name == "P3_DuctHanger") as FamilySymbol;
			if (fs is null)
			{
				fs = new FilteredElementCollector(doc)
				.OfClass(typeof(FamilySymbol))
				.FirstOrDefault(x => x.LookupParameter("P3_Nome")?.AsString() == "P3_DuctHanger") as FamilySymbol; //già questo dovrebbe "attivare" la famiglia
				if (fs is null)
					return false;
			}

			if (!fs.IsActive)
			{
				try
				{
					fs.Activate();
				}
				catch
				{
					return false;
				}
			}
			return true;
		}

		public View3D TrovaVista3D(Document doc)
		{
			sb_log.AppendLine(Resources.Lang.lang.log_Staffaggio_Vista3D1 +
				 Resources.Lang.lang.log_Staffaggio_Vista3D2);
			IList<Element> views3d = new FilteredElementCollector(doc).OfClass(typeof(View3D)).ToElements();
			View3D view3D = null;
			foreach (View3D view in views3d)
			{
				if (view.IsTemplate == false)
				{
					view3D = view;
					sb_log.AppendLine(view.Name);
					break;
				}
			}
			if (view3D == null)
			{
				sb_log.AppendLine(Resources.Lang.lang.log_Staffaggio_Vista3D_No);
			}
			return view3D;
		}
	}

	public class Condotto
	{
		public Element el;
		public ElementId Id;

#if (Rel_25 || Rel_21_24 || DEBUG)
		//2 è l altezza massima?? da controllare!
		public static double AltezzaStaffaggio { get; set; } = UnitUtils.ConvertToInternalUnits(2, UnitTypeId.Meters);
#else
        public static double AltezzaStaffaggio { get; set; } = UnitUtils.ConvertToInternalUnits(2, DisplayUnitType.DUT_METERS);

#endif
		double spiso { get; set; } = 0;
		double spiso_IM { get; set; } = 0;
		public double alt { get; set; } = 0;
		double alt_IM { get; set; } = 0;
		public double largh { get; set; } = 0;
		double largh_IM { get; set; } = 0;
		double per { get; set; } = 0;
		public double lungh { get; set; } = 0;
		double lungh_of1 { get; set; } = 0;
		double angoloRispY { get; set; } = 0;
		int rappT { get; set; }
		public int rappL { get; set; }
		//public int inclinazioneXY = 0;

		public LocationCurve lc;

		public List<XYZ> pts = new List<XYZ>();
		public XYZ dir = XYZ.Zero;
		public XYZ vectorX = XYZ.BasisX;

		#region Valori dimenzionali Excel
		public double InterasseControventoTras = 0;
		public double InterasseControventoLong = 0;
		public double StaffaSupLato = 0;
		public double StaffaSupDist = 0;
		public double ControventoBarre = 0;
		#endregion


		public List<XYZ> ptspavimenti = new List<XYZ>();
		public static List<Element> staffeDaControventare = new List<Element>();
		public List<FamilyInstance> staffaggi = new List<FamilyInstance>(); //sarebbe meglio emlement? in caso castare?

		public Element livello;

		//costruttore
		public Condotto(Document doc, Element _el)
		{
			this.el = _el;
			this.Id = _el.Id;
			this.spiso = CalcolaSpessoreIsolamento(_el, true);
			this.spiso_IM = CalcolaSpessoreIsolamento(_el, false);
			this.alt = CalcolaAltezza(_el, true) + this.spiso;
			this.alt_IM = CalcolaAltezza(_el, false) + this.spiso_IM;
			this.largh = CalcolaLarghezza(_el, true) + this.spiso;
			this.largh_IM = CalcolaLarghezza(_el, false) + this.spiso_IM;
			this.lungh = CalcolaLunghezza(_el);
			this.per = CalcolaPerimetro(alt, largh);
			this.dir = CalcolaDirezione(_el);
			this.livello = CalcolaLivello(doc, _el);
			this.angoloRispY = CalcolaAngolosuY(_el);
			this.lc = _el.Location as LocationCurve;
		}
		#region funzioni che mi calcolano gli attributti belli della classe condotto
		public double CalcolaSpessoreIsolamento(Element dc, Boolean metrico)
		{
			double dc_spiso = dc.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS).AsDouble() * 2;
			if (metrico)
			{
				dc_spiso = Supporto.ConvertiInterne2cm(dc_spiso);
			}
			return dc_spiso;
		}
		public double CalcolaLarghezza(Element dc, Boolean metrico)
		{
			double dc_largh = dc.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).AsDouble();
			if (metrico)
			{
				dc_largh = Supporto.ConvertiInterne2cm(dc_largh);
			}
			return dc_largh;
		}
		public double CalcolaAltezza(Element dc, Boolean metrico)
		{
			double dc_alt = dc.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).AsDouble();
			if (metrico)
			{
				dc_alt = Supporto.ConvertiInterne2cm(dc_alt);
			}
			return dc_alt;
		}
		public double CalcolaLunghezza(Element dc)
		{
			double dc_lungh_IM = dc.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
			double dc_lungh = Supporto.ConvertiInterne2cm(dc_lungh_IM);
			return dc_lungh;
		}
		public double CalcolaPerimetro(double largh, double alt)
		{
			per = (largh * 2 + alt * 2);
			return per;
		}
		public double CalcolaLunghezzaNormalizzata(double x, bool dall_inizio)
		{
			if (dall_inizio)
			{
				lungh_of1 = x / lungh;
			}
			else
			{
				lungh_of1 = (lungh - x) / lungh;
			}
			return lungh_of1;
		}
		public Element CalcolaLivello(Document doc, Element dc)
		{
			Element livello = doc.GetElement(dc.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId());
			return livello;
		}

		public XYZ CalcolaDirezione(Element dc)
		{
			//Leggo le coordinate dei punti inizali e finali del condotto e calcolo la direzione
			LocationCurve Lp = dc.Location as LocationCurve;
			Curve c = Lp.Curve;
			XYZ pt1 = c.GetEndPoint(0);
			XYZ pt2 = c.GetEndPoint(1);
			dir = pt2.Subtract(pt1).Normalize();
			return dir;

		}

		public double CalcolaAngolosuY(Element dc)
		{
			LocationCurve Lp = dc.Location as LocationCurve;
			Curve c = Lp.Curve;
			XYZ pt1 = c.GetEndPoint(0);
			XYZ pt2 = c.GetEndPoint(1);

			angoloRispY = pt1.AngleTo(pt2);

			return angoloRispY;
		}

		public double CalcolaPassoMinMax(bool CalcolaPassoMin)
		{
			double passotemp = 0;
			if (Math.Max(this.alt, this.largh) < 100)
			{
				passotemp = 400;//vert dyn
			}
			else
			{
				passotemp = 200;
			}

			if (CalcolaPassoMin == true)
			{
				double Passomin = (Math.Min(InterasseControventoTras, passotemp));
				return Passomin;
			}
			else
			{
				double Passomax = (Math.Max(InterasseControventoTras, passotemp));
				return Passomax;
			}
		}
		#endregion

		public void CalcolaPuntiStaffe(StringBuilder _sb_log) //valutare se scrivere anche le coordinate...magari con funzione ad hoc
		{
			//pt_iniz
			double passoMin = CalcolaPassoMinMax(true);
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_passo_min} = {passoMin.ToString()} cm");
			double passoMax = CalcolaPassoMinMax(false);
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_passo_max} = {passoMax.ToString()} cm");
			this.rappT = (int)Math.Floor(passoMax / passoMin);
			this.rappL = (int)Math.Floor(InterasseControventoLong / passoMin);
			double offset_iniz_normalizzato = this.CalcolaLunghezzaNormalizzata(Staffaggio.offset_iniz_cm, true);
			pts.Add(this.lc.Curve.Evaluate(offset_iniz_normalizzato, true));
			//_sb_log.AppendLine("Aggiungo il punto iniziale");
			//non devo "debuggare", devo loggare per generare un report..
			//_sb_log.AppendLine($"Lunghezza = {this.lungh.ToString()}");
			if (this.lungh > passoMin)
			{
				//calcolare punti intermedi
				//_sb_log.AppendLine($"La lunghezza del canale è maggiore del passo minimo.");
				int n_suddivisioni_lineari = (int)Math.Ceiling(this.lungh / passoMin);
				for (int i = 1; i < n_suddivisioni_lineari; i++)
				{
					double x = (this.lungh / n_suddivisioni_lineari) * i;

					pts.Add(this.lc.Curve.Evaluate((x / this.lungh), true));
					//_sb_log.AppendLine("Aggiungo un punto intermedio");
				}
			}
			//else
			//{
			//	_sb_log.AppendLine($"La lunghezza del canale è minore o uguale del passo minimo, non servono punto intermedi.");
			//}


			if (this.lungh > 40.666)
			{
				//pt_finale
				pts.Add(this.lc.Curve.Evaluate(this.CalcolaLunghezzaNormalizzata(10, false), true));
				//_sb_log.AppendLine("Aggiungo il punto finale.");
			}
			//else
			//{
			//	_sb_log.AppendLine("La lunghezza è troppo corta, non considero il punto finale.");
			//}
		}


		public void TrovaPavimento(Document doc, View3D view3d, StringBuilder _sb_log)
		{

			//Prima vista3d o quella di defoult, Eccezione se ci non ci sono viste 3d nel progetto?
			//sarebbe da impostare la vista in un menu a tendina anche... 2024/02/06
			// tra l altro questo loop viene eseguito in continuazione, basterebbe all avvio...no?
			//IList<Element> views3d = new FilteredElementCollector(doc).OfClass(typeof(View3D)).ToElements();


			Category p_cat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);

			ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
			_sb_log.AppendLine(Resources.Lang.lang.log_Staffaggio_PavVisibili);
			ReferenceIntersector ri = new ReferenceIntersector(filter, FindReferenceTarget.All, view3d);
			//beccami anche i modelli linkati
			ri.FindReferencesInRevitLinks = true;

			foreach (XYZ pt in pts)
			{
				//_sb_log.Append($"{Resources.Lang.lang.log_Staffaggio_Punto} {pt.X.ToString()}, {pt.Y.ToString()}, {pt.Z.ToString()}	-->	");
				_sb_log.Append($"{Resources.Lang.lang.log_Staffaggio_Punto} {Supporto.ConvertiXYZ2cm_stringa(pt)}	-->	");
				ReferenceWithContext _ref = ri.FindNearest(pt, XYZ.BasisZ);


				if (_ref == null)
				{
					ptspavimenti.Add(null);
					_sb_log.AppendLine(Resources.Lang.lang.log_Staffaggio_Nessuno);
				}
				else
				{
					Reference refel = _ref.GetReference();
					//per log:
					ElementId id_ref = refel.ElementId;
					Element pav_or_link = Supporto.doc.GetElement(id_ref);
					string pav_name = pav_or_link.Name;
					if (pav_or_link is RevitLinkInstance)
					{
						Document link = (pav_or_link as RevitLinkInstance).GetLinkDocument();
						Element link_el = link.GetElement(refel.LinkedElementId);//test
						pav_name = $"{link_el.Name} ({link.Title})";
					}

					if (_ref.Proximity < AltezzaStaffaggio)
					{
						XYZ refp = refel.GlobalPoint;
						ptspavimenti.Add(refp);
						//_sb_log.AppendLine($"{refp.X.ToString()}, {refp.Y.ToString()}, {refp.Z.ToString()}");
						_sb_log.Append(Supporto.ConvertiXYZ2cm_stringa(refp));
					}
					else
					{
						ptspavimenti.Add(null);
						_sb_log.Append($"{Resources.Lang.lang.log_Staffaggio_TroppoLontano} H > 2m");
					}
					_sb_log.AppendLine($"	{pav_name}");
					//_sb_log.AppendLine("");
				}


			}
		}


		public void PosizionaStaffe(Document doc, FamilySymbol _fs, StringBuilder _sb_log) // fa casino perche non è in ordine, forse conviene fare stringbuilder dentro classe condotto?
		{
			_sb_log.AppendLine("");
			XYZ pt;
			XYZ pt_pav;
			FamilyInstance fi;
			int i_L = 0;

			for (int i = 0; i < pts.Count; i++)
			{
				pt = pts[i];
				pt_pav = ptspavimenti[i];

				if (pt_pav == null)
				{
					this.staffaggi.Add(null);
				}
				else
				{
					// posiziona
					fi = doc.Create.NewFamilyInstance(pt, _fs, this.livello, StructuralType.NonStructural);
					this.staffaggi.Add(fi);
					_sb_log.Append($"ID : {fi.Id.IntegerValue.ToString()}");
					// dimensiona
					fi.LookupParameter("P3_Dynamo_Center2Ceiling").Set(Math.Abs(pt.Z - pt_pav.Z));
					fi.LookupParameter("P3_Duct_Width").Set(this.largh_IM);
					fi.LookupParameter("P3_Duct_Height").Set(this.alt_IM);
					//fi.LookupParameter("P3_Duct_Slope").Set(this.angoloRispY);
					// ruota
					Line asseZ = Line.CreateBound(pt, pt.Add(new XYZ(0, 0, 1)));
					double angolo = dir.AngleTo(XYZ.BasisX) * (180 / Math.PI);

					//controllo la direzione e verso
					if ((dir.X < 0 && dir.Y > 0) || (dir.X < 0 && dir.Y < 0))
					{
						ElementTransformUtils.RotateElement(doc, fi.Id, asseZ, dir.AngleTo(XYZ.BasisY));
					}
					else
						ElementTransformUtils.RotateElement(doc, fi.Id, asseZ, -dir.AngleTo(XYZ.BasisY));

					//dipende dalla direzione del condotto, quindi verso quale quadrante si rivolge (di conseguenza dal verso del vettore ovvero i click con cui è stato creato)
					// staffa superiore
					double distanzaControff = fi.LookupParameter("P3_Dynamo_Top2Ceiling").AsDouble();
					distanzaControff = Supporto.ConvertiInterne2cm(distanzaControff);
					if (Math.Max(largh, alt) > StaffaSupLato || distanzaControff > StaffaSupDist)
					{
						fi.LookupParameter("P3_UpperSupport").Set(1);
						_sb_log.Append($"	{Resources.Lang.lang.log_Staffaggio_Sup}");
					}
					else
					{
						fi.LookupParameter("P3_UpperSupport").Set(0);

					}
					// controventamento long e trasv
					if (this.per >= 200 || distanzaControff > this.ControventoBarre)
					{
						//_sb_log.AppendLine($"{this.per} ≥ 200cm");
						// se son vicino ad un 90 gradi non serve longitudinale
						// ma quando parto? non dal 1° ma dal 2°? o dall'rappL-esimo o rappL-esimo + 1?
						// parto da rappL-1....
						if (i == 0 || i == pts.Count - 1)
						{
							if (StaffaVicinaRaccordo90(pt, doc))
							{
								i_L = this.rappL - 1;
							}
						}
						// controvento trasversale
						if (i % this.rappT == 0 || i == pts.Count - 1)
						{
							fi.LookupParameter("P3_Braces_Cross").Set(1);
							_sb_log.Append($"	+	{Resources.Lang.lang.log_Staffaggio_Controv_Trasv}");
						}
						else
						{
							fi.LookupParameter("P3_Braces_Cross").Set(0);
						}
						// controvento longitudinale
						if (i_L % this.rappL == 0)
						{
							fi.LookupParameter("P3_Braces_Longitudinal").Set(1);
							_sb_log.Append($"	+	{Resources.Lang.lang.log_Staffaggio_Controv_Long}");
						}
						else
						{
							fi.LookupParameter("P3_Braces_Longitudinal").Set(0);
						}
						i_L++;
						_sb_log.AppendLine("");
					}
				}
			}

		}
		public bool StaffaVicinaRaccordo90(XYZ pt, Document doc)
		{
			ConnectorSet conns_condotto;
			ConnectorSet conns_collegati;
			Element owner;
			double DirCondotto;

			double angoloRaccordo = -666;
			conns_condotto = (this.el as Duct).ConnectorManager.Connectors;
			foreach (Connector conn_condotto in conns_condotto)
			{
				if (conn_condotto.Origin.DistanceTo(pt) < Staffaggio.offset_iniz_cm * 1.05 / 30.48)
				{
					conns_collegati = conn_condotto.AllRefs;
					foreach (Connector conn_collegato in conns_collegati)
					{
						owner = conn_collegato.Owner;

						if (owner.Category.Id.IntegerValue == doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting).Id.IntegerValue)
						{
							Element e = doc.GetElement(owner.GetTypeId());
							string NameFittingFamily = (e as FamilySymbol).FamilyName;
							if (NameFittingFamily.Contains("P3")
								&& !NameFittingFamily.Contains("deviation")
								&& !NameFittingFamily.Contains("Endcap"))
							{
								//oppure cercare tra le proprietà dell istanza se c è qualcosa che richiama family symbol.

								//potremmo addirittura guardare con un altro if se il raccordo ha un facingorientation con | Z | > 0.1 perche vorrebbe dire che il raccordo è verticale (da testare)
								FamilyInstance fi = owner as FamilyInstance;
								DirCondotto = fi.FacingOrientation.Z;
								if (Math.Abs(DirCondotto) < 0.1)
								{
									angoloRaccordo = 666;
									foreach (Parameter p in owner.Parameters)
									{
										if (p.Definition.Name.Contains("Angle")) //questo perche ogni tanto c è angle sx dx lt rt...
										{
											angoloRaccordo = p.AsDouble() * (180 / Math.PI);
											if (angoloRaccordo > 80) //; 25/1/24 c'era il punto e virgola subito dopo l if!
											{
												return true;
											}
										}

									}
								}

							}
						}

					}

				}

			}
			if (angoloRaccordo > 80) return true;
			else return false;

		}
		public void DimensionaDaTabella(Document doc, StringBuilder _sb_log)
		{
			double per_minimo = 0;
			for (int i_r = 0; i_r < Supporto.ValoriTabella.Count; i_r++)
			{
				List<double> riga = Supporto.ValoriTabella[i_r];

				//per qualche motivo il primo perimetro minimo non è 0 ma 200...quindi se ho perimetro<200 non funziona.. ma prendo lo stesso il primo caso..
				//vorrei aggiornare la tabella, ma vedo che nel 2020 era 0, nel 2021 200...errore o voluto?
				//allora sovrascrivo il perimetro minimo fuori dal loop dicendo =0..
				if (i_r != 0)
					per_minimo = riga[3];

				//controllo in che riga devo stare in base al perimetro.
				if (this.per > per_minimo)
				{
					InterasseControventoTras = riga[4];
					InterasseControventoLong = riga[5];
					StaffaSupLato = riga[6];
					StaffaSupDist = riga[7];
					ControventoBarre = riga[8];
				}
				else
				{
					break;//era giusto il loop precedente
				}
			}
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_Perim} = {this.per.ToString()} ≤ {per_minimo} cm");
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_InterasseTrasv} = {InterasseControventoTras.ToString()} cm");
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_InterasseLong} = {InterasseControventoLong.ToString()} cm");
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_SupLato} = {StaffaSupLato.ToString()} cm");
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_SupDist} = {StaffaSupDist.ToString()} cm");
			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_Controv} = {ControventoBarre.ToString()} cm");
		}

		//public void DimensionaDaTabellaBKP(Document doc, StringBuilder _sb_log)
		//{
		//	for (int i_r = 0; i_r < Supporto.ValoriTabella.Count; i_r++)
		//	{
		//		List<double> riga = Supporto.ValoriTabella[i_r];

		//		if (this.per > riga[3])
		//		{
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_Perim} = {this.per.ToString()} > {riga[3]}");
		//			InterasseControventoTras = riga[4];
		//			InterasseControventoLong = riga[5];
		//			StaffaSupLato = riga[6];
		//			StaffaSupDist = riga[7];
		//			ControventoBarre = riga[8];
		//			//break serve anche qui? forse dovrei girare?

		//			//intanto duplico tutto per il log:
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_InterasseTrasv} = {InterasseControventoTras.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_InterasseLong} = {InterasseControventoLong.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_SupLato} = {StaffaSupLato.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_SupDist} = {StaffaSupDist.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_Controv} = {ControventoBarre.ToString()}");
		//		}
		//		else
		//		{
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_Perim} = {this.per.ToString()}  ≤ {riga[3]}");
		//			riga = Supporto.ValoriTabella[0];
		//			InterasseControventoTras = riga[4];
		//			InterasseControventoLong = riga[5];
		//			StaffaSupLato = riga[6];
		//			StaffaSupDist = riga[7];
		//			ControventoBarre = riga[8];

		//			//intanto duplico tutto per il log:
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_InterasseTrasv} = {InterasseControventoTras.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_InterasseLong} = {InterasseControventoLong.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_SupLato} = {StaffaSupLato.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_SupDist} = {StaffaSupDist.ToString()}");
		//			_sb_log.AppendLine($"{Resources.Lang.lang.log_Staffaggio_Controv} = {ControventoBarre.ToString()}");

		//			//break; //perchè era qui? lo sposto in basso 2024/02/21
		//			break; //no era giusto qui se no non loopa
		//		}

		//		//break;
		//	}


	}
}
