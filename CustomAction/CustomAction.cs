namespace CustomAction
{
	public class CustomActions
	{
		public static string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
		public static string filepath_orig = commonAppData + @"\Autodesk\Revit\Addins\_anno_\P3Ribbon.addin";
		//C:\ProgramData\Autodesk\Revit\Addins

#if (Rel_25 || Debug_25)
		public static string[] anni = new string[] { "2025" };
#elif (Rel_21_24 || DEBUG)
		public static string[] anni = new string[] { "2021", "2022", "2023", "2024" };
#else
		public static string[] anni = new string[] { "2019", "2020" }; //2018 tolto
#endif

		[CustomAction]
		public static ActionResult ManifestAddinScrivi(Session session)
		{
			session.Log("Begin CustomAction");

#if (Rel_25 || Debug_25)
			string percorsodll = session["INSTALLFOLDER"] + "P3Ribbon_2025.dll";
#elif (Rel_21_24 || DEBUG)
			//dovrei sistemare questa key..non capisco da dove venga?
			string percorsodll = session["INSTALLFOLDER"]+"P3Ribbon_2021-24.dll";
			//string input = session["V2021"];
#else
			string percorsodll = session["INSTALLFOLDER"] + "P3Ribbon_2019-20.dll";
			//string input = session["V2020"];
#endif


			//MessageBox.Show(filepath_orig);
			// sarebbe più bello pescare da un file qui dentro visual studio...
			//			string xml_addin = @"<?xml version='1.0' encoding='utf-8' standalone='no'?>
			//<RevitAddIns>
			//	<AddIn Type='Application'>
			//		<Name>P3Ribbon</Name>
			//		<Assembly>_path_P3Ribbon.dll</Assembly>
			//		<AddInId>e731d642-8170-4362-9836-5413682a4a3e</AddInId>
			//		<FullClassName>P3Ribbon.App</FullClassName>
			//		<VendorId>Parallab srl</VendorId>
			//		<VendorDescription>Parallab</VendorDescription>
			//	</AddIn>
			//</RevitAddIns>";

			string xml_addin = @"<?xml version='1.0' encoding='utf-8' standalone='no'?>
<RevitAddIns>
	<AddIn Type='Application'>
		<Name>P3Ribbon</Name>
		<Assembly>_path_</Assembly>
		<AddInId>e731d642-8170-4362-9836-5413682a4a3e</AddInId>
		<FullClassName>P3Ribbon.App</FullClassName>
		<VendorId>Parallab srl</VendorId>
		<VendorDescription>Parallab</VendorDescription>
	</AddIn>
</RevitAddIns>";
			//MessageBox.Show(gnurant);
			try
			{
				string text = xml_addin.Replace("_path_", percorsodll);
				//string text = xml_addin.Replace("_path_", input); //P3Ribbon.dll

				//MessageBox.Show(text);
				foreach (string anno in anni)
				{

					//MessageBox.Show("Considero l'anno " + anno);
					//string filepath_i = filepath_orig;
					//MessageBox.Show(filepath_i);
					string filepath_i = filepath_orig.Replace("_anno_", anno);
					string directory_i = filepath_i.Replace(@"\P3Ribbon.addin", "");
					if (!Directory.Exists(directory_i))
					{
						Directory.CreateDirectory(directory_i);
					}
					//MessageBox.Show(filepath_i);
					File.WriteAllText(filepath_i, text);
				}
			}
			catch (Exception e)
			{ MessageBox.Show("Errore. " + e.Message); }

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult ManifestAddinCancella(Session session)
		{
			//MessageBox.Show("inizio a cancellare");
			session.Log("Begin CustomAction");
			try
			{

				foreach (string anno in anni)
				{
					//MessageBox.Show("Considero l'anno " + anno);
					//string filepath_i = filepath_orig;
					//MessageBox.Show(filepath_i);
					string filepath_i = filepath_orig.Replace("_anno_", anno);
					//MessageBox.Show(filepath_i);
					if (File.Exists(filepath_i))
					{
						File.Delete(filepath_i);
					}
				}
			}
			catch (Exception e)
			{ MessageBox.Show("Errore. " + e.Message); }

			return ActionResult.Success;
		}
	}
}
