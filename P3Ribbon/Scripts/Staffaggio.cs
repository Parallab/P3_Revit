using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;


namespace P3Ribbon.Scripts
{
    class Supporto
    {
        public static List<List<double>> ValoriTabella;

    }

    [Transaction(TransactionMode.Manual)]


    class Staffaggio : IExternalCommand
    {
        //Scripts.Form_Def_Acc frm = new Scripts.Form_Def_Acc();
        public static FamilySymbol fs;
        public static List<Element> dclist = new List<Element>();
        public static List<Condotto> condotti = new List<Condotto>();
        public bool Parametri_presenti = false;
        public static List<Element> raccordi90;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            //Application app = uiApp.Application;

            //verifica presenza parametri, ovvero ricicla bottone già fatto
            //ControllaParametri(doc, app);


            Supporto.ValoriTabella = TabellaExcel.leggitabella(doc);
            List<Condotto> Condotti = FiltraCondottiCortiVert(doc, uiDoc);
            AttivaFamiglia(doc);

            using (var t = new Transaction(doc, "Posiziona staffaggio"))
            {
                t.Start();
                if (condotti.Count != 0)
                {
                    foreach (Condotto c in Condotti)
                    {
                        c.DimensionaDaTabella(doc);
                        c.CalcolaPuntiStaffe();
                        c.TrovaPavimento(doc);
                        c.PosizionaStaffe(doc, fs);

                    }
                    t.Commit();
                }
                else
                {
                    return Result.Cancelled;
                }
            }
            condotti.Clear();
            return Result.Succeeded;

        }
        public void ControllaParametri(Document doc, Application app)
        {
            IList<Element> proj_infos = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).ToElements();
            Element proj_info = proj_infos[0];

