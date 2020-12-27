﻿using System;
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


    [Transaction(TransactionMode.Manual)]


    class Staffaggio : IExternalCommand
    {
        //Scripts.Form_Def_Acc frm = new Scripts.Form_Def_Acc();
        public static FamilySymbol fs;
        public static List<Element> dclist = new List<Element>();
        public static List<Condotto> condotti = new List<Condotto>();
        public bool Parametri_presenti = false;
        public static double offset_iniz_cm = 10;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            //Application app = uiApp.Application;

            //verifica presenza parametri, ovvero ricicla bottone già fatto
            //ControllaParametri(doc, app);

            if (Supporto.ControllaSePresentiParamSismici())
            {
                Supporto.ValoriTabella = TabellaExcel.LeggiTabella(doc);
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
            }
            else
            {
                return Result.Cancelled;

            }
            condotti.Clear();
            return Result.Succeeded;

        }
        public static IList<Element> Seleziona_condotti(Document doc, UIDocument uiDoc)
        {
            string a1 = "Seleziona tutti i condotti all'interno del progetto Revit corrente";
            string b1 = "Selezione manuale da schermo";
            TaskDialog td = new TaskDialog("P3 staffaggio canali")
            {
                MainInstruction = "Selezionare la modalità di input"
            };

            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, a1);
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, b1);
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, b1);
            TaskDialogResult result = td.Show();

            //Seleziono tutti i condotti nel progetto corrente
            if (result == TaskDialogResult.CommandLink1)
            {
                int i = 0;
                IList<Element> dc_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).WhereElementIsNotElementType().ToElements();
                
                //ElementClassFilter DuctFamilyFilter = new ElementClassFilter(typeof(FamilyInstance));
                //FilteredElementCollector FamilyCollector = new FilteredElementCollector(doc);
                //ICollection<Element> AllFamilies = FamilyCollector.WherePasses(DuctFamilyFilter).ToElements();
               
                //lista vuota di famiglie rettangolari
                ICollection<Element> RectangularDuctFamily = new List<Element>();
                
                //Lista vuota di elementi 
                List<Element> dclist1 = new List<Element>();

               

                //posso filtrare direttamente le famiglie invece di falo elemento per elemento?
                //leggo che è più oneroso interrogare i parametri delle famiglie..ne vale la pena??..le filtro per nome?(non mi piace)
                foreach (Element el in dc_coll)
                {
                    ConnectorSet connettors = (el as Duct).ConnectorManager.Connectors;
                    foreach (Connector c in connettors)
                    {
                        if (c.Shape == ConnectorProfileType.Rectangular)
                        {
                    i++;
                    dclist1.Add(el);
                            break;
                        }
                    }
                }

                //TaskDialog td1 = new TaskDialog("P3 staffaggio canali");
                //td1.MainInstruction = "Nel proggetto corrente ci sono " + dclist1.Count + " condotti";
                //TaskDialogResult result2 = td1.Show();
                return dclist1;

            }
            //Seleziono i condotti da finestra 
            else if (result == TaskDialogResult.CommandLink2)
            {
                IList<Element> dclist2 = SelSoloCondottiDaFinestra(uiDoc);

                //TaskDialog td2 = new TaskDialog("P3 staffaggio canali");
                //td2.MainInstruction = "hai selezionato " + dclist2.Count + " condotti";
                //TaskDialogResult result2 = td2.Show();
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
                double catId = element.Category.Id.IntegerValue;

                if (catId == Supporto.doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves).Id.IntegerValue)
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
                //TaskDialog td = new TaskDialog("P3 staffaggio canali");
                //td.MainInstruction = "sono stati indivituati " + i + " canali verticali o troppo corti";
                //TaskDialogResult result = td.Show();
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
            //eccezione se non c'è
            Element StaffaP3 = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).FirstOrDefault(x => x.Name == "P3_DuctHanger");
            int id = StaffaP3.Id.IntegerValue;
            fs = doc.GetElement(new ElementId(id)) as FamilySymbol;
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
        public double angoloRispY = 0;
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
            this.angoloRispY = CalcolaAngolosuY(_el);
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

        public double CalcolaAngolosuY(Element dc)
        {
            LocationCurve Lp = dc.Location as LocationCurve;
            Curve c = Lp.Curve;
            XYZ pt1 = c.GetEndPoint(0);
            XYZ pt2 = c.GetEndPoint(1);

            angoloRispY = pt1.AngleTo(pt2);
            //double xp1 = pt1.X;
            //double yp1 = pt1.Y;
            //double xp2 = pt2.X;
            //double yp2 = pt2.Y;

            //double ipo = Math.Sqrt(Math.Pow((xp2 - xp1), 2) + Math.Pow((yp2 - yp1), 2));
            //double diff_y = Math.Abs(yp2 - yp1);

            //double rads = Math.Acos(diff_y / ipo);
            //angoloRispY = rads * (180 / Math.PI);
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
        //public List<Element> TrovaRaccordi90Gradi(Document doc)
        //{
        //    IList<Element> ra_coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctFitting).WhereElementIsNotElementType().ToElements();
        //    //Creo una lista vuota
        //    List<Element> raccordi = new List<Element>();
        //    foreach (Element el in ra_coll)
        //    {
        //        double angolo = el.LookupParameter("P3 - Angle").AsDouble();
        //        double angolo_dx = el.LookupParameter("P3 - Angle_SX").AsDouble();
        //        double angolo_sx = el.LookupParameter("P3 - Angle_DX").AsDouble();

        //        if (angolo == 90 || angolo_dx == 90 || angolo_sx == 90)
        //        {
        //            raccordi.Add(el);
        //        }
        //    }
        //    return raccordi;
        //}
        #endregion

        public void CalcolaPuntiStaffe()
        {
            //pt_iniz
            double passoMin = CalcolaPassoMinMax(true);
            double passoMax = CalcolaPassoMinMax(false);
            this.rappT = (int)Math.Floor(passoMax / passoMin);
            this.rappL = (int)Math.Floor(InterasseControventoLong / passoMin);
            double offset_iniz_normalizzato = this.CalcolaLunghezzaNormalizzata(Staffaggio.offset_iniz_cm, true); //studiare parametri globali??
            pts.Add(this.lc.Curve.Evaluate(offset_iniz_normalizzato, true));

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


                    //RevitLinkInstance linkinstance = (RevitLinkInstance)doc.GetElement(refel.ElementId); //non serve
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
                    }else
                        ElementTransformUtils.RotateElement(doc, fi.Id, asseZ, - dir.AngleTo(XYZ.BasisY));



                    //forse non bisogna ruotare ma agire sulla trasformata? erche sui canali inclinati non a 90° ogni tanto la staffa è ruotata male (cambia il segno). però non possiamo agire manualmente sul segno, dobbiamo trovare un modo automatico. magari controllare anche lo script dynamo piu aggiornato nella cartella "pacchetto 2.1". forse moltiplicare angleTo con una funzione che mi dice se è pos o neg? secondo me in dynamo l ho gia fatto

                    //dipende dalla direzione del condotto, quindi verso quale quadrante si rivolge (quindi dipiende anche dal verso ovvero i click con cui è stato creato)

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
                        i_L++;
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

                        //BuiltInCategory ownnerBuiltin = System.Enum.ToObject(owner.Category.Id, BuiltInCategory.OST_DuctFitting);
                        //Category.GetCategory(doc, BuiltInCategory.OST_DuctFitting
                        //doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting
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
                                            if (angoloRaccordo > 80) ;
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