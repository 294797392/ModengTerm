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
        private class Registry<TEvent, TDelegate> where TDelegate : Delegate
        {
            private Dictionary<TEvent, List<TDelegate>> eventRegistry;

            public Registry()
            {
                this.eventRegistry = new Dictionary<TEvent, List<TDelegate>>();
            }

            public void Subscribe(TEvent ev, TDelegate @delegate)
            {
                List<TDelegate> delegates;
                if (!this.eventRegistry.TryGetValue(ev, out delegates))
                {
                    delegates = new List<TDelegate>();
                    this.eventRegistry[ev] = delegates;
                }

                delegates.Add(@delegate);
            }

            public void Unsubscribe(TEvent ev, TDelegate @delegate)
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

            public void Publish(TEvent ev, object e)
            {
                List<TDelegate> delegates;
                if (!this.eventRegistry.TryGetValue(ev, out delegates))
                {
                    return;
                }

                foreach (TDelegate @delegate in delegates)
                {
                    @delegate.DynamicInvoke(e);
                }
            }
        }

        private class HotkeyEvent
        {
            public AddonModule Addon { get; set; }

            public ClientHotkeyDelegate Delegate { get; set; }

            public HotkeyScopes Scope { get; set; }
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientEventRegistoryImpl");

        private Registry<ClientEvent, ClientEventDelegate> clientRegistry;
        private Registry<TabEvent, TabEventDelegate> tabRegistry;
        private Dictionary<IClientTab, Dictionary<TabEvent, List<TabEventDelegate>>> tabEventRegistry;
        private Dictionary<string, List<HotkeyEvent>> hotKeyRegistry;

        public ClientEventRegistryImpl()
        {
            this.clientRegistry = new Registry<ClientEvent, ClientEventDelegate>();
            this.tabRegistry = new Registry<TabEvent, TabEventDelegate>();
            this.tabEventRegistry = new Dictionary<IClientTab, Dictionary<TabEvent, List<TabEventDelegate>>>();
            this.hotKeyRegistry = new Dictionary<string, List<HotkeyEvent>>();
        }

        public void SubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate)
        {
            this.clientRegistry.Subscribe(evType, @delegate);
        }

        public void UnsubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate)
        {
            this.clientRegistry.Unsubscribe(evType, @delegate);
        }

        public void PublishEvent(ClientEventArgs evArgs)
        {
            this.clientRegistry.Publish(evArgs.Type, evArgs);
        }



        public void SubscribeTabEvent(TabEvent evType, TabEventDelegate @delegate, IClientTab tab = null)
        {
            if (tab == null)
            {
                this.tabRegistry.Subscribe(evType, @delegate);
            }
            else
            {
                Dictionary<TabEvent, List<TabEventDelegate>> eventLists;
                if (!this.tabEventRegistry.TryGetValue(tab, out eventLists))
                {
                    eventLists = new Dictionary<TabEvent, List<TabEventDelegate>>();
                    this.tabEventRegistry[tab] = eventLists;
                }

                List<TabEventDelegate> delegates;
                if (!eventLists.TryGetValue(evType, out delegates))
                {
                    delegates = new List<TabEventDelegate>();
                    eventLists[evType] = delegates;
                }

                delegates.Add(@delegate);
            }
        }

        public void UnsubscribeTabEvent(TabEvent evType, TabEventDelegate @delegate, IClientTab tab = null)
        {
            if (tab == null)
            {
                this.tabRegistry.Unsubscribe(evType, @delegate);
            }
            else
            {
                Dictionary<TabEvent, List<TabEventDelegate>> eventLists;
                if (!this.tabEventRegistry.TryGetValue(tab, out eventLists))
                {
                    return;
                }

                if (eventLists.Count == 0)
                {
                    this.tabEventRegistry.Remove(tab);
                    return;
                }

                List<TabEventDelegate> delegates;
                if (!eventLists.TryGetValue(evType, out delegates))
                {
                    return;
                }

                // 防止在Publish事件的时候，调用了Unsubscribe方法，造成在foreach循环中删除列表里的元素的问题
                List<TabEventDelegate> newDelegates = delegates.ToList();
                newDelegates.Remove(@delegate);
                eventLists[evType] = newDelegates;

                if (newDelegates.Count == 0)
                {
                    eventLists.Remove(evType);
                    if (eventLists.Count == 0)
                    {
                        this.tabEventRegistry.Remove(tab);
                    }
                }
            }
        }

        public void UnsubscribeTabEvent(IClientTab tab)
        {
            this.tabEventRegistry.Remove(tab);
        }

        public void PublishTabEvent(TabEventArgs evArgs)
        {
            IClientTab tab = evArgs.Sender;

            // 先发布全局Tab事件
            this.tabRegistry.Publish(evArgs.Type, evArgs);

            // 再发布针对于单个订阅的Tab事件
            Dictionary<TabEvent, List<TabEventDelegate>> eventLists;
            if (!this.tabEventRegistry.TryGetValue(tab, out eventLists))
            {
                return;
            }

            List<TabEventDelegate> delegates;
            if (!eventLists.TryGetValue(evArgs.Type, out delegates))
            {
                return;
            }

            foreach (TabEventDelegate @delegate in delegates)
            {
                @delegate.Invoke(evArgs);
            }
        }



        public void RegisterHotkey(AddonModule addon, string hotkey, HotkeyScopes scope, ClientHotkeyDelegate @delegate)
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
                Scope = scope
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
                        ev.Delegate();
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
