using System.Threading;

namespace TextRPG
{
    public enum MonsterType
    {
        Monster1 = 1, // 미니언
        Monster2, // 공허충
        Monster3  // 대포 미니언
    }

    public enum MonsterItemType
    {
        monsterItem1 = 0,
        monsterItem2,
        monsterItem3
    }

    //정원우님 구현
    internal class MonsterManager
    {
        public MonsterManager()
        {
            // arrayOfMonsterTypes 배열 생성
            for(int i = 0; i < maxMonsterType; i++)
            {
                arrayOfMonsterTypes[i] = (MonsterType)(i + 1);
            }
        }
        // maxMonsterType : 생성 가능한 몬스터 종류 수
        static int maxMonsterType = 3;
        // arrayOfMonsterTypes : 생성 가능한 몬스터 타입 배열
        public MonsterType[] arrayOfMonsterTypes = new MonsterType[maxMonsterType];

        // dungeonMonsters : 던전매니저에서 사용할 몬스터 리스트
        List<IObject> dungeonMonsters = [];
        
        public List<IObject> MakeMonsters(int listOfMonsterCount) //몬스터 생성

        {
            Random rnd = new Random();
            
            // 스테이지에 따라 생성 가능한 몬스터 마리 수 정하기
            int monsterCount = 0;
            switch (listOfMonsterCount)
            {
                case 1:     // 1 스테이지
                    monsterCount = rnd.Next(1, 3);
                    break;
                case 2:     // 2 스테이지
                    monsterCount = rnd.Next(2, 4);
                    break;
                case 3:     // 3 스테이지 이상
                    monsterCount = rnd.Next(3, 5);
                    break;
            }

            // 던전매니저에서 호출한 몬스터의 타입 값이 전체 몬스터 종류 수(maxMonsterType)보다 클 경우, 값을 maxMonsterType로 고정하기.
            if (listOfMonsterCount > maxMonsterType)
                listOfMonsterCount = maxMonsterType;
            for (int i = 0; i < monsterCount; i++)
            {
                int randomCount = rnd.Next(0, listOfMonsterCount);// 등록되어 있는 몬스터 중 어떤 몬스터를 고를지
                Monster monster = new Monster(arrayOfMonsterTypes[randomCount]);        // 몬스터 클래스에서 arrayOfMonsterTypes에 맞는 Monster 생성
                dungeonMonsters.Add(monster);// 던전 몬스터 리스트에 몬스터 추가
            }

            return dungeonMonsters;
        }

        // 던전 승리 시 경험치 얻기
        public int GetExp()
        {
            int exp = 0;

            foreach (Monster monster in dungeonMonsters)
            {
                // 해당 몬스터 리스트의 총 경험치량 저장
                exp += monster.Exp; //몬스터의 경험치를 더해서 return
            }
            return exp;
        }

