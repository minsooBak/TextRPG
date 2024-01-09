using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    // 추민규님 구현
    enum ItemType
    {
        Weapon,
        Armor
    }

    internal class ItemManager : IListener
    {
        public void OnEvent(EventType type, object data)
        {
            // 게임 이벤트에 따른 인벤토리 기능 구현
        }
    }

    // 아이템 자료 클래스
    class Item
    {
        public string Name = "";
        public ItemType Type = 0;
        public int ATK = 0;
        public int DEF = 0;
        public string Comment = "";
        public int Cost = 0;

        // 생성자
        public Item(string name, int type, int atk, int def, string comment, int reqGold)
        {
            Name = name;
            Type = (ItemType)type;
            ATK = atk;
            DEF = def;
            Comment = comment;
            Cost = reqGold;
        }
    }
}
