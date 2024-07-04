using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace P3Ribbon.Scripts
{

	[Transaction(TransactionMode.Manual)]
	class LinguaInglese : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			App.lingua_arrivo = App.Lingua.ENG;
			Supporto.CambiaLingua(App.UICapp);

			return Result.Succeeded;

		}
	}

	[Transaction(TransactionMode.Manual)]
	class LinguaItaliano : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			App.lingua_arrivo = App.Lingua.ITA;
			Supporto.CambiaLingua(App.UICapp);

			return Result.Succeeded;

		}
	}

}

