using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TextRPG
{
    internal class Player : IListener
    {
        private static Character player;
        static void GameDataSetting()
        {
            player = new Character("Chad", "전사", 1, 10, 5, 100, 1500);
        }
        public void OnEvent(EventType type, object data)
        {
            //이벤트 받아서 switch문으로 구현
            
        }
        public class Character
        {
            public string Name { get; }
            public string Job { get; }
            public int Level { get; }
            public int Atk { get; }
            public int Def { get; }
            public int Hp { get; }
            public int Gold { get; }

            public Character(string name, string job, int level, int atk, int def, int hp, int gold)
            {
                Name = name;
                Job = job;
                Level = level;
                Atk = atk;
                Def = def;
                Hp = hp;
                Gold = gold;
            }
        }
    }
}
