using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Hooks
{
    public class VTHooks
    {
        private List<VTHook> hooks;
        private VideoTerminal videoTerminal;

        public VTHooks(VideoTerminal vt)
        {
            this.hooks = new List<VTHook>();
            this.videoTerminal = vt;
        }

        /// <summary>
        /// 安装钩子
        /// </summary>
        /// <param name="hook"></param>
        public void Install(VTHook hook)
        {
            this.hooks.Add(hook);
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="hook"></param>
        public void UnInstall(VTHook hook)
        {
        }

        public void Initialize() 
        {
            foreach (VTHook hook in this.hooks)
            {
                hook.Install();
            }
        }

        public void Release() 
        {
            foreach (VTHook hook in this.hooks)
            {
                hook.UnInstall();
            }

            this.hooks.Clear();
        }
    }

    /// <summary>
    /// 终端钩子模块
    /// 通过钩子可以截获终端的任何事件
    /// </summary>
    public abstract class VTHook
    {
        /// <summary>
        /// 该Hook所属的VideoTerminal
        /// </summary>
        public IVideoTerminal OwnerTerminal { get; internal set; }

        public void Install()
        {
            this.OnInstall();
        }

        public void UnInstall()
        {
            this.OnUnInstall();
        }

        /// <summary>
        /// 在安装钩子的时候触发
        /// </summary>
        protected abstract void OnInstall();

        /// <summary>
        /// 在卸载钩子的时候触发
        /// </summary>
        protected abstract void OnUnInstall();

        /// <summary>
        /// 当用户通过键盘输入字符的时候触发
        /// </summary>
        /// <param name="keyInput">用户输入的按键数据</param>
        public abstract void OnKeyboardInput(VTKeyboardInput keyInput);

        /// <summary>
        /// 当一个字符被打印出来的时候触发
        /// </summary>
        /// <param name="row">被打印的字符所在行</param>
        /// <param name="col">被打印的字符所在列</param>
        /// <param name="character">被打印的字符</param>
        /// <param name="characters">该行所有的字符（包括被打印的字符）</param>
        public abstract void OnCharacterPrint(int row, int col, VTCharacter character, List<VTCharacter> characters);

        /// <summary>
        /// 在光标换行的时候触发
        /// </summary>
        /// <param name="number">行号</param>
        /// <param name="characters">该行的所有字符</param>
        public abstract void OnLineFeed(int number, VTHistoryLine historyLine);

        /// <summary>
        /// 切换缓冲区的时候触发
        /// </summary>
        /// <param name="oldDocument">切换之前使用的缓冲区</param>
        /// <param name="newDocument">切换之后使用的缓冲区</param>
        public abstract void OnDocumentChange(VTDocument oldDocument, VTDocument newDocument);
    }
}
