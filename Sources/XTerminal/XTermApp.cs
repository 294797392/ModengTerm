using DotNEToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.ServiceAgents;
using XTerminal.Session.Definitions;

namespace XTerminal
{
    public class XTermManifest : AppManifest
    {
        [JsonProperty("sessions")]
        public List<SessionDefinition> SessionList { get; private set; }

        public XTermManifest()
        {
            this.SessionList = new List<SessionDefinition>();
        }
    }

    public class XTermApp : ModularApp<XTermApp, XTermManifest>
    {
        protected override bool AsyncInitializing => false;

        public ServiceAgent ServiceAgent { get; private set; }

        protected override int OnInitialize()
        {
            this.ServiceAgent = this.Factory.LookupModule<ServiceAgent>();

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }
    }
}
