using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    public enum MonsterType
    {
        Monster1 = 1, // 미니언
        Monster2, // 공허충
        Monster3  // 대포 미니언
    }
    //정원우님 구현
    internal class MonsterManager
    {
        private static readonly MonsterManager instance = new MonsterManager();
        private MonsterManager() { }
        static public MonsterManager Instance { get { return instance; }}
        public List<Monster> monsters = new List<Monster>();//가지고 있는 몬스터 배열 
    }

    public class Monster : IListener
    {
        private string Name { get; set; } //몬스터 이름
        private int Lv { get; set; } // 레벨
        private int Hp { get; set; } // 체력
        private int Atk { get; set; } //몬스터 공격력
        public bool isDead; //죽었으면 true

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
            isDead = false;
        }
        public void MakeMonster(int num) // num 만큼 몬스터를 생성한다.
        {
            Random random = new Random(); 
            int rnd;
            for (int i = 0; i < num; i++)
            {
                rnd = random.Next(1,4); // 랜덤 1 2 3
                MonsterManager.Instance.monsters.Add(new Monster((MonsterType)rnd)); // 몬스터 배열에 저장
            }
        }
        public void OnEvent(EventType type, object? data = null) //이벤트 타입으로 메소드를 실행한다.
        {
            if (type == EventType.eMakeMonster)
            {
                MakeMonster((int)data); //몬스터 생성
            }
        }
    }
}
