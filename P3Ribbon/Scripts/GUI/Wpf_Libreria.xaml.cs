using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.GUI
{
	/// <summary>
	/// Logica di interazione per UserControl1.xa
	/// </summary>
	public partial class Wpf_Libreria : Window, IDisposable
	{
		List<Materiale> list = new List<Materiale>();
		ObservableCollection<Materiale> wpfcboItems = new ObservableCollection<Materiale>();
		private Document _doc;
		private Application _app;
		private UIDocument _UiDoc;
		UIApplication _uiApp;

		public Wpf_Libreria(ExternalCommandData commandData)
		{
			_uiApp = commandData.Application;
			_app = _uiApp.Application;
			_UiDoc = _uiApp.ActiveUIDocument;
			_doc = _UiDoc.Document;

			InitializeComponent();

			//controllare se qualche param globale è gia compilato
			WpfAggiornaLibreria();
			if (wpfCboMateriali.Items.Count == 0)
			{
				string CaricaMat = P3Ribbon.Resources.Lang.lang.wpfPrbpCaricareMat;
				wpfCboMateriali.Items.Add(CaricaMat);
				wpfCboMateriali.SelectedIndex = 0;
			}
		}


		private void WpfBottCaricaLibreria_Click(object sender, RoutedEventArgs e)
		{
			_app.DocumentOpened -= new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);
			// if non è stato già fatto, cioè fare un metodoche controlla se nei tipi del mio documento c è QUEL PARAMETRO NASCOSTO CHE USIAMO X IDENTIFICARE IL TUTTO...
			//{
			using (var t = new Transaction(_doc, "Carica libreria"))
			{
				t.Start();
				TrasferisciStandard.TrasferisciTipiDoc(_app, _doc);  
				WpfAggiornaLibreria();

				App.rbbCboMateriali_Aggiorna();
				////TEMP DA SISTEARE CON BOOLEANI
				//try
				//{
				//    //double test = App.rbbCboMateriali.GetItems().Count;
				//    if (App.rbbCboMateriali.GetItems().Count == 0)
				//    {
				//        App.rbbCboMateriali.AddItems(Materiale.comboBoxMemberDatas);
				//    }
				//}
				//catch
				//{ }
				//}
				t.Commit();
			}
			_app.DocumentOpened += new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(App.Application_DocumentOpened);
		}


		public void WpfAggiornaLibreria()
		{
			wpfcboItems = Materiale.PreAggiorna(_doc);
			if (wpfcboItems != null)
			{
				if (wpfcboItems.Count > 0)
				{
					//items.clear lo si utilizza quando si aggiunge un item con .add
					try
					{
						wpfCboMateriali.Items.Clear();
					}
					catch
					{
					}
					wpfCboMateriali.ItemsSource = null;
					wpfCboMateriali.ItemsSource = wpfcboItems;
					wpfCboMateriali.DisplayMemberPath = "name";

					// combobox ribbon -> combobox wpf
					if (Materiale.IdInsulTipoPreferito == null)
					{
						wpfCboMateriali.SelectedIndex = 1;
					}
					else
					{
						wpfCboMateriali.SelectedIndex = App.rbbCboMateriali.GetItems().IndexOf(App.rbbCboMateriali.Current);
					}
				}
			}
		}


		private void cboMateriali_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void wpfBottScegliMateriale_Click(object sender, RoutedEventArgs e)
		{
			ImpostaMateriale();

			//2024/04/09 provo ad aggiornare già qua il ribbon
			//App.rbbCboMateriali_Aggiorna();


			string nomeInsul_nascosto = "";
			//combobox wpf -> combobox ribbon

			if (App.rbbCboMateriali != null) 
			//if (App.ribbCboMembers != null) //la prima volta che pocarico la libreria parte uesto ma non ho ancora scritto comboboxMembers_ribbon quindi giusto che salti
			{
				try
				{
				nomeInsul_nascosto = Supporto.doc.GetElement(Materiale.IdInsulTipoPreferito).LookupParameter("P3_Nome").AsString();
					
				}
				catch
				{
				}
				//potrebbe stare dentro il try..
				foreach (ComboBoxMember cbm in App.rbbCboMateriali.GetItems())
				{
					string cbm_nome_totale = cbm.Name; 
					if (cbm_nome_totale == nomeInsul_nascosto)
					{
						App.rbbCboMateriali.Current = cbm;
						break;
					}
				}
			}
			this.Close();
		}

		private void ImpostaMateriale()
		{
			Materiale obj = wpfCboMateriali.SelectedItem as Materiale;
			Materiale.IdInsulTipoPreferito = obj.ID;
			Materiale.SpessoreIsolante = obj.spessore;
		}

		public void Dispose()
		{
			//throw new NotImplementedException(); //secondo me va tolto, penso che il dispose serva per cancellare dalla memroai in modo piu o meno sicuro le variabili che non stiamo piu usando, nello specifico noi usimo "using" per "bloccare" le variabili dentr forse anche temporalmente visto che la finestra si chiude da sola...BHOHHHH
		}
	}
}


