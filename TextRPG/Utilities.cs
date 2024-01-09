using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextRPG
{
    enum SaveType
    {
        NONE,
        Player,
        ItemData
    }

    enum LoadType
    {
        Player,
        Map,
        Item,
        ItemData
    }

    struct ObjectState
    {
        public string Name { get; set; }
        public string Class {  get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public int Level { get { return EXP / 100; } }
        public int EXP { get; set; }
        public float InitATK { get; set; }
        public int InitDEF { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
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
            return null;
        }

        public static void SaveFile(SaveType dataType, object data)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "";

            
        }
    }
}
