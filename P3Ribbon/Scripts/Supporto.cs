using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
    
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
    }
}
