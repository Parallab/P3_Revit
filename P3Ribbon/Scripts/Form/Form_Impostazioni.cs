using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.Form
{
    public partial class Form_Impostazioni : System.Windows.Forms.Form
           
    {
        private Document m_doc;
        private Application m_app;
        public Form_Impostazioni(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            m_app = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_doc = uiDoc.Document;
            InitializeComponent();
        }

        private void OnOffUpdater_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(OnOffUpdater.SelectedItem == OnOffUpdater.Items[0] )
            {
                Scripts.DynamicModelUpdater updater = new Scripts.DynamicModelUpdater(m_app.ActiveAddInId);
                UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            }
            if (OnOffUpdater.SelectedItem == OnOffUpdater.Items[1])
            {
                Scripts.DynamicModelUpdater updater = new Scripts.DynamicModelUpdater(m_app.ActiveAddInId);
                UpdaterRegistry.RegisterUpdater(updater);
            }

        }

        private void Form_Impostazioni_Load(object sender, EventArgs e)
        {
            OnOffUpdater.Items.Add("Interrompi");
            OnOffUpdater.Items.Add("Riprendi");
        }
    }
}
