using System.Drawing;

namespace Autodesk.TS.VltPlmAddIn.Forms
{
    partial class WebViewFmItem
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
            FmItem = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(FmItem)).BeginInit();
            SuspendLayout();
            // 
            // FmItem
            // 
            FmItem.AllowExternalDrop = true;
            FmItem.CreationProperties = null;
            FmItem.DefaultBackgroundColor = Color.White;
            FmItem.Dock = System.Windows.Forms.DockStyle.Fill;
            FmItem.Location = new Point(0, 0);
            FmItem.Name = "FmItem";
            FmItem.Size = new Size(150, 150);
            FmItem.TabIndex = 0;
            FmItem.ZoomFactor = 1D;
            // 
            // WebViewFmItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(FmItem);
            Name = "WebViewFmItem";
            ((System.ComponentModel.ISupportInitialize)FmItem).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 FmItem;
    }
}
