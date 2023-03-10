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
using Ephemera.NBagOfTricks.Slog;
using Ephemera.NBagOfUis;


//TODO RTT: https://wiki.segger.com/RTT#TELNET_channel_of_J-Link_software
// J-Link RTT Client acts as a Telnet client, but automatically tries to reconnect to a J-Link connection when a debug session is closed. 


namespace Embuddy
{
    public partial class MainForm : Form
    {
        #region Fields
        UserSettings _settings;


        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        #endregion

        #region Lifecycle
        public MainForm()
        {
            InitializeComponent();

            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Embuddy", "Ephemera");
            _settings = (UserSettings)SettingsCore.Load(appDir, typeof(UserSettings));

            // Set up logging.
            LogManager.MinLevelFile = LogLevel.Trace;
            LogManager.MinLevelNotif = LogLevel.Debug;
            LogManager.LogMessage += LogManager_LogMessage;
            LogManager.Run(Path.Join(appDir, "log.txt"), 100000);

            // Main form.
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;
            WindowState = FormWindowState.Normal;
            //BackColor = _settings.BackColor;
        }

        protected override void OnLoad(EventArgs e)
        {
            Init_TODO();

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
                Thread.Sleep(100);

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


        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="cat"></param>
        // /// <param name="msg"></param>
        // void WriteLine(string msg)// TODO color
        // {
        //     tvTraffic.AppendText($"> {msg}{Environment.NewLine}");
        // }


        void LogManager_LogMessage(object? sender, LogMessageEventArgs e)
        {
            tvTraffic.AppendLine($"> {e.Message}");
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

        ////////////////// SCRIPTY? //////////////////////

        Connection _connDlog = new Telnet();
        Connection _connCli = new Telnet();
        Connection _connUpload = new Tftp();

        void Init_TODO()
        {
            _connCli = new Telnet()
            {
                //ConnType = ConnectionType.Telnet,
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

            _connDlog = new Telnet()
            {
                //ConnType = ConnectionType.Telnet,
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

            _connUpload = new Tftp()
            {
                //ConnType = ConnectionType.Tftp,
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

    }
}
