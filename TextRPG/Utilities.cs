using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace TextRPG
{
    enum SaveType
    {
        Player,
        ItemData
    }

    enum LoadType
    {
        Player,
        Map,
        Item,
        ItemData,
        Monster,
        SkillData,
        Dungeon
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

    internal static class Utilities
    {
        public static KeyValuePair<T, object> EventPair<T>(T t, object a)
        {
            return new KeyValuePair<T, object>(t, a);
        }

        public static KeyValuePair<T, string> EventPair<T>(T t, string a)
        {
            return new KeyValuePair<T, string>(t, a);
        }

        public static KeyValuePair<T, int> EventPair<T>(T t, int a)
        {
            return new KeyValuePair<T, int>(t, a);
        }

        public static KeyValuePair<T, Item> EventPair<T>(T t, Item a)
        {
            return new KeyValuePair<T, Item>(t, a);
        }

        public static KeyValuePair<T, Item[]> EventPair<T>(T t, Item[] a)
        {
            return new KeyValuePair<T, Item[]>(t, a);
        }



        //public static KeyValuePair<T, Item> EventPair<T>(T t, Item data)
        //{
        //    return new KeyValuePair<T, Item>(t, data);
        //}

        //public static KeyValuePair<T, Item[]> EventPair<T>(T t, Item[] data)
        //{
        //    return new KeyValuePair<T, Item[]>(t, data);
        //}

        static StringBuilder sb = new StringBuilder(400);

        public static void AddLine(string str)
        {
            sb.AppendLine(str);
        }
        public static void Add(string str)
        {
            sb.Append(str);
        }

        static public void TextColor(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        static public void TextColorWithNoNewLine(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ResetColor();
        }

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

        public static object? LoadFile(LoadType type)
        {
            string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            switch (type)
            {
                case LoadType.Map:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Map_Data.json";

                        if (File.Exists(path) == false)
                            return null;
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            //return JsonConvert.DeserializeObject<List<Dungeon>>(str);
                        }
                        break;
                    }
                case LoadType.ItemData:
                    {
                        path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Data\I_Data.json";
                        if (File.Exists(path) == false)
                        {
                            return null;
                        }
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JObject json = (JObject)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            // return JsonConvert.DeserializeObject<ItemData>(str);

                        }
                        break;
                    }
                case LoadType.Item:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Item_Data.json";
                        if (File.Exists(path) == false)
                            return null;
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            return JsonConvert.DeserializeObject<List<Item>>(str);
                        }
                        break;
                    }
                case LoadType.Player:
                    {
                        path += @"\Data\P_Data.json";
                        if (File.Exists(path) == false)
                            return null;
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JObject json = (JObject)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);

                            file.Close();
                            return JsonConvert.DeserializeObject<ObjectState>(str);
                        }
                        break;
                    }
                case LoadType.Monster:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Data\Monster_Data.json";
                        if (File.Exists(path) == false)
                            return null;
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
                            return null;
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);

                            file.Close();
                            return JsonConvert.DeserializeObject<Skill[]>(str);
                        }
                        break;
                    }
                case LoadType.Dungeon:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Data\Dungeon_Data.json";
                        if (File.Exists(path) == false)
                            return null;
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JArray json = (JArray)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            return JsonConvert.DeserializeObject<List<Dungeon>>(str);
                        }
                        break;
                    }
            }
            return null;
        }

        public static void SaveFile(SaveType dataType, object data)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "";
            switch (dataType)
            {
                case SaveType.Player:
                    {
                        path += @"\P_Data.json";
                        File.WriteAllText(path, JsonConvert.SerializeObject((ObjectState)data, Formatting.Indented));
                        break;
                    }
                case SaveType.ItemData:
                    {
                        path += @"\I_Data.json";
                        //string json = JsonConvert.SerializeObject((ItemData)data, Formatting.Indented);
                        //File.WriteAllText(path, json);
                        break;
                    }
            }

        }
    }
}