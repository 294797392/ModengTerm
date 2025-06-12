using DotNEToolkit.DataModels;
using ModengTerm.Addons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    public class BroadcastSession : StorageObject
    {
        /// <summary>
        /// 要广播到的会话Id
        /// </summary>
        [JsonProperty("bcsid")]
        public string BroadcastSessionId { get; set; }
    }
}