using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CustomAction
{
    public class CustomActions
    {
        public static string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static string filepath_orig = commonAppData + @"\Autodesk\Revit\Addins\_anno_\P3Ribbon.addin";
        public static string[] anni = new string[] { "2018", "2019", "2020", "2021", "2022", "2023", "2024" };

        [CustomAction]
        public static ActionResult ManifestAddinScrivi(Session session)
        {
            session.Log("Begin CustomAction");
            string input = session["INSTALLFOLDER"];


            //MessageBox.Show(filepath_orig);
            // sarebbe più bello pescare da un file qui dentro visual studio...
            string gnurant = @"<?xml version='1.0' encoding='utf-8' standalone='no'?>
                            <RevitAddIns>
                              <AddIn Type='Application'>
                                <Name>P3Ribbon</Name>
                                <Assembly>pathP3Ribbon.dll</Assembly>
                                <AddInId>e731d642-8170-4362-9836-5413682a4a3e</AddInId>
                                <FullClassName>P3Ribbon.App</FullClassName>
                                <VendorId>Parallab srl</VendorId>
                                <VendorDescription>Parallab</VendorDescription>
                              </AddIn>
                            </RevitAddIns>";
            //MessageBox.Show(gnurant);
            try
            {
                string text = gnurant.Replace("path", input);
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
            { MessageBox.Show("Errore. "+e.Message); }

            return ActionResult.Success;
        }
    }
}
