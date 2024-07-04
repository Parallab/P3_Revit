using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows;

using Application = Autodesk.Revit.ApplicationServices.Application;

namespace P3Ribbon.Scripts.GUI
{

	/// <summary>
	/// Logica di interazione per UserControl1.xaml
	/// </summary>
	public partial class Wpf_impo : Window
	{
		private Document m_doc;
		private Application m_app;

#if (Rel_25 || Rel_21_24 || DEBUG)
		//private UnitTypeId currentUnit = Supporto.doc.GetUnits().GetFormatOptions(SpecTypeId.Length).;
		ForgeTypeId currentUnit = Supporto.doc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();


		//25/1/24 perchè era qui dentro? dava problemi con l input diverso? provo intanto a tirare fuori
		//public Wpf_impo(ExternalCommandData commandData)
		//{
		//    UIApplication uiApp = commandData.Application;
		//    m_app = uiApp.Application;
		//    UIDocument uiDoc = uiApp.ActiveUIDocument;
		//    m_doc = uiDoc.Document;

		//    InitializeComponent();

		//    this.wpfTexBoxDistazaMaxStaffaggio.Text = UnitUtils.ConvertFromInternalUnits(Condotto.AltezzaStaffaggio,currentUnit).ToString();
		//    this.Units.Text = UnitàDiProgetto();
		//    CheckUpgrade.IsChecked = Properties.Settings.Default.updaterAttivo;

		//}
#else
		public DisplayUnitType currentUnit = Supporto.doc.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits; //private?

		//25/1/24 perchè era qui dentro? dava problemi con l input diverso? provo intanto a tirare fuori
		//  public Wpf_impo(ExternalCommandData commandData)
		//{
		//    UIApplication uiApp = commandData.Application;
		//    m_app = uiApp.Application;
		//    UIDocument uiDoc = uiApp.ActiveUIDocument;
		//    m_doc = uiDoc.Document;

		//    InitializeComponent();

		//    this.wpfTexBoxDistazaMaxStaffaggio.Text = UnitUtils.ConvertFromInternalUnits(Condotto.AltezzaStaffaggio,currentUnit).ToString();
		//    this.Units.Text = UnitàDiProgetto();
		//    CheckUpgrade.IsChecked = Properties.Settings.Default.updaterAttivo;

		//}
#endif

		public Wpf_impo(ExternalCommandData commandData)
		{
			UIApplication uiApp = commandData.Application;
			m_app = uiApp.Application;
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			m_doc = uiDoc.Document;

			InitializeComponent();

			this.wpfTexBoxDistazaMaxStaffaggio.Text = UnitUtils.ConvertFromInternalUnits(Condotto.AltezzaStaffaggio, currentUnit).ToString();
			this.Units.Text = UnitàDiProgetto();
			CheckUpgrade.IsChecked = Properties.Settings.Default.updaterAttivo;

		}

		private void DecimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			bool approvedDecimalPoint = false;

			if (e.Text == ".")
			{
				if (!((System.Windows.Controls.TextBox)sender).Text.Contains("."))
					approvedDecimalPoint = true;
			}

			if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
				e.Handled = true;
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.updaterAttivo == false)
			{
				//App.UpdaterAccendi();
				UpdaterRegistry.EnableUpdater(App.updater.GetUpdaterId());
			}
			Properties.Settings.Default.updaterAttivo = true;
			Properties.Settings.Default.Save();
		}

		private void CheckUpgrade_Unchecked(object sender, RoutedEventArgs e)
		{
			try
			{
				UpdaterRegistry.DisableUpdater(App.updater.GetUpdaterId());
			}
			catch (Exception ex)
			{
				DebugUtils.PrintExceptionInfo(ex);
			}
			Properties.Settings.Default.updaterAttivo = false;
			Properties.Settings.Default.Save();
		}


		public string UnitàDiProgetto()
		{
#if (Rel_25 || Rel_21_24 || DEBUG)
			string rtn = "units";
			if (currentUnit == UnitTypeId.Millimeters)
			{
				rtn = "mm";
			}
			else if (currentUnit == UnitTypeId.Centimeters)
			{
				rtn = "cm";
			}
			else if (currentUnit == UnitTypeId.Decimeters)
			{
				rtn = "dm";
			}
			else if (currentUnit == UnitTypeId.Meters)
			{
				rtn = "m";
			}
			return rtn;
#else
			string rtn = "units";
			if (currentUnit == DisplayUnitType.DUT_MILLIMETERS)
			{
				rtn = "mm";
			}
			else if (currentUnit == DisplayUnitType.DUT_CENTIMETERS)
			{
				rtn = "cm";
			}
			else if (currentUnit == DisplayUnitType.DUT_DECIMETERS)
			{
				rtn = "dm";
			}
			else if (currentUnit == DisplayUnitType.DUT_METERS)
			{
				rtn = "m";
			}
			return rtn;
#endif

		}

		private void wpfbButtImposta_Click(object sender, RoutedEventArgs e)
		{
			double tempAltezzaStaffaggio = double.Parse(this.wpfTexBoxDistazaMaxStaffaggio.Text);
			//double altezzaStaffaggioFeet = UnitUtils.Convert(tempAltezzaStaffaggio, currentUnit , DisplayUnitType.DUT_DECIMAL_FEET);
			double altezzaStaffaggioFeet = UnitUtils.ConvertToInternalUnits(tempAltezzaStaffaggio, currentUnit);
			Condotto.AltezzaStaffaggio = altezzaStaffaggioFeet;
		}
	}
}
