﻿using System;


public class CustomAction
{
	
        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {
            session.Log("Begin CustomAction1");
            string origValue = session["INSTALLFOLDER"];
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string path = commonAppData + @"\Autodesk\Revit\Addins\2020.P3Ribbon.addin";

            if (!File.Exists(path))
            {
                string text = File.ReadAllText(path);
                text = text.Replace("[path]", origValue);
                File.WriteAllText(path, text);
            }
            return ActionResult.Success;
        }

    
}
