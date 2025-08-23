using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModengTerm.Addon
{
    public class ClientEventRegistryImpl : IClientEventRegistry
    {
        private class Registry<TEventType, TDelegate> where TDelegate : Delegate
        {
            private class EventData
            {
                public string Condition { get; set; }

                public TDelegate Delegate { get; set; }

                public object UserData { get; set; }

                public EventData Next { get; set; }
            }

            // 为了防止在publish的时候subscribe或者unsubscribe，使用链表存储事件信息
            private Dictionary<TEventType, LinkedList<EventData>> eventRegistry;

            public Registry()
            {
                this.eventRegistry = new Dictionary<TEventType, LinkedList<EventData>>();
            }

            public void Subscribe(TEventType ev, string evcond, TDelegate @delegate, object userData)
            {
                // 此时ev里可能有条件字符串
                // 解析出真正的事件名字
                LinkedList<EventData> evds;
                if (!this.eventRegistry.TryGetValue(ev, out evds))
                {
                    evds = new LinkedList<EventData>();
                    this.eventRegistry[ev] = evds;
                }

                EventData evd = new EventData();
                evd.Delegate = @delegate;
                evd.Condition = evcond;
                evd.UserData = userData;

                evds.AddLast(evd);
            }

            public void Unsubscribe(TEventType ev, TDelegate @delegate)
            {
                LinkedList<EventData> evds;
                if (!this.eventRegistry.TryGetValue(ev, out evds))
                {
                    return;
                }

                LinkedListNode<EventData> current = evds.First;

                while (current != null)
                {
                    EventData evd = current.Value;

                    if (evd.Delegate == @delegate)
                    {
                        evds.Remove(current);
                        break;
                    }
                }
            }

            public void Publish(TEventType ev, EventArgsBase args)
            {
                LinkedList<EventData> evds;
                if (this.eventRegistry.TryGetValue(ev, out evds))
                {
                    LinkedListNode<EventData> current = evds.First;

                    while (current != null)
                    {
                        EventData evd = current.Value;

                        if (!string.IsNullOrEmpty(evd.Condition))
                        {
                            if (!args.MatchCondition(evd.Condition))
                            {
                                current = current.Next;

                                continue;
                            }
                        }

                        evd.Delegate.DynamicInvoke(args, evd.UserData);

                        current = current.Next;
                    }
                }
            }
        }

        private class HotkeyEvent
        {
            public ClientHotkeyDelegate Delegate { get; set; }

            public object UserData { get; set; }
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientEventRegistoryImpl");

        private Registry<ClientEvent, ClientEventDelegate> clientRegistry;
        private Registry<TabEvent, TabEventDelegate> tabRegistry;
        private Dictionary<IClientTab, Registry<TabEvent, TabEventDelegate>> tabEventRegistry;
        private Dictionary<string, List<HotkeyEvent>> hotKeyRegistry;

        public ClientEventRegistryImpl()
        {
            this.clientRegistry = new Registry<ClientEvent, ClientEventDelegate>();
            this.tabRegistry = new Registry<TabEvent, TabEventDelegate>();
            this.tabEventRegistry = new Dictionary<IClientTab, Registry<TabEvent, TabEventDelegate>>();
            this.hotKeyRegistry = new Dictionary<string, List<HotkeyEvent>>();
        }


        public void SubscribeEvent(ClientEvent ev, ClientEventDelegate @delegate, object userData = null)
        {
            this.clientRegistry.Subscribe(ev, string.Empty, @delegate, userData);
        }

        public void SubscribeEvent(ClientEvent ev, string evcond, ClientEventDelegate @delegate, object userData = null)
        {
            this.clientRegistry.Subscribe(ev, evcond, @delegate, userData);
        }

        public void UnsubscribeEvent(ClientEvent ev, ClientEventDelegate @delegate)
        {
            this.clientRegistry.Unsubscribe(ev, @delegate);
        }

        public void PublishEvent(ClientEventArgs e)
        {
            this.clientRegistry.Publish(e.Type, e);
        }





        public void SubscribeTabEvent(TabEvent ev, TabEventDelegate @delegate, IClientTab tab = null, object userData = null)
        {
            this.SubscribeTabEvent(ev, string.Empty, @delegate, tab, userData);
        }

        public void SubscribeTabEvent(TabEvent ev, string evcond, TabEventDelegate @delegate, IClientTab tab = null, object userData = null)
        {
            if (tab == null)
            {
                this.tabRegistry.Subscribe(ev, evcond, @delegate, userData);
            }
            else
            {
                Registry<TabEvent, TabEventDelegate> registry;
                if (!this.tabEventRegistry.TryGetValue(tab, out registry))
                {
                    registry = new Registry<TabEvent, TabEventDelegate>();
                    this.tabEventRegistry[tab] = registry;
                }

                registry.Subscribe(ev, evcond, @delegate, userData);
            }
        }

        public void UnsubscribeTabEvent(TabEvent ev, TabEventDelegate @delegate, IClientTab tab = null)
        {
            if (tab == null)
            {
                this.tabRegistry.Unsubscribe(ev, @delegate);
            }
            else
            {
                Registry<TabEvent, TabEventDelegate> registry;
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
            this.tabRegistry.Publish(e.Type, e);

            // 再发布针对于单个订阅的Tab事件
            Registry<TabEvent, TabEventDelegate> registry;
            if (!this.tabEventRegistry.TryGetValue(tab, out registry))
            {
                return;
            }

            registry.Publish(e.Type, e);
        }




        public void RegisterHotkey(string hotkey, ClientHotkeyDelegate @delegate, object userData = null)
        {
            List<HotkeyEvent> events;
            if (!this.hotKeyRegistry.TryGetValue(hotkey, out events))
            {
                events = new List<HotkeyEvent>();
                this.hotKeyRegistry[hotkey] = events;
            }

            events.Add(new HotkeyEvent()
            {
                Delegate = @delegate,
                UserData = userData
            });
        }

        public void UnregisterHotkey(string hotkey, ClientHotkeyDelegate @delegate)
        {
            List<HotkeyEvent> events;
            if (!this.hotKeyRegistry.TryGetValue(hotkey, out events))
            {
                return;
            }

            HotkeyEvent ev = events.FirstOrDefault(v => v.Delegate == @delegate);
            if (ev == null)
            {
                return;
            }

            // 防止在Publish事件的时候，调用了Unsubscribe方法，造成在foreach循环中删除列表里的元素的问题
            // TODO：修改成链表方式
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

            foreach (HotkeyEvent ev in events)
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

            return true;
        }
    }
}
