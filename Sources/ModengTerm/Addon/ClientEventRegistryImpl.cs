using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModengTerm.Addon
{
    public class ClientEventRegistryImpl : IClientEventRegistry
    {
        private class Registry<TDelegate> where TDelegate : Delegate
        {
            private Dictionary<string, List<TDelegate>> eventRegistry;

            public Registry()
            {
                this.eventRegistry = new Dictionary<string, List<TDelegate>>();
            }

            public void Subscribe(string ev, TDelegate @delegate)
            {
                List<TDelegate> delegates;
                if (!this.eventRegistry.TryGetValue(ev, out delegates))
                {
                    delegates = new List<TDelegate>();
                    this.eventRegistry[ev] = delegates;
                }

                delegates.Add(@delegate);
            }

            public void Unsubscribe(string ev, TDelegate @delegate)
            {
                List<TDelegate> delegates;
                if (!this.eventRegistry.TryGetValue(ev, out delegates))
                {
                    return;
                }

                // 防止在Publish事件的时候，调用了Unsubscribe方法，造成在foreach循环中删除列表里的元素的问题
                List<TDelegate> newDelegates = delegates.ToList();
                newDelegates.Remove(@delegate);
                this.eventRegistry[ev] = newDelegates;
            }

            public void Publish(EventArgsBase ev)
            {
                List<TDelegate> delegates;
                if (this.eventRegistry.TryGetValue(ev.Name, out delegates))
                {
                    foreach (TDelegate @delegate in delegates)
                    {
                        @delegate.DynamicInvoke(ev);
                    }
                }

                if (!string.IsNullOrEmpty(ev.FullName))
                {
                    if (this.eventRegistry.TryGetValue(ev.FullName, out delegates))
                    {
                        foreach (TDelegate @delegate in delegates)
                        {
                            @delegate.DynamicInvoke(ev);
                        }
                    }
                }
            }
        }

        private class HotkeyEvent
        {
            public AddonModule Addon { get; set; }

            public ClientHotkeyDelegate Delegate { get; set; }

            public HotkeyScopes Scope { get; set; }

            public object UserData { get; set; }
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientEventRegistoryImpl");

        private Registry<ClientEventDelegate> clientRegistry;
        private Registry<TabEventDelegate> tabRegistry;
        private Dictionary<IClientTab, Registry<TabEventDelegate>> tabEventRegistry;
        private Dictionary<string, List<HotkeyEvent>> hotKeyRegistry;

        public ClientEventRegistryImpl()
        {
            this.clientRegistry = new Registry<ClientEventDelegate>();
            this.tabRegistry = new Registry<TabEventDelegate>();
            this.tabEventRegistry = new Dictionary<IClientTab, Registry<TabEventDelegate>>();
            this.hotKeyRegistry = new Dictionary<string, List<HotkeyEvent>>();
        }

        public void SubscribeEvent(string ev, ClientEventDelegate @delegate)
        {
            this.clientRegistry.Subscribe(ev, @delegate);
        }

        public void UnsubscribeEvent(string ev, ClientEventDelegate @delegate)
        {
            this.clientRegistry.Unsubscribe(ev, @delegate);
        }

        public void PublishEvent(ClientEventArgs e)
        {
            this.clientRegistry.Publish(e);
        }



        public void SubscribeTabEvent(string ev, TabEventDelegate @delegate, IClientTab tab = null)
        {
            if (tab == null)
            {
                this.tabRegistry.Subscribe(ev, @delegate);
            }
            else
            {
                Registry<TabEventDelegate> registry;
                if (!this.tabEventRegistry.TryGetValue(tab, out registry))
                {
                    registry = new Registry<TabEventDelegate>();
                    this.tabEventRegistry[tab] = registry;
                }

                registry.Subscribe(ev, @delegate);
            }
        }

        public void UnsubscribeTabEvent(string ev, TabEventDelegate @delegate, IClientTab tab = null)
        {
            if (tab == null)
            {
                this.tabRegistry.Unsubscribe(ev, @delegate);
            }
            else
            {
                Registry<TabEventDelegate> registry;
                if (!this.tabEventRegistry.TryGetValue(tab, out registry))
                {
                    return;
                }

                registry.Unsubscribe(ev, @delegate);
            }
        }

        public void UnsubscribeTabEvent(IClientTab tab)
        {
            this.tabEventRegistry.Remove(tab);
        }

        public void PublishTabEvent(TabEventArgs e)
        {
            IClientTab tab = e.Sender;

            // 先发布全局Tab事件
            this.tabRegistry.Publish(e);

            // 再发布针对于单个订阅的Tab事件
            Registry<TabEventDelegate> registry;
            if (!this.tabEventRegistry.TryGetValue(tab, out registry))
            {
                return;
            }

            registry.Publish(e);
        }



        public void RegisterHotkey(AddonModule addon, string hotkey, HotkeyScopes scope, ClientHotkeyDelegate @delegate)
        {
            this.RegisterHotkey(addon, hotkey, scope, @delegate, null);
        }

        public void RegisterHotkey(AddonModule addon, string hotkey, HotkeyScopes scope, ClientHotkeyDelegate @delegate, object userData)
        {
            List<HotkeyEvent> events;
            if (!this.hotKeyRegistry.TryGetValue(hotkey, out events))
            {
                events = new List<HotkeyEvent>();
                this.hotKeyRegistry[hotkey] = events;
            }

            events.Add(new HotkeyEvent()
            {
                Addon = addon,
                Delegate = @delegate,
                Scope = scope,
                UserData = userData
            });
        }

        public void UnregisterHotkey(AddonModule addon, string hotkey)
        {
            List<HotkeyEvent> events;
            if (!this.hotKeyRegistry.TryGetValue(hotkey, out events))
            {
                return;
            }

            HotkeyEvent ev = events.FirstOrDefault(v => v.Addon == addon);
            if (ev == null)
            {
                return;
            }

            // 防止在Publish事件的时候，调用了Unsubscribe方法，造成在foreach循环中删除列表里的元素的问题
            List<HotkeyEvent> newevents = events.ToList();
            newevents.Remove(ev);
            this.hotKeyRegistry[hotkey] = newevents;
        }

        public bool PublishHotkeyEvent(string hotkey)
        {
            List<HotkeyEvent> events;
            if (!this.hotKeyRegistry.TryGetValue(hotkey, out events))
            {
                return false;
            }

            if (events.Count == 0)
            {
                return false;
            }

            ClientFactory factory = ClientFactory.GetFactory();
            IClient client = factory.GetClient();
            IClientTab clientTab = client.GetActiveTab<IClientTab>();

            foreach (HotkeyEvent ev in events)
            {
                bool canExecute = false;

                switch (ev.Scope)
                {
                    case HotkeyScopes.Client:
                        {
                            canExecute = true;
                            break;
                        }

                    case HotkeyScopes.ClientShellTab:
                        {
                            canExecute = clientTab is IClientShellTab;
                            break;
                        }

                    case HotkeyScopes.ClientSftpTab:
                        {
                            canExecute = clientTab is IClientSftpTab;
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                if (canExecute)
                {
                    try
                    {
                        ev.Delegate(ev.UserData);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("快捷键执行异常", ex);
                    }
                }
            }

            return true;
        }
    }
}
