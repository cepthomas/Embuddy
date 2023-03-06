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
    [Serializable]
    public sealed class UserSettings : SettingsCore
    {
        [Editor(typeof(MonospaceFontEditor), typeof(UITypeEditor))]
        [JsonConverter(typeof(JsonFontConverter))]
        public Font ConsoleFont { get; set; } = new("Consolas", 10);

        [JsonConverter(typeof(JsonColorConverter))]
        public Color ValidColor { get; set; } = Color.LightGreen;

        [JsonConverter(typeof(JsonColorConverter))]
        public Color ErrorColor { get; set; } = Color.Pink;

        [JsonConverter(typeof(JsonColorConverter))]
        public Color WarnColor { get; set; } = Color.Plum;

        [JsonConverter(typeof(JsonColorConverter))]
        public Color NeutralColor { get; set; } = SystemColors.Control;
    }
}
