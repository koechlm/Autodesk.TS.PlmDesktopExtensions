using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace InvPlmAddIn.Model
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[ComVisible(true)]
	public class HostObject
	{
		public HostObject(BrowserPanelWindowManager panelManager)
		{
			PanelManager = panelManager;
		}

		public HostObject(DocumentPanelSet documentPanelSet)
		{
			DocumentPanelSet = documentPanelSet;
		}

		public bool IsChanged { get; set; }

		private DocumentPanelSet DocumentPanelSet { get; set; }

		private BrowserPanelWindowManager PanelManager { get; set; }

		public async Task addComponents(object partNumbers)
		{
			var dic = new Dictionary<string, object>
			{
				["Partnumbers"] = CastObjectToStringArray(partNumbers)
			};
			CallILogic("AddComponents", ref dic);
			await Task.CompletedTask;
		}

		public async Task confirmLogin(string a)
		{
			(DocumentPanelSet?.PanelManager ?? PanelManager).UserLoggedIn();
			await Task.CompletedTask;
		}

		/// <summary>
		/// PLM call to query active document; optionally check against contextId
		/// </summary>
		/// <param name="contextId"></param>
		/// <returns>Part Number of Inventor.ActiveDocument</returns>
		public async Task<string> getActiveDocument(string contextId)
		{
			var result = new List<string>();
			var dic = new Dictionary<string, object>
			{
				["contextId"] = contextId
			};

			CallILogic("getActiveDocument", ref dic);
			await Task.FromResult(dic);

			object iLogicResult;
			if (dic.TryGetValue("Result", out iLogicResult) == true)
			{
				return iLogicResult?.ToString();
			}
			else
			{
				return result.ToString();
			}
		}

		public async Task<string> getComponentsLocked(object partNumbers)
		{
			var result = new List<string>();
			var dic = new Dictionary<string, object>
			{
				["Partnumbers"] = CastObjectToStringArray(partNumbers) //let Inventor add the Result object; it is readonly if added here
			};

			CallILogic("GetComponentsLocked", ref dic);
			await Task.FromResult(dic);

			object iLogicResult;
			if (dic.TryGetValue("Result", out iLogicResult) == true)
			{
				return System.Text.Json.JsonSerializer.Serialize((List<string>)iLogicResult).ToString();
			}
			else
			{
				return result.ToString();
			}
		}

		public async Task openComponent(string partNumber)
		{
			var dic = new Dictionary<string, object>
			{
				["Partnumber"] = partNumber
			};
			CallILogic("OpenComponent", ref dic);
			await Task.CompletedTask;
		}

		public async Task selectComponents(object partNumbers)
		{
			BrowserPanelWindowManager.mSelectionSender = "PLM";

			var dic = new Dictionary<string, object>
			{
				["Partnumbers"] = CastObjectToStringArray(partNumbers)
			};

			CallILogic("SelectComponents", ref dic);
			await Task.CompletedTask;

			BrowserPanelWindowManager.mSelectionSender = "Inventor";
		}

		public async Task isolateComponents(object partNumbers)
		{
			BrowserPanelWindowManager.mSelectionSender = "PLM";

			var dic = new Dictionary<string, object>
			{
				["Partnumbers"] = CastObjectToStringArray(partNumbers)
			};

			CallILogic("IsolateComponents", ref dic);
			await Task.CompletedTask;

			BrowserPanelWindowManager.mSelectionSender = "Inventor";
		}

		public async Task setLifecycleState(string folderName, string targetStateName)
		{
			var dic = new Dictionary<string, object>
			{
				["FolderName"] = folderName,
				["TargetStateName"] = targetStateName
			};

			CallILogic("SetLifecycleState", ref dic);
			await Task.CompletedTask;
		}

		public async Task updateProperties(string updateProperties)
		{
			var data = System.Text.Json.JsonSerializer.Deserialize<UpdateProperties[]>(updateProperties, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
			var dic = new Dictionary<string, object>
			{
				["UpdateProperties"] = data
			};

			CallILogic("UpdateProperties", ref dic);

			await Task.CompletedTask;
		}

		private void CallILogic(string rule, ref Dictionary<string, object> dic)
		{
			(DocumentPanelSet?.PanelManager ?? PanelManager)?.CallILogic(rule, ref dic);
		}


		private string[] CastObjectToStringArray(object obj)
			=> (obj as object[])?.Select(x => x.ToString()).ToArray();

		
		/// <summary>
		/// Build assembly context path, e.g. "Assembly:1 | Subassembly1:5 | Part8:2"
		/// </summary>
		/// <returns>Serialized JSON string of iLogic RuleArgument's list object.</returns>
		public async Task<string> getSelectedComponentPaths(object contextId)
		{
			var result = new List<string>();
			var dic = new Dictionary<string, object>
			{
				["contextId"] = CastObjectToStringArray(contextId)
			};

			CallILogic("getSelectedComponentPaths", ref dic);
			await Task.FromResult(dic);

			object iLogicResult;
			if (dic.TryGetValue("Result", out iLogicResult) == true)
			{
				return System.Text.Json.JsonSerializer.Serialize((List<string>)iLogicResult).ToString();
			}
			else
			{
				return result.ToString();
			}
		}

		/// <summary>
		/// PLM call to assign raw material to multi-bodies, or apply Inventor part material
		/// </summary>
		/// <param name="designator"></param>
		/// <returns></returns>
		public async Task assignMaterial(string designator)
		{
			var dic = new Dictionary<string, object>
			{
				["Designator"] = designator
			};

			CallILogic("assignMaterial", ref dic);
			await Task.CompletedTask;
		}

	}
}
