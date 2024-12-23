﻿using DotNEToolkit;
using DotNEToolkit.DataModels;
using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.DataModels
{
    /// <summary>
    /// 保存一个通道的配置信息
    /// </summary>
    public class XTermSession : ModelBase
    {
        /// <summary>
        /// 要连接的会话类型
        /// </summary>
        [EnumDataType(typeof(SessionTypeEnum))]
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 所有的选项列表
        /// Key参见OptionKeyEnum
        /// </summary>
        [JsonProperty("options")]
        public List<SessionOption> Options { get; private set; }

        /// <summary>
        /// 会话所属的分组Id
        /// 如果为空，则表示没有分组
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        public XTermSession()
        {
            this.Options = new List<SessionOption>();
        }

        /// <summary>
        /// TODO：删除该方法，改用带有defaultValue的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public T GetOption<T>(OptionKeyEnum key)
        {
            SessionOption sessionOption = this.Options.FirstOrDefault(v => v.Key == (int)key);
            if (sessionOption == null)
            {
                throw new KeyNotFoundException(string.Format("{0} NotFound", key));
            }

            Type t = typeof(T);
            if (t.IsEnum)
            {
                string value = JSONHelper.Parse<string>(sessionOption.Value);
                return (T)Enum.Parse(t, value);
            }
            else
            {
                return JSONHelper.Parse<T>(sessionOption.Value);
            }
        }

        /// <summary>
        /// 当使用新版本软件缺少某个配置项（配置文件是旧版本）的时候，确保不出问题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetOption<T>(OptionKeyEnum key, T defaultValue) 
        {
            SessionOption sessionOption = this.Options.FirstOrDefault(v => v.Key == (int)key);
            if (sessionOption == null)
            {
                return defaultValue;
            }

            return this.GetOption<T>(key);
        }

        public void SetOption<T>(OptionKeyEnum key, T value)
        {
            SessionOption sessionOption = this.Options.FirstOrDefault(v => v.Key == (int)key);
            if (sessionOption == null)
            {
                sessionOption = new SessionOption()
                {
                    Key = (int)key,
                };
                this.Options.Add(sessionOption);
            }

            sessionOption.Value = JsonConvert.SerializeObject(value);
        }
    }
}
