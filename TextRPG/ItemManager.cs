using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    //추민규님 구현
    internal class ItemManager
    {
        enum ItemType
        {
            Weapon,
            Armor
        }

        class Item
        {
            public string Name = "";
            public ItemType Type = 0;
            public int ATK = 0;
            public int DEF = 0;
            public string Comment = "";

            // 생성자
            public Item(string name, int type, int atk, int def, string comment) 
            { 
                Name = name;
                Type = (ItemType)type;
                ATK = atk;
                DEF = def;
                Comment = comment;
            }
        }
    }
}
