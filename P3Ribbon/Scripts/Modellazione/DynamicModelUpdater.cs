﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;

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
			_updaterId = new UpdaterId(_appId, new Guid("e731d642-8170-4362-9836-5413682a4a3e"));
		}
		public void Execute(UpdaterData data)
		{
			Document doc = data.GetDocument();
			Supporto.AggiornaDoc(doc); //potrebbe aiutare
			if (Materiale.IdInsulTipoPreferito != null)
			{
				foreach (ElementId id in data.GetAddedElementIds())
				{
					try
					{
						Element el = doc.GetElement(id);
						string nome = "";
						double spiso = 0;
						Element tempEl = doc.GetElement(el.GetTypeId());
						Parameter pSpIso = el.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS);

						if (pSpIso != null)
							spiso = pSpIso.AsDouble();

						if (spiso == 0)
						{
							if (tempEl != null)
							{
								nome = doc.GetElement(el.GetTypeId()).LookupParameter("P3_Nome").AsString();
							}
							if (nome.Contains("P3"))
							{
								DuctInsulation.Create(doc, id, Materiale.IdInsulTipoPreferito, Materiale.SpessoreIsolante);
							}
						}


					}
					catch (Exception ex)
					{
						DebugUtils.PrintExceptionInfo(ex);
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
