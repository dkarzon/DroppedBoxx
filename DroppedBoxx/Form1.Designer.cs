using DroppedBoxx.Properties;
namespace DroppedBoxx
{
    partial class Form1
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Host = new Fluid.Controls.FluidHost();
            this.InputPanel = new Microsoft.WindowsCE.Forms.InputPanel(this.components);
            this.SuspendLayout();
            // 
            // Host
            // 
            this.Host.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Host.Location = new System.Drawing.Point(0, 0);
            this.Host.Name = "Host";
            this.Host.Size = new System.Drawing.Size(240, 268);
            this.Host.TabIndex = 0;
            this.Host.Text = "fluidHost1";
            this.Host.BringToFront();
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.Host);
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.ResumeLayout(false);

        }

    }
}

