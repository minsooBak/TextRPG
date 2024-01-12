using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TextRPG
{


    //유시아님 플레이어 구현 매소드 : 스탯출력, 공격
    internal class Player : IListener, IObject
    {
        private ObjectState myState;
        private int InitATK { get; set; }
        private int InitDEF { get; set; }

        public Player()
        {
            //CreatePlayer
            CreatePlayer createPlayer = new CreatePlayer();
            var Name = createPlayer.Create();

            myState.Name = Name.Key;
            myState.Class = Name.Value;

            myState.Health = 100;
            myState.MP = 100;
            myState.Level = 1;
            myState.EXP = 0;
            myState.ATK = 10;
            myState.DEF = 0;
            myState.Gold = 0;
            InitATK = myState.ATK;
            InitDEF = myState.DEF;
        }

        public Player(ObjectState state)
        {
            myState.Name = state.Name;
            myState.Class = state.Class;
            myState.Level = state.Level;
            myState.ATK = state.ATK;
            myState.DEF = state.DEF;
            myState.Health = state.Health;
            myState.Gold = state.Gold;
            myState.MP = state.MP;

            InitATK = state.ATK;
            InitDEF = state.DEF;
        }

        public int Health => myState.Health;
        public int MP => myState.MP;
        public int Level => myState.Level;
        public string Class => myState.Class;
        public bool IsDead => myState.Health <= 0;
        public bool IsUseSkill => myState.Skill.Cost < myState.MP;
        public void SetSkill(Skill skill) => myState.Skill = skill;

        public void OnEvent(EventType type, object data)
        {
            //이벤트 받아서 switch문으로 구현
            
        }

        public int Attack(AttackType attackType)
        {
            int damage = 0;
            double getDamage;

            getDamage = myState.ATK / 100.0 * 10;
            damage = new Random().Next(myState.ATK - (int)Math.Ceiling(getDamage), myState.ATK + (int)Math.Ceiling(getDamage) + 1);
            if (attackType == AttackType.Skill)
                damage = myState.Skill.GetATK(damage);

            if (attackType == AttackType.Attack)
                Console.WriteLine("Chad 의 공격!");
            else
                Console.WriteLine($"Chad 의 {myState.Skill.Name} 스킬 공격!");
            return damage;
        }

        public void TakeDamage(int damage)
        {
            myState.Health -= Math.Clamp(damage, 0 , 100);
        }
    }
}
