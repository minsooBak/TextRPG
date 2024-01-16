//public enum EventType//아이템이벤트
//{
//    NONE,
//    eMakeMonsters,
//    eClearMonsters,
//    eSetMonsters,
//    eUpdateItem,
//    eGetFieldItem,
//    eUpdateGold,
//    eUpdateStat,
//    eGameEnd,
//    eSetSkill,
//    eShowSkill,
//    Item,
//    Quest,
//    Player
//}

public enum EventType //이 타입의 이벤트들을 듣겠다
{
    eMakeMonsters,//변경하기
    eSetMonsters, //변경하기
    eUpdateStat,//변경하기
    eUpdateGold,//변경하기
    eClearMonsters,//변경하기
    Item,
    Quest,
    Player,
    eGameEnd
}

enum eItemType//아이템내의 이벤트타입으로 데이터를 보내겠다
{
    //eUpdateItem, //잘 모르겠음
    eGetFieldItem,
    eGameEnd,
    EquipQuest
}

enum eQuestType//퀘스트이벤트내의 이벤트타입으로 데이터를 보내겠다
{
    Item, 
    Monster,
    Dungeon,
    Stats,
    PlayerLevel
}

enum ePlayerType//플레이어내의 이벤트타입으로 데이터를 보내겠다
{
    HP,
    MP,
    Gold,
    Stats,
    Exp
}

public interface IListener // T == Utilities.EventPair(Enum, data)
{
    void OnEvent<T>(EventType type, T data);
}

namespace TextRPG
{
    internal class EventManager
    {
        private static readonly EventManager instance = new EventManager();//싱글톤 자기자신

        static EventManager() { } 
        private EventManager() { } // 다른곳에서 생성하지못하도록

        public static EventManager Instance { get { return instance; } } // 다른곳에서 참고하기위해
        private Dictionary<EventType, List<IListener>> listener = []; // AddListener로 추가된 IListener를 상속한 클래스들을 EventType에 따라 보관

        //이벤트를 듣겠다고 등록하는 메서드
        public void AddListener(EventType eventType, IListener _listener)
        {
            List<IListener>? listenList = null;

            //들어온 이벤트 타입으로된 List가 있는지 체크하고 있다면 ListenList에 할당하고, 그 리스트에 클래스를 등록
            if (listener.TryGetValue(eventType, out listenList))
            {
                listenList.Add(_listener);
                return;
            }

            //List<IListener>초기화
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
    }
}
