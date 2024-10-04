using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace InvPlmAddIn.Forms
{
    public partial class WaitForm1 :  DevExpress.XtraEditors.XtraForm
    {
        public WaitForm1(string currentTheme, string description)
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;

            ApplyThemes(currentTheme);

            this.progressPanel1.Text = InvPlmAddinSrv.AddInName;
            this.progressPanel1.Description = description;
        }

        //#region Overrides

        //public override void SetCaption(string caption)
        //{
        //    base.SetCaption(caption);
        //    this.progressPanel1.Caption = caption;
        //}
        //public override void SetDescription(string description)
        //{
        //    base.SetDescription(description);
        //    this.progressPanel1.Description = description;
        //}
        //public override void ProcessCommand(Enum cmd, object arg)
        //{
        //    base.ProcessCommand(cmd, arg);
        //}

        //#endregion

        private void ApplyThemes(string currentTheme)
        {
            if (string.IsNullOrEmpty(currentTheme))
                return;

            if (currentTheme == VDF.Forms.SkinUtils.Theme.Light.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.LightThemeName);
            else if (currentTheme == VDF.Forms.SkinUtils.Theme.Dark.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.DarkThemeName);
        }


        public enum WaitFormCommand
        {
        }
    }
}