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
        private SkillManager skillManager = new SkillManager();
        private MonsterManager monsterManager = new MonsterManager();
        public List<Dungeon> dungeons = [];
        public IObject[] monsters;
        public int deadCounter = 0;
        public int dungeonStage = -1; // 1
        public bool showMonsterMode = false;


        public DungeonManager(Player player)
        {
            EventManager.Instance.AddListener(EventType.eSetMonsters, this);
            List<Dungeon>? d = (List<Dungeon>?)Utilities.LoadFile(LoadType.Dungeon);
            dungeons = d;
            // 플레이어 정보 받아오기
            this.player = player;    // Player 완성 시 new Player() 지우고 다시 설정하기
        }

        // 선택된 던전 스테이지의 몬스터 만들기
        public void SelectDungeonStage(int stage) // 2
        {
            deadCounter = 0;

            monsterManager.MakeMonsters(dungeons[stage].dungeonMonsterType);
        }

        // 몬스터 배열을 몬스터 리스트에서 받아 생성하기
        public void Encounter(List<Monster> dungeonMonsters)
        {
            monsters = dungeonMonsters.ToArray();
            
            StartBattle();
        }

        // 전투 돌입하기(ShowBattle에 있는 출력문 & 제어문)
        public void StartBattle()
        {
            //if (deadCounter >= monsters.Length) break; 이 조건문을 while조건문으로 넣었습니다
            // deadCounter를 몬스터가 데미지를 받을때 오르게 바꾸고 deadCounter의 초기화를 던전선택할때만 하게했습니다.
            while (player.IsDead == false && deadCounter < monsters.Length)
            {
                Console.Clear();

                Utilities.TextColor("Battle!!\n", ConsoleColor.DarkYellow);

                ShowMonsterList(showMonsterMode = false);

                player.ShowStats(); // 플레이어 정보창에는 번호가 추가되거나 그런것이 없어서 메서드 제거 후 호출했습니다.


                Utilities.TextColorWithNoNewLine("1.", ConsoleColor.DarkRed);
                Console.WriteLine(" 공격");

                Utilities.TextColorWithNoNewLine("2.", ConsoleColor.DarkRed);
                Console.WriteLine(" 스킬");


                Console.WriteLine("\n원하시는 행동을 입력해주세요.");
                Utilities.TextColorWithNoNewLine(">> ", ConsoleColor.Yellow);

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

            ShowResult(deadCounter, monsters.Length);

        }
        public void SelectMonster(AttackType attackType = AttackType.Attack) //대상 선택
        {
            Console.Clear();

            Console.WriteLine("Battle!! - 대상 선택\n");

            ShowMonsterList(showMonsterMode = true); // ShowMonsterMode = true : 몬스터 앞에 번호 붙여서 출력하기

            player.ShowStats();

            Utilities.TextColorWithNoNewLine("0.", ConsoleColor.DarkRed);
            Console.WriteLine(" 취소\n");

            Console.WriteLine("대상을 선택해주세요.");
            Utilities.TextColorWithNoNewLine(">> ", ConsoleColor.Yellow);

            int input = Utilities.GetInputKey(0, monsters.Length);
            if (input == 0) return;

            //선택한 몬스터가 죽었다면 return하게 변경(이중반복문 돌리면서 찾을 이유가 없기때문에)
            if (monsters[input - 1].IsDead)
            {
                return;
            }

            //플레이어 공격
            ShowBattle(monsters[input - 1], true, attackType);// 공격 종류에 따라 일반 공격 , 스킬 공격 실행됨

            foreach (Monster monster in (Monster[])monsters)
            {
                if (monster.IsDead)
                {
                    //break; 1번을 죽일경우 공격을 안하게 되기에 continue로 수정
                    continue;
                }

                AttackType monsterAttackType = (AttackType)new Random().Next(1, 3);// 1 ~2

                //몬스터가 랜덤으로 스킬을 쓴다면 몬스터의 현재 mp내의 마나소모가 높은 스킬을 쓰도록 바꿧습니다
                if ((AttackType)monsterAttackType == AttackType.Skill)
                    monster.SetSkill(skillManager.GetMonsterSkill(monster.Class, monster.GetMP));
                //monster.SetSkill(skillManager.GetMySkill(monster.Class, 0));

                //몬스터 공격
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

        // 몬스터 보여주기
        private void ShowMonsterList(bool mode)
        {
            int i = 1;
            foreach (Monster monster in monsters)
            {
                if (monster.IsDead)
                {
                    Utilities.TextColorWithNoNewLine($"{(mode ? i + " " : "")}", ConsoleColor.DarkGray);
                }
                else
                {
                    Utilities.TextColorWithNoNewLine($"{(mode ? i + " " : "")}", ConsoleColor.Blue);
                }
                monster.ShowStats();

                Console.WriteLine();
                i++;
            }
        }
        // 공격할 몬스터 고르기(SelectMonster)
        // 플레이어가 선택하는 몬스터를 반환한다.


        // 공격 진행하기(ShowBattle)
        public void ShowBattle(IObject monster, bool isPlayerTurn,AttackType attackType = AttackType.Attack)
        {
            Console.Clear();


            Utilities.TextColor("Battle!! - 전투 진행\n", ConsoleColor.DarkYellow);

            int damage;
            if (isPlayerTurn)
            {
                // 몬스터 공격하기.
                // player Class 추가 후 IObject.Attack()에서 공격 처리하기.
                // player.Attack()에서 damage를 return 받아 monster.TakeDamage()에 넣어주기
                // monster.TakeDamage(player.Attack());

                damage = player.Attack(attackType); //공격 종류에 따라 달리 작동

                monster.TakeDamage(damage);

                deadCounter += monster.IsDead ? 1 : 0; 
            }
            else
            {
                // 몬스터가 플레이어를 공격하기.
                // player.Attack()에서 damage를 return 받아 monster.TakeDamage()에 넣어주기
                // player.TakeDamage(monster.Attack());

                damage = monster.Attack(attackType);
                player.TakeDamage(damage);
            }

            Console.WriteLine("\n0. 다음\n");
            Utilities.TextColorWithNoNewLine(">>", ConsoleColor.Yellow);
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
                Console.WriteLine("Victory\n");

                Console.WriteLine($"던전에서 몬스터 {monster}마리를 잡았습니다\n");

                player.ShowResult();
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
                Console.Clear();
                return;
            }
        }

        public void OnEvent(EventType type, object data)
        {
            if (type == EventType.eSetMonsters)
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