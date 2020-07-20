namespace EMC
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.rtbox_log = new System.Windows.Forms.RichTextBox();
            this.tbox_outpath = new System.Windows.Forms.TextBox();
            this.btn_outpath_browse = new System.Windows.Forms.Button();
            this.btn_convert = new System.Windows.Forms.Button();
            this.btn_dictgen = new System.Windows.Forms.Button();
            this.tbox_inpath = new System.Windows.Forms.TextBox();
            this.btn_inpath_browse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbox_inwidth = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbox_inheight = new System.Windows.Forms.TextBox();
            this.btn_size_info = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.cbox_hidelog = new System.Windows.Forms.CheckBox();
            this.rbtn_map2 = new System.Windows.Forms.RadioButton();
            this.rbtn_map1 = new System.Windows.Forms.RadioButton();
            this.rbtn_map3 = new System.Windows.Forms.RadioButton();
            this.rbtn_map5 = new System.Windows.Forms.RadioButton();
            this.rbtn_map4 = new System.Windows.Forms.RadioButton();
            this.rbtn_map0 = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtn_task_maponly = new System.Windows.Forms.RadioButton();
            this.rbtn_task_mapstatics = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbox_clearfiles = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbtn_task_season_winter = new System.Windows.Forms.RadioButton();
            this.rbtn_task_season_nochange = new System.Windows.Forms.RadioButton();
            this.cbox_task_dds = new System.Windows.Forms.CheckBox();
            this.cbox_task_uop = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Choose folder";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // rtbox_log
            // 
            this.rtbox_log.Location = new System.Drawing.Point(12, 202);
            this.rtbox_log.Name = "rtbox_log";
            this.rtbox_log.ReadOnly = true;
            this.rtbox_log.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbox_log.Size = new System.Drawing.Size(512, 171);
            this.rtbox_log.TabIndex = 0;
            this.rtbox_log.Text = "";
            // 
            // tbox_outpath
            // 
            this.tbox_outpath.Location = new System.Drawing.Point(12, 98);
            this.tbox_outpath.Name = "tbox_outpath";
            this.tbox_outpath.Size = new System.Drawing.Size(381, 20);
            this.tbox_outpath.TabIndex = 4;
            // 
            // btn_outpath_browse
            // 
            this.btn_outpath_browse.Location = new System.Drawing.Point(399, 96);
            this.btn_outpath_browse.Name = "btn_outpath_browse";
            this.btn_outpath_browse.Size = new System.Drawing.Size(125, 22);
            this.btn_outpath_browse.TabIndex = 5;
            this.btn_outpath_browse.Text = "Output path";
            this.btn_outpath_browse.UseVisualStyleBackColor = true;
            this.btn_outpath_browse.Click += new System.EventHandler(this.btn_outpath_browse_Click);
            // 
            // btn_convert
            // 
            this.btn_convert.Location = new System.Drawing.Point(12, 410);
            this.btn_convert.Name = "btn_convert";
            this.btn_convert.Size = new System.Drawing.Size(343, 26);
            this.btn_convert.TabIndex = 6;
            this.btn_convert.Text = "Convert";
            this.btn_convert.UseVisualStyleBackColor = true;
            this.btn_convert.Click += new System.EventHandler(this.btn_convert_Click);
            // 
            // btn_dictgen
            // 
            this.btn_dictgen.Enabled = false;
            this.btn_dictgen.Location = new System.Drawing.Point(372, 410);
            this.btn_dictgen.Name = "btn_dictgen";
            this.btn_dictgen.Size = new System.Drawing.Size(152, 26);
            this.btn_dictgen.TabIndex = 12;
            this.btn_dictgen.Text = "DictGen";
            this.btn_dictgen.UseVisualStyleBackColor = true;
            this.btn_dictgen.Click += new System.EventHandler(this.btn_dictgen_Click);
            // 
            // tbox_inpath
            // 
            this.tbox_inpath.Location = new System.Drawing.Point(12, 12);
            this.tbox_inpath.Name = "tbox_inpath";
            this.tbox_inpath.Size = new System.Drawing.Size(381, 20);
            this.tbox_inpath.TabIndex = 13;
            // 
            // btn_inpath_browse
            // 
            this.btn_inpath_browse.Location = new System.Drawing.Point(399, 10);
            this.btn_inpath_browse.Name = "btn_inpath_browse";
            this.btn_inpath_browse.Size = new System.Drawing.Size(125, 22);
            this.btn_inpath_browse.TabIndex = 14;
            this.btn_inpath_browse.Text = "Input map+statics path";
            this.btn_inpath_browse.UseVisualStyleBackColor = true;
            this.btn_inpath_browse.Click += new System.EventHandler(this.btn_inpath_browse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Input map Width (X)";
            // 
            // tbox_inwidth
            // 
            this.tbox_inwidth.Location = new System.Drawing.Point(119, 39);
            this.tbox_inwidth.Name = "tbox_inwidth";
            this.tbox_inwidth.Size = new System.Drawing.Size(53, 20);
            this.tbox_inwidth.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Input map Height (Y)";
            // 
            // tbox_inheight
            // 
            this.tbox_inheight.Location = new System.Drawing.Point(119, 66);
            this.tbox_inheight.Name = "tbox_inheight";
            this.tbox_inheight.Size = new System.Drawing.Size(53, 20);
            this.tbox_inheight.TabIndex = 18;
            // 
            // btn_size_info
            // 
            this.btn_size_info.Location = new System.Drawing.Point(451, 51);
            this.btn_size_info.Name = "btn_size_info";
            this.btn_size_info.Size = new System.Drawing.Size(20, 22);
            this.btn_size_info.TabIndex = 19;
            this.btn_size_info.Text = "?";
            this.btn_size_info.UseVisualStyleBackColor = true;
            this.btn_size_info.Click += new System.EventHandler(this.btn_size_info_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(10, 381);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(514, 23);
            this.progressBar.TabIndex = 20;
            // 
            // cbox_hidelog
            // 
            this.cbox_hidelog.AutoSize = true;
            this.cbox_hidelog.Location = new System.Drawing.Point(13, 179);
            this.cbox_hidelog.Name = "cbox_hidelog";
            this.cbox_hidelog.Size = new System.Drawing.Size(100, 17);
            this.cbox_hidelog.TabIndex = 23;
            this.cbox_hidelog.Text = "Hide log (faster)";
            this.cbox_hidelog.UseVisualStyleBackColor = true;
            this.cbox_hidelog.CheckedChanged += new System.EventHandler(this.cbox_hidelog_CheckedChanged);
            // 
            // rbtn_map2
            // 
            this.rbtn_map2.AutoSize = true;
            this.rbtn_map2.Enabled = false;
            this.rbtn_map2.Location = new System.Drawing.Point(60, 3);
            this.rbtn_map2.Name = "rbtn_map2";
            this.rbtn_map2.Size = new System.Drawing.Size(51, 17);
            this.rbtn_map2.TabIndex = 9;
            this.rbtn_map2.Text = "map2";
            this.rbtn_map2.UseVisualStyleBackColor = true;
            this.rbtn_map2.CheckedChanged += new System.EventHandler(this.rbtn_map2_CheckedChanged);
            // 
            // rbtn_map1
            // 
            this.rbtn_map1.AutoSize = true;
            this.rbtn_map1.Enabled = false;
            this.rbtn_map1.Location = new System.Drawing.Point(3, 28);
            this.rbtn_map1.Name = "rbtn_map1";
            this.rbtn_map1.Size = new System.Drawing.Size(51, 17);
            this.rbtn_map1.TabIndex = 8;
            this.rbtn_map1.Text = "map1";
            this.rbtn_map1.UseVisualStyleBackColor = true;
            this.rbtn_map1.CheckedChanged += new System.EventHandler(this.rbtn_map1_CheckedChanged);
            // 
            // rbtn_map3
            // 
            this.rbtn_map3.AutoSize = true;
            this.rbtn_map3.Enabled = false;
            this.rbtn_map3.Location = new System.Drawing.Point(60, 29);
            this.rbtn_map3.Name = "rbtn_map3";
            this.rbtn_map3.Size = new System.Drawing.Size(51, 17);
            this.rbtn_map3.TabIndex = 10;
            this.rbtn_map3.Text = "map3";
            this.rbtn_map3.UseVisualStyleBackColor = true;
            this.rbtn_map3.CheckedChanged += new System.EventHandler(this.rbtn_map3_CheckedChanged);
            // 
            // rbtn_map5
            // 
            this.rbtn_map5.AutoSize = true;
            this.rbtn_map5.Enabled = false;
            this.rbtn_map5.Location = new System.Drawing.Point(117, 29);
            this.rbtn_map5.Name = "rbtn_map5";
            this.rbtn_map5.Size = new System.Drawing.Size(51, 17);
            this.rbtn_map5.TabIndex = 24;
            this.rbtn_map5.Text = "map5";
            this.rbtn_map5.UseVisualStyleBackColor = true;
            this.rbtn_map5.CheckedChanged += new System.EventHandler(this.rbtn_map5_CheckedChanged);
            // 
            // rbtn_map4
            // 
            this.rbtn_map4.AutoSize = true;
            this.rbtn_map4.Enabled = false;
            this.rbtn_map4.Location = new System.Drawing.Point(117, 3);
            this.rbtn_map4.Name = "rbtn_map4";
            this.rbtn_map4.Size = new System.Drawing.Size(51, 17);
            this.rbtn_map4.TabIndex = 11;
            this.rbtn_map4.Text = "map4";
            this.rbtn_map4.UseVisualStyleBackColor = true;
            this.rbtn_map4.CheckedChanged += new System.EventHandler(this.rbtn_map4_CheckedChanged);
            // 
            // rbtn_map0
            // 
            this.rbtn_map0.AutoSize = true;
            this.rbtn_map0.Checked = true;
            this.rbtn_map0.Location = new System.Drawing.Point(3, 2);
            this.rbtn_map0.Name = "rbtn_map0";
            this.rbtn_map0.Size = new System.Drawing.Size(51, 17);
            this.rbtn_map0.TabIndex = 7;
            this.rbtn_map0.TabStop = true;
            this.rbtn_map0.Text = "map0";
            this.rbtn_map0.UseVisualStyleBackColor = true;
            this.rbtn_map0.CheckedChanged += new System.EventHandler(this.rbtn_map0_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.rbtn_map0);
            this.panel1.Controls.Add(this.rbtn_map4);
            this.panel1.Controls.Add(this.rbtn_map5);
            this.panel1.Controls.Add(this.rbtn_map3);
            this.panel1.Controls.Add(this.rbtn_map1);
            this.panel1.Controls.Add(this.rbtn_map2);
            this.panel1.Location = new System.Drawing.Point(181, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(174, 51);
            this.panel1.TabIndex = 26;
            // 
            // rbtn_task_maponly
            // 
            this.rbtn_task_maponly.AutoSize = true;
            this.rbtn_task_maponly.Location = new System.Drawing.Point(3, 26);
            this.rbtn_task_maponly.Name = "rbtn_task_maponly";
            this.rbtn_task_maponly.Size = new System.Drawing.Size(95, 17);
            this.rbtn_task_maponly.TabIndex = 22;
            this.rbtn_task_maponly.Text = "Write map only";
            this.rbtn_task_maponly.UseVisualStyleBackColor = true;
            this.rbtn_task_maponly.CheckedChanged += new System.EventHandler(this.rbtn_task_maponly_CheckedChanged);
            // 
            // rbtn_task_mapstatics
            // 
            this.rbtn_task_mapstatics.AutoSize = true;
            this.rbtn_task_mapstatics.Checked = true;
            this.rbtn_task_mapstatics.Location = new System.Drawing.Point(3, 3);
            this.rbtn_task_mapstatics.Name = "rbtn_task_mapstatics";
            this.rbtn_task_mapstatics.Size = new System.Drawing.Size(127, 17);
            this.rbtn_task_mapstatics.TabIndex = 21;
            this.rbtn_task_mapstatics.TabStop = true;
            this.rbtn_task_mapstatics.Text = "Write map and statics";
            this.rbtn_task_mapstatics.UseVisualStyleBackColor = true;
            this.rbtn_task_mapstatics.CheckedChanged += new System.EventHandler(this.rbtn_task_mapstatics_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.rbtn_task_mapstatics);
            this.panel2.Controls.Add(this.rbtn_task_maponly);
            this.panel2.Location = new System.Drawing.Point(12, 124);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(157, 49);
            this.panel2.TabIndex = 27;
            // 
            // cbox_clearfiles
            // 
            this.cbox_clearfiles.AutoSize = true;
            this.cbox_clearfiles.Location = new System.Drawing.Point(182, 179);
            this.cbox_clearfiles.Name = "cbox_clearfiles";
            this.cbox_clearfiles.Size = new System.Drawing.Size(168, 17);
            this.cbox_clearfiles.TabIndex = 28;
            this.cbox_clearfiles.Text = "Clear destination folder (faster)";
            this.cbox_clearfiles.UseVisualStyleBackColor = true;
            this.cbox_clearfiles.CheckedChanged += new System.EventHandler(this.cbox_clearfiles_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.rbtn_task_season_winter);
            this.panel3.Controls.Add(this.rbtn_task_season_nochange);
            this.panel3.Location = new System.Drawing.Point(182, 124);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(173, 49);
            this.panel3.TabIndex = 29;
            // 
            // rbtn_task_season_winter
            // 
            this.rbtn_task_season_winter.AutoSize = true;
            this.rbtn_task_season_winter.Location = new System.Drawing.Point(4, 26);
            this.rbtn_task_season_winter.Name = "rbtn_task_season_winter";
            this.rbtn_task_season_winter.Size = new System.Drawing.Size(95, 17);
            this.rbtn_task_season_winter.TabIndex = 1;
            this.rbtn_task_season_winter.TabStop = true;
            this.rbtn_task_season_winter.Text = "Season: winter";
            this.rbtn_task_season_winter.UseVisualStyleBackColor = true;
            this.rbtn_task_season_winter.CheckedChanged += new System.EventHandler(this.rbtn_task_season_winter_CheckedChanged);
            // 
            // rbtn_task_season_nochange
            // 
            this.rbtn_task_season_nochange.AutoSize = true;
            this.rbtn_task_season_nochange.Checked = true;
            this.rbtn_task_season_nochange.Location = new System.Drawing.Point(4, 3);
            this.rbtn_task_season_nochange.Name = "rbtn_task_season_nochange";
            this.rbtn_task_season_nochange.Size = new System.Drawing.Size(118, 17);
            this.rbtn_task_season_nochange.TabIndex = 0;
            this.rbtn_task_season_nochange.TabStop = true;
            this.rbtn_task_season_nochange.Text = "Season: no change";
            this.rbtn_task_season_nochange.UseVisualStyleBackColor = true;
            this.rbtn_task_season_nochange.CheckedChanged += new System.EventHandler(this.rbtn_task_season_nochange_CheckedChanged);
            // 
            // cbox_task_dds
            // 
            this.cbox_task_dds.AutoSize = true;
            this.cbox_task_dds.Location = new System.Drawing.Point(399, 152);
            this.cbox_task_dds.Name = "cbox_task_dds";
            this.cbox_task_dds.Size = new System.Drawing.Size(131, 17);
            this.cbox_task_dds.TabIndex = 30;
            this.cbox_task_dds.Text = "Write DDS map image";
            this.cbox_task_dds.UseVisualStyleBackColor = true;
            this.cbox_task_dds.CheckedChanged += new System.EventHandler(this.cbox_task_dds_CheckedChanged);
            // 
            // cbox_task_uop
            // 
            this.cbox_task_uop.AutoSize = true;
            this.cbox_task_uop.Location = new System.Drawing.Point(399, 128);
            this.cbox_task_uop.Name = "cbox_task_uop";
            this.cbox_task_uop.Size = new System.Drawing.Size(95, 17);
            this.cbox_task_uop.TabIndex = 31;
            this.cbox_task_uop.Text = "Pack into .uop";
            this.cbox_task_uop.UseVisualStyleBackColor = true;
            this.cbox_task_uop.CheckedChanged += new System.EventHandler(this.cbox_task_uop_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(536, 444);
            this.Controls.Add(this.cbox_task_uop);
            this.Controls.Add(this.cbox_task_dds);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.cbox_clearfiles);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbox_hidelog);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btn_size_info);
            this.Controls.Add(this.tbox_inheight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbox_inwidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_inpath_browse);
            this.Controls.Add(this.tbox_inpath);
            this.Controls.Add(this.btn_dictgen);
            this.Controls.Add(this.btn_convert);
            this.Controls.Add(this.btn_outpath_browse);
            this.Controls.Add(this.tbox_outpath);
            this.Controls.Add(this.rtbox_log);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "UO Enhanced Map Converter 3.5 by Nolok";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Disposed += new System.EventHandler(this.MainForm_Disposed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RichTextBox rtbox_log;
        private System.Windows.Forms.TextBox tbox_outpath;
        private System.Windows.Forms.Button btn_outpath_browse;
        private System.Windows.Forms.Button btn_convert;
        private System.Windows.Forms.Button btn_dictgen;
        private System.Windows.Forms.TextBox tbox_inpath;
        private System.Windows.Forms.Button btn_inpath_browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbox_inwidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbox_inheight;
        private System.Windows.Forms.Button btn_size_info;
        public System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox cbox_hidelog;
        private System.Windows.Forms.RadioButton rbtn_map2;
        private System.Windows.Forms.RadioButton rbtn_map1;
        private System.Windows.Forms.RadioButton rbtn_map3;
        private System.Windows.Forms.RadioButton rbtn_map5;
        private System.Windows.Forms.RadioButton rbtn_map4;
        private System.Windows.Forms.RadioButton rbtn_map0;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtn_task_maponly;
        private System.Windows.Forms.RadioButton rbtn_task_mapstatics;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbox_clearfiles;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbtn_task_season_winter;
        private System.Windows.Forms.RadioButton rbtn_task_season_nochange;
        private System.Windows.Forms.CheckBox cbox_task_dds;
        private System.Windows.Forms.CheckBox cbox_task_uop;
    }
}

