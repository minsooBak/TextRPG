using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    //송상화님 던전 구현 
    // 전투 결과 구현하기
    public enum AttackType
    {
        Attack = 1,
        Skill
    }

    internal class DungeonManager : IListener
    {
        public DungeonManager() 
        {
            // 몬스터 정보 받아오기
            // 플레이어 정보 받아오기
        }
        public int deadCounter = 0;
        public Monster[] monsters;
        public Player player;

        public bool showMonsterMode = false;

        public int playerHp = 100;  // 임시 플레이어 체력
        public int playerAtk = 10;  // 임시 플레이어 공격력

        public void GetPlayer(Player player)
        {
            this.player = player;
        }

        // 몬스터 배열을 몬스터 리스트에서 받아 생성하기
        public void Encounter(List<Monster> dungeonMonster)
        {
            monsters = dungeonMonster.ToArray();

            StartBattle();
        }

        // 전투 돌입하기(ShowBattle에 있는 출력문 & 제어문)
        public void StartBattle()
        {
            while (playerHp > 0)
            {
                deadCounter = 0;
                foreach (Monster monster in monsters)
                {
                    if(monster.isDead) deadCounter++;
                }
                if (deadCounter >= monsters.Length) break;

                Console.Clear();

                Utilities.TextColor("Battle!!\n", ConsoleColor.DarkYellow);

                ShowMonsterList(showMonsterMode = false);

                ShowPlayerStats();

                Utilities.TextColorWithNoNewLine("1.", ConsoleColor.DarkRed);
                Console.WriteLine(" 공격");

                Utilities.TextColorWithNoNewLine("2.", ConsoleColor.DarkRed);
                Console.WriteLine(" 스킬");


                Console.WriteLine("\n원하시는 행동을 입력해주세요.");
                Utilities.TextColorWithNoNewLine(">> ", ConsoleColor.Yellow);

                switch((AttackType)Utilities.GetInputKey(1, 2))
                {
                    case AttackType.Attack:
                        SelectMonster();
                        break;
                        case AttackType.Skill:
                        Console.WriteLine("스킬");
                        break;
                }
            }
            ShowResult(deadCounter, monsters.Length);
        }

        private void ShowPlayerStats()
        {
            Console.WriteLine("\n[내 정보]");
            Console.Write("Lv.");
            Utilities.TextColorWithNoNewLine("1 ", ConsoleColor.DarkRed);        // 나중에 player.Lv로 수정하기
            Console.WriteLine("Chad (전사)");         // 나중에 player.Name, player.Job으로 수정하기

            Console.Write("HP ");
            Utilities.TextColorWithNoNewLine($"{playerHp}", ConsoleColor.DarkRed);      // 나중에 player.Hp로 수정하기
            Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
            Utilities.TextColorWithNoNewLine("100\n\n", ConsoleColor.DarkRed);
        }

        private void ShowMonsterList(bool mode)
        {
            int i = 1;
            foreach (Monster monster in monsters)
            {
                if (monster.isDead)
                {
                    Utilities.TextColor($"{(mode ? i + " " : "")}Lv.{monster.Lv} {monster.Name} Dead", ConsoleColor.DarkGray);
                }
                else
                {
                    Utilities.TextColorWithNoNewLine($"{(mode ? i + " " : "")}", ConsoleColor.Blue);
                    Console.Write("Lv.");
                    Utilities.TextColorWithNoNewLine($"{monster.Lv} ", ConsoleColor.DarkRed);
                    Console.Write($"{monster.Name} HP ");
                    Utilities.TextColor($"{monster.Hp}", ConsoleColor.DarkRed);
                }
                i++;
            }
        }

        // 공격할 몬스터 고르기(SelectMonster)
        // 플레이어가 선택하는 몬스터를 반환한다.
        public void SelectMonster()
        {
            Console.Clear();

            bool isPlayerTurn = true;

            Console.WriteLine("Battle!! - 대상 선택\n");

            ShowMonsterList(showMonsterMode = true);

            ShowPlayerStats();

            Utilities.TextColorWithNoNewLine("0.", ConsoleColor.DarkRed);
            Console.WriteLine(" 취소\n");

            Console.WriteLine("대상을 선택해주세요.");
            Utilities.TextColorWithNoNewLine(">> ", ConsoleColor.Yellow);

            int input = Utilities.GetInputKey(0, monsters.Length);
            if (input == 0) return;

            for (int i = 1; i <= monsters.Length; i++)
            {
                if (input == i)
                {
                    if (monsters[i - 1].isDead)
                    {
                        Console.WriteLine("이미 죽은 몬스터입니다.\n 다시 선택해주세요!");
                        Console.ReadKey();
                        break;
                    }

                    ShowBattle(monsters[i - 1], player, isPlayerTurn);
                    isPlayerTurn = false;

                    foreach (Monster monster in monsters)
                    {
                        if (monster.isDead)
                        {
                            continue;
                        }
                        ShowBattle(monster, player, isPlayerTurn);
                    }
                    isPlayerTurn = true;
                }
            }
        }


        // 플레이어 혹은 몬스터가 상대를 공격했을 때 결과
        // 결과 : 
        // 누가 공격했는지
        // 누구를 공격했는지, 준 데미지 표기

        // 주는 데미지 결정
        // Hp 변화가 있을 시 Event.Type(eHpChange)로 postevent 해주기

        // 공격할 몬스터 고르기(SelectMonster)

        // 공격 진행하기(ShowBattle)
        // 공격 받은 객체의 HP 상태 출력, 반환
        
        public void ShowBattle(Monster monster, Player player, bool isPlayerTurn)
        {
            Console.Clear();

            double getDamage;
            int damage;

            Utilities.TextColor("Battle!! - 전투 진행\n", ConsoleColor.DarkYellow);

            if (isPlayerTurn)
            {
                getDamage = playerAtk / 100.0 * 10;
                damage = new Random().Next(playerAtk - (int)Math.Ceiling(getDamage), playerAtk + (int)Math.Ceiling(getDamage) + 1);

                Console.WriteLine("Chad 의 공격!");
                Console.WriteLine($"Lv.{monster.Lv} {monster.Name} 을(를) 맞췄습니다. [데미지 : {damage}]\n");

                Console.WriteLine($"Lv.{monster.Lv} {monster.Name}");
                Console.Write($"{monster.Hp} -> ");

                monster.TakeDamage(damage);

                Console.WriteLine($"{(monster.isDead ? "Dead" : monster.Hp)}");
            }
            else
            {
                damage = monster.Attack();
                Console.WriteLine($"Chad 을(를) 맞췄습니다. [데미지 : {damage}]\n");

                Console.WriteLine($"Lv.1 Chad");
                Console.Write($"{playerHp} -> ");
                playerHp -= damage;

                Console.WriteLine($"{(playerHp <= 0 ? playerHp = 0 : playerHp)}");
            }

            Console.WriteLine("\n0. 다음\n");
            if (Utilities.GetInputKey(0, 0, ConsoleColor.Yellow, ">> ") == 0)
            {
                Console.Clear();
                return;
            }
        }

        public void ShowResult(int deadCounter, int monster)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Battle!! - Result\n");
            Console.ResetColor();

            if(deadCounter >= monster)
            {
                Console.WriteLine("Victory\n");

                Console.WriteLine($"던전에서 몬스터 {monster}마리를 잡았습니다\n");

                Console.WriteLine($"Lv.1 Chad\n HP 100 -> {playerHp}\n");

                Console.WriteLine("0. 다음\n>> ");

                if (Utilities.GetInputKey(0, 0) == 0) return;
            }
            else
            {
                Console.WriteLine("You Lose\n");

                Console.WriteLine("Lv.1 Chad\nHP 100 -> 0\n");

                Console.WriteLine("0. 다음\n>> ");

                if (Utilities.GetInputKey(0, 0) == 0) return;
            }
        }

        public void OnEvent(EventType type, object data)
        {
            throw new NotImplementedException();
        }
    }
}
