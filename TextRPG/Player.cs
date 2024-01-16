
using System.Numerics;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace TextRPG
{
    struct PlayerData
    {
        ObjectState os;
        //변수값
        //스테이지
    }

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
        private int PrevExp;

        private string PlayerClass { get; set; } // 플레이어 직업

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
            myState.Gold = 1000;
            switch (Name.Value)
            {
                case "전사":
                   { 
                    myState.Health += 400;
                    myState.MP = 100;
                    myState.ATK += 200;
                    myState.DEF += 100;
                    break;
                    }
                case "마법사":
                    {
                        myState.Health += 100;
                        myState.MP += 400;
                        myState.ATK += 150;
                        myState.DEF += 50;
                        break;
                    }
                case "궁수":
                    {
                        myState.Health += 150;
                        myState.MP += 200;
                        myState.ATK += 300;
                        myState.DEF += 80;
                        break;
                    }
                case "도적":
                    {
                        myState.Health += 250;
                        myState.MP += 200;
                        myState.ATK += 350;
                        myState.DEF += 60;
                        break;
                    }
            }
            InitATK = myState.ATK;
            InitDEF = myState.DEF;
            maxHealth = myState.Health;
            maxMP = myState.MP;
            PrevExp = myState.EXP;
        }
        /*직업 : Warrior, Mage, Archer, Thief
         * 기본 체력 100,마나 100, 공격력 100, 방어력 0 
         * 전사 : 체력 +400, 마나 그대로, 공격력+200, 방어력+100
         * 마법사 : 체력 +200, 마나+500, 공격력+150, 방어력+50
         * 궁수 : 체력 +150, 마나+100, 공격력+300, 방어력+80
         * 도적 : 체력 +350, 마나+200, 공격력+350, 방어력+60
        
        */


        public Player(ObjectState state)
        {
            myState.Level = state.Level;

            myState.Name = state.Name;
            myState.Class = state.Class;
            
            myState.Health = Health;
            myState.MP = state.MP;
            myState.ATK = state.ATK; // 기존 공격력 + 추가 공격력
            myState.DEF = state.DEF;
        
            myState.Gold = state.Gold;
            InitATK = state.ATK;
            InitDEF = state.DEF;
        }             

        public int Health => myState.Health;
        public int MP => myState.MP;
        public int Level => myState.Level;
        public int EXP => myState.EXP;
        public string Name => myState.Name;
        public string Class => myState.Class;
        public int ATK => myState.ATK;
        public int DEF => myState.DEF; 
        public int Gold => myState.Gold;

        public bool IsDead => myState.Health <= 0;
        public bool IsUseSkill => myState.Skill.Cost <= myState.MP;
        public void SetSkill(Skill skill) => myState.Skill = skill;

        public int dungeonStage = 0;    // 플레이어가 입장 가능한 던전 스테이지

        
        public void OnEvent<T>(EventType type, T data)
        {
            //이벤트 받아서 switch문으로 구현
            if(type == EventType.Player)
            {
                //as : data에 KeyValuePair로 형변환이 가능하다면 KeyValuePair로 넣어주고 아닐경우 null값을 넣어줌
                //is : data에 KeyValuePair로 형변환 가능 여부를 리턴해줌
                var a = data as KeyValuePair<ePlayerType, int>?;
                var b = data as KeyValuePair<ePlayerType, Item>?;
                var d = data as KeyValuePair<ePlayerType, string>?;

                if (a != null)
                {
                    var c = a.Value;
                    switch (c.Key)
                    {
                        case ePlayerType.Gold: //골드 추가
                            {
                                myState.Gold = Math.Clamp(myState.Gold + c.Value, 0, 999999999);
                                break;
                            }
                        case ePlayerType.Exp: //경험치 추가
                            {
                                PrevExp = myState.EXP;      // 이전 경험치 저장
                                myState.EXP += Math.Clamp(c.Value, 0, 300);
                                int LevelUp = 0;
                                if (myState.EXP / 100 != 0)
                                {
                                    LevelUp = myState.EXP / 100;
                                    myState.Level += LevelUp;
                                    myState.EXP = myState.EXP % 100;
                                    Console.WriteLine("레벨 업!!\n");
                                    EventManager.Instance.PostEvent(EventType.Quest, Utilities.EventPair(eQuestType.PlayerLevel, LevelUp.ToString()));
                                }
                                break;
                            }
                    }
                }
                else if (b != null)
                {
                    var c = b.Value;
                    if (c.Key == ePlayerType.Stats)
                    {
                        myState.ATK += c.Value.ATK;
                        myState.DEF += c.Value.DEF;
                    }
                }
            }
        }

        public void ShowStats()
        {
            Console.WriteLine("\n[내 정보]");
            Console.Write("Lv.");
            Utilities.TextColorWithNoNewLine($"{myState.Level} ", ConsoleColor.DarkRed);
            Console.WriteLine($"{myState.Name} ({myState.Class})");

            Console.Write("HP ");

            Utilities.TextColorWithNoNewLine($"{myState.Health}", ConsoleColor.DarkRed);
            Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
            Utilities.TextColorWithNoNewLine($"{maxHealth}\n", ConsoleColor.DarkRed);

            Console.Write("MP ");
            Utilities.TextColorWithNoNewLine($"{myState.MP}", ConsoleColor.DarkRed);
            Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
            Utilities.TextColorWithNoNewLine($"{maxMP}\n\n", ConsoleColor.DarkRed);

            Console.Write("공격력: ");
            Utilities.TextColorWithNoNewLine($"{myState.ATK}\n", ConsoleColor.DarkRed);

            Console.Write("방어력: ");
            Utilities.TextColorWithNoNewLine($"{myState.DEF}\n", ConsoleColor.DarkRed);

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
                myState.MP = Math.Clamp(myState.MP - myState.Skill.Cost, 0, maxMP);
            }

            if (attackType == AttackType.Attack)
                Console.WriteLine($"{myState.Name} 의 공격!");
            else
                Console.WriteLine($"{myState.Name} 의 {myState.Skill.Name} 스킬 공격!");
            return damage;
        }

        public void TakeDamage(int damage)
        {
            Console.WriteLine($"{myState.Name} 을(를) 맞췄습니다. [데미지 : {damage}]\n");

            Console.WriteLine($"Lv.{myState.Level} {myState.Name}");

            Console.Write($"{myState.Health} ->");
            //myState.Health -= Math.Clamp(damage, 0 , 100);
            myState.Health -= damage;
            if( myState.Health <= 0 ) myState.Health = 0;       // 플레이어의 체력이 0 이하가 되면 0으로 변경
            Console.Write($" {myState.Health}\n");
        }

        public void ShowResult()
        {
            //PrevExp = myState.EXP;
            //myState.EXP += exp;

            Console.WriteLine("[캐릭터 정보]");
            Console.Write($"Lv.");
            Utilities.TextColorWithNoNewLine($"{myState.Level} ", ConsoleColor.DarkRed);
            Console.Write($"{myState.Name}\nHP ");
            Utilities.TextColorWithNoNewLine($"{PrevHealth} ", ConsoleColor.DarkRed);
            Utilities.TextColorWithNoNewLine("-> ", ConsoleColor.DarkYellow);
            Utilities.TextColor($"{myState.Health}", ConsoleColor.DarkRed);

            if (IsDead)
            {
                myState.Health = 60;
                myState.MP = 60;
            }
            else
            {
                Console.Write($"MP ");
                Utilities.TextColorWithNoNewLine($"{PrevMp} ", ConsoleColor.DarkRed);
                Utilities.TextColorWithNoNewLine("-> ", ConsoleColor.DarkYellow);
                Utilities.TextColor($"{myState.MP}", ConsoleColor.DarkRed);

                Console.Write($"EXP ");
                Utilities.TextColorWithNoNewLine($"{PrevExp} ", ConsoleColor.DarkRed);
                Utilities.TextColorWithNoNewLine("-> ", ConsoleColor.DarkYellow);
                Utilities.TextColor($"{myState.EXP}", ConsoleColor.DarkRed);

                //if (myState.EXP / 100 != 0)  //경험치가 100을 넘는다면
                //{
                //    myState.Level += (myState.EXP / 100); //레벨 올리고
                //    myState.EXP = myState.EXP % 100; //남은 경험치를 현재 경험치로 설정
                //}

                // 던전 클리어 시 입장 가능한 스테이지 증가
                if (dungeonStage >= 3)
                    dungeonStage = 3;   // 4층 이상 진입하지 못하도록 제어.
                else dungeonStage++;

                PrevHealth = 0;
                PrevMp = 0;
            }  
        }
    }
}