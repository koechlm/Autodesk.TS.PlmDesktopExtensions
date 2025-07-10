using System.Threading;
using System.Windows.Forms;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace Autodesk.TS.VltPlmAddIn.Utils
{
    /// <summary>
    /// Themed dialog for progress feedback
    /// </summary>
    public partial class ProgressForm : DevExpress.XtraEditors.XtraForm
    {

        private string mCurrentTheme;

        /// <summary>
        /// 
        /// </summary>
        public ProgressForm(string description)
        {
            InitializeComponent();

            mCurrentTheme = VDF.Forms.SkinUtils.WinFormsTheme.Instance.CurrentTheme.ToString();

            if (mCurrentTheme == VDF.Forms.SkinUtils.Theme.Light.ToString())
            {
                this.LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.LightThemeName);
            }
            if (mCurrentTheme == VDF.Forms.SkinUtils.Theme.Dark.ToString())
            {
                this.LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.DarkThemeName);
            }
            if (mCurrentTheme == VDF.Forms.SkinUtils.Theme.Default.ToString())
            {
                this.LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.DefaultThemeName);
            }

            this.lblProgress.Text = description;
        }

        public void UpdateProgress(string description)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => UpdateProgress(description)));
            }
            else
            {
                this.lblProgress.Text = description;
            }
            this.Refresh();
        }

        public void CloseProgress()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(CloseProgress));
            }
            else
            {
                this.Close();
            }
        }

    }
}
