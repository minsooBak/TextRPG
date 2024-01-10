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

    struct ItemData
    {
        public string[] inventory;
        public string[] equippedItem;
        public string[] fieldItem;
    }

    internal class ItemManager : IListener
    {
        readonly Item[] items;
        List<Item> inventory;

        public int GetInventorySize { get { return inventory.Count; } }

        public ItemManager()
        // ItemManager 생성자 : 
        {
            List<Item>? list = (List<Item>?)Utilities.LoadFile(LoadType.Item);
            items = list.ToArray();

            ItemData? data = (ItemData?)Utilities.LoadFile(LoadType.ItemData);
            inventory = [];

            if (data != null)
            // 아이템 정보를 data에서 읽어와서 inventory에 할당하기
            {
                foreach (string item in data.Value.inventory)
                {
                    Item _item = list.Find(x => x.Name == item);

                    inventory.Add(_item);
                }
                foreach (string item in data.Value.equippedItem)
                {
                    Item _item = list.Find(x => x.Name == item);
                    _item.IsEquipped = true;
                }
            }
        }

        public void ShowInventory()
        {
            // 인벤토리 보기 관련 메서드
        }

        public void GetFieldItem()
        {
            // 필드에 드랍된 아이템 줍줍
        }

        public void EquipItem()
        {
            // 장비 착용 관련 메서드
        }

        public void OnEvent(EventType type, object data)
        {
            // 게임 이벤트에 따른 인벤토리 기능 구현
        }
    }

    class Item
    // 아이템 자료 클래스
    {
        public string Name = "";
        public ItemType Type = 0;
        public int ATK = 0;
        public int DEF = 0;
        public string Comment = "";
        public int Cost = 0;

        public bool IsEquipped = false;
        public bool IsOnField = false; // 던전에서 얻을 수 있는 지의 여부

        // 생성자
        public Item(string name, int type, int atk, int def, string comment, int reqGold, bool isOnField)
        {
            Name = name;
            Type = (ItemType)type;
            ATK = atk;
            DEF = def;
            Comment = comment;
            Cost = reqGold;
            IsOnField = isOnField;
        }
    }
}
