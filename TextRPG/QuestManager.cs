using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    internal class QuestManager : IListener
    {
        Player player;
        private Quest[] questMenu; //퀘스트 전체 목록
        private List<Quest> quests = new List<Quest> { }; //화면에 노출되는 퀘스트 목록
        private static int clearCount = 0; //퀘스트 클리어 횟수
        public QuestManager(Player player)
        {
            EventManager.Instance.AddListener(EventType.Quest, this);
            questMenu = (Quest[])Utilities.LoadFile(LoadType.QuestData); //퀘스트 목록 추가
            quests.Add(questMenu[0]);
            quests.Add(questMenu[1]);
            quests.Add(questMenu[2]);
            this.player = player;
        }
        public int QuestCount { get { return quests.Count;} } //
        public void ShowQuests() //최대 3개의 퀘스트 목록 보여주기
        {
            if (quests.Count < 3)
                AddQuest();
            int i = 1;
            foreach (Quest quest in quests) 
            {
                Utilities.TextColorWithNoNewLine(i.ToString(), ConsoleColor.DarkRed);
                Console.Write($". {quest.Name}");
                if (quest.isActive == true && !quest.isClear)
                    Console.Write(" (진행중)");
                else if(quest.isActive == true && quest.isClear)
                    Console.Write(" (완료)");
                Console.WriteLine();
                i++;
            }
            Console.WriteLine();
        }
        public void ShowQuest(int idx) //idx에 해당하는 퀘스트 보여주기
        {
            if (quests[idx] == null)
                return;

            Utilities.TextColor($"Quest!!", ConsoleColor.Yellow);
            Console.WriteLine();

            Console.WriteLine(quests[idx].Name);
            Console.WriteLine();

            Console.WriteLine(quests[idx].Explanation);
            Console.WriteLine();

            //string conditionStr = quests[idx].Condition;//string.Format("미니언 {0}마리 처치",Max);
            Utilities.TextColorWithNoNewLine($"-", ConsoleColor.Yellow);
            Console.WriteLine($"{quests[idx].Condition} ({quests[idx].current}/{quests[idx].Max})");
            Console.WriteLine();
            Console.ReadKey(); 
            Utilities.TextColorWithNoNewLine($"-", ConsoleColor.Yellow);
            Console.WriteLine("보상 ");
            Utilities.TextColorWithNoNewLine($"-", ConsoleColor.Yellow);
            //if (quests[idx].ItemName)
            //    Console.WriteLine($"{quests[idx].Gold}G");
            //if (quests[idx].Gold != 0)
            //    Console.WriteLine($"{quests[idx].Gold}G") ;


        }
        public void SelectQuest() 
        {

        }
        public void AddQuest()
        {
            if (quests.Count < 3 && questMenu[clearCount + 3] != null)//퀘스트 3개 미만이고 다음 퀘스트가 있다면
            {
                for(int i = quests.Count; i < 3;)
                quests.Add(questMenu[clearCount + 3]);//3번 부터 추가
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
                    case eQuestType.PlayerLevel:
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
        public bool isClear = false;
        public bool isActive = false; //수락했을 시
        public int Max; //최대치
        public int current = 0; //현재 달성도
        public string Condition; //퀘스트 조건

        public Quest(int idx, string name, string explanation, int gold, string itemName, int exp, int max, string condition)
        {
            Idx = idx;
            Name = name;
            Explanation = explanation;
            Gold = gold;
            ItemName = itemName;
            Exp = exp;
            Max = max;
            Condition = condition;
        }
    }
}
