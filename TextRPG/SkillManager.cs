namespace TextRPG
{
    internal class SkillManager
    {
        private readonly Dictionary<string, List<Skill>> skillDictionary = []; //직업별 스킬 데이터 { [ 직업이름 ] , 스킬 리스트}
        public SkillManager()
        {

            List<string> classNames = new List<string>(); //클래스 이름 배열 초기에는 아무 값도 없다.
            Skill[] skillList = Utilities.LoadFile<Skill[]>(LoadType.SkillData);//스킬 데이터 배열을 가져온다.

            if (skillList == null)
            {
                Console.Error.WriteLine("SkillLoad Faill!");
            }
            foreach (var skill in skillList) //스킬 배열에서 하나씩 꺼내서
            {
                if (classNames.Find(x => x == skill.Class) == null)//클래스 이름 배열에 등록된 이름과 해당 스킬의 직업과 같으면 넘어가고 다르면 클래스 이름 배열에 해당 스킬 직업 이름 등록
                    classNames.Add(skill.Class); //이후 스킬 딕셔너리에 Key값으로 스킬을 추가하기 위해서 임시로 직업의 이름을 할당하는 것.
            }
            foreach (string className in classNames) //이름 배열로 스킬들 할당
            {
                AddSkills(className, skillList); //직업마다 스킬 저장
            }
        }

        private void AddSkills(string className, Skill[] skillList) //스킬 추가 - 생성자에서 호출
        {
            List<Skill> list = new List<Skill>();
            foreach (Skill skill in skillList) //전체 스킬 목록에서 스킬을 차례대로 고르고
            {
                if (skill.Class == className) //클래스(직업)에 맞게 스킬을 리스트에 넣음
                {
                    list.Add(skill);
                }
            }

            if (list == null) //비어있다면
            {
                Console.Error.WriteLine("SkillsAdd Fail! ClassName : " + className);
                return;
            }

            skillDictionary.Add(className, list); // {[직업] , 직업에 할당된 스킬 리스트} 이런 식으로 딕셔너리에 추가.
        }

        public void ShowSkillList(string className) //매개변수 : 직업이름 -  직업에 해당하는 스킬을 화면에 출력
        {
            List<Skill> list;
            if (skillDictionary.TryGetValue(className, out list) == false) // 딕셔너리 [ 직업이름 ]에 저장된 스킬이 없다면 에러
            {
                Console.Error.WriteLine("Skill Show ClassName Null! ClassName : " + className);
            }

            for (int i = 0; i < list.Count; i++)//스킬 리스트가 있다면 스킬 이름, 스킬 설명 출력
            {
                Utilities.TextColorWithNoNewLine($"{i + 1}. ", ConsoleColor.DarkRed);
                Console.WriteLine($"{list[i].Name} - mp {list[i].Cost}"); //스킬 이름 , 비용 출력
                Console.WriteLine($"   {list[i].Description}."); //스킬 설명 출력
            }
        }

        public Skill GetMonsterSkill(string className, int mp) //몬스터 클래스 , 현재 몬스터 mp
        {
            List<Skill> list;
            if (skillDictionary.TryGetValue(className, out list) == false) //딕셔너리 [ 몬스터 클래스 ]에 저장된 스킬이 없다면 에러
            {
                Console.Error.WriteLine("GetMonsterSkill Fail! ClassName : " + className);
                return null;
            }

            for (int i = list.Count - 1; i >= 0; i--) //스킬이 있다면
            {
                if (list[i].Cost <= mp) //스킬 비용 보다 마나가 많다면
                {
                    return list[i]; //스킬 반환
                }
            }
            return null; 
        }
        public Skill GetMySkill(string className, int number) //직업이름 , 몇 번째 스킬인지
        {
            return skillDictionary[className][number]; //직업의 스킬중 number에 해당하는 스킬 반환
        }

        public int GetMySkillCount(string className) //직업이름
        {
            return skillDictionary[className].Count; //직업 스킬 개수 반환
        }
    }
    public class Skill
    {
        public readonly string Name; //이름
        public readonly string Class; //클래스
        public readonly float ATKRatio; //공격 퍼센트
        public readonly int Cost; //비용
        public readonly string Description;//스킬 설명

        public Skill(string name, string Class, float ATKRatio, int cost, string description) //생성자 인자로 초기화
        {
            Name = name;
            this.Class = Class;
            this.ATKRatio = ATKRatio;
            Cost = cost;
            Description = description;
        }
        public int GetATK(int ATK)
        {
            return (int)(ATK * ATKRatio);
        }
    }
}
