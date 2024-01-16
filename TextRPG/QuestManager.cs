using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    struct QuestData
    {
        public List<Quest> myQuest; //진행도 저장된 퀘스트 리스트
        public List<string> clearQuest;//완전 클리어된 퀘스트 이름 리스트
    }
    internal class QuestManager : IListener
    {
        private Quest[] questMenu; //퀘스트 전체 목록
        private List<Quest> quests; //화면에 노출되는 퀘스트 목록
        private List<string> Clearquest = new List<string>(); //클리어한 퀘스트 이름 리스트
        private static int clearCount = 0; //퀘스트 클리어 횟수(사용을 안하는 중)
        private static int idxCount = 0; // 현재 퀘스트를 나타내는 idx 
        public QuestManager()
        {
            EventManager.Instance.AddListener(EventType.eGameEnd, this);//게임 종료시 저장이벤트 등록
            EventManager.Instance.AddListener(EventType.Quest, this); //Add //퀘스트 관련 이벤트 등록
            questMenu = Utilities.LoadFile<Quest[]>(LoadType.QuestData); //퀘스트 전체 목록 받아오기\
            QuestData? data = Utilities.LoadFile<QuestData?>(LoadType.QuestSaveData); //QuestData 타입(현재 진행도 리스트, 클리어한 퀘스트 이름 리스트)으로 받아옴
            if (data != null)
            {
                foreach (var quest in data.Value.clearQuest) // 클리어한 퀘스트 이름 리스트에서 하나씩 꺼내서
                {
                    for (int i = 0; i < questMenu.Length; i++) //전체 배열을 탐색하고
                    {
                        if (questMenu[i].Name == quest) //이미 클리어한 것과 퀘스트 전체 배열의 퀘스트 이름이 같다면
                        {
                            questMenu[i].isClear = true; //전체 배열의 해당 퀘스트를 클리어로 변경
                            break;
                        }
                    }

                }
                quests = data.Value.myQuest; //현재 노출될 퀘스트는 세이브 파일의 저장된 진행도를 가진 퀘스트 리스트를 복사
                int count = 0;
                for (int i = 0; i < questMenu.Length; i++)
                {
                    if (questMenu[i].Name == quests[count].Name)
                    {
                        count++;
                        questMenu[i].isActive = true;
                    }
                }
            }
            else
            {
                quests = new List<Quest> { };
                Clearquest = new List<string>();
            }

            AddQuest();
        }
        public int QuestCount { get { return quests.Count;} } //
        public bool ShowQuests() //최대 3개의 퀘스트 목록 보여주기
        {
            if (quests.Count < 3) //다음 퀘스트가 있다면 채워준다.
                AddQuest();

            if (quests.Count == 0)//퀘스트가 없다면 
            {
                Console.WriteLine("퀘스트가 없습니다.");
                Console.WriteLine();
                return false;
            }

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
            return true;
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

            Utilities.TextColorWithNoNewLine($"- ", ConsoleColor.Yellow);
            Console.Write("보상 ");
            Utilities.TextColorWithNoNewLine($"-", ConsoleColor.Yellow);
            Console.WriteLine();

            if (quests[idx].ItemName != "")
            {
                Console.Write($"{quests[idx].ItemName} x");
                Utilities.TextColor($" 1", ConsoleColor.DarkRed);
            }

            if (quests[idx].Gold != 0)
                Console.WriteLine($"{quests[idx].Gold}G");

            if(quests[idx].Exp != 0)
                Console.WriteLine($"{quests[idx].Exp} exp");

            Console.WriteLine();
            int input =0;// 사용자 입력값
            if (!quests[idx].isClear) //진행중일 때
            {
                if (!quests[idx].isActive) //수락을 안 했다면
                {
                    Utilities.TextColorWithNoNewLine($"1. ", ConsoleColor.DarkRed);
                    Console.WriteLine("수락");

                    Utilities.TextColorWithNoNewLine($"2. ", ConsoleColor.DarkRed);
                    Console.WriteLine("거절");
                    Console.WriteLine();

                    input = Utilities.GetInputKey(1, 2);
                }
                else if (quests[idx].isActive) //수락을 했다면 
                {
                    Utilities.TextColorWithNoNewLine($"0. ", ConsoleColor.DarkRed);
                    Console.WriteLine("나가기");
                    Console.WriteLine();

                    input = Utilities.GetInputKey(0,0);
                }
                switch (input)
                {
                    case 0:
                        break;
                    case 1:
                        quests[idx].isActive = true; //수락을 누르면 진행중 상태
                        break;
                    case 2:
                        break;
                }
            }
            else if (quests[idx].isClear)//클리어 시
            {
                Utilities.TextColorWithNoNewLine($"1. ", ConsoleColor.DarkRed);
                Console.WriteLine("보상 받기");

                Utilities.TextColorWithNoNewLine($"2. ", ConsoleColor.DarkRed);
                Console.WriteLine("돌아가기");
                Console.WriteLine();
                input = Utilities.GetInputKey(1, 2);
                switch (input)
                {
                    case 1: //보상 받기
                        EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Exp, quests[idx].Exp)); //플레이어 경험치 추가
                        EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Gold, quests[idx].Gold)); //플레이어 골드 추가
                        EventManager.Instance.PostEvent(EventType.Item, Utilities.EventPair(eItemType.eGetFieldItem, quests[idx].ItemName));//아이템 획득
                        //아이템 얻는 이벤트 추가해야 함.
                        Clearquest.Add(quests[idx].Name);//클리어 
                        quests.RemoveAt(idx); //노출되는 목록에서 삭제
                        clearCount++; //클리어 횟수 증가
                        break;
                    case 2: // 돌아가기
                        break;
                }
            }
        }
        public void AddQuest() //퀘스트 추가 최대 3개까지 보인다.
        {
            for (int i = quests.Count; i < 3; i++) // 0 1 2 현재 퀘스트 개수가 3개 미만이고 
            {

                if (idxCount < questMenu.Length)//다음 퀘스트가 있다면 // 0  1 2 3<  4
                {
                    if (questMenu[idxCount].isClear || questMenu[idxCount].isActive)
                        idxCount++;
                    else
                        quests.Add(questMenu[idxCount++]);//추가한다.
                }
                else
                {
                    break;
                }
            }
        }
        public void OnEvent<T>(EventType type, T data)
        {
            if (type == EventType.Quest)
            {
                var a = data as KeyValuePair<eQuestType, string>?; //키 벨류 데이터로 변환이 가능하면 하고
                switch (a.Value.Key) //a의 속성 value로 <키,벨류> 쌍을 가르키고 그 중 key로 접근
                {
                    case eQuestType.Item:
                        {
                            foreach (var quest in quests)
                            {
                                if (quest.isActive && quest.Target == a.Value.Value) //이름이 같은 몬스터 이고 퀘스트가 활성화 되어있을 때
                                {
                                    quest.current++;
                                    if (quest.current >= quest.Max) //최고치에 도달하면 
                                    {
                                        quest.isClear = true; //퀘스트 클리어
                                    }
                                }
                            }
                            break;
                        }
                    case eQuestType.Dungeon:
                        {

                            break;
                        }
                    case eQuestType.Monster: //몬스터가 죽은 수만큼 Quest의 currnt(현재 달성도)를 증가시킨다.
                        {
                            foreach (var quest in quests)
                            {
                                if (quest.isActive && quest.Target == a.Value.Value) //이름이 같은 몬스터 이고 퀘스트가 활성화 되어있을 때
                                {
                                    quest.current++;
                                    if (quest.current >= quest.Max) //최고치에 도달하면 
                                    {
                                        quest.isClear = true; //퀘스트 클리어
                                    }
                                }
                            }
                            break;
                        }
                    case eQuestType.Stats:
                        {

                            break;
                        }
                    case eQuestType.PlayerLevel:
                        {
                            foreach (var quest in quests)
                            {
                                if (quest.isActive && quest.Target == "레벨업") //이름이 같은 몬스터 이고 퀘스트가 활성화 되어있을 때
                                {
                                    quest.current += Int32.Parse(a.Value.Value);
                                    if (quest.current >= quest.Max) //최고치에 도달하면 
                                    {
                                        quest.isClear = true; //퀘스트 클리어
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            else if (type == EventType.eGameEnd)
            {
                QuestData myQuestData = new QuestData { myQuest = quests, clearQuest = Clearquest };
                Utilities.SaveFile(SaveType.Quest, myQuestData);
            }
        }
    }
    public class Quest
    {
        public readonly string Target; //퀘스트 목적 대상
        public readonly string Name; //퀘스트 이름
        public readonly string Explanation; //퀘스트 전체 설명
        public readonly int Gold; // 보상 골드
        public readonly string ItemName; // 보상 아이템 이름
        public readonly int Exp;//보상 경험치
        public bool isClear = false; //클리어 했는지 여부
        public bool isActive = false; //수락했을 시
        public int Max; //조건 최대치
        public int current = 0; //현재 조건 달성도
        public string Condition; //퀘스트 조건 설명

        public Quest(string target, string name, string explanation, int gold, string itemName, int exp, int max, string condition)
        {
            Target = target;
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
