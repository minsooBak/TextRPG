public enum EventType//아이템이벤트
{
    NONE,
    eMakeMonsters,
    eClearMonsters,
    eSetMonsters,
    eUpdateItem,
    eGetFieldItem,
    eUpdateGold,
    eUpdateStat,
    eGameEnd,
    eSetSkill,
    eShowSkill,
    Player
}

//public enum EventType //이 이벤트들을 듣겠다
//{
//    Item,
//    Quest, 
//    Player,
//    eGameEnd
//}

enum eItemType//아이템이벤트
{
    eUpdateItem,
    eGetFieldItem,
}

enum eQuestType//퀘스트이벤트의 조건
{
    Item, 
    Monster,
    Dungeon,
    Stats
}

enum ePlayerType//플레이어이벤트
{
    HP,
    MP,
    Gold,
    Stats
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
