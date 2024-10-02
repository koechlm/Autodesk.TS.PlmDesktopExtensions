using Inventor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Autodesk.iLogic.Automation;
using Autodesk.iLogic.Interfaces;

namespace InvPlmAddIn.Utils
{
	public static class iLogicUtil
	{
		public static void ExecuteILogicRule(Application app, Document document, string rulename)
		{
			var dic = new Dictionary<string, object>();
			ExecuteILogicRule(app, document, rulename, ref dic);
		}

		public static void ExecuteILogicRule(Application app, Document document, string rulename, ref Dictionary<string, object> args)
		{
			const string iLogicAddinGuid = "{3BDD8D79-2179-4B11-8A5A-257B1C0263AC}";
			var addin = GetAddIn(app, iLogicAddinGuid);

			if (addin == null)
			{
				Debug.WriteLine("ILogic mAddIn not found.");
				throw new ApplicationException("ILogic mAddIn not found.");
			}

			var addInActiv = addin.Activated;

			if (!addin.Activated)
				addin.Activate();

			Autodesk.iLogic.Automation.iLogicAutomation _iLogicAutomation = (iLogicAutomation)addin.Automation;

			if (args?.Any() == true)
			{
				var nvm = app.TransientObjects.CreateNameValueMap();
				foreach (var a in args)
					nvm.Add(a.Key, a.Value);
				_iLogicAutomation.RunExternalRuleWithArguments(document, rulename, nvm);

				//check for a returned result
				for (int i = 1; i <= nvm.Count; i++)
				{
					if (nvm.Name[i] == "Result")
					{
						args.Add(nvm.Name[i], nvm.Item[i]);
					}
				}
			}
			else
				_iLogicAutomation.RunExternalRule(document, rulename);

			if (!addInActiv)
				addin.Deactivate();
		}

		public static Dictionary<string, List<Inventor.ComponentOccurrence>> iLogicOccCache(Application app, Document document)
		{
			const string iLogicAddinGuid = "{3BDD8D79-2179-4B11-8A5A-257B1C0263AC}";
			var addin = GetAddIn(app, iLogicAddinGuid);

			if (addin == null)
			{
				Debug.WriteLine("ILogic mAddIn not found.");
				throw new ApplicationException("ILogic mAddIn not found.");
			}

			if (!addin.Activated)
				addin.Activate();

			Autodesk.iLogic.Automation.iLogicAutomation iLogicAutomation = (iLogicAutomation)addin.Automation;

			var args = app.TransientObjects.CreateNameValueMap();
			iLogicAutomation.RunExternalRuleWithArguments(document, "GetOccCache", args);

			for (int i = 1; i <= args.Count; i++)
			{
				if (args.Name[i] == "Result")
				{
					//toDo: check that the PLM Addin Inventor.Interop library version matches the installed Inventor version! The object type comparison will fail, if the dll update level differs!
					if (args.Item[i] is Dictionary<string, List<ComponentOccurrence>>)
					{
						Dictionary<string, List<Inventor.ComponentOccurrence>> keyValuePairs = (Dictionary<string, List<ComponentOccurrence>>)args.Item[i];
						return keyValuePairs;
					}
				}
			}

			return null;
		}

		private static ApplicationAddIn GetAddIn(Inventor.Application application, string guid)
		{
			try
			{
				return application.ApplicationAddIns.ItemById[guid];
			}
			catch
			{
			}
			return null;
		}
	}
}