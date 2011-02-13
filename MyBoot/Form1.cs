using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MyBoot
{

    public partial class Form1 : Form
    {
        class Application
        {
            public String Name;
            public String ExePath;
            public int Number;

            public void Start()
            {
                string MyBootExePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString());
                string AppExePath;
                if ((ExePath[0].CompareTo('.') == 0) && (ExePath[1].CompareTo('\\') == 0))
                {
                    // path relative to MyBoot exe
                    AppExePath = MyBootExePath + ExePath.Substring(1);
                }
                else
                {
                    AppExePath = ExePath;
                }
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = AppExePath;
                process.Start();
            }
        }

        private List<Application> ApplicationList = new List<Application>();
        private int AutoStart = -1;

        private List<Button> ButtonList = new List<Button>();


        static public string AppPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString());
        }

        public Form1()
        {
            InitializeComponent();

            ButtonList.Add(button1);
            ButtonList.Add(button2);
            ButtonList.Add(button3);
            ButtonList.Add(button4);
            ButtonList.Add(button5);

            // read ini file
            string ApplicationPath = AppPath();
            ReadIniFile(ApplicationPath + "\\myboot.ini");

            // configure buttons            
            foreach (Application a in ApplicationList)
            {
                if (a.Number <= ButtonList.Count)
                {
                    ButtonList[a.Number - 1].Text = a.Name;
                    ButtonList[a.Number - 1].Visible = true;
                    ButtonList[a.Number - 1].Click +=
                    new System.EventHandler(
                        delegate(object sender, EventArgs e)
                        {
                            Button theButton = (Button) sender;
                            int AppNumber = System.Convert.ToInt16(theButton.Name.Substring(6));
                            Application theApplication = ApplicationList[AppNumber - 1];

                            theApplication.Start();

                            AutoStart = -1; // cancel auto start, if any
                            timer1.Enabled = false;
                            labelStarting.Text = "";
                        }
                    );

                }

                if (ApplicationList.Count >= 4)
                    pictureBoxChakira.Visible = false;

            }
        }

        public bool ReadIniFile(String FileName)
        {
            if (System.IO.File.Exists(FileName))
            {
                try
                {
                    FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(file);
                    // Button1 = "\MyDir\Myapp.exe"; MyApp
                    String line="";
                    String right;
                    String ButtonName;
                    String autostartCmd="";

                    while (line != null)
                    {
                        line = sr.ReadLine(); if (line == null) { sr.Close(); return true; }

                        Application app = new Application();

                        ButtonName = line.Split('=')[0];
                        app.Number = System.Convert.ToInt16(ButtonName.Substring(6));

                        right = line.Split('=')[1];
                        app.ExePath = right.Split(';')[0].Trim().Replace("\"","");
                        app.Name = right.Split(';')[1].Trim();

                        ApplicationList.Add(app);

                        if (right.Split(';').Length > 2)
                        {
                            autostartCmd = right.Split(';')[2].Trim();
                            

                            if (autostartCmd.Substring(0, 9) == "autostart")
                                if (autostartCmd.Split(' ').Length >= 2)
                                {
                                    AutoStart = System.Convert.ToInt16(autostartCmd.Split(' ')[1]);
                                    timer1.Interval = 1000;
                                    timer1.Enabled = true;
                                }
                        }
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
            else return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString());
            AppPath = AppPath + "\\MobileNavigator_Igo.exe";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = AppPath;
            process.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString());
            AppPath = AppPath + "\\maplorer\\maplorer.exe";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = AppPath;
            process.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonMobileNavigator_Click(object sender, EventArgs e)
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString());
            AppPath = AppPath + "\\MobileNavigator_wake.exe";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = AppPath;
            process.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelStarting.Text = "Starts in " + AutoStart.ToString() + " s";
            labelStarting.Visible = true;
            AutoStart--;

            if (AutoStart <= 0) 
            {
                Application theApplication = ApplicationList[0];
                theApplication.Start();

                timer1.Enabled = false;

                labelStarting.Visible = false;
            }
        }       
    }
}