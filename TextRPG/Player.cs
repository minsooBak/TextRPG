namespace TextRPG
{
    //유시아님 플레이어 구현 매소드 : 스탯출력, 공격
    internal class Player : IListener, IObject
    {
        private ObjectState myState;
        private int InitATK { get; set; }
        private int InitDEF { get; set; }
        private int maxHP;
        private int maxMP;
        private int PrevHP { get; set; } // 이전 hp값
        private int PrevMp { get; set; }


        public Player() // 플레이어 기본 정보 설정
        {
            EventManager.Instance.AddListener(EventType.Player, this); // 이벤트 매니저에 열거형으로 저장된 변수로
            //CreatePlayer
            CreatePlayer createPlayer = new CreatePlayer();
            var Name = createPlayer.Create();

            myState.Name = Name.Key;
            myState.Class = Name.Value;
<<<<<<< Updated upstream

            myState.HP = 100;
=======
            // 기본 스탯 설정, 이후 직업에 따라 +-값으로 스탯 조절
            myState.Health = 100; 
>>>>>>> Stashed changes
            myState.MP = 100;
            myState.Level = 1;
            myState.EXP = 0;
            myState.ATK = 100;
            myState.DEF = 0;
            myState.Gold = 0;
            InitATK = myState.ATK;
            InitDEF = myState.DEF;

            maxHP = myState.HP;
            maxMP = myState.MP;
<<<<<<< Updated upstream
        }
=======
            PrevExp = myState.EXP;

            switch (Name.Value)
            { // 직업: 전사, 마법사, 궁수, 도적
                case "전사": // 전사 : 체력 +400, 마나 그대로, 공격력+200, 방어력+100
                    { 
                    myState.Health += 400;
                    myState.MP = 100;
                    myState.ATK += 200;
                    myState.DEF += 100;
                    break;
                    }
                case "마법사": // 마법사 : 체력 +200, 마나+500, 공격력+150, 방어력+50
                    {
                        myState.Health += 100;
                        myState.MP += 400;
                        myState.ATK += 150;
                        myState.DEF += 50;
                        break;
                    }
                case "궁수": // 궁수 : 체력 +150, 마나+100, 공격력+300, 방어력+80
                    {
                        myState.Health += 150;
                        myState.MP += 200;
                        myState.ATK += 300;
                        myState.DEF += 80;
                        break;
                    }
                case "도적": // 도적 : 체력 + 350, 마나 + 200, 공격력 + 350, 방어력 + 60
                    {
                        myState.Health += 250;
                        myState.MP += 200;
                        myState.ATK += 350;
                        myState.DEF += 60;
                        break;
                    }

            }
        }
    

>>>>>>> Stashed changes

        public Player(ObjectState state)
        {
            myState.Name = state.Name;
            myState.Class = state.Class;

            myState.HP = state.HP;
            myState.MP = state.MP;
            myState.Level = state.Level;
            myState.ATK = state.ATK; // 기존 공격력 + 추가 공격력
            myState.DEF = state.DEF;
            myState.Gold = state.Gold;
            InitATK = state.ATK;
            InitDEF = state.DEF;
        }

<<<<<<< Updated upstream
        public int HP => myState.HP;
=======
            
        }             

        //람다 사용, 플레이어의 행동에 따라 변화하는 스탯값 변화 저장?
        public int Health => myState.Health;
