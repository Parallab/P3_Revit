using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace P3Ribbon.Scripts
{
    class Materiale //quello che scelgo
    {
        //public static Document doc;
        public static IList<ComboBoxMemberData> comboBoxMemberDatas = new List<ComboBoxMemberData>();
        public static ElementId IdInsulTipoPreferito;
        public static double SpessoreIsolante;


        //proprietà
        public ElementId ID { get; set; }
        public string name { get; set; }
        public double spessore { get; set; }
        public string peso { get; set; }



        public static ObservableCollection<Materiale> PreAggiorna(Document doc)
        {
            ObservableCollection<Materiale> insulList = new ObservableCollection<Materiale>();
            List<ElementId> P3InsulationTypeIds = new List<ElementId>();


            P3InsulationTypeIds = (List<ElementId>)new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds();

            if (P3InsulationTypeIds.Count != 0) //testare eventuali errori
            {
                double _spessore;
                int i = 0;

                foreach (ElementId id in P3InsulationTypeIds)
                {
                    Element el = doc.GetElement(id);

                    try
                    {
                        string nome = el.LookupParameter("P3_Nome").AsString();
                        if (nome.StartsWith("P3"))
                        {
                            _spessore = el.LookupParameter("P3_Insulation_Thickness").AsDouble();

                            //ComboBoxMemberData cmbInsualtionData = new ComboBoxMemberData(el.Id.ToString() + "_" + _spessore.ToString(), el.Name);
                            // 
                            ComboBoxMemberData cmbInsualtionData = new ComboBoxMemberData(nome, el.Name);
                            comboBoxMemberDatas.Add(cmbInsualtionData);

                            //insulList.Add(new Materiale() { ID = el.Id, name = el.Name, spessore = _spessore });
                            // bisogna modificare anche questo??
                            insulList.Add(new Materiale() { ID = el.Id, name = el.Name, spessore = _spessore });

                        }
                    }
                    catch
                    {
                    }

                }
                return insulList;
            }

            else
            {
                return null;
            }


        }
        public static void AggiornaTendinaRibbon(string _nome)
        {
            //prima concatenavamoa nel nome della tendina un po di info utili (id e spessore...)
            //int indice_ = _nome.IndexOf("_");
            //Materiale.IdInsulTipoPreferito = new ElementId(Int32.Parse(_nome.Substring(0, indice_)));
            //Materiale.SpessoreIsolante = Convert.ToDouble(_nome.Substring(indice_ + 1)); //perceh abbiamo fatto questo invece di elggere lo spessore dal tipo dell isoalnate?

            // ora invece vogliamo cercare il nome tra i tipi di isolante nel progetto, così che tra un document e l altro non ci siano problei di id..
            Materiale.IdInsulTipoPreferito = TrovaIdIsolante(_nome); //cerca tra tutti i tipi l id di questo nome;
            Materiale.SpessoreIsolante = TrovaSpessore(Materiale.IdInsulTipoPreferito); //leggi spessore da tipo ;
        }
        private static ElementId TrovaIdIsolante(string _nome_nascosto)
        {
            ElementId rtn = null;
            //seleziona tutti itipi presenti nel modello
            // spero che supporto doc veng aggiornato quando cmbio documento..
            List<ElementId> P3InsulationTypeIds = (List<ElementId>)new FilteredElementCollector(Supporto.doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds();

            // per ciascuno leggi il parmaetro nascosto e controla se è uguale a quello di input
            foreach (ElementId id in P3InsulationTypeIds)
            {
                Element el = Supporto.doc.GetElement(id);

                try
                {
                    string nome = el.LookupParameter("P3_Nome").AsString();
                    if (nome == _nome_nascosto)
                    {
                        rtn = id;
                        break;
                    }
                }
                catch
                {
                    // se non trva niente è perchè non ci sono neanche i parametri, devo caricare la libreria
                    // aprire finestra..
                    //////////////
                    //più semplice : faccio avviso
                    TaskDialog.Show(Resources.Lang.lang.taskdErrore, Resources.Lang.lang.wpfPrbpCaricareMat);
                    break;
                }
            }
            return rtn;
        }
        private static double TrovaSpessore(ElementId id)
        {
            // speriamo che supporto doc venga aggiornato..
            double _spessore = 0.1;
            try
            {
                if (id != null)
                {
                    Element el = Supporto.doc.GetElement(id);
                    _spessore = el.LookupParameter("P3_Insulation_Thickness").AsDouble();
                }
            }
            catch
            {

            }
            return _spessore;

        }

    }




}
