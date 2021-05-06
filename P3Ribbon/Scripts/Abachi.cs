using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;



namespace P3Ribbon.Scripts
{

    [Transaction(TransactionMode.Manual)]
    class ElencoPezzi : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Element nlist = null;
           

            if (Supporto.ControllaAbachiP3Presenti("P3 - Duct Fitting Schedule - All"))
            {

                ViewSchedule vistaAbacoP3All = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.LookupParameter("P3_Nome_i").AsString() == "P3 - Duct Fitting Schedule - All") as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3All;
            }
            else
            {
                TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
                td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdAbachiNoPresenti;
                td.MainContent = P3Ribbon.Resources.Lang.lang.taskdAbachiCaricare;
                TaskDialogResult result = td.Show();

                GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
                using (wpf)
                {
                    wpf.ShowDialog();
                    Supporto.ChiudiFinestraCorrente(uiDoc);
                }
            }
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class ElencoPunti : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;

            if (Supporto.ControllaAbachiP3Presenti("P3 - Duct Hangers"))
            {


                ViewSchedule vistaAbacoP3pts = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.LookupParameter("P3_Nome_i").AsString() == "P3 - Duct Hangers") as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3pts;

                
            }
            else
            {
                TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
                td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdAbachiNoPresenti;
                td.MainContent = P3Ribbon.Resources.Lang.lang.taskdAbachiCaricare;
                TaskDialogResult result = td.Show();

                GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
                using (wpf)
                {

                    //using (var t = new Transaction(doc, "FinestraInfo"))
                    //{
                    //t.Start();
                    wpf.ShowDialog();
                    //    t.Commit();
                    //}


                    Supporto.ChiudiFinestraCorrente(uiDoc);
                }
            }
            return Result.Succeeded;

        }
    }

    [Transaction(TransactionMode.Manual)]
    class ElencoMateriali : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Application app = uiApp.Application;

            string nome_abaco = "P3 - Duct Insulation Schedule - PLUGIN";
            if (App.lingua_plugin == App.Lingua.ITA)
            {
                nome_abaco += " - ITA";
            }
            else
            {
                nome_abaco += " - ENG";
            }

            if (Supporto.ControllaAbachiP3Presenti(nome_abaco))
            {

                using (var t = new Transaction(doc, "Migra Parametro Area Isolamento"))
                {
                    t.Start();
                    MigraAreaIsolamento.ControllaParametriSeEsistenti(doc, app);
                    if (MigraAreaIsolamento.parPresenti == true)
                    {
                        MigraAreaIsolamento.MigraParaetriIsolamento(doc);
                    }
                    t.Commit();
                }


                ViewSchedule vistaAbacoP3insul = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.LookupParameter("P3_Nome_i").AsString() == nome_abaco) as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3insul;

               
            }
            else
            {
                TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
                td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdAbachiNoPresenti;
                td.MainContent = P3Ribbon.Resources.Lang.lang.taskdAbachiCaricare;
                TaskDialogResult result = td.Show();
                GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
                using (wpf)
                {
                    wpf.ShowDialog();
           
                    Supporto.ChiudiFinestraCorrente(uiDoc);
                }

            }
            return Result.Succeeded;

        }
    }

    [Transaction(TransactionMode.Manual)]
    class ElencoStaffaggio : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Application app = uiApp.Application;
            string nome_abaco = "P3 - Duct Hangers - Components";
            if (App.lingua_plugin == App.Lingua.ITA)
            { 
                nome_abaco += " - ITA"; 
            }
            else 
            {
                nome_abaco += " - ENG";
            }
            if (Supporto.ControllaAbachiP3Presenti(nome_abaco))
            {
                ViewSchedule vistaAbacoP3staff = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).FirstOrDefault(x => x.LookupParameter("P3_Nome_i").AsString() == nome_abaco) as ViewSchedule;
                uiDoc.ActiveView = vistaAbacoP3staff;
            }
            else
            {
                TaskDialog td = new TaskDialog(P3Ribbon.Resources.Lang.lang.taskdErrore);
                td.MainInstruction = P3Ribbon.Resources.Lang.lang.taskdAbachiNoPresenti;
                td.MainContent = P3Ribbon.Resources.Lang.lang.taskdAbachiCaricare;
                TaskDialogResult result = td.Show();

                GUI.Wpf_Libreria wpf = new GUI.Wpf_Libreria(commandData);
                using (wpf)
                {
                    wpf.ShowDialog();
                
                    Supporto.ChiudiFinestraCorrente(uiDoc);
                }
            }
            return Result.Succeeded;

        }
    }

}