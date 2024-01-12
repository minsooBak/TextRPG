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
        private Player player;
        private SkillManager skillManager= new SkillManager();
        public DungeonManager(Player player) 
        {
            EventManager.Instance.AddListener(EventType.eSetMonsters, this);
            List<Dungeon>? d = (List<Dungeon>?)Utilities.LoadFile(LoadType.Dungeon);
            dungeons = d;

            // 플레이어 정보 받아오기
            this.player = player;    // Player 완성 시 new Player() 지우고 다시 설정하기
        }
        public List<Dungeon> dungeons = [];
        public int deadCounter = 0;
        public int dungeonStage = 1;
        public Monster[] monsters;

        // 선택된 던전 스테이지의 몬스터 만들기
        public void SelectDungeonStage(int stage)
        {
            deadCounter = 0;
            dungeonStage = stage - 1;
            MakeMonsters(dungeons[dungeonStage].dungeonMonsterType);
        }

        public void MakeMonsters(int dungeonMonsterType)
        {
            // 몬스터 생성
            EventManager.Instance.PostEvent(EventType.eMakeMonsters, dungeonMonsterType);
        }
        
        public bool showMonsterMode = false;

        public int playerHp = 100;  // 임시 플레이어 체력
        public int playerAtk = 10;  // 임시 플레이어 공격력
        public string playerJob = "전사"; // 임시 플레이어 직업


        // 몬스터 배열을 몬스터 리스트에서 받아 생성하기
        public void Encounter(List<Monster>dungeonMonsters)
        {
            monsters = dungeonMonsters.ToArray();

            StartBattle();
        }

        // 전투 돌입하기(ShowBattle에 있는 출력문 & 제어문)
        public void StartBattle()
        {
            while (playerHp > 0)
            {
                // MonsterManager에서 몬스터가 죽으면 리스트에서 제거되는 로직 추가 후 수정하기(List.Count = 0이 되면 while문 탈출)
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
                        SelectMonster(AttackType.Attack);
                        break;
                        case AttackType.Skill:
                        SelectSkill();
                        break;
                }
            }
            ShowResult(deadCounter, monsters.Length);
        }
        public void SelectMonster(AttackType attackType = AttackType.Attack) //대상 선택
        {
            Console.Clear();

            bool isPlayerTurn = true;

            Console.WriteLine("Battle!! - 대상 선택\n");

            ShowMonsterList(showMonsterMode = true);        // ShowMonsterMode = true : 몬스터 앞에 번호 붙여서 출력하기

            ShowPlayerStats();

            Utilities.TextColorWithNoNewLine("0.", ConsoleColor.DarkRed);
            Console.WriteLine(" 취소\n");

            Console.WriteLine("대상을 선택해주세요.");
            Utilities.TextColorWithNoNewLine(">> ", ConsoleColor.Yellow);

            Random rnd = new Random();
            int monsterAttackType;
            int input = Utilities.GetInputKey(0, monsters.Length);
            if (input == 0) return;

            for (int i = 1; i <= monsters.Length; i++)
            {
                if (input == i)
                {
                    if (monsters[i - 1].IsDead())
                    {
                        break;
                    }

                    ShowBattle(monsters[i - 1], isPlayerTurn, attackType);// 공격 종류에 따라 일반 공격 , 스킬 공격 실행됨
                    isPlayerTurn = false;

                    foreach (Monster monster in monsters)
                    {
                        if (monster.isDead)
                        {
                            continue;
                        }

                        monsterAttackType = rnd.Next(1, 3);// 1 ~2

                        if ((AttackType)monsterAttackType == AttackType.Skill)
                        {
                            Console.WriteLine("몬스터 스킬");
                            monster.SetSkill(skillManager.GetMySkill(monster.Name, 0));
                        }
                        ShowBattle(monster, isPlayerTurn, (AttackType)monsterAttackType);
                    }
                    isPlayerTurn = true;
                }
            }
        }
        // 스킬 선택
        public void SelectSkill()
        {
            Console.Clear();

            bool isPlayerTurn = true;

            Utilities.TextColor("Battle!!\n", ConsoleColor.DarkYellow);

            ShowMonsterList(showMonsterMode = false); //몬스터 번호 없이 출력

            ShowPlayerStats(); // 플레이어 상태 표시

            //EventManager.Instance.PostEvent(EventType.eShowSkill, playerJob); // 플레이어 직업의 스킬 출력 
            skillManager.ShowSkillList(playerJob);// 플레이어 직업의 스킬 출력 

            Utilities.TextColorWithNoNewLine("0.", ConsoleColor.DarkRed);
            Console.WriteLine(" 취소");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            
            int input = Utilities.GetInputKey(0, skillManager.GetMySkillCount(playerJob)); //임시 플레이어 직업 전사 
            input--;
            if (0 <= input && input < skillManager.GetMySkillCount(playerJob))
            {
                player.SetSkill(skillManager.GetMySkill(playerJob, input)); //선택한 스킬 할당
                SelectMonster(AttackType.Skill);
            }
            else
            {
                StartBattle();
                return;
            }
        }
        // 내 스탯 보여주기
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

        // 몬스터 보여주기
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
        

        // 공격 진행하기(ShowBattle)
        public void ShowBattle(Monster monster, bool isPlayerTurn,AttackType attackType = AttackType.Attack)
        {
            Console.Clear();

            int damage;

            Utilities.TextColor("Battle!! - 전투 진행\n", ConsoleColor.DarkYellow);

            if (isPlayerTurn)
            {
                // 몬스터 공격하기.
                // player Class 추가 후 IObject.Attack()에서 공격 처리하기.
                // player.Attack()에서 damage를 return 받아 monster.TakeDamage()에 넣어주기
                // monster.TakeDamage(player.Attack());

                damage = player.Attack(attackType); //공격 종류에 따라 달리 작동

                monster.TakeDamage(damage);
            }
            else
            {
                // 몬스터가 플레이어를 공격하기.
                // player.Attack()에서 damage를 return 받아 monster.TakeDamage()에 넣어주기
                // player.TakeDamage(monster.Attack());

                damage = monster.Attack(attackType);
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

        // 결과 화면 보여주기
        public void ShowResult(int deadCounter, int monster)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Battle!! - Result\n");
            Console.ResetColor();

            // 승리 시
            if(deadCounter >= monster)
            {
                Console.WriteLine("Victory\n");

                Console.WriteLine($"던전에서 몬스터 {monster}마리를 잡았습니다\n");

                Console.WriteLine($"Lv.1 Chad\nHP 100 -> {playerHp}\n");
            }
            else
            {
                Console.WriteLine("You Lose\n");

                Console.WriteLine("Lv.1 Chad\nHP 100 -> 0\n");
            }
            Console.WriteLine("0. 다음\n>> ");

            if (Utilities.GetInputKey(0, 0) == 0)
            {
                Console.Clear();
                return;
            }
        }

        public void OnEvent(EventType type, object data)
        {
            if(type == EventType.eSetMonsters)
            {
                // MonsterManager에서 생성된 몬스터 리스트 받기
                Encounter((List<Monster>)data);
            }
        }
    }

    public class Dungeon
    {
        public int dungeonStage { get; private set; }
        // 난이도별 생성 가능한 몬스터 목록
        public int dungeonMonsterType {  get; private set; }
        
        public Dungeon(int dungeonStage, int dungeonMonsterType)
        {
            this.dungeonStage = dungeonStage;
            this.dungeonMonsterType = dungeonMonsterType;
        }
    }
}
