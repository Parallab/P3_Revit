using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3Ribbon.Scripts
{
    class TabellaExcel
    {
        public static List<List<double>> leggitabella(Autodesk.Revit.DB.Document doc)
        {
            IList<Element> proj_infos = new FilteredElementCollector(doc).OfClass(typeof(ProjectInfo)).ToElements();
            Element proj_info = proj_infos[0];

            int ClasseUso = proj_info.LookupParameter("P3_InfoProg_ClasseUso").AsInteger();
            int ZonaSismica = proj_info.LookupParameter("P3_InfoProg_ZonaSismica").AsInteger();

            List<List<double>> tabella_leggera = new List<List<double>>();
            var lines = System.IO.File.ReadAllLines(Par_Sismici.TrovaPercorsoRisorsa("20020_P3_TabelleDiPredimensionamento.txt"));
            for (int i_r = 0; i_r < lines.Length; i_r++)
            {
                List<double> sotto_lista = new List<double>();

                var fields = lines[i_r].Split(';');
                if (fields[1] == ClasseUso.ToString() && fields[3] == ZonaSismica.ToString())
                {
                    for (int i = 1; i < fields.Count(); i++)
                    {
                        string field = fields[i];

                        sotto_lista.Add(double.Parse(field));
                    }
                    tabella_leggera.Add(sotto_lista);
                }
            }
            return tabella_leggera;
        }
    }
}