>>>>>>> Stashed changes
        public int MP => myState.MP;
        public int Level => myState.Level;
        public string Name => myState.Name;
        public string Class => myState.Class;
        public int ATK => myState.ATK;
        public int DEF => myState.DEF;
        public int Gold => myState.Gold;
        public bool IsDead => myState.HP <= 0;
        public bool IsUseSkill => myState.Skill.Cost <= myState.MP;
        public void SetSkill(Skill skill) => myState.Skill = skill;

        public void OnEvent(EventType type, object data)
        {
            //이벤트 받아서 switch문으로 구현
            if (type == EventType.Player)
            {

                var a = (KeyValuePair<ePlayerType, int>)data;

                switch (a.Key)
                {
<<<<<<< Updated upstream
                    case ePlayerType.HP:
                        {
                            myState.HP = Math.Clamp(myState.HP + (int)a.Value, 0, maxHP);
                            break;
                        }
                    case ePlayerType.MP:
                        {
                            myState.MP = Math.Clamp((int)a.Value, 0, maxMP);
                            break;
                        }
                    case ePlayerType.Gold:
                        {
                            myState.Gold = Math.Clamp((int)a.Value, 0, 100);
                            break;
                        }
                    case ePlayerType.ATK:
                        {
                            myState.ATK = a.Value;
                            break;
                        }
                    case ePlayerType.DEF:
                        {
                            myState.DEF = a.Value;
                            break;
                        }
=======
                    var c = a.Value;
                    switch (c.Key)
                    {
                        case ePlayerType.Gold: //골드 추가 // ??? 
                            {
                                myState.Gold += Math.Clamp(myState.Gold + c.Value, 0, 999999999);
                                break;
                            }
                        case ePlayerType.Exp: //경험치 추가
                            {
                                PrevExp = myState.EXP;      // 이전 경험치 저장
                                myState.EXP += Math.Clamp(c.Value, 0, 300);
                                int LevelUp = 0;
                                if (myState.EXP / 100 != 0) // 경험치 변화 후 레벨링 함수
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
>>>>>>> Stashed changes
                }
            }
        }

        public int ShowHealth()
        {
            return myState.HP;
        }

        public void ShowStats()
        {
            Console.WriteLine("\n[내 정보]");
            Console.Write("Lv.");
            Utilities.TextColorWithNoNewLine($"{myState.Level} ", ConsoleColor.DarkRed);
            Console.WriteLine($"{myState.Name} ({myState.Class})");

            Console.Write("HP ");
            Utilities.TextColorWithNoNewLine($"{myState.HP}", ConsoleColor.DarkRed);
            Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
            Utilities.TextColorWithNoNewLine($"{maxHP}\n", ConsoleColor.DarkRed);

            Console.Write("MP ");
            Utilities.TextColorWithNoNewLine($"{myState.MP}", ConsoleColor.DarkRed);
            Utilities.TextColorWithNoNewLine("/", ConsoleColor.DarkYellow);
            Utilities.TextColorWithNoNewLine($"{maxMP}\n\n", ConsoleColor.DarkRed);
        }

        public int Attack(AttackType attackType) // 전투 중 공격 부분
        {
            if (PrevHP == 0)
            {
                PrevHP = myState.HP;
                PrevMp = myState.MP;
            }

            int damage = 0;
            double getDamage;
            getDamage = myState.ATK / 100.0 * 10; // 데미지 함수
            damage = new Random().Next(myState.ATK - (int)Math.Ceiling(getDamage), myState.ATK + (int)Math.Ceiling(getDamage) + 1); // 일반공격시
            if (attackType == AttackType.Skill) // 스킬 사용시
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

        public void TakeDamage(int damage) // 전투중 데미지 받았을 때 표시
        {
            Console.WriteLine($"{myState.Name} 을(를) 맞췄습니다. [데미지 : {damage}]\n");

            Console.WriteLine($"Lv.{myState.Level} {myState.Name}");
<<<<<<< Updated upstream
            Console.Write($"{myState.HP} ->");
            myState.HP = Math.Clamp(myState.HP - damage, 0, 100);
            Console.Write($"{myState.HP}");
=======

            Console.Write($"{myState.Health} ->");
            //myState.Health -= Math.Clamp(damage, 0 , 100);
            myState.Health -= damage; // 플레이어의 턴이 끝난 후 몬스터의 공격 시작, 맞았을 때 데미지를 받은 만큼 hp 감소
            if( myState.Health <= 0 ) myState.Health = 0;       // 플레이어의 체력이 0 이하가 되면 0으로 변경
            Console.Write($" {myState.Health}\n");
>>>>>>> Stashed changes
        }

        public void ShowResult() // 몬스터를 처치해서 받은 레벨, 소모한 HP나 MP 등 전투 결과 표시  
        {
            Console.WriteLine($"Lv.{myState.Level} {myState.Name}");

            Console.WriteLine($"HP {PrevHP} -> {myState.HP}");
            Console.WriteLine($"MP {PrevMp} -> {myState.MP}\n");
            PrevHP = 0;
            PrevMp = 0;

<<<<<<< Updated upstream
            //죽엇을때 부활 체력 마나
            if (IsDead)
=======
            if (IsDead) // 전투 중 사망했을 때 HP와 MP값의 변화
>>>>>>> Stashed changes
            {
                myState.HP = 60;
                myState.MP = 60;
            }
<<<<<<< Updated upstream
=======
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

                // 던전 클리어 시 입장 가능한 스테이지 증가 -상화님
                if (dungeonStage >= 3)
                    dungeonStage = 3;   // 4층 이상 진입하지 못하도록 제어.
                else dungeonStage++;

                PrevHealth = 0;
                PrevMp = 0;
            }  
>>>>>>> Stashed changes
        }
    }
}