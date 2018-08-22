using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monkey_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "Monkey Test";
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            init_start();
            TipShow();
            //关闭窗口事件
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);

        }

        private void TipShow() {
            // Set up the delays for the ToolTip.
            //保持悬停仍然消失的时间
            toolTip1.AutoPopDelay = 5000;
            //悬停时间
            toolTip1.InitialDelay = 100;
            //鼠标移开后消失时间
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active. 
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox. 
            toolTip1.SetToolTip(label12, "相同seed值会重现相同的事件步骤");
        }

        private void init_start()
        {
            label0.Text = "测试对象包名";
            label1.Text = "随机事件数量";
            label2.Text = "延迟时间(ms)";
            label3.Text = "测试模式";
            label4.Text = "触摸事件概率(%)";
            label5.Text = "滑动事件概率(%)";
            label6.Text = "轨迹球概率(%)";
            label7.Text = "系统按键概率(%)";
            label8.Text = "启动Activity(%)";
            label9.Text = "导航事件概率(%)";
            label10.Text = "主要导航概率(%)";
            label11.Text = "任意事件概率(%)";
            label12.Text = "种子值(seed)";
            label13.Text = "忽略安全异常";
            label14.Text = "忽略超时";
            label15.Text = "忽略崩溃";
            label16.Text = "日志级别";

            radioButton1.Text = "简易模式";
            radioButton2.Text = "高级模式";

            //radioButton1.Checked = true;
            //radioButton5.Checked = true;

            textBox0.Text = GetConfigValue("package_name", "");
            textBox1.Text = GetConfigValue("event_number", "0");
            textBox2.Text = GetConfigValue("delay_time", "0");
            textBox4.Text = GetConfigValue("touch", "0");
            textBox5.Text = GetConfigValue("motin", "0");
            textBox6.Text = GetConfigValue("trackball", "0");
            textBox7.Text = GetConfigValue("syskeys", "0");
            textBox8.Text = GetConfigValue("appswitch", "0");
            textBox9.Text = GetConfigValue("nav", "0");
            textBox10.Text = GetConfigValue("majornav", "0");
            textBox11.Text = GetConfigValue("anyevent", "0");
            textBox12.Text = GetConfigValue("seed", "");

            int test_mode = int.Parse(GetConfigValue("test_mode", "-1"));
            if (test_mode == 0)
            {
                radioButton1.Checked = true;
            }
            else if (test_mode == 1)
            {
                radioButton2.Checked = true;
            }
            else {
                radioButton1.Checked = true;
            }

            checkBox1.Text = "";
            checkBox2.Text = "";
            checkBox3.Text = "";

            if (GetConfigValue("ignore_security_exceptions", "true") == "true") {
                checkBox1.Checked = true;
            }
            if (GetConfigValue("ignore_timeouts", "true") == "true")
            {
                checkBox2.Checked = true;
            }
            if (GetConfigValue("ignore_crashes", "true") == "true")
            {
                checkBox3.Checked = true;
            }


            int log_level = int.Parse(GetConfigValue("log_level", "-1"));
            if (log_level == 1)
            {
                radioButton3.Checked = true;
            }
            else if (log_level == 2)
            {
                radioButton4.Checked = true;
            }
            else if (log_level == 3)
            {
                radioButton5.Checked = true;
            }
            else {
                radioButton5.Checked = true;
            }

        }

        private String IfInt(String num) {
            return IfInt(num, true);
        }

        private String IfInt(String num, Boolean showMessageBox)
        {
            if (int.TryParse(num, out int tmp1) && int.Parse(num) >= 0)
            {
                return num;
            }
            else
            {
                if (showMessageBox) { MessageBox.Show(num + "不是正整数,已做为0处理！"); }
                return "0";

            }
        }

        //初始值
        String package_name = "";
        String event_number = "";
        String delay_time = "";
        String touch = "";
        String motin = "";
        String trackball = "";
        String syskeys = "";
        String appswitch = "";
        String nav = "";
        String majornav = "";
        String anyevent = "";
        String seed = "";

        private void getTextBoxValue() {
            //if (int.TryParse(textBox1.Text, out int tmp1)) { event_number = textBox1.Text; }
            package_name = textBox0.Text;
            event_number = IfInt(textBox1.Text);
            delay_time = IfInt(textBox2.Text);
            touch = IfInt(textBox4.Text);
            motin = IfInt(textBox5.Text);
            trackball = IfInt(textBox6.Text);
            syskeys = IfInt(textBox7.Text);
            appswitch = IfInt(textBox8.Text);
            nav = IfInt(textBox9.Text);
            majornav = IfInt(textBox10.Text);
            anyevent = IfInt(textBox11.Text);

            if (int.TryParse(textBox12.Text, out int tmp1) && int.Parse(textBox12.Text) >=0)
            {
                seed = IfInt(textBox12.Text);
            }
            else
            {
                seed = "";
                Console.WriteLine("种子值不是正整数，已做空处理！");

            }


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            getTextBoxValue();
            String cmd = GetCMD(radioButton1.Checked);
            WriteFile(@".\Monkey Run.bat", cmd);
            RunBat(@".\", "Monkey Run.bat");
            saveConfig();


        }


        private string GetCMD(Boolean easyMode) {
            string ignore_error = "";
            if (checkBox1.Checked == true) { ignore_error += " --ignore-security-exceptions";  }
            if (checkBox2.Checked == true) { ignore_error += " --ignore-timeouts"; }
            if (checkBox3.Checked == true) { ignore_error += " --ignore-crashes"; }
            string log_level = "";
            if (radioButton3.Checked == true)
            {
                log_level = " -v ";
            }
            else if (radioButton4.Checked == true) {
                log_level = " -v-v ";

            } else if (radioButton5.Checked == true) {
                log_level = " -v-v-v ";
            }

            if (seed != "")
            {
                seed = " -s " + seed;
            }

            String cmd = "";
            cmd += @"@echo off" + "\r\n\r\n";

            /*
            DateTime now = DateTime.Now;
            Console.WriteLine(now.ToString("yyyyMMddHHmmss"));
            */

            if (GetConfigValue("is_group", "false") == "true")
            {
                cmd += @"::生成当前日期" + "\r\n";
                cmd += @"set dateTmp=%date:~0,4%%date:~5,2%%date:~8,2%" + "\r\n";
                cmd += "if \"%time:~0,2%\" lss \"10\" (set hour=0%time:~1,1%) else (set hour=%time:~0,2%)" + "\r\n";
                cmd += @"set timeTmp=%hour%%time:~3,2%%time:~6,2%" + "\r\n";
                cmd += @"set d=%dateTmp%%timeTmp%" + "\r\n";
                cmd += @"echo 当前时间: %d%" + "\r\n";
                cmd += "\r\n";
                cmd += "mkdir \"%d%\"" + "\r\n";
                cmd += "\r\n";
            }

            cmd += @"::---------配置文件-------------" + "\r\n";
            if (GetConfigValue("is_group", "false") == "true")
            {
                cmd += @"::LOG日志存放路径" + "\r\n";
                cmd += @"set FILE_PATH=.\%d%" + "\r\n";
                cmd += @"set LOG=%FILE_PATH%\Log.txt" + "\r\n";
            }
            else {
                cmd += @"::LOG日志存放路径" + "\r\n";
                cmd += @"set FILE_PATH=.\" + "\r\n";
                cmd += @"if not exist %FILE_PATH%\Log ( md %FILE_PATH%\Log )" + "\r\n";
                cmd += @"set LOG=%FILE_PATH%\Log\Log.txt" + "\r\n";
            }
            cmd += @"::------------------------------" + "\r\n\r\n";

            cmd += @"echo.开始测试 && echo...." + "\r\n";
            if (easyMode)
            {
                cmd += @"adb shell monkey -p " + package_name + " --throttle " + delay_time + " -v " + event_number + " >%LOG%" + "\r\n";
            }
            else
            {
                cmd += @"adb shell monkey -p " + package_name + seed + " --pct-touch " + touch + " --pct-motion " + motin + " --pct-syskeys " + syskeys + " --pct-trackball " + trackball + ignore_error + " --monitor-native-crashes --throttle " + delay_time + log_level + event_number + " >%LOG%" + "\r\n";
            }
            cmd += @"echo.测试结束 && echo." + "\r\n\r\n";
            cmd += "findstr " + "\"" + "ANR" + "\"" + " " + "\"" + "%LOG%" + "\"" + @" > nul && if not exist %FILE_PATH%\Error ( md %FILE_PATH%\Error ) && echo.发现ANR，提取Log中...&& adb pull data/anr/traces.txt > %FILE_PATH%\Error\traces.txt && adb shell logcat -d -v time > %FILE_PATH%\Error\Log_ANR.txt && adb bugreport > %FILE_PATH%\Error\BugReport.txt && echo ------提取完成！" + "\r\n";
            cmd += "findstr " + "\"" + "CRASH" + "\"" + " " + "\"" + "%LOG%" + "\"" + @" > nul &&  if not exist %FILE_PATH%\Error ( md %FILE_PATH%\Error ) && echo.发现CRASH，提取Log中...&& adb shell logcat -d -v time > %FILE_PATH%\Error\Log_Crash.txt && adb bugreport > %FILE_PATH%\Error\BugReport.txt && echo ------提取完成！" + "\r\n";
            cmd += "\r\n";
            cmd += @"echo." + "\r\n";
            cmd += @"pause" + "\r\n";
            return cmd;
        }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //string apptPath = @"D:\android-sdk-windows\build-tools\28.0.1\aapt.exe";
            string apptPath = GetConfigValue("android_sdk_path", @".\Tools") + @"\aapt.exe";

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "apk文件|*.apk|所有文件|*.*"; //设置要选择的文件的类型
            string file = "";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                file = fileDialog.FileName;//返回文件的完整路径  
                String result = RunCmd("\"" + apptPath + "\"" + @" dump badging " + "\"" + file + "\"");
                //Console.WriteLine(result);
                string pattern = @"package: name='*.*.*' versionCode";
                string package_name = "";
                foreach (Match match in Regex.Matches(result, pattern))
                {
                    package_name = match.Value.Replace("package: name='", "").Replace("' versionCode", "");
                    Console.WriteLine(package_name);
                }
                textBox0.Text = package_name;
            }


            
        }

        private void WriteFile(string path, string batStr)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("GB2312"));
            //开始写入
            sw.Write(batStr);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        private void RunBat(String fileDir, String fileName)
        {
            Process proc = null;
            try
            {
                string targetDir = string.Format(fileDir);//this is where mybatch.bat lies
                proc = new Process();
                proc.StartInfo.WorkingDirectory = targetDir;
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.Arguments = string.Format("10");//this is argument
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }

        private String RunCmd(string strInput) {
            Process p = new Process();
            //设置要启动的应用程序
            p.StartInfo.FileName = "cmd.exe";
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            p.StartInfo.CreateNoWindow = true;
            //启动程序
            p.Start();

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(strInput + " && exit");

            p.StandardInput.AutoFlush = true;

            //获取输出信息
            string strOuput = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();

            Console.WriteLine(strOuput);
            //MessageBox.Show(strOuput);
            //Console.ReadKey();
            return strOuput;
        }

        // 读取指定key的值 //ConfigurationSettings.AppSettings[“conn”]
        public static string GetConfigValue(string key, string default_value)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[key] == null)
                return default_value;
            else
                return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
        }

        private void SetConfigValue(string key, string new_value)
        {
            //获取Configuration对象
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根据Key读取<add>元素的Value
            if (config.AppSettings.Settings[key] == null)
            {
                //增加<add>元素
                config.AppSettings.Settings.Add(key, new_value);
            }
            else
            {
                //写入<add>元素的Value
                config.AppSettings.Settings[key].Value = new_value;
            }
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label4.Enabled = false;
            label5.Enabled = false;
            label6.Enabled = false;
            label7.Enabled = false;
            label8.Enabled = false;
            label9.Enabled = false;
            label10.Enabled = false;
            label11.Enabled = false;
            label12.Enabled = false;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            textBox8.ReadOnly = true;
            textBox9.ReadOnly = true;
            textBox10.ReadOnly = true;
            textBox11.ReadOnly = true;
            textBox12.ReadOnly = true;

            label13.Enabled = false;
            label14.Enabled = false;
            label15.Enabled = false;
            label16.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            radioButton5.Enabled = false;

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label4.Enabled = true;
            label5.Enabled = true;
            label6.Enabled = true;
            label7.Enabled = true;
            label8.Enabled = true;
            label9.Enabled = true;
            label10.Enabled = true;
            label11.Enabled = true;
            label12.Enabled = true;
            textBox4.ReadOnly = false;
            textBox5.ReadOnly = false;
            textBox6.ReadOnly = false;
            textBox7.ReadOnly = false;
            textBox8.ReadOnly = false;
            textBox9.ReadOnly = false;
            textBox10.ReadOnly = false;
            textBox11.ReadOnly = false;
            textBox12.ReadOnly = false;

            label13.Enabled = true;
            label14.Enabled = true;
            label15.Enabled = true;
            label16.Enabled = true;
            checkBox1.Enabled = true;
            checkBox2.Enabled = true;
            checkBox3.Enabled = true;
            radioButton3.Enabled = true;
            radioButton4.Enabled = true;
            radioButton5.Enabled = true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string message = "";
            message += "        作者：" + "\r\n";
            message += "        tomoya_chen" + "\r\n";
            message += "        优氏英语测试部" + "\r\n";
            message += "        版本: v1.2" + "\r\n";
            message += "        \r\n";
            message += "        帮助：" + "\r\n";
            message += "        \\Tools\\aapt.exe用于分析包名" + "\r\n";
            message += "        .config文件可修改各项参数默认值" + "\r\n";
            message += "        \\Log目录存放执行日志" + "\r\n";
            message += "        \\Error目录存放错误日志" + "\r\n";

            MessageBox.Show(message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getTextBoxValue();
            String cmd = GetCMD(radioButton1.Checked);
            WriteFile(@".\Monkey Run.bat", cmd);
            RunBat(@".\", "Monkey Run.bat");
        }
        //保存配置功能
        private void saveConfig()
        {

            SetConfigValue("package_name", package_name);
            SetConfigValue("event_number", event_number);
            SetConfigValue("delay_time", delay_time);
            if (radioButton1.Checked)
            {
                SetConfigValue("test_mode", "0");
            }
            else if (radioButton2.Checked)
            {
                SetConfigValue("test_mode", "1");
            }
            else
            {
                SetConfigValue("test_mode", "1");
            }
            SetConfigValue("touch", touch);
            SetConfigValue("motin", motin);
            SetConfigValue("trackball", trackball);
            SetConfigValue("syskeys", syskeys);
            SetConfigValue("appswitch", appswitch);
            SetConfigValue("nav", nav);
            SetConfigValue("majornav", majornav);
            SetConfigValue("anyevent", anyevent);
            SetConfigValue("seed", seed);
            if (checkBox1.Checked)
            {
                SetConfigValue("ignore_security_exceptions", "true");
            }
            else
            {
                SetConfigValue("ignore_security_exceptions", "false");
            }
            if (checkBox1.Checked)
            {
                SetConfigValue("ignore_timeouts", "true");
            }
            else
            {
                SetConfigValue("ignore_timeouts", "false");
            }
            if (checkBox1.Checked)
            {
                SetConfigValue("ignore_crashes", "true");
            }
            else
            {
                SetConfigValue("ignore_crashes", "false");
            }
            if (radioButton3.Checked)
            {
                SetConfigValue("log_level", "1");
            }
            else if (radioButton4.Checked)
            {
                SetConfigValue("log_level", "2");
            }
            else if (radioButton5.Checked)
            {
                SetConfigValue("log_level", "3");
            }
            else
            {
                SetConfigValue("log_level", "3");
            }
        }

        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            getTextBoxValue();
            saveConfig();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
 
        }

        private void traces日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void bugreportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        //根据文件夹全路径创建文件夹
        public static void CreateDir(string path)
        {

            //string path = @".\Error";
            if (Directory.Exists(path))
            {
                Console.WriteLine("此文件夹已经存在，无需创建！");
            }
            else
            {
                Directory.CreateDirectory(path);
                Console.WriteLine(path + " 创建成功!");
            }
        }

        private void 导出logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String result = RunCmd(@"adb devices");
            Console.WriteLine(result);
            MessageBox.Show(result);
        }

        private void 导出tracesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDir(@".\Error");
            String result = RunCmd(@"adb shell logcat -d -v time > .\Error\Log.txt");
            Console.WriteLine(result);
            MessageBox.Show(result);
        }

        private void 导出bugreportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDir(@".\Error");
            String result = RunCmd(@"adb pull data/anr/traces.txt > .\Error\traces.txt");
            Console.WriteLine(result);
            MessageBox.Show(result);
        }

        private void 导出bugreportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CreateDir(@".\Error");
            String result = RunCmd(@"adb bugreport > .\Error\BugReport.txt");
            Console.WriteLine(result);
            MessageBox.Show(result);
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
