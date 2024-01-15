using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    internal class QuestManager : IListener
    {
        private Quest[] questMenu;
        public QuestManager()
        {
            EventManager.Instance.AddListener(EventType.Quest, this);
            questMenu = (Quest[])Utilities.LoadFile(LoadType.QuestData);
        }
        public void PrintAll()
        {
            foreach (Quest quest in questMenu) 
            {
                Console.WriteLine($"{quest.Name}");
            }
        }
        public void OnEvent(EventType type, object data)
        {
            if (type == EventType.Quest)
            {
                var a = (KeyValuePair<eQuestType, string>)data;
                switch (a.Key)
                {
                    case eQuestType.Item:
                        {

                            break;
                        }
                    case eQuestType.Dungeon:
                        {

                            break;
                        }
                    case eQuestType.Monster:
                        {

                            break;
                        }
                    case eQuestType.Stats:
                        {

                            break;
                        }
                }
            }
        }
    }
    public class Quest
    {
        public readonly int Idx;
        public readonly string Name; //퀘스트 이름
        public readonly string Explanation; //퀘스트 설명
        public readonly int Gold; // 보상 골드
        private readonly string ItemName; // 보상 아이템 이름
        public readonly int Exp;//보상 경험치
        public Quest(int idx, string name, string explanation, int gold, string itemName, int exp)
        {
            Idx = idx;
            Name = name;
            Explanation = explanation;
            Gold = gold;
            ItemName = itemName;
            Exp = exp;
        }
    }
}
