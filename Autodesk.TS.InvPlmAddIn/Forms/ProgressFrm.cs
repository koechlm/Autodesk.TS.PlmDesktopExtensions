using DevExpress.XtraEditors;
using DevExpress.XtraSpellChecker.Rules;
using InvPlmAddIn.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace InvPlmAddIn
{
    public partial class ProgressFrm : DevExpress.XtraEditors.XtraForm
    {
        private readonly CancellationToken _cancellationToken;

        public ProgressFrm(string mCurrentTheme, CancellationToken cancellationToken)
        {
            InitializeComponent();

            ApplyThemes(mCurrentTheme);

            _cancellationToken = cancellationToken;
            _cancellationToken.Register(CloseForm);
        }

        private void ApplyThemes(string currentTheme)
        {
            if (string.IsNullOrEmpty(currentTheme))
                return;

            if (currentTheme == VDF.Forms.SkinUtils.Theme.Light.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.LightThemeName);
            else if (currentTheme == VDF.Forms.SkinUtils.Theme.Dark.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.DarkThemeName);
        }

        private void CloseForm()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Close));
            }
            else
            {
                Close();
            }
        }
    }
}