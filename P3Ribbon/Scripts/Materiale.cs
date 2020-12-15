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


        //class MaterialeIsolante // per l elenco dei combobox. va unito sopra!!
        //{


        //questi nomi son da sistemare con quelli sopra
        public ElementId ID { get; set; }
        public string Name { get; set; }
        public double Spessore { get; set; }
        public string Peso { get; set; }
        //public Materiale Materiale { get; set; }


        public static ObservableCollection<Materiale> PreAggiorna(Document doc)
        {
            ObservableCollection<Materiale> list = new ObservableCollection<Materiale>();
            List<ElementId> P3InsulationTypeIds = new List<ElementId>();
            bool tempComBoxCompilato = false;

            P3InsulationTypeIds = (List<ElementId>)new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_DuctInsulations).ToElementIds();
            if (P3InsulationTypeIds.Count != 0) //testare eventuali errori
            {
                double _spessore;
                int i = 0;

                foreach (ElementId id in P3InsulationTypeIds)
                {
                    Element el = doc.GetElement(id);
                    if (el.Name.Contains("P3"))
                    {
                        //i++; // "Combo item " + i
                        try
                        {
                            if (tempComBoxCompilato == false)
                            {
                                //parametro aggiunto nel nuovo tempalte
                                ComboBoxMemberData cmbInsualtionData = new ComboBoxMemberData(el.Id.ToString() + "_" + el.LookupParameter("P3_Insulation_Thickness").AsDouble().ToString(), el.Name);
                                comboBoxMemberDatas.Add(cmbInsualtionData);
                            }
                            _spessore = el.LookupParameter("P3_Insulation_Thickness").AsDouble();
                            //list.Add(new MaterialeIsolante() { ID = el.Id, Name = el.Name, Spessore = _spessore });
                            list.Add(new Materiale() { ID = el.Id, Name = el.Name, Spessore = _spessore });
                        }
                        catch
                        {

                        }
                    }

                }
                tempComBoxCompilato = true;
                return list;
            }
                
            else
            {
                    return null;
            }


        }
        public static void AggiornaTendinaRibbon(string _nome)
        {
            int indice_ = _nome.IndexOf("_");
            //TaskDialog.Show("test", test.Name );
            Materiale.IdInsulTipoPreferito = new ElementId(Int32.Parse(_nome.Substring(0, indice_)));
            Materiale.SpessoreIsolante = Convert.ToDouble(_nome.Substring(indice_ + 1)); //perceh abbiamo fatto questo invece di elggere lo spessore dal tipo dell isoalnate?
        }



    }

 

    
}
