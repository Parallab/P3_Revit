using Autodesk.Revit.DB;
using P3Ribbon.Scripts.GUI.SeismicViews;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace P3Ribbon.Scripts.GUI
{
	/// <summary>
	/// Logica di interazione per Wpf_ParamSismici.xaml
	/// </summary>
	public partial class Wpf_ParamSismici : Window, IDisposable
	{
		//assegno valore temporaneo alla classe uso e zona sismica
		public static int zona_wpf = 4;
		public static int classe_wpf = 1;
		public static bool ok_premuto = false;
		public static string accValue = "";

		BrushConverter bc = new BrushConverter();



		public Wpf_ParamSismici()
		{

			InitializeComponent();
			if (App.lingua_plugin == App.Lingua.ITA)
			{
				// DataContext = new GUI.SeismicViewModels.Wpf_ScegliZonaAccModel();
				wpfButtScegliZona.Background = (Brush)bc.ConvertFromString("#FF81B2BF");
				DataContext = new GUI.SeismicViewModels.Wpf_ZonaSismicaModels();
			}
			else
			{
				wpfButtScegliZona.Visibility = System.Windows.Visibility.Hidden;
				wpfButtScegliAcc.Background = (Brush)bc.ConvertFromString("#FF81B2BF");
				DataContext = new GUI.SeismicViewModels.Wpf_AccelerazioneModels();
			}

			DaPojInfoaWpf();
			ColoraBottoniSeParametriCompilati();

		}

		public void CambiaFinestra(bool FinestraAcc)
		{
			if (FinestraAcc == true)
			{
				DataContext = new GUI.SeismicViewModels.Wpf_AccelerazioneModels();
			}
			else
			{
				DataContext = new GUI.SeismicViewModels.Wpf_ZonaSismicaModels();
			}

		}

		private Button TrovaBottoneClasseUso(int c)
		{
			System.Windows.Controls.Button rtn = new Button();
			if (c == 1) rtn = wpfBottCUsoI;
			else if (c == 2) rtn = wpfBottCUsoII;
			else if (c == 3) rtn = wpfBottCUsoIII;
			else if (c == 4) rtn = wpfBottCUsoIV;

			return rtn;
		}



		private void DecimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			bool approvedDecimalPoint = false;

			if (e.Text == ".")
			{
				if (!((TextBox)sender).Text.Contains("."))
					approvedDecimalPoint = true;
			}

			if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
				e.Handled = true;
		}

		private void ColoraBottoniSeParametriCompilati()
		{
			if (classe_wpf != 666)
			{
				Button bc = TrovaBottoneClasseUso(classe_wpf);
				ColoraBottoniClasse(bc, classe_wpf);
			}
			//if (zona_wpf != 666)
			//{
			//   Button bz = TrovaBottoneZona(zona_wpf);
			//    Wpf_Accelerazione.ColoraBottoneZona(bz, zona_wpf);
			//}
		}


		private void ColoraBottoniClasse(System.Windows.Controls.Button bottone, int c)
		{
			//colora i bottoni non selezionati del loro colore originale
			BrushConverter bc = new BrushConverter();
			wpfBottCUsoI.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
			wpfBottCUsoII.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
			wpfBottCUsoIII.Background = (Brush)bc.ConvertFrom("#FF81B2BF");
			wpfBottCUsoIV.Background = (Brush)bc.ConvertFrom("#FF81B2BF");

			//colora solo il bottone selezionato
			bottone.Background = (Brush)bc.ConvertFrom("#FF31788B");
			classe_wpf = c;
		}


		#region Eventi al click dei bottoni ClasseUso e Zona
		//private void WpfbottZona1_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona1, 1);}
		//private void WpfbottZona2_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona2, 2);}
		//private void WpfbottZona3_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona3, 3);}
		//private void WpfbottZona4_Click(object sender, RoutedEventArgs e){ColoraBottoneZona(wpfBottZona4, 4);}
		private void WpfbottCUsoI_Click(object sender, RoutedEventArgs e) { ColoraBottoniClasse(wpfBottCUsoI, 1); }
		private void wpfBottCUsoII_Click(object sender, RoutedEventArgs e) { ColoraBottoniClasse(wpfBottCUsoII, 2); }
		private void WpfbottCUsoIII_Click(object sender, RoutedEventArgs e) { ColoraBottoniClasse(wpfBottCUsoIII, 3); }
		private void WpfbottCUsoIV_Click(object sender, RoutedEventArgs e) { ColoraBottoniClasse(wpfBottCUsoIV, 4); }
		#endregion

		private void wpfBottOk_Click(object sender, RoutedEventArgs e)
		{
			ok_premuto = true;
			ParSismici.classe = classe_wpf;
			ParSismici.zona = zona_wpf;

			accValue = Wpf_Accelerazione.wpftbAcc.Text;
			this.Close();
		}

		private void wpfBottAnnulla_Click(object sender, RoutedEventArgs e)
		{

			this.Close();
		}
		public void Dispose()
		{

		}
		public void DaPojInfoaWpf()
		{
			try
			{
				Element proj_info = new FilteredElementCollector(Supporto.doc).OfClass(typeof(ProjectInfo)).FirstElement();

				Parameter param_classe = proj_info.LookupParameter("P3_InfoProg_ClasseUso");
				Parameter param_zona = proj_info.LookupParameter("P3_InfoProg_ZonaSismica");
				if (param_classe != null && param_zona != null)
				{
					classe_wpf = param_classe.AsInteger();
					zona_wpf = param_zona.AsInteger();
				}
			}
			catch (Exception ex)
			{
				DebugUtils.PrintExceptionInfo(ex);
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void wpfButtScegliZona_Click(object sender, RoutedEventArgs e)
		{
			DataContext = new GUI.SeismicViewModels.Wpf_ZonaSismicaModels();
			ColoraBottoniSeParametriCompilati();
			wpfButtScegliZona.Background = (Brush)bc.ConvertFromString("#FF81B2BF");
			wpfButtScegliAcc.Background = (Brush)bc.ConvertFromString("#3BB5CAFF");
		}

		private void wpfButtScegliAcc_Click(object sender, RoutedEventArgs e)
		{
			DataContext = new GUI.SeismicViewModels.Wpf_AccelerazioneModels();
			wpfButtScegliAcc.Background = (Brush)bc.ConvertFromString("#FF81B2BF");
			wpfButtScegliZona.Background = (Brush)bc.ConvertFromString("#3BB5CAFF");
		}
	}
}
