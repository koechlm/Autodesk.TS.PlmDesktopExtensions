namespace Autodesk.TS.VltPlmAddIn.Forms
{
    partial class XtraFormFmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraFormFmLogin));
            WebViewFmLogin = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)WebViewFmLogin).BeginInit();
            SuspendLayout();
            // 
            // WebViewFmLogin
            // 
            WebViewFmLogin.AllowExternalDrop = true;
            WebViewFmLogin.CreationProperties = null;
            WebViewFmLogin.DefaultBackgroundColor = System.Drawing.Color.White;
            WebViewFmLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            WebViewFmLogin.Location = new System.Drawing.Point(0, 0);
            WebViewFmLogin.Name = "WebViewFmLogin";
            WebViewFmLogin.Size = new System.Drawing.Size(565, 566);
            WebViewFmLogin.TabIndex = 0;
            WebViewFmLogin.ZoomFactor = 1D;
            // 
            // XtraFormFmLogin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(565, 566);
            Controls.Add(WebViewFmLogin);
            IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("XtraFormFmLogin.IconOptions.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "XtraFormFmLogin";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Vault plm - Autodesk Account Login";
            TopMost = true;
            Shown += XtraFormFmLogin_Shown;
            ((System.ComponentModel.ISupportInitialize)WebViewFmLogin).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 WebViewFmLogin;
    }
}