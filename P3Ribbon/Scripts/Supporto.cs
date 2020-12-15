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

    }
}
