using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace TextRPG
{
    enum SaveType // 데이터 저장 타입
    {
        Player,
        SaveData,
        Quest
    }

    enum LoadType // 데이터 불러오기 타입
    {
        Player,
        Map,
        Item,
        SaveData,
        Monster,
        SkillData,
        Dungeon,
        QuestData,
        QuestSaveData
    }
    struct ObjectState //공통변수들
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public int Level { get; set; }
        public int EXP { get; set; }   
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MP { get; set; }
        public Skill Skill { get; set; }
    }

    interface IObject //던전에서 실행될 메서드
    {
        string Class { get; }
        bool IsUseSkill { get; }
        bool IsDead { get; }
        void SetSkill(Skill skill);
        int Attack(AttackType attackType);
        void TakeDamage(int damage);
        void ShowStats();
    }

    //Load Save쪽은 제네릭으로 변경할수도있음 [최적화]
    internal static class Utilities
    {
        static StringBuilder sb = new StringBuilder(400); //GetInputKey에서 호출될 String

        //| T1 == Type / T2 == data | 원래 데이터만 보내던것을 type이라는 enum열거형으로 묶어서 보낸다
        //where는 제약조건으로 T1은 Enum으로밖에 못보낸다고 제약조건을 설정했습니다.
        public static KeyValuePair<T1, T2> EventPair<T1, T2>(T1 type, T2 data) where T1 : Enum
        {
            return new KeyValuePair<T1, T2>(type, data);
        }

        //Console.WriteLine
        public static void AddLine(string str)
        {
            sb.AppendLine(str);
        }

        //Console.Write
        public static void Add(string str)
        {
            sb.Append(str);
        }

        //str에 color로 들어온 값으로 색깔을 변경하고 Console.WriteLine 출력한 뒤 컬러 초기화
        static public void TextColor(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        //str에 color로 들어온 값으로 색깔을 변경하고 Console.Write 출력한 뒤 컬러 초기화
        static public void TextColorWithNoNewLine(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ResetColor();
        }

        //min, max값을 받아 무한반복문을돌며 전에 입력한 StringBuilder을 출력하고 min, max 값 내의 키를 입력했을때만 그 키를 반환하며 무한반복문 종료
        static public int GetInputKey(int min, int max)
        {
            while (true)
            {
                Console.Write(sb.ToString());
                string? str = Console.ReadLine();
                if (str != null && int.TryParse(str, out int a))
                {
                    int key = int.Parse(str);
                    if (key >= min && key <= max)
                    {
                        sb.Clear();
                        return key;
                    }
                    else
                    {
                        //Console.Clear();
                        Console.WriteLine("잘못된 입력입니다!");
                        Console.Write(">>");
                    }
                }
                else
                {
                    //Console.Clear();
                    Console.WriteLine("잘못된 입력입니다!");
                    Console.Write(">>");
                }
            }
        }

        //min, max값을 받아 무한반복문을돌며 전에 입력한 StringBuilder을 출력하고 min, max 값 내의 키를 입력했을때만 그 키를 반환하며 무한반복문 종료
        static public int GetInputKey(int min, int max, ConsoleColor color, string intro)
        {
            while (true)
            {
                TextColor(intro, color);
                Console.Write(sb.ToString());
                string? str = Console.ReadLine();
                if (str != null && int.TryParse(str, out int a))
                {
                    int key = int.Parse(str);
                    if (key >= min && key <= max)
                    {
                        sb.Clear();
                        return key;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다!");
                        Console.Write(">>");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다!");
                    Console.Write(">>");
                }
            }
        }

        //LoadType에 따라 경로를 설정하고 Json으로 파일 불러와서 데이터 반환
        public static T? LoadFile<T>(LoadType type)
        {
            string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            switch (type)
            {
                case LoadType.Map:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Map_Data.json";

                        if (File.Exists(path) == false) // 파일이 존재하는지 체크
                            return default(T);
                        StreamReader? file = File.OpenText(path); // 파일열기
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file); //읽어올 Reader선언

                            JArray json = (JArray)JToken.ReadFrom(reader); // JToken을 reader로 생성 한뒤 JObject나 JArray로 형변환하여 json에 데이터를 넘겨줍니다.
                            string? str = JsonConvert.SerializeObject(json); // json을 직렬화(내부데이터를 byte형식의 데이터로 변경)해서 str에 넣어줍니다
                            file.Close();//파일 닫아줍니다
                            return JsonConvert.DeserializeObject<T>(str); // str을 역직렬화해서 <T>로 형변환하여 반환합니다
                        }
                        break;
                    }
                case LoadType.SaveData:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Save_Data.json";
                        if (File.Exists(path) == false)
                        {
                            Console.WriteLine("저장된 파일이 없습니다.");
                            return default(T);
                        }
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            Console.WriteLine("저장된 파일을 불러옵니다.");
                            JsonTextReader reader = new JsonTextReader(file);

                            JObject json = (JObject)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            return JsonConvert.DeserializeObject<T>(str);

                        }
                        break;
                    }
                case LoadType.Item:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Item_Data.json";
                        if (File.Exists(path) == false)
                            return default(T);
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            return JsonConvert.DeserializeObject<T>(str);
                        }
                        break;
                    }
                case LoadType.Player:
                    {
                        path += @"\Data\P_Data.json";
                        if (File.Exists(path) == false)
                            return default(T);
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JObject json = (JObject)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);

                            file.Close();
                            return JsonConvert.DeserializeObject<T>(str);
                        }
                        break;
                    }
                case LoadType.Monster:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Data\Monster_Data.json";
                        if (File.Exists(path) == false)
                            return default(T);
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            //return JsonConvert.DeserializeObject<List<Monster>>(str);
                        }
                        break;
                    }
                case LoadType.SkillData:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Skill_Data.json";
                        if (File.Exists(path) == false)
                            return default(T);
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);

                            file.Close();
                            return JsonConvert.DeserializeObject<T>(str);
                        }
                        break;
                    }
                case LoadType.QuestData:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Quest_Data.json";
                        if (File.Exists(path) == false)
                            return default(T);
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);

                            file.Close();
                            return JsonConvert.DeserializeObject<T>(str);
                        }
                        break;
                    }
                case LoadType.QuestSaveData:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Save_Quest.json";
                        if (File.Exists(path) == false)
                            return default(T);
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JObject json = (JObject)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);

                            file.Close();
                            return JsonConvert.DeserializeObject<T>(str);
                        }
                        break;
                    }
                case LoadType.Dungeon:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Dungeon_Data.json";
                        if (File.Exists(path) == false)
                            return default(T);
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            return JsonConvert.DeserializeObject<T>(str);
                        }
                        break;
                    }
            }
            return default(T);
        }

        //SaveType에 따라 경로를 정하고 데이터를 저장
        public static void SaveFile<T>(SaveType dataType, T data)
        {
            string path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName;
            switch (dataType)
            {
                case SaveType.Player:
                    {
                        path += @"\P_Data.json";
                        //path경로에 데이터를를 직렬화하여 라인 들여쓰기(Formatting.Indented)를 마친 text를 파일이 있다면 덮어쓰고 없다면 생성하여 작성
                        File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
                        break;
                    }
                case SaveType.SaveData:
                    {
                        path += @"\Data\Save_Data.json";
                        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                        File.WriteAllText(path, json);
                        break;
                    }
                case SaveType.Quest:
                    {
                        path += @"\Data\Save_Quest.json";
                        File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
                        break;
                    }
            }
        }
    }
}