using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3Ribbon.Scripts
{
    class Materiale
    {
        //public static Document doc;
        public static IList<ComboBoxMemberData> comboBoxMemberDatas = new List<ComboBoxMemberData>();
        public static ElementId IdInsulTipoPreferito;
        public static double SpessoreIsolante;

        public static List<MaterialeIsolante> PreAggiorna(Document doc)
        {
            List<MaterialeIsolante> list = new List<MaterialeIsolante>();
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
                                ComboBoxMemberData cmbInsualtionData = new ComboBoxMemberData(el.Id.ToString() + "_" + el.LookupParameter("P3_Insulation_Thickness").AsDouble().ToString(), "" + el.Name);
                                comboBoxMemberDatas.Add(cmbInsualtionData);
                            }
                            _spessore = el.LookupParameter("P3_Insulation_Thickness").AsDouble();
                            list.Add(new MaterialeIsolante() { ID = el.Id, Name = el.Name, Spessore = _spessore });
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
            Scripts.Materiale.IdInsulTipoPreferito = new ElementId(Int32.Parse(_nome.Substring(0, indice_)));
            Scripts.Materiale.SpessoreIsolante = Convert.ToDouble(_nome.Substring(indice_ + 1));
        }

    }

    class MaterialeIsolante
    {
        public ElementId ID { get; set; }
        public string Name { get; set; }
        public double Spessore { get; set; }

    }
}
