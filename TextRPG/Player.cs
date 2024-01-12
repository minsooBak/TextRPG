namespace TextRPG
{
    //유시아님 플레이어 구현 매소드 : 스탯출력, 공격
    internal class Player : IListener, IObject
    {
        private ObjectState myState;
        private int InitATK { get; set; }
        private int InitDEF { get; set; }
        private int maxHealth;
        private int maxMP;
        private int PrevHealth { get; set; } // 이전 hp값
        private int PrevMp { get; set; }


        public Player()
        {
            EventManager.Instance.AddListener(EventType.Player, this);
            //CreatePlayer
            CreatePlayer createPlayer = new CreatePlayer();
            var Name = createPlayer.Create();

            myState.Name = Name.Key;
            myState.Class = Name.Value;

            myState.Health = 100;
            myState.MP = 100;
            myState.Level = 1;
            myState.EXP = 0;
            myState.ATK = 100;
            myState.DEF = 0;
            myState.Gold = 0;
            InitATK = myState.ATK;
            InitDEF = myState.DEF;

            maxHealth = myState.Health;
            maxMP = myState.MP;
        }

        public Player(ObjectState state)
        {
            myState.Name = state.Name;
            myState.Class = state.Class;

            myState.Health = state.Health;
            myState.MP = state.MP;
            myState.Level = state.Level;
            myState.ATK = state.ATK; // 기존 공격력 + 추가 공격력
            myState.DEF = state.DEF;
            myState.Gold = state.Gold;
            InitATK = state.ATK;
            InitDEF = state.DEF;
        }

        public int Health => myState.Health;
        public int MP => myState.MP;
        public int Level => myState.Level;
        public string Name => myState.Name;
        public string Class => myState.Class;
        public int ATK => myState.ATK;
        public int DEF => myState.DEF;
        public int Gold => myState.Gold;
        public bool IsDead => myState.Health <= 0;
        public bool IsUseSkill => myState.Skill.Cost <= myState.MP;
        public void SetSkill(Skill skill) => myState.Skill = skill;

        public void OnEvent(EventType type, object data)
        {
            //이벤트 받아서 switch문으로 구현
            if(type == EventType.Player)
            {
                
                var a = (KeyValuePair<ePlayerType, object>)data;

                switch(a.Key)
                {
                    case ePlayerType.HP:
                        {
                            myState.Health = Math.Clamp(myState.Health + (int)a.Value, 0, maxHealth);
                            break;
                        }
                    case ePlayerType.MP:
                        {
                            myState.Health = Math.Clamp((int)a.Value, 0, maxMP);
                            break;
                        }
                    case ePlayerType.Gold:
                        {
                            myState.Gold = Math.Clamp((int)a.Value, 0, 100);
                            break;
                        }
                    case ePlayerType.Stats:
                        {
                            var num = (int[])a.Value;
                            myState.ATK = num[0];
                            myState.DEF = num[1];
                            break;
                        }
                }
            }
        }

        public int ShowHealth()
        {
            return myState.Health;
        }

        public void ShowStats()
        {
            Console.WriteLine("\n[내 정보]");
            Console.Write("Lv.");
            Utilities.TextColorWithNoNewLine($"{myState.Level} ", ConsoleColor.DarkRed);        // 나중에 player.Lv로 수정하기
            Console.WriteLine($"{myState.Name} ({myState.Class})");         // 나중에 player.Name, player.Job으로 수정하기

            Console.Write("HP ");
            Utilities.TextColorWithNoNewLine($"{myState.Health}", ConsoleColor.DarkRed);      // 나중에 player.Hp로 수정하기
            Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
            Utilities.TextColorWithNoNewLine($"{maxHealth}\n", ConsoleColor.DarkRed);

            Console.Write("MP ");
            Utilities.TextColorWithNoNewLine($"{myState.MP}", ConsoleColor.DarkRed);      // 나중에 player.Mp로 수정하기
            Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
            Utilities.TextColorWithNoNewLine($"{maxMP}\n\n", ConsoleColor.DarkRed);
        }

        public int Attack(AttackType attackType)
        {
            if (PrevHealth == 0)
            {
                PrevHealth = myState.Health;
                PrevMp = myState.MP;
            }

            int damage = 0;
            double getDamage;
            getDamage = myState.ATK / 100.0 * 10;
            damage = new Random().Next(myState.ATK - (int)Math.Ceiling(getDamage), myState.ATK + (int)Math.Ceiling(getDamage) + 1);
            if (attackType == AttackType.Skill)
            {
                damage = myState.Skill.GetATK(damage);
                myState.MP -= myState.Skill.Cost;
            }

            if (attackType == AttackType.Attack)
                Console.WriteLine("Chad 의 공격!");
            else
                Console.WriteLine($"Chad 의 {myState.Skill.Name} 스킬 공격!");
            return damage;
        }

        public void TakeDamage(int damage)
        {
            Console.WriteLine($"Chad 을(를) 맞췄습니다. [데미지 : {damage}]\n");

            Console.WriteLine($"Lv.1 Chad");
            Console.Write($"{myState.Health} ->");
            myState.Health -= Math.Clamp(damage, 0 , 100);
            Console.Write($"{myState.Health}");
        }

        public void ShowResult()
        {
            Console.WriteLine($"Lv.1 Chad\nHP {PrevHealth} -> {myState.Health}");
            Console.WriteLine($"Lv.1 Chad\nMP {PrevMp} -> {myState.MP}\n");
            PrevHealth = 0;
            PrevMp = 0;
        }
    }
}