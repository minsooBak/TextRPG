﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    internal class QuestManager : IListener
    {
        private Quest[] questMenu; //퀘스트 전체 목록
        private List<Quest> quests = new List<Quest> { }; //화면에 노출되는 퀘스트 목록
        private static int clearCount = 0; //퀘스트 클리어 횟수
        private static int idxCount = 0; // 현재 퀘스트를 나타내는 idx
        public QuestManager()
        {
            EventManager.Instance.AddListener(EventType.Quest, this);
            questMenu = (Quest[])Utilities.LoadFile(LoadType.QuestData); //퀘스트 목록 추가
            quests.Add(questMenu[0]);
            quests.Add(questMenu[1]);
            quests.Add(questMenu[2]);
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
                        //아이템 얻는 이벤트 추가해야 함.
                        quests.RemoveAt(idx); //노출되는 목록에서 삭제
                        clearCount++; //클리어 횟수 증가
                        break;
                    case 2: // 돌아가기
                        break;
                }
            }
        }
        public void SelectQuest() 
        {

        }
        public void AddQuest() //퀘스트 추가
        {
            for (int i = quests.Count; i < 3;) // 0 1 2 현재 퀘스트 개수가 3개 미만이고 
            {
                if (clearCount + 2 < questMenu.Length)//다음 퀘스트가 있다면 // 0  1 2 <  3
                {
                    quests.Add(questMenu[idxCount++]);//추가한다.
                }
                else
                {
                    break;
                }
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
                    case eQuestType.Monster: //몬스터가 죽은 수만큼 Quest의 currnt를 증가시킨다.
                        {
                            Int32.Parse(a.Value);
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
        public readonly string ItemName; // 보상 아이템 이름
        public readonly int Exp;//보상 경험치
        public bool isClear = true;
        public bool isActive = true; //수락했을 시
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
