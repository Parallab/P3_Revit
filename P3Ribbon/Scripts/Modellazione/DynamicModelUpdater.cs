using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    public class DynamicModelUpdater : IUpdater
    {
       
        static AddInId _appId;
        static UpdaterId _updaterId;



        public DynamicModelUpdater(AddInId id)
        {
            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("604b1052-f742-4951-8576-c261d1993108"));
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();

            if (Scripts.Materiale.IdInsulTipoPreferito != null)
                {
                foreach (ElementId id in data.GetAddedElementIds())
                {
                    try
                    {
                        Element el = doc.GetElement(id);
                        string nome = "";

                        // CONTOLLARE SE C é P3 NEL NOME OPPURE QUEI PARAMETI NASCOSTI...
                        if (el.GetType() == typeof(Duct))
                        {
                            nome = doc.GetElement(el.GetTypeId()).Name;
                        }
                        else if (el.GetType() == typeof(FamilyInstance))
                        {
                            if (el.Category.Id.IntegerValue == doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting).Id.IntegerValue) //forse metterlo come param globale nella classe di supprto (lo usiamo anche per igra isolamento ma anche nell allow FiltraCondotti : ISelectionFilter
                            {
                                nome = (doc.GetElement(el.GetTypeId()) as ElementType).FamilyName;
                            }
                        }
                        if (nome.Contains("P3")) // IN FUTURO USARE UN PARAMETO (NASCOSTO?)
                        {
                            DuctInsulation.Create(doc, id, Materiale.IdInsulTipoPreferito, Materiale.SpessoreIsolante);
                        }
                    }
                    catch (System.Exception ex)
                    {
                       // TaskDialog.Show("Exception", ex.ToString());
                    }
                }
            }

        }

        #region metodi dell updater
        public string GetAdditionalInformation()
        {
            return "P3 - Applicazione materiale isolante";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }
        public string GetUpdaterName()
        {
            return "P3 - Applicazione materiale isolante";
        }
        #endregion

        public static DynamicModelUpdater _updater = null;
    }

    public class ChiudiUpdater : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

        return Result.Succeeded;
        }
    }


}
