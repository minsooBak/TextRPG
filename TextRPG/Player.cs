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


    //유시아님 플레이어 구현 매소드 : 스탯출력, 공격
    internal class Player : IListener, IObject
    {
        ObjectState myState;

        public Player(string name = "chad", string job = "전사", int level = 1, int atk = 10, int def = 5, int hp = 100, int gold = 1500)
        {
            myState.Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
            Mp = 0;
        }
        public Skill Skill { get; set; }

        public int Health => myState.Health;

        public int Level => throw new NotImplementedException();

        public string Class => throw new NotImplementedException();

        bool IObject.IsDead => throw new NotImplementedException();

        public void SetSkill(Skill skill)
        {
            Skill = skill;
        }
        //static void GameDataSetting()
        //{
        //    // 캐릭터 정보 세팅
        //    this = new Character("Chad", "전사", 1, 10, 5, 100, 1500);
        //    // 아이템 정보 세팅
        //}
        public void OnEvent(EventType type, object data)
        {
            //이벤트 받아서 switch문으로 구현
            
        }

        public int Attack(AttackType attackType)
        {
            int damage = 0;
            double getDamage;

            getDamage = Atk / 100.0 * 10;
            damage = new Random().Next(Atk - (int)Math.Ceiling(getDamage), Atk + (int)Math.Ceiling(getDamage) + 1);
            if (attackType == AttackType.Skill)
                damage *= (int)this.Skill.ATKRatio;

            if (attackType == AttackType.Attack)
                Console.WriteLine("Chad 의 공격!");
            else
                Console.WriteLine($"Chad 의 {Skill.Name} 스킬 공격!");
            return damage;
        }

        public bool IsDead()
        {
            throw new NotImplementedException();
        }

        public void TakeDamage(int damage)
        {
            throw new NotImplementedException();
        }

        public bool PirntDead()
        {
            throw new NotImplementedException();
        }
    }
}
