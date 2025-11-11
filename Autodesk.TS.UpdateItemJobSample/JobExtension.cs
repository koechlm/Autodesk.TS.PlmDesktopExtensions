using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.Connectivity.JobProcessor.Extensibility;
using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: ApiVersion("18.0")]
[assembly: ExtensionId("3c9a58ef-2adf-42b5-b94e-df59605833b7")]


namespace Autodesk.TS.UpdateItemJobSample
{
    /// <summary>
    /// Job handler sample to update Vault and Fusion Manage Items
    /// </summary>
    public class JobExtension : IJobHandler
    {
        private static string JOB_TYPE = "Autodesk.TS.UpdateItemJobSample";

        #region IJobHandler Implementation
        public bool CanProcess(string jobType)
        {
            return jobType == JOB_TYPE;
        }

        public JobOutcome Execute(IJobProcessorServices context, IJob job)
        {
            try
            {
                MessageBox.Show("Hello World", "Job-Template-Messenger", MessageBoxButtons.OK);
                return JobOutcome.Success;
            }
            catch (Exception ex)
            {
                context.Log(ex, "Job-Template Job failed: " + ex.ToString() + " ");
                return JobOutcome.Failure;
            }

        }

        public void OnJobProcessorShutdown(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }

        public void OnJobProcessorSleep(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }

        public void OnJobProcessorStartup(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }

        public void OnJobProcessorWake(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }
        #endregion IJobHandler Implementation
    }
}
