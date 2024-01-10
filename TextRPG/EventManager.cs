using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EventType
{
    eUpdateItem,
    eGetItem,
    eGameEnd
}

public interface IListener
{
    void OnEvent(EventType type, object data);
}

namespace TextRPG
{
    internal class EventManager
    {
        private static readonly EventManager instance = new EventManager();

        static EventManager() { }
        private EventManager() { }

        public static EventManager Instance { get { return instance; } }
        private Dictionary<EventType, List<IListener>> listener = [];

        public void AddListener(EventType eventType, IListener _listener)
        {
            List<IListener>? listenList = null;

            if (listener.TryGetValue(eventType, out listenList))
            {
                listenList.Add(_listener);
                return;
            }

            listenList = [_listener];
            listener.Add(eventType, listenList);
        }

        public void PostEvent(EventType eventType, object? param = null)
        {
            List<IListener>? listenList;
            if (listener.TryGetValue(eventType, out listenList) == false)
                return;

            for (int i = 0; i < listenList.Count; i++)
            {
                listenList?[i].OnEvent(eventType, param);
            }
        }

        public void RemoveListener(EventType eventType)
        {
            Dictionary<EventType, List<IListener>> newListeners = [];

            foreach (KeyValuePair<EventType, List<IListener>> item in listener)
            {
                for (int i = item.Value.Count - 1; i >= 0; i--)
                {
                    if (item.Value[i].Equals(null))
                        item.Value.RemoveAt(i);
                }

                if (item.Value.Count > 0)
                    newListeners.Add(item.Key, item.Value);
            }

            listener = newListeners;
        }

    }
}
