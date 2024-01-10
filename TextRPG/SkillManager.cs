using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    //enum SkillType
    //{
    //    Active,
    //    Passive
    //}
    internal class SkillManager
    {
        private readonly Skill[] skillList = []; //Player의 SkillList
        public SkillManager()
        {
            skillList = (Skill[])Utilities.LoadFile(LoadType.SkillData);
            if (skillList == null)
            {
                Console.Error.WriteLine("SkillLoad Faill!");
            }
        }

        /// <summary>
        /// 클래스네임에 따라 다른 스킬배열을 넣어줌(PlayerClass, MonsterName)
        /// </summary>
        public List<Skill> GetSkills(string className)
        {
            List<Skill> list = new List<Skill>();
            foreach (Skill skill in skillList)
            {
                if (skill.Class == className)
                {
                    list.Add(skill);
                }
            }
            return list;
        }
    }

    class Skill
    {
       //public SkillType Type { get; private set; } 나중에 패시브 추가할 경우
        public string Name { get; private set; }
        public string Class { get; private set; }//나중에 enum으로도 변경가능
        private float ATK { get; set; } // 퍼뎀
        public int Cost { get; private set; }
        public int CoolDown { get; private set; }
        public int MaxCoolDown { get; private set; }
        public string Description { get; private set; }

        public Skill(string name, string Class, int ATK, int cost, int coolDown, string description)
        {
            Name = name;
            this.Class = Class;
            this.ATK = ATK;
            Cost = cost;
            CoolDown = coolDown;
            MaxCoolDown = coolDown;
            Description = description;
        }

        public bool isUse()
        {
            if (CoolDown == 0) return true;
            else
            {
                CoolDown--;
                return false;
            }
        }

        /// <summary>
        /// 총 피해량을 리턴
        /// </summary>
        public int GetATK(int ATK)
        {
            CoolDown = MaxCoolDown;
            return (int)(ATK * this.ATK);
        }
    }
}
