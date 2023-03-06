namespace Embuddy
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnUpdateAgg = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUpdateIM1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUpdateIM2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnVersion = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDebug = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tvTraffic = new Ephemera.NBagOfUis.TextViewer();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUpdateAgg,
            this.toolStripSeparator1,
            this.btnUpdateIM1,
            this.toolStripSeparator2,
            this.btnUpdateIM2,
            this.toolStripSeparator3,
            this.btnVersion,
            this.toolStripSeparator4,
            this.btnDebug,
            this.toolStripSeparator5,
            this.btnSettings,
            this.toolStripSeparator6});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(957, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnUpdateAgg
            // 
            this.btnUpdateAgg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUpdateAgg.Name = "btnUpdateAgg";
            this.btnUpdateAgg.Size = new System.Drawing.Size(94, 24);
            this.btnUpdateAgg.Text = "Update Agg";
            this.btnUpdateAgg.Click += new System.EventHandler(this.UpdateAgg_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnUpdateIM1
            // 
            this.btnUpdateIM1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUpdateIM1.Name = "btnUpdateIM1";
            this.btnUpdateIM1.Size = new System.Drawing.Size(95, 24);
            this.btnUpdateIM1.Text = "Update IM 1";
            this.btnUpdateIM1.Click += new System.EventHandler(this.UpdateIM_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnUpdateIM2
            // 
            this.btnUpdateIM2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUpdateIM2.Name = "btnUpdateIM2";
            this.btnUpdateIM2.Size = new System.Drawing.Size(95, 24);
            this.btnUpdateIM2.Text = "Update IM 2";
            this.btnUpdateIM2.Click += new System.EventHandler(this.UpdateIM_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // btnVersion
            // 
            this.btnVersion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnVersion.Name = "btnVersion";
            this.btnVersion.Size = new System.Drawing.Size(61, 24);
            this.btnVersion.Text = "Version";
            this.btnVersion.Click += new System.EventHandler(this.Version_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // btnDebug
            // 
            this.btnDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(58, 24);
            this.btnDebug.Text = "Debug";
            this.btnDebug.Click += new System.EventHandler(this.Debug_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSettings
            // 
            this.btnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(66, 24);
            this.btnSettings.Text = "Settings";
            this.btnSettings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            // 
            // tvTraffic
            // 
            this.tvTraffic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvTraffic.Location = new System.Drawing.Point(12, 45);
            this.tvTraffic.MaxText = 50000;
            this.tvTraffic.Name = "tvTraffic";
            this.tvTraffic.Prompt = "";
            this.tvTraffic.Size = new System.Drawing.Size(696, 292);
            this.tvTraffic.TabIndex = 3;
            this.tvTraffic.WordWrap = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 573);
            this.Controls.Add(this.tvTraffic);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.Text = "SHM Tool";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton btnUpdateAgg;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnUpdateIM1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnUpdateIM2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton btnVersion;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton btnDebug;
        private ToolStripSeparator toolStripSeparator5;
        private Ephemera.NBagOfUis.TextViewer tvTraffic;
        private ToolStripButton btnSettings;
        private ToolStripSeparator toolStripSeparator6;
    }
}