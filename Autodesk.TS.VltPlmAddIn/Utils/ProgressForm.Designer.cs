using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace Autodesk.TS.VltPlmAddIn.Utils
{
    partial class ProgressForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            formAssistant1 = new DevExpress.XtraBars.FormAssistant();
            lblProgress = new DevExpress.XtraEditors.LabelControl();
            ProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            ((System.ComponentModel.ISupportInitialize)ProgressBarControl1.Properties).BeginInit();
            SuspendLayout();
            // 
            // lblProgress
            // 
            lblProgress.Location = new Point(12, 12);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(54, 13);
            lblProgress.TabIndex = 1;
            lblProgress.Text = "Progress...";
            // 
            // ProgressBarControl1
            // 
            ProgressBarControl1.EditValue = 0;
            ProgressBarControl1.Location = new Point(12, 31);
            ProgressBarControl1.Name = "ProgressBarControl1";
            ProgressBarControl1.Size = new Size(423, 20);
            ProgressBarControl1.TabIndex = 2;
            // 
            // VdsSampleProgressForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(447, 63);
            Controls.Add(ProgressBarControl1);
            Controls.Add(lblProgress);
            IconOptions.Icon = (Icon)resources.GetObject("VdsSampleProgressForm.IconOptions.Icon");
            Name = "VdsSampleProgressForm";
            Text = "VDS-Sample-Configuration";
            ((System.ComponentModel.ISupportInitialize)ProgressBarControl1.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.FormAssistant formAssistant1;
        internal DevExpress.XtraEditors.LabelControl lblProgress;
        private DevExpress.XtraEditors.MarqueeProgressBarControl ProgressBarControl1;
    }
}