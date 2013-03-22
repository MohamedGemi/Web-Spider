using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace mission_impossible_code_assassin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            System.Threading.Thread Check_Internet_Thread = new System.Threading.Thread(() => Performance.check_internet());
            Check_Internet_Thread.IsBackground = true;
            Check_Internet_Thread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            button4.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = false;
            numericUpDown1.Enabled = false;
            if (textBox1.Text.Length < 1) // Check if The User Put A Link Or Not
                textBox1.Text = "https://www.google.com/webhp?hl=ar";
            if (textBox2.Text.Length < 1) // Check if The User Choose A Path Or Not
                textBox2.Text = "C:\\newcorpus";
            Data.user_defThreads = (int)numericUpDown1.Value;
            Data.Links_Queue.Enqueue(new Uri(textBox1.Text));
            Data.webcrawl_thread = new Thread(()=>webCrawler.start());
            Data.webcrawl_thread.Priority = ThreadPriority.Lowest;
            Data.webcrawl_thread.IsBackground = true;
            Data.webcrawl_thread.Start();
            Data._AREvt.WaitOne(10, true);
            Data.update_thread = new Thread(()=>timer1.Start());
            Data.update_thread.IsBackground = true;
            Data.update_thread.Priority = ThreadPriority.Lowest;
            Data.update_thread.Start();
            Data._AREvt.WaitOne(10, true);
            Data.performance_thread = new Thread(()=>Performance.check_state());
            Data.performance_thread.IsBackground = true;
            Data.performance_thread.Priority = ThreadPriority.Lowest;
            Data.performance_thread.Start();
            Data._AREvt.WaitOne(10, true);
            Data.loop_thread = new Thread(()=>timer2.Start());
            Data.loop_thread.IsBackground = true;
            Data.loop_thread.Priority = ThreadPriority.Lowest;
            Data.loop_thread.Start();
            Data.start_flag = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath.ToString();
                Data.parent_path = textBox2.Text;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label6.Text = Data.crawled_links.ToString();
            if(Data.start_flag)
            numericUpDown1.Value = Data.nThreads;
            label4.Text = Data.Links_Queue.Count.ToString();
            label5.Text = Data.Links_found.ToString();
            label6.Text = Data.crawled_links.ToString();
            label11.Text = Data.is_there_internet.ToString();
            label10.Text = Data.downloaded_links.ToString();
            if(Data.Working!=null)
            label12.Text = Data.Working.AbsoluteUri;
            toolStripStatusLabel2.Text = listBox3.Items.Count.ToString();
            if(Data.cpuUsage!=null)
            toolStripStatusLabel4.Text = Data.cpuUsage.ToString() + "%";
            if (Data.available_ram != null)
                toolStripStatusLabel6.Text = Data.available_ram.ToString() + "MB";
                Data._AREvt.WaitOne(10, true);
            if (Data.start_flag)
            {
                if(Data.Working!=null)
                lock (Data.Working)
                {
                    if(!listBox2.Items.Contains(Data.Working))
                    listBox2.Items.Add(Data.Working);
                }
                if(Data.small_links!=null)
                lock (Data.small_links)
                {
                    foreach (Uri small_link in Data.small_links)
                        if(!listBox1.Items.Contains(small_link))
                        listBox1.Items.Add(small_link);
                }
                if(Data.Error!=null)
                    lock (Data.Error)
                    {
                        listBox3.Items.Add(Data.Error);
                        Data.Error = null;
                    }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Data.nThreads == 0 && Data.start_flag)
            {
                Data.webcrawl_thread = new Thread(() => webCrawler.start());
                Data.webcrawl_thread.Priority = ThreadPriority.Lowest;
                Data.webcrawl_thread.IsBackground = true;
                Data.webcrawl_thread.Start();
                Data._AREvt.WaitOne(10, true);
            }
            if (!Data.downloading && Data.start_flag)
            {
                webCrawler.start_download();
                Data._AREvt.WaitOne(10, true);
            }
            Performance.update_Usage();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = false;
            webCrawler.ContinueParsing();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button5.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = true;
            numericUpDown1.Enabled = true;
            webCrawler.StopParsing();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button5.Enabled = true;
            button4.Enabled = false;
            button3.Enabled = false;
            webCrawler.PauseParsing();

        }
    }
}
