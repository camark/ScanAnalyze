using ScanAnalyze.Util;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScanAnalyze
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        string[] levels = { "1楼", "2楼", "3楼", "4楼" };

        string localIP = String.Empty;
        string[] printerIP
            =
            {
                "10.10.2.251",
                "10.10.1.251",
                "10.10.3.251",
                "10.10.4.200",
            };

        string addrUrl = @"http://{0}/addr/cgi-bin/addrDatLst.cgi?pagN=1&sortBy=MFDN&sortOrder=0&notAuth=1";

        RegUtil regUtil = new RegUtil();
        private void Form1_Load(object sender, EventArgs e)
        {
            

            localIP= new IPUtil().GetIpAddress();

            cbbLevel.SelectedIndex = getLevel(localIP);

            var reg_username= regUtil.ReadLastUserName();

            if (!String.IsNullOrEmpty(reg_username))
            {
                tbName.Text = reg_username;
            }
            else
            {
                tbName.Text = Environment.UserName;
            }

            tsslIP.Text = $"本机IP地址:{localIP}";
        }

        public string getRange(string ip)
        {
            string subnet = ip.Substring(0, "10.10.1".Length);

            return subnet;
        }

        public int getLevel(string ip)
        {
            int j = -1;

            for(int i=0;i<printerIP.Length;i++)
            {
                var ip1 = printerIP[i];
                if (getRange(localIP).Equals(getRange(ip1)))
                {
                    return i;
                }
            }

            return j;
        }

        private string RemoveDot(string s)
        {
            string[] dots = { "{DN:", "HtmlTagOppAdjust", "'", "(", ")","CN:","CT:" };

            String temp = s;

            foreach(var dot in dots)
            {
                temp = temp.Replace(dot, "");
            }

            return temp;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string userName = tbName.Text.Trim();

            if (userName.Length == 0)
            {
                MessageUtil.ShowError("请首先设定扫描所用的姓名！");
                return;
            }

            listView1.Items.Clear();
            try
            {
                string realUrl = String.Format(addrUrl, printerIP[cbbLevel.SelectedIndex]);

                string htmlData = HttpUtil.SimpleRequest(realUrl, cbbLevel.SelectedIndex == 3);
                //textBox1.Text = htmlData;

                bool find_it = false;
                List<String> lines = new List<string>();
                foreach (var text in htmlData.Split(new char[] { '\n','\r' }))
                {
                    if (text == "var addrDataArr = [")
                    {
                        //MessageBox.Show("Found It!");
                        //break;
                        find_it = true;
                    }

                    if (find_it)
                    {
                        lines.Add(text);
                    }

                    if(find_it && Text == "-->")
                    {
                        break;
                    }
                }

                bool bFindUser = false;
                bool bIPRight = false;
                string findip = String.Empty;
                
                foreach (var l in lines)
                {
                    string[] t1 = l.Split(new char[] { ',' });

                    if (t1.Length > 4)
                    {
                        var item = new ListViewItem();
                        item.Text = RemoveDot(t1[0]);
                       
                        string printer_username = RemoveDot(t1[2]);
                        item.SubItems.Add(printer_username);
                        string printer_ip = RemoveDot(t1[3]);
                        item.SubItems.Add(printer_ip);

                        if(printer_username.Trim()==userName)
                        {
                            bFindUser = true;

                            if (printer_ip.Trim().Equals(localIP))
                            {
                                bIPRight = true;
                            }
                            else
                            {
                                findip = printer_ip.Trim();
                            }
                        }

                        listView1.Items.Add(item);
                    }
                }

                if (!bFindUser)
                {
                    MessageUtil.ShowError("发现错误：打印机当前没有对应的扫描用户");
                    return;
                }

                if(bFindUser && !bIPRight)
                {
                    MessageUtil.ShowError($"发现错误：打印机设定的IP与本地IP不对应，请修改！设定的IP为{findip},实际IP为{localIP}");
                    return;
                }

                if(bIPRight && bFindUser)
                {
                    MessageUtil.ShowInformation("打印机扫描设定没有问题，恭喜您！");
                    //return;
                }

                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message+"，请重新分析！");
            }

            AnalyzeFtpRunning();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //AnalyzeFtpRunning();
        }

        private void AnalyzeFtpRunning()
        {
            try
            {
                FTPClient ftpClient = new FTPClient(localIP, "/", "anonymous", "", 21);
                ftpClient.Connect();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError("本机接收软件可能没有运行！");
                return;
            }

            MessageUtil.ShowInformation("本地电脑接收没有发现问题！");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            regUtil.WriteLastUserName(tbName.Text.Trim());
        }
    }
}
