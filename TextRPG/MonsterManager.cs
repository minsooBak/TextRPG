using System.Threading;

namespace TextRPG
{
    public enum MonsterType
    {
        Monster1 = 1, // 미니언
        Monster2, // 공허충
        Monster3  // 대포 미니언
    }
    //정원우님 구현
    internal class MonsterManager : IListener
    {
        public MonsterManager()
        {
            EventManager.Instance.AddListener(EventType.eMakeMonsters, this);

            // arrayOfMonsterTypes 배열 생성
            for(int i = 0; i < maxMonsterType; i++)
            {
                arrayOfMonsterTypes[i] = (MonsterType)(i + 1);
            }
            monsterItemName[0] = "낡은 대검";
            monsterItemName[1] = "초보자의 갑옷";
            monsterItemName[2] = "가시 갑옷";
        }
        // maxMonsterType : 생성 가능한 몬스터 종류 수
        static int maxMonsterType = 3;
        // arrayOfMonsterTypes : 생성 가능한 몬스터 타입 배열
        public MonsterType[] arrayOfMonsterTypes = new MonsterType[maxMonsterType];

        List<IObject> dungeonMonsters = [];
        string[] monsterItemName = new string[maxMonsterType]; // 돌아가는지 체크용으로
        
        public List<IObject> MakeMonsters(int listOfMonsterCount) //몬스터 생성 //스테이지 1 2 3 4

        {
            Random rnd = new Random();
            
            // 스테이지에 따라 생성 가능한 몬스터 마리 수 정하기
            int monsterCount = 0;
            switch (listOfMonsterCount)
            {
                case 1:
                    monsterCount = rnd.Next(1, 3);
                    break;
                case 2:
                    monsterCount = rnd.Next(2, 4);
                    break;
                case 3:
                    monsterCount = rnd.Next(3, 5);
                    break;
            }

            // 던전매니저에서 호출한 몬스터의 타입 값이 전체 몬스터 종류 수(maxMonsterType)보다 클 경우, 값을 maxMonsterType로 고정하기.
            if (listOfMonsterCount > maxMonsterType)
                listOfMonsterCount = maxMonsterType; //1 2 3 
            for (int i = 0; i < monsterCount; i++)
            {
                int randomCount = rnd.Next(0, listOfMonsterCount);// 0 1 2 등록되어 있는 몬스터 중 어떤 몬스터를 고를지
                Monster monster = new Monster(arrayOfMonsterTypes[randomCount]);
                dungeonMonsters.Add(monster);// 1 2 3 던전 몬스터 리스트에 몬스터 추가
            }

            //EventManager.Instance.PostEvent(EventType.eSetMonsters, Utilities.EventPair(EventType.eSetMonsters, dungeonMonsters));
            return dungeonMonsters;
        }

        // 던전 승리 시 경험치 얻기
        public int GetExp()
        {
            int exp = 0;

            foreach (Monster monster in dungeonMonsters)
            {
                // 해당 몬스터 리스트의 총 경험치량 저장
                exp += monster.Exp; //몬스터의 경험치를 더하는 것이 괜찮을 것 같아서 -> 저도 이 코드가 더 괜찮은 것 같습니다!
            }
            return exp;
        }

        // 던전 승리 시 아이템 얻기
        public void GetReward()
        {
            Random rnd = new Random();
            int[] itemsCounter = new int[3];    // 낡은 대검, 초보자의 갑옷, 가시 갑옷 총 3개의 아이템만 떨군다.
            int gold = 0;

            foreach (Monster monster in dungeonMonsters)
            {
                if (rnd.Next(1, 101) <= 8) //8퍼 확률로
                {
                    EventManager.Instance.PostEvent(EventType.Item, Utilities.EventPair(eItemType.eGetFieldItem,monster.item)); //아이템 드랍 이벤트 수정

                    if (monster.Level == 2)
                    {
                        itemsCounter[0]++; //떨어뜨릴 아이템 리스트에 추가 
                    }
                    else if (monster.Level == 3)
                    {
                        itemsCounter[1]++;
                    }
                    else
                    {
                        itemsCounter[2]++;
                    }
                }
                EventManager.Instance.PostEvent(EventType.Quest, Utilities.EventPair(eQuestType.Monster, monster.Class)); //몬스터 수만큼 처치 이벤트 발생
                gold += monster.Gold; //몬스터 골드 총합
            }

            EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Gold, gold)); //플레이어 골드 증가 이벤트
            Console.WriteLine();
            Console.WriteLine("[획득 아이템]");
            Utilities.TextColorWithNoNewLine($"{gold}", ConsoleColor.DarkRed);
            Console.WriteLine($" Gold");

