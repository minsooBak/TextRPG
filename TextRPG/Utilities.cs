using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
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
