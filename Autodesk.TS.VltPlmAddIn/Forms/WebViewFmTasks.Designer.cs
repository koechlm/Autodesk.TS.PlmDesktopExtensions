namespace Autodesk.TS.VltPlmAddIn.Forms
{
    partial class WebViewFmTasks
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
            FmTasks = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)FmTasks).BeginInit();
            SuspendLayout();
            // 
            // FmTasks
            // 
            FmTasks.AllowExternalDrop = true;
            FmTasks.CreationProperties = null;
            FmTasks.DefaultBackgroundColor = System.Drawing.Color.White;
            FmTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            FmTasks.Location = new System.Drawing.Point(0, 0);
            FmTasks.Name = "FmTasks";
            FmTasks.Size = new System.Drawing.Size(150, 150);
            FmTasks.TabIndex = 0;
            FmTasks.ZoomFactor = 1D;
            // 
            // WebViewFmTasks
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(FmTasks);
            Name = "WebViewFmTasks";
            ((System.ComponentModel.ISupportInitialize)FmTasks).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 FmTasks;
    }
}
