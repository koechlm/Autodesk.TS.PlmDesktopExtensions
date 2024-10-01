namespace InvPlmAddIn.Forms
{
    partial class FMExtensionLogin
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
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            SuspendLayout();
            // 
            // simpleButton1
            // 
            simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            simpleButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            simpleButton1.Location = new System.Drawing.Point(309, 407);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(75, 23);
            simpleButton1.TabIndex = 0;
            simpleButton1.Text = "Cancel";
            // 
            // simpleButton2
            // 
            simpleButton2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            simpleButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            simpleButton2.Location = new System.Drawing.Point(390, 407);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new System.Drawing.Size(75, 23);
            simpleButton2.TabIndex = 1;
            simpleButton2.Text = "Proceed";
            // 
            // panelControl1
            // 
            panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelControl1.Location = new System.Drawing.Point(12, 12);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(453, 389);
            panelControl1.TabIndex = 2;
            // 
            // FMExtensionLogin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(477, 442);
            Controls.Add(panelControl1);
            Controls.Add(simpleButton2);
            Controls.Add(simpleButton1);
            Name = "FMExtensionLogin";
            Text = "FMExtensionLogin";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}