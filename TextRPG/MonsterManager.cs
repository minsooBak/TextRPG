using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    public enum MonsterType
    {
        Monster1, // 미니언
        Monster2, // 공허충
        Monster3  // 대포 미니언
    }
    //정원우님 구현
    internal class MonsterManager
    {
        //private static MonsterManager instance = new MonsterManager();

        //public static MonsterManager Instance { get { return instance; } }
        private Monster[] monsters;

        public MonsterManager(MonsterType monsterType)
        {
            
        }

        private void GetMonster(int monsterNumber)
        {
            monsters = new Monster[monsterNumber];
        }
    }

    public class Monster : IListener
    {
        public string Name { get; private set; }
        public int Lv { get; private set; }
        public int Hp { get; private set; }
        public int Atk { get; private set; }

        public Monster(MonsterType monsterType = MonsterType.Monster1) //몬스터 초기화
        {
            if (monsterType == MonsterType.Monster1)
            {
                Name = "미니언";
                Lv = 2;
                Hp = 15;
                Atk = 5;
            }
            else if (monsterType == MonsterType.Monster2)
            {
                Name = "공허충";
                Lv = 3;
                Hp = 10;
                Atk = 9;
            }
            else if (monsterType == MonsterType.Monster3)
            {
                Name = "대포미니언";
                Lv = 5;
                Hp = 25;
                Atk = 8;
            }
        }

        public void OnEvent(EventType type, object data)
        {
            
        }
    }
}
