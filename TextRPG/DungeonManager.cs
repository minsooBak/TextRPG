using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
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

    internal class DungeonManager
    {
        private Player player;
        private SkillManager skillManager = new SkillManager();
        private MonsterManager monsterManager = new MonsterManager();
        public List<Dungeon> dungeons = [];
        // monsters : 던전매니저에서 사용할 몬스터 배열
        public IObject[] monsters;
        // deadCounter : 이번 던전에서 죽은 몬스터 마리 수 저장
        public int deadCounter = 0;
        
        public bool showMonsterMode = false;    // 몬스터 선택창에서 번호 출력 여부. true = 출력,  false = 출력하지 않음
        //public int getExp = 0;


        public DungeonManager(Player player)
        {
            List<Dungeon>? d = Utilities.LoadFile<List<Dungeon>>(LoadType.Dungeon);

            dungeons = d;
            // 플레이어 정보 받아오기
            this.player = player;
        }

        // 선택된 던전 스테이지의 몬스터 만들기
        public void SelectDungeonStage(int stage)
        {
            // deadCounter 초기화
            deadCounter = 0;

            // MonsterManager에서 Monster 배열 받아오기
            monsters = monsterManager.MakeMonsters(dungeons[stage].dungeonMonsterType).ToArray();

            // 전투 시작
            StartBattle();
        }

        // 전투 돌입하기(ShowBattle에 있는 출력문 & 제어문)
        public void StartBattle()
        {
            //if (deadCounter >= monsters.Length) break; 이 조건문을 while조건문으로 넣었습니다
            // deadCounter를 몬스터가 데미지를 받을때 오르게 바꾸고 deadCounter의 초기화를 던전선택할때만 하게했습니다.
            // 플레이어가 살아있거나, 죽은 몬스터 마리 수가 생성된 마리 수 보다 적을 동안 전투 진행.
            while (player.IsDead == false && deadCounter < monsters.Length)
            {
                Console.Clear();

                Utilities.TextColor("Battle!!\n", ConsoleColor.DarkYellow);

                // 선택할 몬스터 출력
                ShowMonsterList(showMonsterMode = false);

                // 현재 플레이어의 스탯 출력 : 플레이어의 Level, Name, Class, HP, MP
                player.ShowStats();

                Utilities.TextColorWithNoNewLine("1.", ConsoleColor.DarkRed);
                Console.WriteLine(" 공격");

                Utilities.TextColorWithNoNewLine("2.", ConsoleColor.DarkRed);
                Console.WriteLine(" 스킬");


                Console.WriteLine("\n원하시는 행동을 입력해주세요.");
                Utilities.TextColorWithNoNewLine(">> ", ConsoleColor.Yellow);

                // 플레이어의 공격 방식에 따라 함수 호출.
                // 1 : 공격. 바로 SelectMonster()를 호출합니다.
                // 2 : 스킬. SelectSkill()을 호출합니다.
                switch ((AttackType)Utilities.GetInputKey(1, 2))
                {
                    case AttackType.Attack:
                        SelectMonster();//매개변수의 값이 기본값이 Attack임으로 지워놨습니다.
                        break;
                    case AttackType.Skill:
                        SelectSkill();
                        break;
                }
            }

            // 전투 종료 후 결과창 띄우기
            ShowResult(deadCounter, monsters.Length);
        }

        public void SelectMonster(AttackType attackType = AttackType.Attack) //대상 선택
        {
            Console.Clear();

            Console.WriteLine("Battle!! - 대상 선택\n");

            ShowMonsterList(showMonsterMode = true); // ShowMonsterMode = true : 몬스터 앞에 번호 붙여서 출력하기

            // 플레이어 스탯 띄우기
            player.ShowStats();

            Utilities.TextColorWithNoNewLine("0.", ConsoleColor.DarkRed);
            Console.WriteLine(" 취소\n");

            Console.WriteLine("대상을 선택해주세요.");
            Utilities.TextColorWithNoNewLine(">> ", ConsoleColor.Yellow);

            // input : 플레이어의 대상 선택 값.
            int input = Utilities.GetInputKey(0, monsters.Length);
            // 플레이어가 0을 선택한 경우, 다시 StartBattle()로 돌아가기
            if (input == 0) return;

            //선택한 몬스터가 죽었다면 return하게 변경(이중반복문 돌리면서 찾을 이유가 없기때문에)
            if (monsters[input - 1].IsDead)
            {
                Console.WriteLine("이미 죽은 몬스터입니다.\n다시 선택해주세요!");
                Thread.Sleep(1000);     // 1초 동안 "이미 죽은 몬스터입니다. 다시 선택해주세요!" 출력하기
                //Console.ReadKey();

                return;
            }

            //플레이어 공격
            ShowBattle(monsters[input - 1], true, attackType);// 공격 종류에 따라 일반 공격 , 스킬 공격 실행됨

            // 몬스터의 공격
            foreach (Monster monster in monsters)       // foreach(Monster monster in (Monster[])monsters)
            {
                // 이미 죽은 몬스터는 건너뛰기
                if (monster.IsDead)
                {
                    //break; 1번을 죽일경우 공격을 안하게 되기에 continue로 수정
                    continue;
                }

                // 몬스터의 공격 타입 랜덤으로 정하기
                AttackType monsterAttackType = (AttackType)new Random().Next(1, 3);// 1 ~2

                //몬스터가 랜덤으로 스킬을 쓴다면 몬스터의 현재 mp내의 마나소모가 높은 스킬을 쓰도록 바꿧습니다
                if ((AttackType)monsterAttackType == AttackType.Skill)
                    monster.SetSkill(skillManager.GetMonsterSkill(monster.Class, monster.GetMP));
                //monster.SetSkill(skillManager.GetMySkill(monster.Class, 0));

                //몬스터 공격
                // 플레이어가 죽으면 종료
                if (!player.IsDead)
                    ShowBattle(monster, false, (AttackType)monsterAttackType);
            }
        }

        // 스킬 선택
        public void SelectSkill()
        {
            Console.Clear();

            Utilities.TextColor("Battle!!\n", ConsoleColor.DarkYellow);

            ShowMonsterList(showMonsterMode = false); //몬스터 번호 없이 출력

            player.ShowStats(); // 플레이어 상태 표시

            //EventManager.Instance.PostEvent(EventType.eShowSkill, playerJob); // 플레이어 직업의 스킬 출력 
            skillManager.ShowSkillList(player.Class);// 플레이어 직업의 스킬 출력 

            Utilities.TextColorWithNoNewLine("0.", ConsoleColor.DarkRed);
            Console.WriteLine(" 취소");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");

            // 마나가 부족할 경우, SelectSkill()을 다시 호출.
            int skillIdx = Utilities.GetInputKey(0, skillManager.GetMySkillCount(player.Class)); //임시 플레이어 직업 전사 
            if (skillIdx == 0) return;
            player.SetSkill(skillManager.GetMySkill(player.Class, skillIdx - 1)); //선택한 스킬 할당

            if (player.IsUseSkill == false)  //마나가 모자르다면
            {
                Console.WriteLine("마나가 부족합니다.");
            }
            else
                SelectMonster(AttackType.Skill);
        }

        // 몬스터 보여주기. 매개변수인 mode는 showMonsterMode 입니다.
        private void ShowMonsterList(bool mode)
        {
            int i = 1;
            foreach (Monster monster in monsters)
            {
                if (monster.IsDead)
                {
                    // 죽은 몬스터일 경우 회색으로 출력
                    Utilities.TextColorWithNoNewLine($"{(mode ? i + " " : "")}", ConsoleColor.DarkGray);
                }
                else
                {
                    Utilities.TextColorWithNoNewLine($"{(mode ? i + " " : "")}", ConsoleColor.Blue);
                }
                // 몬스터 상태 출력
                monster.ShowStats();

                Console.WriteLine();
                i++;
            }
        }


        // 공격 진행하기(ShowBattle)
        // isPlayerTurn : 참일 경우 플레이어 차례, 거짓일 경우 몬스터의 차례
        // attackType : 공격 방식.
        public void ShowBattle(IObject monster, bool isPlayerTurn,AttackType attackType = AttackType.Attack)
        {
            Console.Clear();


            Utilities.TextColor("Battle!! - 전투 진행\n", ConsoleColor.DarkYellow);

            int damage;
            if (isPlayerTurn)
            {
                // 몬스터 공격하기.

                // 플레이어의 공격 데미지 받아오기
                damage = player.Attack(attackType); //공격 종류에 따라 달리 작동

                // 몬스터에게 플레이어의 공격 데미지 넘겨주기
                monster.TakeDamage(damage);

                // 이번 공격으로 몬스터가 죽었을 경우, deadCounter 증가
                deadCounter += monster.IsDead ? 1 : 0;
            }
            else
            {
                // 몬스터가 플레이어를 공격하기.

                // 몬스터의 공격 데미지 받아오기
                damage = monster.Attack(attackType);

                // 플레이어에게 공격 데미지 넘기기
                player.TakeDamage(damage);
            }

            Console.WriteLine("\n0. 다음\n");
            Utilities.TextColorWithNoNewLine(">>", ConsoleColor.Yellow);
            // 다음 화면으로 넘기기
            if (Utilities.GetInputKey(0, 0) == 0)
            {
                Console.Clear();
                return;
            }
        }

        // 결과 화면 보여주기
        public void ShowResult(int deadCounter, int monster )
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Battle!! - Result\n");
            Console.ResetColor();

            // 승리 시
            if (deadCounter >= monster)
            {
                Utilities.TextColor("Victory\n", ConsoleColor.DarkGreen);

                Console.Write($"던전에서 몬스터 ");
                Utilities.TextColorWithNoNewLine($"{monster}", ConsoleColor.DarkRed);       // 이번 던전에서 잡은 몬스터 수 출력
                Console.WriteLine("마리를 잡았습니다.\n");

                EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Exp,monsterManager.GetExp())); //잡은 몬스터들의 경험치 양 만큼 플레이어 exp 증가 
                player.ShowResult(); //던전 몬스터 배열의 경험치들을 다 더하고 리턴
        
                GetReward(); //아이템 드랍, 퀘스트 이벤트 , 골드 추가,
            }
            else
            {
                Console.WriteLine("You Lose\n");

                player.ShowResult();
            }
            Console.WriteLine("\n0. 다음\n");
            Utilities.TextColorWithNoNewLine(">>", ConsoleColor.Yellow);
            if (Utilities.GetInputKey(0, 0) == 0)
            {
                // 몬스터 매니저에 만들어져 있는 몬스터 리스트 초기화
                monsterManager.ClearMonsterList();
                Console.Clear();
                return;
            }
        }

        //public void OnEvent<T>(EventType type, T data) 기존 OnEvent
        public void GetReward()
        {
            // 몬스터 매니저에 전리품 획득 처리 및 출력
            monsterManager.GetReward();
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