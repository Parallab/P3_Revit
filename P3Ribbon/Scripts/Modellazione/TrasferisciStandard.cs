﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using System.Collections.ObjectModel;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts
{
    [Transaction(TransactionMode.Manual)]
    class TrasferisciStandard : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Application app = uiApp.Application;

            TrasferisciTipiDoc(app, doc);

            return Result.Succeeded;
        }
        public static void TrasferisciTipiDoc(Application app, Document doc)
        {

            List<string> nomiTipiPresenti = new List<string>();

            FilteredElementCollector collTipiPresenti = new FilteredElementCollector(doc).WherePasses(Supporto.CatFilterDuctAndInsul).WhereElementIsElementType();

            //guardo tutti i tipi che mi interessamno presenti nel mio doc
            foreach (ElementType type in collTipiPresenti)
            {
                string nome = type.Name;
                if (nome.StartsWith("P3"))
                {
                    nomiTipiPresenti.Add(nome);
                }
            }


            // guardo i tipi nel documento template
            ICollection<ElementId> IdTipiDaCopiare = new Collection<ElementId>();

            Document docSource = app.OpenDocumentFile(Supporto.TrovaPercorsoRisorsa("P3 - Duct system template20.rte"));
            FilteredElementCollector collTipiRisorsa = new FilteredElementCollector(docSource).WherePasses(Supporto.CatFilterDuctAndInsul).WhereElementIsElementType();
            CopyPasteOptions option = new CopyPasteOptions();
            option.SetDuplicateTypeNamesHandler(new HideAndAcceptDuplicateTypeNamesHandler());

            foreach (ElementType type in collTipiRisorsa)
            {
                string nome = type.Name;
                if (nome.StartsWith("P3"))
                {
                    // contollRE SE ESISTE NEL DOC
                    if (!(nomiTipiPresenti.Contains(nome)))
                    {
                        IdTipiDaCopiare.Add(type.Id);
                    }

                }
            }
            
            //COLLETTORE STAFFE 
            //staffe presenti nel documento
            FilteredElementCollector collStaffeDoc = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).WhereElementIsElementType();
            foreach (var type in collStaffeDoc)
            {
                //da usare poi parametri nascosti
                if (type.Name == "P3_DuctHanger")
                {
                    nomiTipiPresenti.Add(type.Name);
                }

            }

            //staffe presenti nella risorsa
            FilteredElementCollector collStaffeRisorsa = new FilteredElementCollector(docSource).OfCategory(BuiltInCategory.OST_SpecialityEquipment).WhereElementIsElementType();
            foreach (var type in collStaffeRisorsa)
            {
                //da usare poi parametri nascosti
                string typeName = type.Name;
                if (typeName == "P3_DuctHanger")
                {
                    if (!(nomiTipiPresenti.Contains(typeName)))
                    {
                         IdTipiDaCopiare.Add(type.Id);
                        
                    }
                }

            }


            //IMPORTO ABACHI

            //abachi presenti nel documento corrente
            IList<Element> collAbachiPresenti = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().ToElements();
            List<string> nomiAbachiPresenti = new List<string>();



            foreach (Element schedule in collAbachiPresenti)
            {
                string nome = schedule.Name;
                if (nome.StartsWith("P3"))
                {
                    nomiAbachiPresenti.Add(nome);

                }
            }

            //abachi presenti nel documento risorsa
            ICollection<ElementId> collAbachiRisorsa = new Collection<ElementId>();
            IList<Element> AbachiRisorsa = new FilteredElementCollector(docSource).OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().ToElements();

            foreach (Element abaco in AbachiRisorsa)
            {
                string nome = abaco.Name;
                if (nome.StartsWith("P3"))
                {
                    // contollRE SE ESISTE NEL DOC
                    if (!(nomiAbachiPresenti.Contains(nome))) //perchè non va?
                    {
                        collAbachiRisorsa.Add(abaco.Id);
                    }

                }
            }

            //gestione delle eccezioni se sono già presenti i tipi e le viste
            try
            {
                ICollection<ElementId> ids = ICollectionIds_Estendi(IdTipiDaCopiare, collAbachiRisorsa);
                ElementTransformUtils.CopyElements(docSource, ids, doc, Transform.Identity, option);
                ids.Clear();
            }
            catch (Exception ex)
            {
                
            }
            collAbachiRisorsa.Clear();
            IdTipiDaCopiare.Clear();
            docSource.Close(false);
        }

        class HideAndAcceptDuplicateTypeNamesHandler : IDuplicateTypeNamesHandler
        {

            public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
            {
                return DuplicateTypeAction.UseDestinationTypes;
                //e se volessi modificare il nome??
            }
        }
        private static ICollection<ElementId> ICollectionIds_Estendi(ICollection<ElementId> coll1, ICollection<ElementId> coll2)
        {
            ICollection<ElementId> unione = coll1;
            foreach (ElementId id in coll2)
            {
                unione.Add(id);
            }

            return unione;
        }
    }
}