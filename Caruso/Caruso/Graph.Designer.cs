namespace Caruso
{
    partial class Graph
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
            this.components = new System.ComponentModel.Container();
            this.ZGraph = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.ZGraph.Location = new System.Drawing.Point(12, 12);
            this.ZGraph.Name = "zedGraphControl1";
            this.ZGraph.ScrollGrace = 0D;
            this.ZGraph.ScrollMaxX = 0D;
            this.ZGraph.ScrollMaxY = 0D;
            this.ZGraph.ScrollMaxY2 = 0D;
            this.ZGraph.ScrollMinX = 0D;
            this.ZGraph.ScrollMinY = 0D;
            this.ZGraph.ScrollMinY2 = 0D;
            this.ZGraph.Size = new System.Drawing.Size(923, 389);
            this.ZGraph.TabIndex = 0;
            this.ZGraph.Load += new System.EventHandler(this.zedGraphControl1_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 413);
            this.Controls.Add(this.ZGraph);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl ZGraph;
    }
}