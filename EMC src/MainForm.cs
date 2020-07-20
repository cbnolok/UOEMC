using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Ultima;

namespace EMC
{
	public partial class MainForm : Form
	{
        byte map = 0;
        byte task_mapstatics = 0;       //0: convert map+statics, 1: convert only map
        byte task_season = 0;           //0: don't change season, 1: winter
        bool task_uop = false;          //false: don't pack the .bin files to uop, true: pack them into facetX.uop and then delete .bin files
        bool task_dds = false;          //false: don't write dds, true: write dds
        public bool hidelog = false;    //false: don't hide the log, true: hide it
        bool clearfiles = false;        //false: don't clear dest. files, true: clear them
        string outpath,inpath,binpath;

        delegate void DelegateString(string msg);
		Thread _thread;


		public MainForm()
		{
			InitializeComponent();
		}


		public void AppendToLog( string msg )
		{
			rtbox_log.AppendText( msg );
		}

        private int m_progress_maxPercent = 300;
        private int m_progress_maxReal = 0;
        private int m_progress_curPercent = 0, m_progress_curReal = 0;

        public void ProgressbarSetMarquee(bool on)
        {
            if (on)
                progressBar.Style = ProgressBarStyle.Marquee;
            else
                progressBar.Style = ProgressBarStyle.Continuous;
        }

        public void ProgressbarSetMax(int val)
        {
            m_progress_curPercent = m_progress_curReal = 0;
            m_progress_maxReal = val;
            progressBar.Maximum = m_progress_maxPercent;
        }

        public void ProgressbarIncrease()
        {
            ++m_progress_curReal;
            int newProgressPercent = (m_progress_curReal * m_progress_maxPercent) / m_progress_maxReal;
            if (newProgressPercent > m_progress_curPercent)
            {
                m_progress_curPercent = newProgressPercent;
                progressBar.Value = m_progress_curPercent;
            }
        }

        public void ProgressbarSetVal(int val)
        {
            m_progress_curReal = val;
            int newProgressPercent = (m_progress_curReal * m_progress_maxPercent) / m_progress_maxReal;
            if (newProgressPercent != m_progress_curPercent)
            {
                m_progress_curPercent = newProgressPercent;
                progressBar.Value = m_progress_curPercent;
            }
        }

        public void SetItemsState(bool state)
        {
            tbox_inpath.Enabled = state;
            tbox_outpath.Enabled = state;
            tbox_inheight.Enabled = state;
            tbox_inwidth.Enabled = state;
            btn_inpath_browse.Enabled = state;
            btn_outpath_browse.Enabled = state;
            rbtn_map0.Enabled = state;
            //rbtn_map1.Enabled = state;
            //rbtn_map2.Enabled = state;
            //rbtn_map3.Enabled = state;
            //rbtn_map4.Enabled = state;
            //rbtn_map5.Enabled = state;
            rbtn_task_mapstatics.Enabled = state;
            rbtn_task_maponly.Enabled = state;
            rbtn_task_season_nochange.Enabled = state;
            rbtn_task_season_winter.Enabled = state;
            cbox_task_uop.Enabled = state;
            cbox_task_dds.Enabled = state;
            btn_convert.Enabled = state;
            //btn_dictgen.Enabled = state;
            cbox_clearfiles.Enabled = state;
        }

        //----

        private void btn_inpath_browse_Click( object sender, EventArgs e )
		{
            if (!string.IsNullOrEmpty(tbox_inpath.Text))
            {
                if (Directory.Exists(tbox_inpath.Text))
                    folderBrowserDialog1.SelectedPath = tbox_inpath.Text;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK )
				tbox_inpath.Text = folderBrowserDialog1.SelectedPath.ToString();
		}