            Parameter Cu = proj_info.LookupParameter("P3_InfoProg_ClasseUso");
            Parameter En = proj_info.LookupParameter("P3_InfoProg_Eng");
            Parameter Vn = proj_info.LookupParameter("P3_InfoProg_VitaNominale");
            Parameter Zs = proj_info.LookupParameter("P3_InfoProg_ZonaSismica");
            if (Cu == null || En == null || Vn == null || Zs == null)
            {
                TaskDialog td = new TaskDialog("Errore");
                td.MainInstruction = "Parametri sismici non inseriti nel progetto";
                td.MainContent = "Parametri sismici non inseriti nel progetto, inserire i parametri sismici?";
                td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;

                TaskDialogResult result = td.Show();

                if (result == TaskDialogResult.Yes)
                {
                    P3Ribbon.Par_Sismici.CreaParametriCondivisi(doc, app);

                    //Par_Sismici.Migra_Parametri_Presenti(doc);
                    //frm.ShowDialog();
                    //if (Form_Def_Acc.ok_premuto == true)
                    //{
                    //	Par_Sismici.Proj_Info_Scrivi_Parametri(Par_Sismici.classe, Par_Sismici.eng, Par_Sismici.vita, Par_Sismici.zona, doc);
                    //}

                    Parametri_presenti = true;
                }
                else
                {
                    Parametri_presenti = false;
                }
            }
        }
        public static IList<Element> Seleziona_condotti(Document doc, UIDocument uiDoc)
        {
            TaskDialog td = new TaskDialog("P3 staffaggio canali");
            td.MainInstruction = "Selezionare la modalità di input";
            string a1 = "Seleziona tutti i condotti all'interno del progetto Revit corrente";
            string b1 = "Selezione manuale da schermo";
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, a1);
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, b1);
            TaskDialogResult result = td.Show();

            //Seleziono tutti i condotti nel progetto corrente
            if (result == TaskDialogResult.CommandLink1)
            {
                int i = 0;
                IList<Element> dc_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).WhereElementIsNotElementType().ToElements();
                //Creo una lista vuota
                List<Element> dclist1 = new List<Element>();
                foreach (Element el in dc_coll)
                {
                    i++;
                    dclist1.Add(el);
                }

                TaskDialog td1 = new TaskDialog("P3 staffaggio canali");
                td1.MainInstruction = "Nel proggetto corrente ci sono " + dclist1.Count + " condotti";
                TaskDialogResult result2 = td1.Show();
                return dclist1;

            }
            //Seleziono i condotti da finestra 
            else if (result == TaskDialogResult.CommandLink2)
            {
                IList<Element> dclist2 = SelSoloCondottiDaFinestra(uiDoc);

                TaskDialog td2 = new TaskDialog("P3 staffaggio canali");
                td2.MainInstruction = "hai selezionato " + dclist2.Count + " condotti";
                TaskDialogResult result2 = td2.Show();
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

            IList<Element> dc_coll = uidoc.Selection.PickElementsByRectangle(selFilterdc, "seleziona solo i condotti");
            return dc_coll;
        }


        public class FiltraCondotti : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {

                if (element.Category.Name == "Condotto")
                {
                    return true;
                }
                return false;
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                return false;
            }
        }
        #endregion

        //filtro elementi verticali e lugni < di 250 mm
        public static List<Condotto> FiltraCondottiCortiVert(Document doc, UIDocument uiDoc)
        {
            dclist = (List<Element>)Seleziona_condotti(doc, uiDoc);
            ;
            int i = 0;
            if (dclist.Count != 0)
            {

                foreach (Element dc in dclist)
                {
                    Condotto condotto = new Condotto(doc, dc);
                    //condizione se verticale o minore di 250 mm
                    if (condotto.dir.Z == 1 || condotto.lungh < 25)
                    {
                        i++;
                    }
                    else
                    {
                        condotti.Add(condotto);
                    }
                }
                TaskDialog td = new TaskDialog("P3 staffaggio canali");
                td.MainInstruction = "sono stati indivituati " + i + " canali verticali o troppo corti";
                TaskDialogResult result = td.Show();
                return condotti;
            }
            else
            {
                return null;
            }

        }
        public static List<XYZ> coordinate = new List<XYZ>();

        public void AttivaFamiglia(Document doc)
        {
            fs = doc.GetElement(new ElementId(2415672)) as FamilySymbol;
            if (!fs.IsActive)
            {
                fs.Activate();
            }
        }

    }

    public class Condotto
    {
        public Element el;
        public ElementId Id;
        public double spiso = 0;
        public double spiso_IM = 0;
        public double alt = 0;
        public double alt_IM = 0;
        public double largh = 0;
        public double largh_IM = 0;
        public double per = 0;
        public double lungh = 0;
        public double lungh_of1 = 0;
        public double inlcinazioneZ = 0; 
        //public double passoMin = 0;
        //public double passoMax = 0;
        public int rappT;
        public int rappL;
        //public int inclinazioneXY = 0;

        public LocationCurve lc;

        public List<XYZ> pts = new List<XYZ>();
        public XYZ dir = XYZ.Zero;
        public XYZ vectorX = XYZ.BasisX;

        ////////////valori dimensionali excel////////////
        public double InterasseControventoTras = 0;
        public double InterasseControventoLong = 0;
        public double StaffaSupLato = 0;
        public double StaffaSupDist = 0;
        public double ControventoBarre = 0;
        ///////////////////////////////////////////////

        public List<XYZ> ptspavimenti = new List<XYZ>();
        public static List<Element> staffeDaControventare = new List<Element>();
        public List<FamilyInstance> staffaggi = new List<FamilyInstance>(); //sarebbe meglio emlement? in caso castare?

        public Element livello;

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
            //this.passoMin = CalcolaPassoMinMax(true);
            //this.passoMax = CalcolaPassoMinMax(false);
            this.dir = CalcolaDirezione(_el);
            this.lc = _el.Location as LocationCurve;
            this.livello = CalcolaLivello(doc, _el);
            //this.inclinazioneXY = CalcolaInclinazioneSuXY();
            this.inlcinazioneZ = CalcolaInclinazione(_el);
        }
        #region funzioni che mi calcolano gli attributti belli della classe condotto
        public double CalcolaSpessoreIsolamento(Element dc, Boolean metrico)
        {
            double dc_spiso = dc.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS).AsDouble() * 2;
            if (metrico)
            {
                dc_spiso = UnitUtils.ConvertFromInternalUnits(dc_spiso, DisplayUnitType.DUT_CENTIMETERS);
            }
            return dc_spiso;
        }
        public double CalcolaLarghezza(Element dc, Boolean metrico)
        {
            double dc_largh = dc.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).AsDouble();
            if (metrico)
            {
                dc_largh = UnitUtils.ConvertFromInternalUnits(dc_largh, DisplayUnitType.DUT_CENTIMETERS);
            }
            return dc_largh;
        }
        public double CalcolaAltezza(Element dc, Boolean metrico)
        {
            double dc_alt = dc.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).AsDouble();
            if (metrico)
            {
                dc_alt = UnitUtils.ConvertFromInternalUnits(dc_alt, DisplayUnitType.DUT_CENTIMETERS);
            }
            return dc_alt;
        }
        public double CalcolaLunghezza(Element dc)
        {
            double dc_lungh_IM = dc.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
            double dc_lungh = UnitUtils.ConvertFromInternalUnits(dc_lungh_IM, DisplayUnitType.DUT_CENTIMETERS);
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
        public double CalcolaInclinazione(Element dc)
        {
            double inclinazione = dc.get_Parameter(BuiltInParameter.RBS_DUCT_SLOPE).AsDouble();
            double inclinazioneZ = Math.Atan(inclinazione);
            return inlcinazioneZ;
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

        public List<Element> TrovaRaccordi90Gradi(Document doc)
        {
            IList<Element> ra_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctFitting).WhereElementIsNotElementType().ToElements();
            //Creo una lista vuota
            List<Element> raccordi = new List<Element>();
            foreach (Element el in ra_coll)
            {
                double angolo = el.LookupParameter("P3 - Angle").AsDouble();
                double angolo_dx = el.LookupParameter("P3 - Angle_SX").AsDouble();
                double angolo_sx = el.LookupParameter("P3 - Angle_DX").AsDouble();

                if (angolo == 90 || angolo_dx == 90 || angolo_sx == 90)
                {
                    raccordi.Add(el);
                }
            }
            return raccordi;
        }
        #endregion


        public void CalcolaPuntiStaffe()
        {
            //pt_iniz
            double passoMin = CalcolaPassoMinMax(true);
            double passoMax = CalcolaPassoMinMax(false);
            this.rappT = (int)Math.Floor(passoMax / passoMin);
            this.rappL = (int)Math.Floor(InterasseControventoLong / passoMin);
            double offset_iniz = this.CalcolaLunghezzaNormalizzata(10, true);
            pts.Add(this.lc.Curve.Evaluate(offset_iniz, true));

            if (this.lungh > passoMin)
            {
                //calcolare punti intermedi
                // stiamo ignorando l'offset sia nel calcolo delle suddivisioni(facile) che nel calcolo dei punti (più incasinato)
                int n_suddivisioni_lineari = (int)Math.Ceiling(this.lungh / passoMin);
                for (int i = 1; i < n_suddivisioni_lineari; i++)
                {
                    double x = (this.lungh / n_suddivisioni_lineari) * i;

                    pts.Add(this.lc.Curve.Evaluate((x / this.lungh), true));
                }
            }

            if (this.lungh > 40.666)
            {
                //pt_finale
                pts.Add(this.lc.Curve.Evaluate(this.CalcolaLunghezzaNormalizzata(10, false), true));
            }
        }

        public void TrovaPavimento(Document doc)
        {
            View3D view3d;
            //Prima vista3d o quella di defoult, Eccezione se ci non ci sono viste 3d nel progetto?
            view3d = new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>().FirstOrDefault();

            Category p_cat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            ReferenceIntersector ri = new ReferenceIntersector(filter, FindReferenceTarget.All, view3d);
            //beccami anche i modelli linkati
            ri.FindReferencesInRevitLinks = true;

            foreach (XYZ pt in pts)
            {
                ReferenceWithContext _ref = ri.FindNearest(pt, XYZ.BasisZ);
                if (_ref == null)
                {
                    ptspavimenti.Add(null);
                }
                else
                {
                    Reference refel = _ref.GetReference();
                    RevitLinkInstance linkinstance = (RevitLinkInstance)doc.GetElement(refel.ElementId);
                    XYZ refp = refel.GlobalPoint;
                    ptspavimenti.Add(refp);
                }
            }
        }

        public void PosizionaStaffe(Document doc, FamilySymbol _fs)
        {
            XYZ pt;
            XYZ pt_pav;
            FamilyInstance fi;
            int i_L;
            for (int i = 0; i < pts.Count; i++)
            {
                i_L = i;
                pt = pts[i];
                pt_pav = ptspavimenti[i];

                if (pt_pav == null)
                {
                    // cosa vogliamo fare?
                    this.staffaggi.Add(null);
                }
                else
                {
                    // posiziona
                    fi = doc.Create.NewFamilyInstance(pt, _fs, this.livello, StructuralType.NonStructural);
                    this.staffaggi.Add(fi);
                    // dimensiona
                    fi.LookupParameter("P3_Dynamo_Center2Ceiling").Set(Math.Abs(pt.Z - pt_pav.Z));
                    fi.LookupParameter("P3_Duct_Width").Set(this.largh_IM);
                    fi.LookupParameter("P3_Duct_Height").Set(this.alt_IM);
                    fi.LookupParameter("P3_Duct_Slope").Set(this.inlcinazioneZ);
                    // ruota
                    Line asseZ = Line.CreateBound(pt, pt.Add(new XYZ(0, 0, 1)));
                    ElementTransformUtils.RotateElement(doc, fi.Id, asseZ, dir.AngleTo(XYZ.BasisY));
                    // staffa superiore
                    double distanzaControff = fi.LookupParameter("P3_Dynamo_Top2Ceiling").AsDouble();
                    distanzaControff = UnitUtils.ConvertFromInternalUnits(distanzaControff, DisplayUnitType.DUT_CENTIMETERS);
                    if (Math.Max(largh, alt) > StaffaSupLato || distanzaControff > StaffaSupDist)
                    {
                        fi.LookupParameter("P3_UpperSupport").Set(1);
                    }
                    else
                    {
                        fi.LookupParameter("P3_UpperSupport").Set(0);
                    }
                    // controventamento long e trasv
                    if (this.per >= 200 || distanzaControff > this.ControventoBarre)
                    {

                        // se son vicino ad un 90 gradi non serve longitudinale
                        // ma quando parto? non dal 1° ma dal 2°? o dall'rappL-esimo o rappL-esimo + 1?
                        // parto da rappL-1....
                        if (StaffaVicinaRaccordo90(pt))
                        {
                            i_L += this.rappL - 1;
                        }
                        // controvento trasversale
                        if (i % this.rappT == 0 || i == pts.Count - 1)
                        {
                            fi.LookupParameter("P3_Braces_Cross").Set(1);
                        }
                        else
                        {
                            fi.LookupParameter("P3_Braces_Cross").Set(0);
                        }
                        // controvento longitudinale
                        if (i_L % this.rappL == 0)
                        {
                            fi.LookupParameter("P3_Braces_Longitudinal").Set(1);
                        }
                        else
                        {
                            fi.LookupParameter("P3_Braces_Longitudinal").Set(0);
                        }

                    }
                }
            }

        }
 
        public bool StaffaVicinaRaccordo90(XYZ pt)
        {
            // INVECE DI SLEZIONARE TUTI I RACCORDI DEVO:
            // 1) partire dal condotto
            // 2) mi leggo i connettori con : .ConnectorManager.Connectors
            // 3) per ogni connettore guardo l'owner (se ha P3 nel nome ma non "deviation" o "endcap" (ma non è bello perche se qualcuno rinomina le famiglie non funziona più. usare i codici interni? ToDo))
            // 4) se l'origine del connettore è vicina al punto pt (in cui posiziono la staffa)
            // 5) se il parametro angle (o un parametro che contiene "angle" con il loop già scritto)
            // 6) se l'angle é 666 (nullo) oppure maggiore di 80 allora ritorna true.

            ConnectorSet conns_condotto;
            ConnectorSet conns_collegati;
            Element owner;
            double angoloRaccordo = 666;
            conns_condotto = (this.el as Duct).ConnectorManager.Connectors;
            foreach (Connector conn_condotto in conns_condotto)
            {
                // dobbiamo cercare i connettori collegati, perche voglio quello del rfaccordo
                // prima guardo il connettore vicino
                //dovrebbe essere l'offset iniziale (quei 100mm) piu qualche otllerana, occhio alle unità di misura
               if (conn_condotto.Origin.DistanceTo(pt) < 5)
                {
                    conns_collegati = conn_condotto.AllRefs;
                    foreach (Connector conn_collegato in conns_collegati)
                    {
                        owner = conn_collegato.Owner;
                        //non riesco a leggere in nome P3 da family etc..
                        if  (owner.Category.Name == "Raccordi condotto" )
                        {
                            foreach (Parameter p in owner.Parameters)
                            {
                                if (p.Definition.Name.Contains("Angle")) //questo perche ogni tanto c è angle sx dx lt rt...
                                {
                                    angoloRaccordo = owner.LookupParameter("P3 - Angle").AsDouble();
                                    double angologradi = angoloRaccordo * 180 / Math.PI;
                                    if (angologradi  > 80 || angoloRaccordo == 0);
                                    {
                                        return true;
                                    }
                                }
                                
                            }
                        }
                        
                    }
                     
                }
                
            }
            return false;
        }
        public void DimensionaDaTabella(Document doc)
        {
            for (int i_r = 0; i_r < Supporto.ValoriTabella.Count; i_r++)
            {
                List<double> riga = Supporto.ValoriTabella[i_r];

                if (this.per > riga[3])
                {
                    InterasseControventoTras = riga[4];
                    InterasseControventoLong = riga[5];
                    StaffaSupLato = riga[6];
                    StaffaSupDist = riga[7];
                    ControventoBarre = riga[8];
                }
            }
        }
    }
}
