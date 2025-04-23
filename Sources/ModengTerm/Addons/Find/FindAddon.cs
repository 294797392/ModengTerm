using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.Find
{
    public class FindAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            RegisterEvent(AddonEventTypes.SelectedSessionChanged);
            RegisterCommand("E71680AF-F5D8-4F18-A0BF-BB60DD4DAA1C", OpenFindWindowCommandHandler);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes evt, params object[] evp)
        {
            switch (evt)
            {
                case AddonEventTypes.SelectedSessionChanged:
                    {
                        // 如果选中的会话是Shell会话并且显示了查找窗口，那么搜索选中的会话

                        // TODO：打开搜索窗口的同时，新打开了一个会话，此时会话里的VideoTerminal为空，因为还没打开完
                        ShellSessionVM selectedSession = evp[1] as ShellSessionVM;
                        if (selectedSession != null && selectedSession.VideoTerminal != null)
                        {
                            if (FindWindowMgr.WindowShown)
                            {
                                FindWindowMgr.Show(selectedSession);
                            }
                        }
                        break;
                    }

                case AddonEventTypes.SessionStatusChanged:
                    {
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private void OpenFindWindowCommandHandler()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //if (shellSessionVM == null)
            //{
            //    return;
            //}

            //FindWindowMgr.Show(shellSessionVM);
        }
    }
}
