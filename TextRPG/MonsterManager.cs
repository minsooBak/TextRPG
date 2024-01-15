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
        public List<Monster> dungeonMonsters;

        public MonsterManager()
        {
            EventManager.Instance.AddListener(EventType.eMakeMonsters, this);
            EventManager.Instance.AddListener(EventType.eClearMonsters, this);
            dungeonMonsters = new List<Monster>();
        }
        public void MakeMonsters(int listOfMonsterCount) //몬스터 생성 //스테이지 0 1 2 3 3 3 3 3
        {
            Random rnd = new Random();
            int monsterCount = rnd.Next(1, 5); // 1~ 4 마리 선택
            if ((MonsterType)listOfMonsterCount > MonsterType.Monster3)
                listOfMonsterCount = (int)MonsterType.Monster3; // 등록된 몬스터 수 보다 값이 클 때
            for (int i = 0; i < monsterCount; i++)
            {
                int randomCount = rnd.Next(0, listOfMonsterCount);// 0 1 2 등록되어 있는 몬스터 중 어떤 몬스터를 고를지
                dungeonMonsters.Add(CreateMonster(++randomCount));// 1 2 3 던전 몬스터 리스트에 몬스터 추가
            }

            EventManager.Instance.PostEvent(EventType.eSetMonsters, Utilities.EventPair(EventType.eSetMonsters, dungeonMonsters));
        }
        public Monster CreateMonster(int random) // random에 해당하는 몬스터 생성
        {
            Monster newMonster = new Monster((MonsterType)random);
            return newMonster;
        }
        public int DungeonMonstersCount() //던전에 나온 몬스터 마리수
        {
            return dungeonMonsters.Count;
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
            else if (type == EventType.eClearMonsters)
            {
                ClearMonsterList();
            }
        }
    }

    public class Monster : IObject
    {
        private ObjectState myState;

        public int MP => myState.MP;

        public int Health => myState.Health;

        public int Level => myState.Level;

        public string Class => myState.Class;

        public bool IsUseSkill => myState.Skill.Cost < MP;//사용할 수 있는지 체크후 bool

        public bool IsDead => myState.Health <= 0;
        public void SetSkill(Skill skill) => myState.Skill = skill;
        public int Attack(AttackType attackType = AttackType.Attack)
        {
            int damage = 0;
            double getDamage;

            getDamage = myState.ATK / 100.0 * 10;
            damage = new Random().Next(myState.ATK - (int)Math.Ceiling(getDamage), myState.ATK + (int)Math.Ceiling(getDamage) + 1);
            if (attackType == AttackType.Skill)
                damage *= (int)myState.Skill.ATKRatio;
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
                Utilities.TextColor($"Lv.{myState.Level} {myState.Class} Dead", ConsoleColor.DarkGray);
            }
            else
            {
                // 안죽었다면 Level, Class, Hp 출력하기
                Console.Write("Lv.");
                Utilities.TextColorWithNoNewLine($"{myState.Level} ", ConsoleColor.DarkRed);
                Console.Write($"{myState.Class} HP ");
                Utilities.TextColor($"{myState.Health}", ConsoleColor.DarkRed);
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
                myState.Health = 15;
                myState.ATK = 5;
            }
            else if (monsterType == MonsterType.Monster2)
            {
                myState.Class = "공허충";
                myState.Level = 3;
                myState.Health = 10;
                myState.ATK = 9;     
            }
            else if (monsterType == MonsterType.Monster3)
            {
                myState.Class = "대포미니언";
                myState.Level = 5;
                myState.Health = 25;
                myState.ATK = 8;
            }
        }
        
    }
}