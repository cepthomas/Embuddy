using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net;
using Ephemera.NBagOfTricks;

namespace Embuddy
{
    public enum ProtocolType { None, Tftp, Telnet, CAN };

    public enum ResponseCat { INF, WRN, ERR, LOG };

    
    public partial class Connection : UserControl
    {
        #region Fields
        // string _prompt = "";

        #endregion

        #region Properties
        public ProtocolType Protocol { get; set; } = ProtocolType.None;

        public string Name { get; set; } = "???";

        public IPAddress IP { get; set; } = IPAddress.Parse("127.0.0.1");

        public int Port { get; set; } = 0;

        [JsonConverter(typeof(JsonColorConverter))]
        public Color DisplayColor { get; set; } = Color.AliceBlue;

        [Browsable(false)]
        public List<string> RecentCommands { get; set; } = new();

        [Browsable(false)]
        [JsonIgnore]
        public int CommandListSize { get; set; } = 20;

        /// <summary>Log on/off.</summary>
        public bool LogEnable { get; set; }

        /// <summary>Connected.</summary>
        public bool Valid { get; private set; }
        #endregion

        #region Events
        public event EventHandler<ResponseEventArgs>? Response;
        #endregion


        #region Lifecycle
        public Connection()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            rtbInput.KeyDown += RtbInput_KeyDown;

            lblName.Text = $"{Name}>";

            //rtbInput.BackColor = DisplayColor;

            base.OnLoad(e);
        }

        #endregion

        #region Public functions

        #endregion

        #region Private functions

        void RtbInput_KeyDown(object? sender, KeyEventArgs e)
        {
            // up/down goes through mru

       //     //  Check whether we are in the read-only zone.
       //     var isInReadOnlyZone = richTextBoxConsole.SelectionStart < inputStart;
       //     //  Are we sending keyboard commands to the process?
       //     if (SendKeyboardCommandsToProcess && IsProcessRunning)
       //     {
       //         //  Get key mappings for this key event?
       //         var mappings = from k in keyMappings
       //                        where 
       //                        (k.KeyCode == e.KeyCode &&
       //                        k.IsAltPressed == e.Alt &&
       //                        k.IsControlPressed == e.Control &&
       //                        k.IsShiftPressed == e.Shift)
       //                        select k;
       //         //  Go through each mapping, send the message.
       //         //foreach (var mapping in mappings)
       //         //{
       //             //SendKeysEx.SendKeys(CurrentProcessHwnd, mapping.SendKeysMapping);
       //             //inputWriter.WriteLine(mapping.StreamMapping);
       //             //WriteInput("\x3", Color.White, false);
       //         //}
       //         //  If we handled a mapping, we're done here.
       //         if (mappings.Any())
       //         {
       //             e.SuppressKeyPress = true;
       //             return;
       //         }
       //     }
       //     //  If we're at the input point and it's backspace, bail.
       //     if ((richTextBoxConsole.SelectionStart <= inputStart) && e.KeyCode == Keys.Back) e.SuppressKeyPress = true;
       //     //  Are we in the read-only zone?
       //     if (isInReadOnlyZone)
       //     {
       //         //  Allow arrows and Ctrl-C.
       //         if (!(e.KeyCode == Keys.Left ||
       //             e.KeyCode == Keys.Right ||
       //             e.KeyCode == Keys.Up ||
       //             e.KeyCode == Keys.Down ||
       //             (e.KeyCode == Keys.C && e.Control)))
       //         {
       //             e.SuppressKeyPress = true;
       //         }
       //     }
       //     //  Write the input if we hit return and we're NOT in the read only zone.
       //     if (e.KeyCode == Keys.Return && !isInReadOnlyZone)
       //     {
       //         //  Get the input.
       //         string input = richTextBoxConsole.Text.Substring(inputStart, (richTextBoxConsole.SelectionStart) - inputStart);
       //         //  Write the input (without echoing).
       //         WriteInput(input, Color.White, false);
       //     }
       // }
        }

        protected virtual void OnResponse(ResponseEventArgs e)
        {
            Response?.Invoke(this, e);
        }


        /// <summary>
        /// Update the RecentCommands.
        /// </summary>
        /// <param name="newVal">New value to insert.</param>
        void UpdateMru(string newVal)
        {
            RecentCommands.Insert(0, newVal);
            CleanMru();
        }

        /// <summary>
        /// Remove duplicates.
        /// </summary>
        void CleanMru()
        {
            // Clean up mru. This is a bit klunky looking but safest way to maintain order.
            var newlist = new List<string>();
            RecentCommands.ForEach(f => { if (!newlist.Contains(f) && newlist.Count <= CommandListSize) newlist.Add(f); });
            RecentCommands = newlist;
        }
        #endregion
    }


    /// <summary>Something to tell the client.</summary>
    public class ResponseEventArgs : EventArgs
    {
        public string Content { get; set; }

        public ResponseCat Cat { get; set; }

        public ResponseEventArgs(ResponseCat cat, string content)
        {
            Cat = cat;
            Content = content;
        }
    }
}
