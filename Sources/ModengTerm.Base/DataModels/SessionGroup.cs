using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    public class SessionGroup : ModelBase
    {
        /// <summary>
        /// 父分组的Id
        /// </summary>
        [JsonProperty("pid")]
        public string ParentId { get; set; }
    }
}
