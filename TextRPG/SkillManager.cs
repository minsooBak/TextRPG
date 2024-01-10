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
        private readonly List<Skill>? skillList = []; //Player의 SkillList
        public SkillManager(string className)
        {
            //스킬데이터 불러오기 후 클래스네임에 따라 다른 스킬배열을 넣어줌
            //만약 해금하는 식으로 할 경우 SKill에 해금 여부와 조건
            Skill[] skillList = (Skill[])Utilities.LoadFile(LoadType.SkillData);
            if(skillList == null)
            {
                Console.Error.WriteLine("SkillLoad Faill!");
                return;
            }
            foreach(Skill skill in skillList)
            {
                if(skill.Class == className)
                {
                    this.skillList.Add(skill);
                }
            }

        }
        public Skill[] GetPlayerSkill()
        {
            return skillList.ToArray();
        }

        public Skill GetSkill(int number)
        {
            if(number >= skillList.Count)
            {
                Console.Error.WriteLine("Skill Get Faill : GetNumber = " + number);
                return null;
            }
            return skillList[number];
        }
    }

    class Skill
    {
       //public SkillType Type { get; private set; } 나중에 패시브 추가할 경우
        public string Name { get; private set; }
        public string Class { get; private set; }//나중에 enum으로도 변경가능
        public int ATK { get; private set; } // 퍼뎀
        public int Cost { get; private set; }
        public int CoolDown { get; private set; }
        public string Description { get; private set; }

        public Skill(string name, string Class, int ATK, int cost, int coolDown, string description)
        {
            Name = name;
            this.Class = Class;
            this.ATK = ATK;
            Cost = cost;
            CoolDown = coolDown;
            Description = description;
        }
    }
}
