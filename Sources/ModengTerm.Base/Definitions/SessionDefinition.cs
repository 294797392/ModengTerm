﻿using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Base.Definitions
{
    public class SessionDefinition
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [EnumDataType(typeof(SessionTypeEnum))]
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 该会话对应的选项菜单的Id
        /// </summary>
        [JsonProperty("menuId")]
        public string MenuId { get; set; }
    }
}
