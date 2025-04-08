namespace Autodesk.TS.VltPlmAddIn.Forms
{
    partial class WebViewFmSearch
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
            FmSearch = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)FmSearch).BeginInit();
            SuspendLayout();
            // 
            // FmSearch
            // 
            FmSearch.AllowExternalDrop = true;
            FmSearch.CreationProperties = null;
            FmSearch.DefaultBackgroundColor = System.Drawing.Color.White;
            FmSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            FmSearch.Location = new System.Drawing.Point(0, 0);
            FmSearch.Name = "FmSearch";
            FmSearch.Size = new System.Drawing.Size(150, 150);
            FmSearch.TabIndex = 0;
            FmSearch.ZoomFactor = 1D;
            // 
            // WebViewFmSearch
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(FmSearch);
            Name = "WebViewFmSearch";
            ((System.ComponentModel.ISupportInitialize)FmSearch).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 FmSearch;
    }
}
