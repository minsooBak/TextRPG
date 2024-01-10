using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    internal class SkillManager
    {
        private readonly Skill[] skillList = []; //전체 스킬데이터
        private Dictionary<string, List<Skill>> skillDictionary = [];//직업별 스킬데이터
        public SkillManager()
        {
            skillList = (Skill[])Utilities.LoadFile(LoadType.SkillData);
            if (skillList == null)
            {
                Console.Error.WriteLine("SkillLoad Faill!");
            }
        }

        /// <summary>
        /// 클래스네임에 따라 다른 스킬배열을 넣어줌(Player Class, Monster Name)
        /// </summary>
        public void AddSkills(string className)
        {
            List<Skill> list = new List<Skill>();
            foreach (Skill skill in skillList)
            {
                if (skill.Class == className)
                {
                    list.Add(skill);
                }
            }

            if(list == null)
            {
                Console.Error.WriteLine("SkillsAdd Fail! ClassName : " + className);
                return;
            }

            skillDictionary.Add(className, list);
        }

        public int ShowSkillList(string className)
        {
            if (skillDictionary.Count == 0)
            {
                Console.Error.WriteLine("Skills Empty");
                return 0;
            }

            List<Skill> list;
            if(skillDictionary.TryGetValue(className, out list) == false)
            {
                Console.Error.WriteLine("Skill Show ClassName Null! ClassName : " + className);
                return 0;
            }

            for(int i = 0; i < list.Count; i++)
            {
                Utilities.AddLine($"{i + 1}. {list[i].Name} - mp {list[i].Cost}");
                Utilities.AddLine($"   {list[i].Description}.");
            }
            return list.Count;
        }

        public Skill GetMonsterSkill(string className, int mp)
        {
            if (skillDictionary.Count == 0)
            {
                Console.Error.WriteLine("Skills Empty");
                return null;
            }

            List<Skill> list;
            if (skillDictionary.TryGetValue(className, out list) == false)
            {
                Console.Error.WriteLine("MonsterSkillUse ClassName Null! ClassName : " + className);
                return null;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Cost <= mp)
                {
                    return list[i];
                }
            }

            return null;
        }

        public Skill GetPlayerSkill(string className, int number)
        {
            List<Skill> list;
            if (skillDictionary.Count == 0)
            {
                Console.Error.WriteLine("Skills Empty");
                return null;
            }else if (skillDictionary.TryGetValue(className, out list) == false)
            {
                Console.Error.WriteLine($"GetSkillATK Faill! SkillDictionary[{className}] is not Add");
                return null;
            }

            return list[number]; 
        }
    }

    class Skill
    {
        public readonly string Name;
        public readonly string Class;
        public readonly float ATKRatio;
        public readonly int Cost;
        public readonly string Description;

        public Skill(string name, string Class, float ATKRatio, int cost, string description)
        {
            Name = name;
            this.Class = Class;
            this.ATKRatio = ATKRatio;
            Cost = cost;
            Description = description;
        }

        /// <summary>
        /// 총 피해량을 리턴
        /// </summary>
        public int GetATK(int ATK)
        {
            return (int)(ATK * ATKRatio);
        }
    }
}
