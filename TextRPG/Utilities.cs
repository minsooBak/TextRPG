using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
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

    struct ObjectState
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public int Level { get { return EXP / 100; } }
        public int EXP { get; set; }
        public float InitATK { get; set; }
        public int InitDEF { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
    }

    interface IObject
    {
        int Attack();
        void TakeDamage(int damage);
        bool IsDead();
    }


    internal static class Utilities
    {
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
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다!");
                        Console.WriteLine("===================================================");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다!");
                    Console.WriteLine("===================================================");
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
                        Console.WriteLine("===================================================");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다!");
                    Console.WriteLine("===================================================");
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
                        path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\I_Data.json";
                        if (File.Exists(path) == false)
                            return null;
                        StreamReader? file = File.OpenText(path);
                        if (file != null)
                        {
                            JsonTextReader reader = new JsonTextReader(file);

                            JObject json = (JObject)JToken.ReadFrom(reader);
                            string? str = JsonConvert.SerializeObject(json);
                            file.Close();
                            //return JsonConvert.DeserializeObject<ItemData>(str);

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
                            //return JsonConvert.DeserializeObject<List<Item>>(str);
                        }
                        break;
                    }
                case LoadType.Player:
                    {
                        path += @"\P_Data.json";
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
                            .Parent.Parent.Parent.FullName + @"\Data\Monster_Data.json";
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
                case LoadType.Dungeon:
                    {
                        path = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                            .Parent.Parent.Parent.FullName + @"\Dungeon_Data.json";
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
