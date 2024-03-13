using ModengTerm.Document;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.TestCase
{
    public class CaseStep
    {
        /// <summary>
        /// 测试用例输入数据
        /// </summary>
        public List<UserInput> UserInputs { get; set; }

        /// <summary>
        /// 输入完后所有行的状态
        /// </summary>
        public List<CaseLine> OutputLines { get; set; }

        /// <summary>
        /// 光标所在行数
        /// </summary>
        public int CursorRow { get; set; }

        /// <summary>
        /// 光标所在列数
        /// </summary>
        public int CursorColumn { get; set; }

        /// <summary>
        /// 第一行文本的物理行号
        /// </summary>
        public int FirstLinePhysicsRow { get; set; }

        /// <summary>
        /// 最后一行文本的物理行号
        /// </summary>
        public int LastLinePhysicsRow { get; set; }

        public CaseStep()
        {
            this.UserInputs = new List<UserInput>();
            this.OutputLines = new List<CaseLine>();
        }
    }

    public class TestCase
    {
        /// <summary>
        /// 测试用例名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 操作系统版本号
        /// </summary>
        [JsonProperty("osver")]
        public string OSVersion { get; set; }

        /// <summary>
        /// 测试步骤
        /// </summary>
        [JsonProperty("steps")]
        public List<CaseStep> Steps { get; set; }

        public TestCase()
        {
            this.Steps = new List<CaseStep>();
        }
    }
}
