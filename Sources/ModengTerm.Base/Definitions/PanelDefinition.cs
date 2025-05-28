using Newtonsoft.Json;
using WPFToolkit.MVVM;

namespace ModengTerm.Base.Definitions
{
    public class PanelItemDefinition : MenuDefinition
    {
        /// <summary>
        /// 支持的会话类型
        /// </summary>
        [JsonProperty("sessionTypes")]
        public List<int> SessionTypes { get; set; }

        public PanelItemDefinition()
        {
            this.SessionTypes = new List<int>();
        }
    }
}
