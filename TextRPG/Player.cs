using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG;

namespace TextRPG
{
    internal class Player : IListener
    {
        private ObjectState state;
        private int level = 1;
        private int itemATK;
        private int itemDEF;
        public int ATK { get { return state.ATK; } }
        public int DEF { get { return state.DEF; } }
        public int Gold
        {
            get { return state.Gold; }
            private set
            {
                state.Gold = Math.Clamp(value, 0, 999999999);
            }
        }
        public int Health
        {
            get { return state.Health; }
            private set
            {
                state.Health = Math.Clamp(value, 0, 100);
            }
        }

        public Player()
        {
            EventManager.Instance.AddListener(EventType.eGameInit, this);
            EventManager.Instance.AddListener(EventType.eGameEnd, this);
            EventManager.Instance.AddListener(EventType.eHealthChage, this);
            EventManager.Instance.AddListener(EventType.eGoldChage, this);
            EventManager.Instance.AddListener(EventType.eItemChage, this);
            EventManager.Instance.AddListener(EventType.eExpChage, this);
        }

        public void Init(string name, string _class, int? _gold = null, int? _health = null, int? _exp = null)
        {
            if (_health > 100)
                _health = 100;

            state.Name = name;
            state.Class = _class;
            state.Gold = _gold ?? 1500;
            state.Health = _health ?? 100;
            state.EXP = _exp ?? 100;

            switch (_class)
            {
                case "전사":
                    {
                        state.InitATK = 1;
                        state.InitDEF = 2;
                        break;
                    }
                case "마법사":
                    {
                        state.InitATK = 3;
                        state.InitDEF = 0;
                        break;
                    }
                case "궁수":
                    {
                        state.InitATK = 2;
                        state.InitDEF = 1;
                        break;
                    }
                case "도적":
                    {
                        state.InitATK = 3;
                        state.InitDEF = 0;
                        break;
                    }
            }
            LevelCheck();
            state.ATK = ATK > 1 ? ATK + (int)state.InitATK : (int)state.InitATK;
            state.DEF = DEF > 1 ? DEF + (int)state.InitDEF : (int)state.InitDEF;
        }

        public void LevelCheck()
        {
            int result = state.Level - level;
            if (level != state.Level)
            {
                state.InitATK += 0.5f * result;
                state.InitDEF += 1 * result;
                level = state.Level;
            }
            else
                level = state.Level;

        }

        public void ShowStat()
        {
            Console.Write($"Lv : {state.Level}");
            Console.WriteLine($"\tEXP : {state.EXP % 100}");
            Console.WriteLine($"{state.Name}  ( {state.Class} )");
            Console.WriteLine($"공격력 : {ATK}");
            Console.WriteLine($"방어력 : {DEF}");
            Console.WriteLine($"체력 : {Health}");
            Console.WriteLine($"Gold : {Gold}G");
        }

        void IListener.OnEvent(EventType type, object data)
        {
            switch (type)
            {
                case EventType.eHealthChage:
                    {
                        Health += (int)data;
                        break;
                    }
                case EventType.eGameInit:
                    {
                        ObjectState? stateLode = (ObjectState?)Utilities.LoadFile(LoadType.Player);
                        if (stateLode != null)
                            Init(stateLode.Value.Name, stateLode.Value.Class, stateLode.Value.Gold, stateLode.Value.Health, stateLode.Value.EXP);

                        break;
                    }
                case EventType.eGameEnd:
                    {
                        ObjectState playerState = new ObjectState
                        {
                            Name = state.Name,
                            Class = state.Class,
                            Gold = Gold,
                            Health = Health,
                            EXP = state.EXP,

                        };
                        Utilities.SaveFile(SaveType.Player, playerState);
                        break;
                    }
                case EventType.eGoldChage:
                    {
                        Gold += (int)data;
                        break;
                    }
                case EventType.eItemChage:
                    {
                        List<Item> items = [];
                        items = (List<Item>)data;
                        if (items != null)
                        {
                            itemATK = 0;
                            itemDEF = 0;
                            foreach (Item item in items)
                            {
                                itemATK += item.Power;
                                itemDEF += item.Armor;
                            }
                        }
                        state.ATK = (int)state.InitATK + itemATK;
                        state.DEF = state.InitDEF + itemDEF;

                        break;
                    }
                case EventType.eExpChage:
                    {
                        LevelCheck();
                        state.EXP += (int)data;
                        LevelCheck();
                        EventManager.Instance.PostEvent(EventType.eItemChage);

                        break;
                    }
                default:
                    {
                        Console.WriteLine("Player OnEvent Type Error!");
                        break;
                    }
            }
        }
    }
}
