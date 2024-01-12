using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public List<Monster> dungeonMonsters;

        public MonsterManager()
        {
            EventManager.Instance.AddListener(EventType.eMakeMonsters, this);
            EventManager.Instance.AddListener(EventType.eClearMonsters, this);
            dungeonMonsters = new List<Monster>();
        }
        public void MakeMonsters(int listOfMonsterCount) //몬스터 생성 //스테이지 1 2 3 4
        {
            Random rnd = new Random();
            int monsterCount = rnd.Next(1, 5); // 1~ 4 마리 선택
            if ((MonsterType)listOfMonsterCount > MonsterType.Monster3)
                listOfMonsterCount = (int)MonsterType.Monster3; //1 2 3 
            for (int i = 0; i < monsterCount; i++)
            {
                int randomCount = rnd.Next(0, listOfMonsterCount);// 0 1 2 등록되어 있는 몬스터 중 어떤 몬스터를 고를지
                dungeonMonsters.Add(CreateMonster(++randomCount));// 1 2 3 던전 몬스터 리스트에 몬스터 추가
            }

            EventManager.Instance.PostEvent(EventType.eSetMonsters, dungeonMonsters);
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

        public void OnEvent(EventType type, object data)
        {
            if (type == EventType.eMakeMonsters)
            {
                MakeMonsters((int)data);
            }
            else if(type == EventType.eClearMonsters)
            {
                ClearMonsterList();
            }
        }
    }

    public class Monster : IListener, IObject
    {
        
        public Skill Skill { get; set; }
        public string Name { get; private set; } //몬스터 이름
        public int Lv { get; private set; } // 레벨
        public int Hp { get; private set; } // 체력
        public int Atk { get; private set; } //몬스터 공격력
        public bool isDead; //죽었으면 true

        public void SetSkill(Skill skill)
        {
            this.Skill = skill;
        }
        public int Attack(AttackType attackType = AttackType.Attack)
        {
            int damage = 0;
            double getDamage;

            getDamage = this.Atk / 100.0 * 10;
            damage = new Random().Next(this.Atk - (int)Math.Ceiling(getDamage), this.Atk + (int)Math.Ceiling(getDamage) + 1);
            if (attackType == AttackType.Skill)
                damage *= (int)this.Skill.ATKRatio;
            if(attackType == AttackType.Attack)
                Console.WriteLine($"Lv.{this.Lv} {this.Name} 의 공격!");
            else
                Console.WriteLine($"Lv.{this.Lv} {this.Name} 의 {this.Skill.Name} 스킬 공격!");

            return damage;
        }

        public void TakeDamage(int damage)
        {
            int criticalDamage = damage;
            
            int r = new Random().Next(0, 101);

            // 공격 미스. 10%의 확률로 공격이 적중하지 않음
            if(r > 90)
            {
                Console.Write($"Lv.{this.Lv} {this.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.\n");
                return;
            }

            Console.Write($"Lv.{this.Lv} {this.Name} 을(를) 맞췄습니다. [데미지 : ");
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

            Console.WriteLine($"\n\nLv.{this.Lv} {this.Name}");
            Console.Write($"{this.Hp} -> ");

            if (r <= 15)
                this.Hp -= criticalDamage;
            else
                this.Hp -= damage;

            if (this.Hp <= 0) this.isDead = true;

            Console.WriteLine($"{(this.isDead ? "Dead" : this.Hp)}");
        }

        public bool PrintDead()
        {
            if (this.isDead)
            {
                Console.WriteLine("이미 죽은 몬스터입니다.\n 다시 선택해주세요!");
                Console.ReadKey();
            }

            return this.isDead;
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
