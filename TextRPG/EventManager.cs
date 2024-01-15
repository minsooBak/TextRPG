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
    Quest,
    Player
}

//public enum EventType //이 이벤트들을 듣겠다
//{
//    eMakeMonsters,//변경하기
//    eSetMonsters, //변경하기
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
    Stats,
    PlayerLevel
}

enum ePlayerType//플레이어이벤트
{
    HP,
    MP,
    Gold,
    Stats,
    Exp
}

public interface IListener
{
    void OnEvent<T>(EventType type, T data);
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

            listenList = [_listener]; // listenList = new List<IListener> {_listener}; 와 같다. 리스트를 만들고 첫 요소로 리스너를 넣는다.
            listener.Add(eventType, listenList); //딕셔너리에 추가
        }

        public void PostEvent<T>(EventType eventType, T param)
        {
            List<IListener>? listenList;
            if (listener.TryGetValue(eventType, out listenList) == false)
                return;

            for (int i = 0; i < listenList.Count; i++) // 이벤트 타입에 있는  리스너형들이 있는 리스트 목록에서 하나씩 OnEvent 함수를 실행한다.
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
