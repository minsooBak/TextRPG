using System;
using System.Collections;
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
        private readonly Skill[] skillList = []; //전체 스킬데이터
        private Dictionary<string, List<Skill>> skillDictionary = [];//클래스별 스킬데이터
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

        /// <summary>
        /// 몬스터가 스킬을 사용할수 있는지 여부 체크 및 제일 피해량이 높은 스킬 의 피해량과 코스트, number를 배열로 리턴
        /// / 없다면 null로 리턴
        /// </summary>
        public int[] MonsteSkillUse(string className, int ATK, int mp)
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

            Skill? s = null;
            int max = 0;
            int count = -1;
            foreach(Skill skill in list)
            {
                if(skill.Cost <= mp)
                {
                    if(skill.GetATK(ATK) > max)
                    {
                        max = skill.GetATK(ATK);
                        s = skill;
                        count++;
                    }
                }
            }

            if(s == null)
            {
                return null;
            }

            return [max, s.Cost, count];
        }

        /// <summary>
        /// 플레이어의 스킬 피해량
        /// </summary>
        public int GetSkillATK(string className, int number, int ATK)
        {
            List<Skill> list;
            if (skillDictionary.Count == 0)
            {
                Console.Error.WriteLine("Skills Empty");
                return 0;
            }else if (skillDictionary.TryGetValue(className, out list) == false)
            {
                Console.Error.WriteLine($"GetSkillATK Faill! SkillDictionary[{className}] is not Add");
                return 0;
            }

            return list[number].GetATK(ATK); 
        }

        public string GetSkillName(string className, int number)
        {
            List<Skill> list;
            if (skillDictionary.Count == 0)
            {
                Console.Error.WriteLine("Skills Empty");
                return null;
            }
            else if (skillDictionary.TryGetValue(className, out list) == false)
            {
                Console.Error.WriteLine($"GetSkillATK Faill! SkillDictionary[{className}] is not Add");
                return null;
            }

            return list[number].Name;
        }
        
        public int GetCost(string className, int number)
        {
            List<Skill> list;
            if (skillDictionary.Count == 0)
            {
                Console.Error.WriteLine("Skills Empty");
                return 0;
            }else if(skillDictionary.TryGetValue(className, out list) == false)
            {
                Console.Error.WriteLine($"GetSkillATK Faill! SkillDictionary[{className}] is not Add");
                return 0;
            }

            return list[number].Cost;
        }
    }

    class Skill
    {
       //public SkillType Type { get; private set; } 나중에 패시브 추가할 경우
        public string Name { get; private set; }
        public string Class { get; private set; }//나중에 enum으로도 변경가능
        private float ATK { get; set; } // 퍼뎀
        public int Cost { get; private set; }
        public string Description { get; private set; }

        public Skill(string name, string Class, float ATK, int cost, string description)
        {
            Name = name;
            this.Class = Class;
            this.ATK = ATK;
            Cost = cost;
            Description = description;
        }

        /// <summary>
        /// 총 피해량을 리턴
        /// </summary>
        public int GetATK(int ATK)
        {
            return (int)(ATK * this.ATK);
        }
    }
}
