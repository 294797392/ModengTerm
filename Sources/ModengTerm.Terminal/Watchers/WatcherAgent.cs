using DotNEToolkit.Modular;
using log4net.Util;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Session;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace ModengTerm.Terminal.Watchers
{
    public delegate void ProcessChangedDlg(List<ProcessInfo> addProcess, List<ProcessInfo> removeProcess, List<ProcessInfo> updateProcess);
    public delegate void SystemInfoChangedDlg(SystemInfo systemInfo);
    public delegate void DiskInfoChangedDlg(List<DiskInfo> addDisk, List<DiskInfo> removeDisk, List<DiskInfo> updateDisks);

    public class WatchCallback
    {
        public ProcessChangedDlg ProcessChanged { get; set; }

        public SystemInfoChangedDlg SystemInfoChanged { get; set; }

        public DiskInfoChangedDlg DiskInfoChanged { get; set; }
    }

    public class WatcherAgent : ModuleBase
    {
        /// <summary>
        /// 需要被监控的项目
        /// </summary>
        private class WatchItem
        {
            public string Uri { get; set; }

            public List<WatchSession> Sessions { get; set; }

            public WatchItem()
            {
            }
        }

        private class WatchSession
        {
            public XTermSession Session { get; private set; }

            public SessionTransport Transport { get; set; }

            public WatchCallback Callback { get; set; }

            public WatchSession(XTermSession session)
            {
                this.Session = session;
            }
        }

        #region 实例变量

        private ManualResetEvent watchEvent;
        private int watchInterval;

        private List<WatchItem> watchList;
        private bool watchListChanged;

        #endregion

        #region ModuleBase

        protected override int OnInitialize()
        {
            this.registerMap = new Dictionary<string, List<WatchSession>>();
            this.watchList = new List<WatchItem>();

            this.watchEvent = new ManualResetEvent(false);
            this.watchInterval = VTermApp.Context.ReadSetting<int>(MTermConsts.VTermAppKey_WatchInterval, MTermConsts.DefaultWatchInterval);
            Task.Factory.StartNew(this.WatchThreadProc);

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {

        }

        #endregion

        #region 公开接口

        public void RegisterSession(XTermSession session, SessionTransport transport, WatchCallback callback)
        {
            string uri = this.GetURI(session);

            WatchItem watchItem = this.watchList.FirstOrDefault(v => v.Uri == uri);
            if (watchItem == null)
            {
                watchItem = new WatchItem();
                this.watchList.Add(watchItem);
            }

            WatchSession watchSession = new WatchSession(session)
            {
                Transport = transport,
                Callback = callback
            };
            watchItem.Sessions.Add(watchSession);

            lock (this.watchList)
            {
                this.watchList.Add(watchItem);

                this.watchListChanged = true;
            }

            this.watchEvent.Set();
        }

        public void UnregisterSession(XTermSession session)
        {
            string uri = this.GetURI(session);

            WatchItem watchItem = this.watchList.FirstOrDefault(v => v.Uri == uri);
            if (watchItem == null)
            {
                return;
            }

            WatchSession watchSession = watchItem.Sessions.FirstOrDefault(v => v.Session == session);
            if (watchSession == null)
            {
                return;
            }

            lock (this.watchList)
            {
                watchItem.Sessions.Remove(watchSession);

                if (watchItem.Sessions.Count == 0)
                {
                    this.watchList.Remove(watchItem);
                }

                this.watchListChanged = true;
            }

            if (this.watchList.Count == 0)
            {
                this.watchEvent.Reset();
            }
        }

        #endregion

        #region 实例方法

        private string GetURI(XTermSession session)
        {
            throw new NotImplementedException();
        }

        private List<WatchItem> CopyWatchList(List<WatchItem> copyFrom)
        {
            List<WatchItem> watchItems = new List<WatchItem>();

            foreach (WatchItem srcItem in copyFrom)
            {
                WatchItem watchItem = new WatchItem();
                watchItem.Uri = srcItem.Uri;
                watchItem.Sessions = srcItem.Sessions.ToList();
                watchItems.Add(watchItem);
            }

            return watchItems;
        }

        private void CleanupCaches(List<WatchItem> watchItems, Dictionary<string, List<ProcessInfo>> processCaches)
        {
            List<string> allKeys = processCaches.Keys.ToList();

            foreach (string uri in allKeys)
            {
                WatchItem watchItem = watchItems.FirstOrDefault(v => v.Uri == uri);
                if (watchItem == null)
                {
                    processCaches.Remove(uri);
                }
            }
        }

        private void UpdateLocalHostProcesses()
        {
            Process[] newProcs = Process.GetProcesses();
        }

        private void UpdateLinuxProcessList(SessionDriver driver) 
        {
        }

        #endregion

        #region 事件处理器

        private void WatchThreadProc()
        {
            List<WatchItem> watchItems = new List<WatchItem>();
            List<ProcessInfo> addProcs = new List<ProcessInfo>();
            List<ProcessInfo> removeProcs = new List<ProcessInfo>();
            List<ProcessInfo> updateProcs = new List<ProcessInfo>();
            Dictionary<string, List<ProcessInfo>> cacheProcMap = new Dictionary<string, List<ProcessInfo>>();

            while (true)
            {
                this.watchEvent.WaitOne();

                if (this.watchListChanged)
                {
                    lock (this.watchList)
                    {
                        watchItems = this.CopyWatchList(this.watchList);

                        this.watchListChanged = false;
                    }

                    // Cleanup Caches
                    this.CleanupCaches(watchItems, cacheProcMap);
                }

                foreach (WatchItem watchItem in watchItems)
                {
                    SessionTransport transport = watchItem.Sessions[0].Transport;

                    #region 处理进程列表

                    List<ProcessInfo> newProcs = transport.GetProcesses();
                    List<ProcessInfo> oldProcs;
                    if (!cacheProcMap.TryGetValue(watchItem.Uri, out oldProcs))
                    {
                        // 没有缓存，说明是第一次显示
                        oldProcs = new List<ProcessInfo>(newProcs);
                        addProcs.AddRange(oldProcs);
                    }
                    else
                    {
                        // 有缓存，和缓存做比较

                        // oldProcs里有，newProcs里没有的
                        IEnumerable<ProcessInfo> processInfos1 = oldProcs.ExceptBy<ProcessInfo, int>(newProcs.Select(v => v.PID), (v) => { return v.PID; });
                        // newProcs里有，oldProcs里没有的
                        IEnumerable<ProcessInfo> processInfos2 = newProcs.ExceptBy<ProcessInfo, int>(oldProcs.Select(v => v.PID), (v) => { return v.PID; }); 
                        // 计算新的数据和旧的数据差异
                        foreach (ProcessInfo newProc in newProcs)
                        {
                            ProcessInfo oldProc = oldProcs.FirstOrDefault(v => v.PID == newProc.PID);
                            if (oldProc == null)
                            {
                                continue;
                            }

                            if (newProc.CompareTo(oldProc))
                            {
                                // 新的数据和旧的数据一致
                                continue;
                            }

                            updateProcs.Add(newProc);
                        }

                        removeProcs.AddRange(processInfos1);
                        addProcs.AddRange(processInfos2);

                        oldProcs.Clear();
                        oldProcs.AddRange(newProcs);
                    }

                    // 触发进程回调
                    IEnumerable<ProcessChangedDlg> procDlgs = watchItem.Sessions.Select(v => v.Callback.ProcessChanged);
                    foreach (ProcessChangedDlg procDlg in procDlgs)
                    {
                        procDlg(addProcs, removeProcs, updateProcs);
                    }

                    addProcs.Clear();
                    removeProcs.Clear();
                    updateProcs.Clear();

                    #endregion
                }

                Thread.Sleep(this.watchInterval);
            }
        }

        #endregion
    }
}