        private void btn_outpath_browse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbox_outpath.Text))
            {
                if (Directory.Exists(tbox_outpath.Text))
                    folderBrowserDialog1.SelectedPath = tbox_outpath.Text;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                tbox_outpath.Text = folderBrowserDialog1.SelectedPath.ToString();
        }

        private void rbtn_map0_CheckedChanged(object sender, EventArgs e)
        {
            map = 0;
            tbox_inwidth.Text = "7168";
            tbox_inheight.Text = "4096";
        }

        private void rbtn_map1_CheckedChanged(object sender, EventArgs e)
        {
            map = 1;
            tbox_inwidth.Text = "7168";
            tbox_inheight.Text = "4096";
        }

        private void rbtn_map2_CheckedChanged(object sender, EventArgs e)
        {
            map = 2;
            tbox_inwidth.Text = "2304";
            tbox_inheight.Text = "1600";
        }

        private void rbtn_map3_CheckedChanged(object sender, EventArgs e)
        {
            map = 3;
            tbox_inwidth.Text = "2560";
            tbox_inheight.Text = "2048";
        }

        private void rbtn_map4_CheckedChanged(object sender, EventArgs e)
        {
            map = 4;
            tbox_inwidth.Text = "1448";
            tbox_inheight.Text = "1448";
        }

        private void rbtn_map5_CheckedChanged(object sender, EventArgs e)
        {
            map = 5;
            tbox_inwidth.Text = "1280";
            tbox_inheight.Text = "4096";
        }

        private void rbtn_task_mapstatics_CheckedChanged(object sender, EventArgs e)
        {
            task_mapstatics = (byte)(rbtn_task_mapstatics.Checked ? 0 : 1);
        }

        private void rbtn_task_maponly_CheckedChanged(object sender, EventArgs e)
        {
            task_mapstatics = (byte)(rbtn_task_maponly.Checked ? 1 : 0);
        }

        private void rbtn_task_season_nochange_CheckedChanged(object sender, EventArgs e)
        {
            task_season = (byte)(rbtn_task_season_nochange.Checked ? 0 : 1);
        }

        private void rbtn_task_season_winter_CheckedChanged(object sender, EventArgs e)
        {
            task_season = (byte)(rbtn_task_season_winter.Checked ? 1 : 0);
        }

        private void cbox_task_uop_CheckedChanged(object sender, EventArgs e)
        {
            task_uop = cbox_task_uop.Checked;
        }

        private void cbox_task_dds_CheckedChanged(object sender, EventArgs e)
        {
            task_dds = cbox_task_dds.Checked;
        }

        private void cbox_hidelog_CheckedChanged(object sender, EventArgs e)
        {
            hidelog = cbox_hidelog.Checked;
            rtbox_log.Focus();
        }

        private void cbox_clearfiles_CheckedChanged(object sender, EventArgs e)
        {
            clearfiles = cbox_clearfiles.Checked;
        }

        //--------------------


        private void btn_convert_Click( object sender, EventArgs e )
		{
            int tst;
            if ( !int.TryParse(tbox_inwidth.Text, out tst) )
            {
                MessageBox.Show("Invalid map width!");
                return;
            }
            if ( !int.TryParse(tbox_inheight.Text, out tst) )
            {
                MessageBox.Show("Invalid map height!");
                return;
            }

            if (string.IsNullOrEmpty(tbox_inpath.Text))
            {
                MessageBox.Show("You have to specify the folder containing map and statics mul files!");
                return;
            }
			if (string.IsNullOrEmpty(tbox_outpath.Text))
			{
				MessageBox.Show("You have to specify the folder to convert to!");
				return;
			}

            if (tbox_inpath.Text.EndsWith("/") || tbox_inpath.Text.EndsWith("\\"))
                inpath = tbox_inpath.Text;
            else
                inpath = tbox_inpath.Text + "/";
            if (tbox_outpath.Text.EndsWith("/") || tbox_outpath.Text.EndsWith("\\"))
                outpath = tbox_outpath.Text;
            else
                outpath = tbox_outpath.Text + "/";
            binpath = outpath + string.Format("build/sectors/facet_0{0}/", map);

            FileStream test = null;
            try
            {
                test = new FileStream(inpath + "map" + map.ToString() + ".mul", FileMode.Open, FileAccess.Read);
                test.Close();
            }
            catch
            {
                MessageBox.Show("Can't find map" + map.ToString() + ".mul in the selected folder!");
                return;
            }
            try
            {
                test = new FileStream(inpath + "statics" + map.ToString() + ".mul", FileMode.Open, FileAccess.Read);
                test.Close();
            }
            catch
            {
                MessageBox.Show("Can't find statics" + map.ToString() + ".mul in the selected folder!");
                return;
            }
            try
            {
                test = new FileStream(inpath + "staidx" + map.ToString() + ".mul", FileMode.Open, FileAccess.Read);
                test.Close();
            }
            catch
            {
                MessageBox.Show("Can't find staidx" + map.ToString() + ".mul in the selected folder!");
                return;
            }
            if (task_dds)
            {
                try
                {
                    test = new FileStream("radarcol.mul", FileMode.Open, FileAccess.Read);
                    test.Close();
                }
                catch
                {
                    MessageBox.Show("Can't find radarcol.mul in current folder! I need it to render the DDS image!");
                    return;
                }
                try
                {
                    test = new FileStream("hues.mul", FileMode.Open, FileAccess.Read);
                    test.Close();
                }
                catch
                {
                    MessageBox.Show("Can't find hues.mul in current folder! I need it to render the DDS image!");
                    return;
                }
            }

            if ( !System.IO.Directory.Exists(outpath) )
            {
                MessageBox.Show("Output folder does not exist!");
                return;
            }

            try
            {
                Directory.CreateDirectory(binpath);
            }
            catch
            {
                MessageBox.Show("Can't create directory tree inside output folder!");
                return;
            }

            SetItemsState(false);
            try
            {
                if (clearfiles)
                {
                    AppendToLog("Clearing files... ");

                    if (Directory.Exists(binpath))
                    {
                        System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(binpath);
                        foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                            file.Delete();
                    }
                    if (task_uop)
                            File.Delete(outpath + String.Format("facet{0}.uop", map));
                    if (task_dds)
                            File.Delete(outpath + String.Format("facet0{0}.dds", map));

                    AppendToLog("Done!\n");
                }
            }
            finally
            {
                rtbox_log.Focus();
                _thread = new Thread(new ThreadStart(Convert));
                _thread.Start();
            }
		}

        public void Convert()
        {
            Converter convert = new Converter(this, int.Parse(tbox_inwidth.Text), int.Parse(tbox_inheight.Text), map, inpath, outpath, binpath, task_mapstatics, task_season, task_uop, task_dds);
            convert.ConvertMap();
        }

        public void Success()
        {
            progressBar.Value = 0;
            SetItemsState(true);
            _thread = null;
            //GC.Collect();
            //MessageBox.Show("Successiful! See the log for details");
            //this.Dispose();
        }


        //--------------------------------


        private void btn_dictgen_Click(object sender, EventArgs e)
        {
            SetItemsState(false);
            progressBar.Value = 0;
            //progressBar.Maximum = ((int.Parse(tbox_inwidth.Text) / 64) * (int.Parse(tbox_inheight.Text) / 64));
            progressBar.Maximum = int.Parse(tbox_inwidth.Text);
            rtbox_log.Focus();
            _thread = new Thread(new ThreadStart(DictGen));
            _thread.Start();
        }

        public void DictGen()
        {
            ExtraFunctions.GenerateDictionary_CC_CC(this);
        }


        //-----------------------------


        private void btn_size_info_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Map sizes (width x height):" +
                            "\n\nmap0 (pre-ML):\t6144 x 4096" +
                            "\nmap0 (ML and on):\t7168 x 4096" +
                            "\nmap1 (pre-ML):\t6144 x 4096" +
                            "\nmap1 (ML and on):\t7168 x 4096" +
                            "\nmap2:\t\t2304 x 1600" +
                            "\nmap3:\t\t2560 x 2048" +
                            "\nmap4:\t\t1448 x 1448" +
                            "\nmap5:\t\t1280 x 4096" +
                            "\n\nThose are the default values, but you can" +
                            "\nenter the sizes you want for custom maps." +
                            "\nRemember: width and height must be" +
                            "\nmultiples of 64." + 
                            "\n\nItems with art.mul ID > 0x7FFE " +
                            "\n(32776) are not supported.");
        }


        //-------------------------------

        //-----------------//
        //--SAVE SETTINGS--//
        //-----------------//

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                StreamReader reader = new StreamReader("EMC.cfg");
                tbox_inpath.Text = reader.ReadLine();
                tbox_outpath.Text = reader.ReadLine();
                switch (int.Parse(reader.ReadLine()))
                {
                    case 0: rbtn_map0.Checked = true; break;
                    case 1: rbtn_map1.Checked = true; break;
                    case 2: rbtn_map2.Checked = true; break;
                    case 3: rbtn_map3.Checked = true; break;
                    case 4: rbtn_map4.Checked = true; break;
                    case 5: rbtn_map5.Checked = true; break;
                }
                tbox_inwidth.Text = reader.ReadLine();
                tbox_inheight.Text = reader.ReadLine();
                switch (int.Parse(reader.ReadLine()))
                {
                    case 0: rbtn_task_mapstatics.Checked = true; break;
                    case 1: rbtn_task_maponly.Checked = true; break;
                }
                switch (int.Parse(reader.ReadLine()))
                {
                    case 0: rbtn_task_season_nochange.Checked = true; break;
                    case 1: rbtn_task_season_winter.Checked = true; break;
                }
                switch (int.Parse(reader.ReadLine()))
                {
                    case 0: cbox_task_uop.Checked = true; break;
                    case 1: cbox_task_uop.Checked = true; break;
                }
                switch (int.Parse(reader.ReadLine()))
                {
                    case 0: cbox_task_dds.Checked = true; break;
                    case 1: cbox_task_dds.Checked = true; break;
                }
                switch (int.Parse(reader.ReadLine()))
                {
                    case 0: cbox_hidelog.Checked = false; break;
                    case 1: cbox_hidelog.Checked = true; break;
                }
                switch (int.Parse(reader.ReadLine()))
                {
                    case 0: cbox_clearfiles.Checked = false; break;
                    case 1: cbox_clearfiles.Checked = true; break;
                }
                reader.Close();
            }
            catch
            {
                tbox_inpath.Text = "";
                tbox_outpath.Text = "";
                rbtn_map0.Checked = false;
                rbtn_map1.Checked = false;
                rbtn_map2.Checked = false;
                rbtn_map3.Checked = false;
                rbtn_map4.Checked = false;
                rbtn_map5.Checked = false;
                tbox_inwidth.Text = "";
                tbox_inheight.Text = "";
                rbtn_task_maponly.Checked = false;
                rbtn_task_mapstatics.Checked = true;
                rbtn_task_season_winter.Checked = false;
                rbtn_task_season_nochange.Checked = true;
                cbox_task_uop.Checked = false;
                cbox_task_dds.Checked = false;
                cbox_hidelog.Checked = false;
                cbox_clearfiles.Checked = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FileStream stream = new FileStream("EMC.cfg", FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(tbox_inpath.Text);
                writer.WriteLine(tbox_outpath.Text);
                if (rbtn_map0.Checked)
                    writer.WriteLine("0");
                else if (rbtn_map1.Checked)
                    writer.WriteLine("1");
                else if (rbtn_map2.Checked)
                    writer.WriteLine("2");
                else if (rbtn_map3.Checked)
                    writer.WriteLine("3");
                else if (rbtn_map4.Checked)
                    writer.WriteLine("4");
                else if (rbtn_map5.Checked)
                    writer.WriteLine("5");
                else
                    writer.WriteLine("-1");
                writer.WriteLine(tbox_inwidth.Text);
                writer.WriteLine(tbox_inheight.Text);
                if (rbtn_task_mapstatics.Checked)
                    writer.WriteLine("0");
                else if (rbtn_task_maponly.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("-1");
                if (rbtn_task_season_nochange.Checked)
                    writer.WriteLine("0");
                else if (rbtn_task_season_winter.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("-1");
                if (cbox_task_uop.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                if (cbox_task_dds.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                if (cbox_hidelog.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                if (cbox_clearfiles.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                writer.Close();
            }
            catch { }
        }

        //---

        void MainForm_Disposed(object sender, System.EventArgs e)
        {
            if (_thread != null && _thread.IsAlive)
                _thread.Abort();
        }

    }
}
