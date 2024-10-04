namespace InvPlmAddIn
{
    partial class ProgressFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressFrm));
            lblProgress = new DevExpress.XtraEditors.LabelControl();
            marqueeProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            ((System.ComponentModel.ISupportInitialize)marqueeProgressBarControl1.Properties).BeginInit();
            SuspendLayout();
            // 
            // lblProgress
            // 
            lblProgress.Location = new System.Drawing.Point(12, 3);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new System.Drawing.Size(58, 13);
            lblProgress.TabIndex = 1;
            lblProgress.Text = "Loading . . .";
            lblProgress.UseWaitCursor = true;
            // 
            // marqueeProgressBarControl1
            // 
            marqueeProgressBarControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            marqueeProgressBarControl1.EditValue = 0;
            marqueeProgressBarControl1.Location = new System.Drawing.Point(12, 29);
            marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
            marqueeProgressBarControl1.Size = new System.Drawing.Size(474, 22);
            marqueeProgressBarControl1.TabIndex = 2;
            // 
            // ProgressFrm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(498, 63);
            Controls.Add(marqueeProgressBarControl1);
            Controls.Add(lblProgress);
            IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ProgressFrm.IconOptions.Icon");
            IconOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ProgressFrm.IconOptions.LargeImage");
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(500, 95);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(500, 95);
            Name = "ProgressFrm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Caption";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)marqueeProgressBarControl1.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        internal DevExpress.XtraEditors.LabelControl lblProgress;
        private DevExpress.XtraEditors.MarqueeProgressBarControl marqueeProgressBarControl1;
    }
}