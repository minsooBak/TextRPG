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

    }

    public class Monster : IListener
    {
        private string Name { get; set; }
        private int Lv { get; set; }
        private int Hp { get; set; }
        private int Atk { get; set; }

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
                Name = "미니언";
                Lv = 3;
                Hp = 10;
                Atk = 9;
            }
            else if (monsterType == MonsterType.Monster3)
            {
                Name = "미니언";
                Lv = 5;
                Hp = 25;
                Atk = 8;
            }
        }

        public void OnEvent(EventType type, object data)
        {
            throw new NotImplementedException();
        }
    }
}