            for(int i = 0; i < itemsCounter.Length; i++)
            {
                if (itemsCounter[i] > 0)
                {
                    //Console.WriteLine($"{dungeonMonsters[i].item} - {itemsCounter[i]}"); // 몬스터가 가지고 있는 아이템 이름 - 개수 
                    //Console.WriteLine($"{monsterItemName[i]} - {itemsCounter[i]}"); // 테스트로 monsterItemName 이라는 아이템 배열을 만들어서 수정 필요
                    Console.Write($"{monsterItemName[i]} ");
                    Utilities.TextColorWithNoNewLine("- ", ConsoleColor.DarkYellow);
                    Utilities.TextColorWithNoNewLine($"{itemsCounter[i]}", ConsoleColor.DarkRed);
                }
            }
        }

        public void ClearMonsterList()
        {
            dungeonMonsters.Clear();
        }

        public void OnEvent<T>(EventType type, T data)
        {
            var d = data as KeyValuePair<EventType, int>?;
            if (type == EventType.eMakeMonsters)
            {
                MakeMonsters(d.Value.Value);
            }
        }
    }

    public class Monster : IObject
    {
        private ObjectState myState;
        public int Exp => myState.EXP;

        public int MP => myState.MP;

        public int Health => myState.Health;

        public int Level => myState.Level;

        public string Class => myState.Class;

        public string item { get; set; }
        public int Gold => myState.Gold;

        public bool IsUseSkill => myState.Skill.Cost < myState.MP;//사용할 수 있는지 체크후 bool
        public bool IsDead => myState.Health <= 0;
        public int GetMP => myState.MP;

        public void SetSkill(Skill skill) => myState.Skill = skill;

        public int Attack(AttackType attackType = AttackType.Attack)
        {
            int damage = 0;
            double getDamage;

            getDamage = myState.ATK / 100.0 * 10;
            damage = new Random().Next(myState.ATK - (int)Math.Ceiling(getDamage), myState.ATK + (int)Math.Ceiling(getDamage) + 1);
            if (attackType == AttackType.Skill)
                damage += (int)myState.Skill.ATKRatio;
            if(attackType == AttackType.Attack)
                Console.WriteLine($"Lv.{myState.Level} {myState.Class} 의 공격!");
            else
                Console.WriteLine($"Lv.{myState.Level} {myState.Class} 의 {myState.Skill.Name} 스킬 공격!");

            return damage;
        }
        public void ShowStats()
        {
            // 몬스터가 죽었는지 확인하기 -> 죽어있다면 Dead, 색깔 변경하기
            if (IsDead)
            {
                Utilities.TextColorWithNoNewLine($"Lv.{myState.Level} {myState.Class} Dead", ConsoleColor.DarkGray);
            }
            else
            {
                // 안죽었다면 Level, Class, Hp 출력하기
                Console.Write("Lv.");
                Utilities.TextColorWithNoNewLine($"{myState.Level} ", ConsoleColor.DarkRed);        // 나중에 player.Lv로 수정하기
                Console.Write($"{myState.Class}");         // 나중에 player.Name, player.Job으로 수정하기

                Console.Write(" HP ");
                Utilities.TextColorWithNoNewLine($"{myState.Health}", ConsoleColor.DarkRed);      // 나중에 player.Hp로 수정하기
            }
        }

        public void TakeDamage(int damage)
        {
            int criticalDamage = damage;
            
            int r = new Random().Next(0, 101);

            // 공격 미스. 10%의 확률로 공격이 적중하지 않음
            if(r > 90)
            {
                Console.Write($"Lv.{myState.Level} {myState.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.\n");
                return;
            }

            Console.Write($"Lv.{myState.Level} {myState.Name} 을(를) 맞췄습니다. [데미지 : ");
            // 치명타 공격
            if (r <= 15)
            {
                criticalDamage += (damage * 60 / 100);

                Utilities.TextColorWithNoNewLine($"{criticalDamage}", ConsoleColor.DarkRed);
                Console.Write("]");

                Utilities.TextColorWithNoNewLine(" -", ConsoleColor.Yellow);
                Console.Write(" 치명타 공격");
                Utilities.TextColorWithNoNewLine("!!", ConsoleColor.Yellow);
            }
            else
            {
                Utilities.TextColorWithNoNewLine($"{damage}", ConsoleColor.DarkRed);
                Console.Write("]");
            }

            Console.WriteLine($"\n\nLv.{myState.Level} {myState.Name}");
            Console.Write($"{myState.Health} -> ");

            if (r <= 15)
                myState.Health -= criticalDamage;
            else
                myState.Health -= damage;

            Console.WriteLine($"{(IsDead ? "Dead" : myState.Health)}");
        }

        public bool PrintDead()
        {
            if (IsDead)
            {
                Console.WriteLine("이미 죽은 몬스터입니다.\n 다시 선택해주세요!");
                Console.ReadKey();
            }

            return IsDead;
        }

        public Monster(MonsterType monsterType = MonsterType.Monster1) //몬스터 초기화
        {
            if (monsterType == MonsterType.Monster1)
            {
                myState.Class = "미니언";
                myState.Level = 2;
                myState.EXP = 15;
                myState.Health = 15;
                myState.MP = 100;
                myState.ATK = 5;
                myState.Gold = 100;
                item = "낡은 대검";
            }
            else if (monsterType == MonsterType.Monster2)
            {
                myState.Class = "공허충";
                myState.Level = 3;
                myState.EXP = 20;
                myState.Health = 10;
                myState.MP = 100;
                myState.ATK = 9;
                myState.Gold = 200;
                item = "초보자의 갑옷";
            }
            else if (monsterType == MonsterType.Monster3)
            {
                myState.Class = "대포미니언";
                myState.Level = 5;
                myState.EXP = 25;
                myState.Health = 25;
                myState.MP = 100;
                myState.ATK = 8;
                myState.Gold = 500;
                item = "가시 갑옷";
            }

            myState.Name = myState.Class;
        }
        
    }
}