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
        private Dictionary<string, List<Skill>> skillDictionary = [];//직업별 스킬 데이터{ [직업] , 스킬 리스트}
        public List<string> classNames; //직업 이름 배열
        public SkillManager()
        {
            classNames = new List<string>();
            skillList = (Skill[])Utilities.LoadFile(LoadType.SkillData);
            if (skillList == null)
            {
                Console.Error.WriteLine("SkillLoad Faill!");
            }
            AddName(); //스킬 쓰는 직업들 이름 배열 저장
            foreach (string className in classNames)
            {
                AddSkills(className); //직업마다 스킬 저장해두기
            }
        }
        public void AddName() //스킬 쓰는 직업들 이름 배열 저장 - 생성자에서 호출
        {
            classNames.Add("Null"); //클래스 첫 인자는 아무것도 없다는 의미
            foreach (Skill skill in skillList) //전체 스킬 목록에서 
            {
                bool isIn = false;//클래스 이름 배열에 존재하는지
                for (int i = 0; i < classNames.Count; i++)  //클래스 개수+1 만큼 진행(앞에 Null 있어서)
                {
                    if (skill.Class == classNames[i]) //클래스 이름이 겹치면
                    {
                        isIn = true;
                    }
                }
                if (!isIn)//존재하지 않을 때만 추가
                {
                    classNames.Add(skill.Class);//클래스 이름 배열에 추가
                }
            }
            classNames.RemoveAt(0); //널 삭제
        }
        /// <summary>
        /// 클래스네임에 따라 다른 스킬배열을 넣어줌(Player Class, Monster Name)
        /// </summary>
        public void AddSkills(string className) //스킬 추가 - 생성자에서 호출
        {
            List<Skill> list = new List<Skill>();
            foreach (Skill skill in skillList) //전체 스킬 목록에서 
            {
                if (skill.Class == className) //클래스에 맞게 스킬을 리스트에 넣음
                {
                    list.Add(skill);
                }
            }

            if(list == null)
            {
                Console.Error.WriteLine("SkillsAdd Fail! ClassName : " + className);
                return;
            }

            skillDictionary.Add(className, list); // {[직업] , 스킬 목록}
        }

        public int ShowSkillList(string className) // 직업에 해당하는 스킬을 화면에 출력
        {
            if (skillDictionary.Count == 0)
            {
                Console.Error.WriteLine("Skills Empty");
                return 0;
            }

            List<Skill> list;
            if(skillDictionary.TryGetValue(className, out list) == false) // 스킬이 없다면 에러
            {
                Console.Error.WriteLine("Skill Show ClassName Null! ClassName : " + className);
                return 0;
            }

            for(int i = 0; i < list.Count; i++)
            {
                Utilities.AddLine($"{i + 1}. {list[i].Name} - mp {list[i].Cost}"); //스킬 이름 , 비용 출력
                Utilities.AddLine($"   {list[i].Description}."); //스킬 설명 출력
            }
            return list.Count; //스킬 개수 반환
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
                Console.Error.WriteLine("GetMonsterSkill Fail! ClassName : " + className);
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
                Console.Error.WriteLine("GetPlayerSkill Fail! ClassName : " + className);
                return null;
            }

            return list[number]; 
        }
    }

    class Skill
    {
        public readonly string Name; //이름
        public readonly string Class; //클래스
        public readonly float ATKRatio; //공격 퍼센트
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
