using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static TextRPG.Player;

namespace TextRPG
{
    //추민규님 마을 <-> 던전or상태보기 구현
    internal class Map
    {
        enum GameState
        {
            PlayerInfo = 1,
            StartBattle
        }

        MonsterManager monsterManager = new MonsterManager();
        DungeonManager dungeonManager = new DungeonManager();
        Player player = new Player();

        public void StartGame()
        {
            Utilities.AddLine("스파르타 던전에 오신 여러분 환영합니다.");
            Utilities.AddLine("이제 전투를 시작할 수 있습니다.");
            Utilities.AddLine("");

            Utilities.AddLine("1. 상태 보기");
            Utilities.AddLine("2. 전투 시작");
            Utilities.AddLine("");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            switch ((GameState)Utilities.GetInputKey(1, 2, ConsoleColor.Yellow, ">>"))
            {
                case GameState.PlayerInfo: // 상태 보기

                    break;
                case GameState.StartBattle: // 전투 시작
                    ShowBattle();
                    break;
            }
        }

        private void ShowBattle()
        { 
            // 몬스터 출력
            Random random = new Random();
            int monsterCount = random.Next(1, 5);   // 생성할 몬스터의 수
            int deadCounter = 0;


            EventManager.Instance.PostEvent(EventType.eMakeMonster, monsterCount);      // 몬스터 생성하기

            dungeonManager.GetMonsterList(monsterManager.monsters);

            while (deadCounter < monsterCount)  // 모든 몬스터가 죽거나 플레이어 체력이 0이 될 시 종료
            {
                Console.Clear();
                Console.WriteLine("Battle!!\n");

                foreach (Monster monster in monsterManager.monsters)
                {
                    if (monster.Hp <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"Lv.{(monster.Lv)} {monster.Name} Dead");
                        Console.ResetColor();
                    }
                    else
                        Console.WriteLine($"Lv.{(monster.Lv)} {monster.Name} HP {monster.Hp}");
                }

                Console.WriteLine("\n[내 정보]");
                // 플레이어 출력하기
                Console.WriteLine("Lv.1 Chad (전사)");
                Console.WriteLine("HP 100/100");

                Console.WriteLine("\n1. 공격");
                Console.WriteLine("원하시는 행동을 입력해주세요.");

                switch (Utilities.GetInputKey(1, 1, ConsoleColor.Yellow, ">> "))
                {
                    case 1:
                        SelectMonster(monsterCount, ref deadCounter);
                        break;
                }
            }

            dungeonManager.ShowResult(deadCounter, monsterManager.monsters.Count);

        }

        public void SelectMonster(int monsterCount, ref int deadCounter)
        {
            Console.Clear();
            bool isPlayerTurn = true;
            int playerHp = 100;     // 임시 플레이어 체력
            int playerAtk = 10;     // 임시 플레이어 공격력

            Console.WriteLine("Battle!!\n");

            int i = 0;
            foreach (Monster monster in monsterManager.monsters)
            {
                if (monster.Hp <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"{i + 1} Lv.{(monster.Lv)} {monster.Name} Dead");
                    Console.ResetColor();
                }
                else
                    Console.WriteLine($"{i + 1} Lv.{(monster.Lv)} {monster.Name}  HP {monster.Hp}");
                i++;
            }
            Console.WriteLine("\n[내 정보]");
            // 플레이어 출력하기
            Console.WriteLine("Lv.1 Chad (전사)");
            Console.WriteLine("HP 100/100");

            Console.WriteLine("\n0. 취소\n");

            Console.WriteLine("대상을 선택해주세요.");
            int input = (Utilities.GetInputKey(0, monsterCount, ConsoleColor.Yellow, ">> "));

            for(int j = 0; j <= monsterCount; j++)
            {
                deadCounter = 0;
                if (input == 0)
                {
                    break;
                }
                else if (j == input)
                {
                    if (monsterManager.monsters[j - 1].isDead)
                    {
                        Console.WriteLine("다시 선택해주세요!");
                        Console.ReadKey();
                        break;
                    }
                    dungeonManager.ShowBattle(monsterManager.monsters[j - 1], player, isPlayerTurn);
                    isPlayerTurn = false;

                    foreach (Monster monster in monsterManager.monsters)
                    {
                        if (monster.isDead)
                        {
                            deadCounter++;
                            continue;
                        }
                        dungeonManager.ShowBattle(monster, player, isPlayerTurn);
                    }
                    isPlayerTurn = true;
                }
            }
        }
    }
}