        // 던전 승리 시 아이템 얻기
        public void GetReward()
        {
            Random rnd = new Random();
            int[] itemsCounter = new int[3];    // 낡은 대검, 초보자의 갑옷, 가시 갑옷 총 3개의 아이템만 떨어트린다.
            int gold = 0;

            foreach (Monster monster in dungeonMonsters)
            {
                if (rnd.Next(1, 101) <= 8) //8퍼 확률로
                {
                    // 아이템 드랍 이벤트 전송
                    EventManager.Instance.PostEvent(EventType.Item, Utilities.EventPair(eItemType.eGetFieldItem,monster.Item)); 

                    // itemsCounter : 아이템 별 획득한 개수. 
                    // itemsCounter[0] : "낡은 대검"
                    // itemsCounter[1] : "초보자의 갑옷"
                    // itemsCounter[2] : "가시 갑옷"
                    if (monster.Level == 2)
                    {
                        itemsCounter[0]++; // 획득한 아이템 리스트에 추가 
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
            Utilities.TextColorWithNoNewLine($"{gold}", ConsoleColor.DarkRed);      // 플레이어가 얻은 골드 출력
            Console.Write($" Gold");

            for (int i = 0; i < itemsCounter.Length; i++)
            {
                if (itemsCounter[i] > 0)
                {
                    Console.Write($"\n{GetMonsterItemName(i)} ");     // 아이템 이름 가져오기
                    Utilities.TextColorWithNoNewLine("- ", ConsoleColor.DarkYellow);
                    Utilities.TextColorWithNoNewLine($"{itemsCounter[i]}", ConsoleColor.DarkRed);
                }
            }
            Console.WriteLine();
        }

        // 아이템 이름 가져오기
        string GetMonsterItemName(int i)
        {
            switch ((MonsterItemType)i)
            {
                case MonsterItemType.monsterItem1:
                    return "낡은 대검";
                case MonsterItemType.monsterItem2:
                    return "초보자의 갑옷";
                default:
                    return "가시 갑옷";
            }
        }

        public void ClearMonsterList()
        {
            // 던전 종료 후 생성되어 있던 몬스터 리스트 초기화
            dungeonMonsters.Clear();
        }
    }

    public class Monster : IObject
    {
        private ObjectState myState;
        public int Exp => myState.EXP;

        public int MP => myState.MP;
        public int MaxMP {  get; set; }

        public int Health => myState.Health;
        public int MaxHealth { get; set; }

        public int Level => myState.Level;

        public string Class => myState.Class;

        public string Item { get; set; }
        public int Gold => myState.Gold;

        public bool IsUseSkill => myState.Skill != null;//사용할 수 있는지 체크후 bool
        public bool IsDead => myState.Health <= 0;
        public int GetMP => myState.MP;

        public void SetSkill(Skill skill) => myState.Skill = skill;

        // 몬스터가 공격할 경우
        public int Attack(AttackType attackType = AttackType.Attack)
        {
            int damage = 0;
            double getDamage;

            // 몬스터의 공격 데미지 : 공격력의 10% 오차를 가집니다.
            getDamage = myState.ATK / 100.0 * 10;       // 몬스터의 공격 데미지의 10%
            damage = new Random().Next(myState.ATK - (int)Math.Ceiling(getDamage), myState.ATK + (int)Math.Ceiling(getDamage) + 1);     // 오차를 계산해서 몬스터 공격 데미지로 반환
            // 몬스터의 공격 타입이 스킬이라면
            if (attackType == AttackType.Skill)
            {
                if (myState.MP < myState.Skill.Cost)
                    attackType = AttackType.Attack;
                else if (myState.MP >= myState.Skill.Cost)
                {
                    damage *= (int)myState.Skill.ATKRatio;  // 공격 데미지에 스킬의 공격 계수만큼 더해주기     // GetAtk()를 호출해서 데미지 반환하기
                    myState.MP -= myState.Skill.Cost;
                    if (myState.MP <= 0) myState.MP = 0;     // 몬스터의 MP가 0 이하가 될 경우 0으로 변경
                }
            }
            // 공격 출력문
            if (attackType == AttackType.Attack)
                Console.WriteLine($"Lv.{myState.Level} {myState.Class} 의 공격!");     // Attack Type이 Attack일 경우 출력문
            else
                Console.WriteLine($"Lv.{myState.Level} {myState.Class} 의 {myState.Skill.Name} 스킬 공격!");     // Attack Type이 Skill일 경우 출력문

            return damage;
        }

        // 몬스터 상태 출력하기
        public void ShowStats()
        {
            // 몬스터가 죽었는지 확인하기 -> 죽어있다면 Dead, 색깔 변경하기
            if (IsDead)
            {
                // 해당 몬스터가 죽어있다면, 텍스트 색을 DarkGray로 몬스터 상태 출력
                Utilities.TextColorWithNoNewLine($"Lv.{myState.Level} {myState.Class} Dead", ConsoleColor.DarkGray);
            }
            else
            {
                // 안죽었다면 Level, Class, Hp 출력하기
                Console.Write("Lv.");
                Utilities.TextColorWithNoNewLine($"{myState.Level} ", ConsoleColor.DarkRed);
                Console.Write($"{myState.Class}");

                Console.Write(" HP ");
                Utilities.TextColorWithNoNewLine($"{myState.Health}", ConsoleColor.DarkRed);

                Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
                Utilities.TextColorWithNoNewLine($"{MaxHealth} ", ConsoleColor.DarkRed);

                Console.Write("MP ");
                Utilities.TextColorWithNoNewLine($"{myState.MP}", ConsoleColor.Blue);
                Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
                Utilities.TextColorWithNoNewLine($"{MaxMP}", ConsoleColor.Blue);
            }
        }

        // 몬스터가 피해를 입을 경우
        public void TakeDamage(int damage)
        {
            int criticalDamage = damage;        // criticalDamage : 치명타 데미지
            
            int r = new Random().Next(0, 101);

            // 공격 미스. 10%의 확률로 공격이 적중하지 않음
            if(r > 90)
            {
                Console.Write($"Lv.{myState.Level} {myState.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.\n");
                return;
            }

            Console.Write($"Lv.{myState.Level} {myState.Name} 을(를) 맞췄습니다. [데미지 : ");
            // 15% 확률로 치명타 공격
            if (r <= 15)
            {
                criticalDamage += (damage * 60 / 100);      // 공격력의 160% 데미지

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

            // 몬스터 공격 결과 출력
            Console.WriteLine($"\n\nLv.{myState.Level} {myState.Name}");
            Console.Write($"{myState.Health} -> ");

            // 몬스터의 체력 차감
            if (r <= 15)
                myState.Health -= criticalDamage;
            else
                myState.Health -= damage;

            // 해당 몬스터가 죽었을 경우 Dead로 출력, 아니면 HP를 출력
            Console.WriteLine($"{(IsDead ? "Dead" : myState.Health)}");
        }

        // 몬스터 클래스
        public Monster(MonsterType monsterType = MonsterType.Monster1) //몬스터 초기화
        {
            if (monsterType == MonsterType.Monster1)
            {
                myState.Class = "미니언";
                myState.Level = 2;
                myState.EXP = 15;
                myState.Health = 15;
                MaxHealth = myState.Health;
                myState.MP = 100;
                MaxMP = myState.MP;
                myState.ATK = 5;
                myState.Gold = 100;
                Item = "낡은 대검";
            }
            else if (monsterType == MonsterType.Monster2)
            {
                myState.Class = "공허충";
                myState.Level = 3;
                myState.EXP = 20;
                myState.Health = 10;
                MaxHealth = myState.Health;
                myState.MP = 100;
                MaxMP = myState.MP;
                myState.ATK = 9;
                myState.Gold = 200;
                Item = "초보자의 갑옷";
            }
            else if (monsterType == MonsterType.Monster3)
            {
                myState.Class = "대포미니언";
                myState.Level = 5;
                myState.EXP = 25;
                myState.Health = 25;
                MaxHealth = myState.Health;
                myState.MP = 100;
                MaxMP = myState.MP;
                myState.ATK = 8;
                myState.Gold = 500;
                Item = "가시 갑옷";
            }

            myState.Name = myState.Class;
        }
        
    }
}