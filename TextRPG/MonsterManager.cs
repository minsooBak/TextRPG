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
    internal class MonsterManager : IListener
    {
        //private static readonly MonsterManager instance = new MonsterManager();
        //static public MonsterManager Instance { get { return instance; } }
        public List<Monster> listOfMonsters;//몬스터 데이터 목록 리스트
        public List<Monster> dungeonMonsters;
        public int listOfMonsterCount;

        public MonsterManager()
        {
            EventManager.Instance.AddListener(EventType.eMakeMonsters, this);
            listOfMonsters = new List<Monster>();
            dungeonMonsters = new List<Monster>();

            listOfMonsterCount = 3;
            for (int i = 0; i < listOfMonsterCount; i++)
            {
                listOfMonsters.Add(new Monster((MonsterType)(i + 1))); // 몬스터 목록 데이터 저장

                //확인용
                //Console.WriteLine("몬스터 목록 등록");
                //Console.WriteLine($"{listOfMonsters[i].Name}");
                //Console.WriteLine($"현재 등록개수 :{listOfMonsters.Count}");
            }
        }
        public void MakeMonsters() //몬스터 생성
        {
            Random rnd = new Random();
            int monsterCount = rnd.Next(1, 5); // 1~ 4 마리 선택
            for (int i = 0; i < monsterCount; i++)
            {
                int randomCount = rnd.Next(0, listOfMonsters.Count); //등록되어 있는 몬스터 중 어떤 몬스터를 고를지
                dungeonMonsters.Add(CreateMonster(++randomCount));// 던전 몬스터 리스트에 몬스터 추가
            }
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

        public void OnEvent(EventType type, object data)
        {
            if (type == EventType.eMakeMonsters)
            {
                MakeMonsters();
            }
        }
    }

    public class Monster : IListener, IAttack, ITakeDamage
    {
        public string Name { get; private set; } //몬스터 이름
        public int Lv { get; private set; } // 레벨
        public int Hp { get; private set; } // 체력
        public int Atk { get; private set; } //몬스터 공격력
        public bool isDead; //죽었으면 true

        public int Attack()
        {
            int damage = 0;
            double getDamage;

            getDamage = this.Atk / 100.0 * 10;
            damage = new Random().Next(this.Atk - (int)Math.Ceiling(getDamage), this.Atk + (int)Math.Ceiling(getDamage) + 1);

            Console.WriteLine($"Lv.{this.Lv} {this.Name} 의 공격!");

            return damage;
        }

        public void TakeDamage(int damage)
        {
            this.Hp -= damage;
            if (this.Hp <= 0) this.isDead = true;
        }

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

        public void OnEvent(EventType type, object? data = null) //이벤트 타입으로 메소드를 실행한다.
        {
            //Console.WriteLine();
        }
    }
}
