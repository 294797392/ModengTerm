using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public class ClientEventRegistoryImpl : IClientEventRegistory
    {
        private Dictionary<ClientEvent, List<ClientEventDelegate>> eventRegistory;
        private Dictionary<IClientTab, Dictionary<TabEvent, List<TabEventDelegate>>> tabEventRegistory;

        public ClientEventRegistoryImpl()
        {
            this.eventRegistory = new Dictionary<ClientEvent, List<ClientEventDelegate>>();
            this.tabEventRegistory = new Dictionary<IClientTab, Dictionary<TabEvent, List<TabEventDelegate>>>();
        }

        public void SubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate)
        {
            List<ClientEventDelegate> delegates;
            if (!this.eventRegistory.TryGetValue(evType, out delegates))
            {
                delegates = new List<ClientEventDelegate>();
                this.eventRegistory[evType] = delegates;
            }

            delegates.Add(@delegate);
        }

        public void UnsubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate)
        {
            List<ClientEventDelegate> delegates;
            if (!this.eventRegistory.TryGetValue(evType, out delegates))
            {
                return;
            }

            delegates.Remove(@delegate);
        }

        public void PublishEvent(ClientEventArgs evArgs)
        {
            List<ClientEventDelegate> delegates;
            if (!this.eventRegistory.TryGetValue(evArgs.Type, out delegates))
            {
                return;
            }

            foreach (ClientEventDelegate @delegate in delegates)
            {
                @delegate.Invoke(evArgs.Type, evArgs);
            }
        }



        public void SubscribeTabEvent(IClientTab tab, TabEvent evType, TabEventDelegate @delegate)
        {
            Dictionary<TabEvent, List<TabEventDelegate>> eventLists;
            if (!this.tabEventRegistory.TryGetValue(tab, out eventLists))
            {
                eventLists = new Dictionary<TabEvent, List<TabEventDelegate>>();
                this.tabEventRegistory[tab] = eventLists;
            }

            List<TabEventDelegate> delegates;
            if (!eventLists.TryGetValue(evType, out delegates))
            {
                delegates = new List<TabEventDelegate>();
                eventLists[evType] = delegates;
            }

            delegates.Add(@delegate);
        }

        public void UnsubscribeTabEvent(IClientTab tab, TabEvent evType, TabEventDelegate @delegate)
        {
            Dictionary<TabEvent, List<TabEventDelegate>> eventLists;
            if (!this.tabEventRegistory.TryGetValue(tab, out eventLists))
            {
                return;
            }

            if (eventLists.Count == 0)
            {
                this.tabEventRegistory.Remove(tab);
                return;
            }

            List<TabEventDelegate> delegates;
            if (!eventLists.TryGetValue(evType, out delegates))
            {
                return;
            }

            delegates.Remove(@delegate);

            if (delegates.Count == 0)
            {
                eventLists.Remove(evType);
                if (eventLists.Count == 0) 
                {
                    this.tabEventRegistory.Remove(tab);
                }
            }
        }

        public void UnsubscribeTabEvent(IClientTab tab)
        {
            this.tabEventRegistory.Remove(tab);
        }

        public void PublishTabEvent(IClientTab tab, TabEventArgs evArgs)
        {
            Dictionary<TabEvent, List<TabEventDelegate>> eventLists;
            if (!this.tabEventRegistory.TryGetValue(tab, out eventLists))
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
    }
}
