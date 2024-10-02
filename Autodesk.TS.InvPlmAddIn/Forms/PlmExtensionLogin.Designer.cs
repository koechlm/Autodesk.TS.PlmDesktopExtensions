namespace InvPlmAddIn.Forms
{
    partial class PlmExtensionLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlmExtensionLogin));
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(651, 521);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.ToolTip = "Inventor Vault plm will not load on Cancel or Failure.";
            btnCancel.ToolTipTitle = "Autodesk Account Login";
            // 
            // panelControl1
            // 
            panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelControl1.Location = new System.Drawing.Point(12, 12);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(714, 503);
            panelControl1.TabIndex = 2;
            // 
            // FMExtensionLogin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(738, 556);
            Controls.Add(panelControl1);
            Controls.Add(btnCancel);
            IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("FMExtensionLogin.IconOptions.Icon");
            IconOptions.LargeImage = (System.Drawing.Image)resources.GetObject("FMExtensionLogin.IconOptions.LargeImage");
            Name = "FMExtensionLogin";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "Vault plm - Autodesk Account Login";
            TopMost = true;
            Shown += PlmExtensionLogin_Shown;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}