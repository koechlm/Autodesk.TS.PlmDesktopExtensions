namespace InvPlmAddIn.Forms
{
    partial class mDockWindowChild
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            webViewPanel = new System.Windows.Forms.Panel();
            SuspendLayout();
            // 
            // webViewPanel
            // 
            webViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            webViewPanel.Location = new System.Drawing.Point(0, 0);
            webViewPanel.Name = "webViewPanel";
            webViewPanel.Size = new System.Drawing.Size(150, 150);
            webViewPanel.TabIndex = 0;
            // 
            // mDockWindowChild
            // 
            Appearance.BackColor = System.Drawing.Color.White;
            Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(webViewPanel);
            Name = "mDockWindowChild";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel webViewPanel;
    }
}
