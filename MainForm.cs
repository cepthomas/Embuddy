using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Design;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;

namespace Embuddy
{
    public partial class MainForm : Form
    {
        #region Fields
        UserSettings _settings;

        Connection _connDlog;

        Connection _connCli;

        Connection _connUpload;

        #endregion

        #region Lifecycle
        public MainForm()
        {
            InitializeComponent();

            _settings = (UserSettings)SettingsCore.Load(".", typeof(UserSettings));


            // Main form.
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;
            WindowState = FormWindowState.Normal;
            //BackColor = _settings.BackColor;
        }

        protected override void OnLoad(EventArgs e)
        {
            _connCli = new()
            {
                Protocol = ProtocolType.Telnet,
                Name = "CLI",
                IP = IPAddress.Parse("192.168.0.1"),
                Port = 8001,
                DisplayColor = Color.Bisque,
                Font = _settings.ConsoleFont,
                Location = new(tvTraffic.Left, tvTraffic.Bottom + 5),
               // Size = new(300, 30)
            };
            _connCli.Response += Conn_Response;
            Controls.Add(_connCli);

            _connDlog = new()
            {
                Protocol = ProtocolType.Telnet,
                Name = "DBG",
                IP = IPAddress.Parse("192.168.0.1"),
                Port = 8002,
                DisplayColor = Color.Aqua,
                Font = _settings.ConsoleFont,
                Location = new(tvTraffic.Left, _connCli.Bottom + 5),
                //Size = new(300, 30)
            };
            _connDlog.Response += Conn_Response;
            Controls.Add(_connDlog);

            _connUpload = new()
            {
                Protocol = ProtocolType.Tftp,
                Name = "UPL",
                IP = IPAddress.Parse("192.168.0.1"),
                Port = 69,
                DisplayColor = Color.LightYellow,
                Font = _settings.ConsoleFont,
                Location = new(tvTraffic.Left, _connDlog.Bottom + 5),
                //Size = new(300, 30)
            };
            _connUpload.Response += Conn_Response;
            Controls.Add(_connUpload);


            tvTraffic.MatchColors.Add(_connCli.Name, _connCli.DisplayColor);
            tvTraffic.MatchColors.Add(_connDlog.Name, _connDlog.DisplayColor);
            tvTraffic.MatchColors.Add(_connUpload.Name, _connUpload.DisplayColor);
            tvTraffic.MatchColors.Add("ERR", _settings.ErrorColor);
            //tvTraffic.BackColor = Color.Cornsilk;
            tvTraffic.Font = _settings.ConsoleFont;
            //tvTraffic.Prompt = "NOT> ";

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            // Just in case.
            //KillAll();

            // Save user settings.
            _settings.FormGeometry = new()
            {
                X = Location.X,
                Y = Location.Y,
                Width = Width,
                Height = Height
            };

            //_settings.WordWrap = textViewer.WordWrap;

            _settings.Save();

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Resource clean up.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Wait a bit in case there are some lingering events.
                System.Threading.Thread.Sleep(100);

                _connDlog.Dispose();
                _connCli.Dispose();
                _connUpload.Dispose();

                components?.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Private functions

        void Conn_Response(object? sender, ResponseEventArgs e)
        {
            Connection? conn = sender as Connection;

            string s = $"{conn!.Name} {e.Cat} {e.Content}";
            tvTraffic.AppendLine(s);
        }

        void UpdateAgg_Click(object sender, EventArgs e)
        {
            // TODO select a local file.
            //var localFilename = "????";


            //## Installation or Update of Aggregator Firmware
            //- Connect to the SHM system ethernet port.
            //- Run `putty`:
            //  - Connect to `192.168.0.1 port 8001`.
            //  - `reflash` to put the aggregator in loader mode.
            //- Open a terminal to upload the file:
            //  - `tftp -i 192.168.0.1 PUT<aggregator_xxxx_fw.bin> aggregator.bin` where where `xxxx` identifies a specific version.
            //  - `echo > reboot`.
            //  - `tftp -i 192.168.0.1 PUT reboot` to execute the new aggregator file.
            //- In the putty window:
            //  - Reconnect to `192.168.0.1 port 8001`.
            //  - `version` to confirm firmware was updated correctly.
        }

        void UpdateIM_Click(object sender, EventArgs e)
        {
            // TODO select a local file.
            //var localFilename = "????";


            //## Installation or Update of Input Module Firmware
            //- Connect to the SHM system ethernet port.
            //- Run `putty`:
            //  - Connect to `192.168.0.1 port 8001`.
            //  - Type `reflash` to put the system in loader mode.
            //- Open a terminal to upload the file:
            //  - `tftp -i 192.168.0.1 PUT<inmod1_xxxx_fw.bin inmod1.bin>` **OR**
            //    `tftp -i 192.168.0.1 PUT<inmod2_xxxx_fw.bin inmod2.bin>` where `xxxx` identifies a specific version.
            //  - Wait at least 10 seconds.
            //  - `echo > reboot`.
            //  - `tftp -i 192.168.0.1 PUT reboot` to restart the loader.
            //- In the putty window:
            //  - Reconnect to `192.168.0.1 port 8001`.
            //  - `modselect 1`  **OR** `modselect 2` depending on which file was selected in the preceding step.
            //  - `modboot`.
            //  - Reconnect to `192.168.0.1 port 8001`.
            //  - `modinfo` to confirm firmware was updated correctly.
        }

        void Version_Click(object sender, EventArgs e)
        {
            // "version",     1, 1,  "",                 "Display firmware version"},
            // "loglevel",    1, 2,  "[level]",          "Show or set log output level"},
            // "modinfo",     1, 1,  "",                 "Report the version information for input modules"},
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="msg"></param>
        void WriteLine(string msg)// TODO color
        {
            tvTraffic.AppendText($"> {msg}{Environment.NewLine}");
        }

        void Debug_Click(object sender, EventArgs e)
        {
            //DoChecksum();

            tvTraffic.Clear();

            var ls = File.ReadAllLines(@"C:\Dev\AL\lmes\Embuddy\files\some.txt");

            foreach (var l in ls)
            {
                WriteLine(l);

            }
        }

        /// <summary>
        /// Edit the common options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(_settings, "User Settings", 500);

            // Detect changes of interest.
            bool restart = false;

            foreach (var (name, cat) in changes)
            {
                switch (name)
                {
                    case "TODO":
                        restart = true;
                        break;
                }
            }

            if (restart)
            {
                MessageBox.Show("Restart required for changes to take effect");
            }
        }
        #endregion

        ////////////////// STUFF //////////////////////

        void DoChecksum()
        {
            //- bins\dummy5120.bin has 5120 bytes = 1280 uint = 640 msg/block = 2.5 pages
            //- TBD> Num:5120 Chk1:83178260 Chk2:40 XOR:0 
            //- bins\dummy5121.bin has 5121 bytes = 1281 uint = 640 msg/block = 2.5 pages
            //- TBD> Num:5121 Chk1:83178281 Chk2:61 XOR:21
            //- agg rounds up to 3 pages = 6144 bytes = 1536 uint = 768 msg

            byte[] data = File.ReadAllBytes(@"C:\Dev\AL\lmes\Embuddy\files\dummy5120.bin");


            var numBytes = data.Length;
            // Pad.
            while(numBytes % 4 != 0) numBytes++;
            byte[] data2 = new byte[numBytes];
            byte[] xors = new byte[numBytes];
            data.CopyTo(data2, 0);

            byte chk_xor = 0;
            uint chk_add1 = 0;
            byte chk_add2 = 0;

            // Simple int add chksum.
            for (int i = 0; i < data2.Length; i += 4)
            {
                uint w = data2[i];
                w += (uint)(data2[i + 1]) << 8;
                w += (uint)(data2[i + 2]) << 16;
                w += ((uint)data2[i + 3]) << 24;
                chk_add1 += w;
            }

            // Simple byte add chksum.
            for (int i = 0; i < data2.Length; i++)
            {
                chk_add2 += data2[i];
            }

            // XOR
            //for each byte b in the buffer do
            //   lrc:= (lrc + b) and 0xFF
            //lrc:= (((lrc XOR 0xFF) +1) and 0xFF)


            for (int i = 0; i < data2.Length; i++)
            {



                //chk_xor ^= data2[i];
                xors[i] = chk_xor;
            }

            //string formatString = String.Format("{0,10:G}: {0,10:X}", value);

            string msg = $"Num:{data.Length} Chk1:{chk_add1:X} Chk2:{chk_add2:X} XOR:{chk_xor:X}";
            WriteLine(msg);
        }

        void Fill()
        {
            tvTraffic.Clear();

            var ls = File.ReadAllLines(@"C:\Dev\AL\lmes\Embuddy\some.txt");

            foreach (var l in ls)
            {
                tvTraffic.AppendText($"> {l}{Environment.NewLine}");
            }
        }
    }
}
