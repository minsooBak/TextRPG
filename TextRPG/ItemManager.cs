﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
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
        public string[] saleItem;
    }

    internal class ItemManager : IListener
    {
        readonly Item[] items;
        readonly Item[] fieldItems;
        readonly Item[] shopItems;

        List<Item> inventory;
        List<Item> shopDisplay;
        List<Item> fieldDisplay;

        public int GetInventorySize { get { return inventory.Count; } }
        public int GetShopDisplaySize { get { return shopDisplay.Count; } }
        public int GetFieldDisplaySize { get { return fieldDisplay.Count; } }


        public ItemManager() 
        // ItemManager 생성자 : 
        {
            List<Item>? list = (List<Item>?)Utilities.LoadFile(LoadType.Item);
            items = list.ToArray();

            ItemData? data = (ItemData?)Utilities.LoadFile(LoadType.ItemData);
            inventory = [];
            shopDisplay = [];
            fieldDisplay = [];

            if (data != null)
            // 아이템 정보를 data에서 읽어와서 inventory및 기타 배열에 할당하기
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
                foreach (string item in data.Value.saleItem)
                {
                    Item _item = list.Find(x => x.Name == item);
                    shopDisplay.Add(_item);
                }
                foreach (string item in data.Value.fieldItem)
                {
                    Item _item = list.Find(x => x.Name == item);
                    fieldDisplay.Add(_item);
                }
            }

            shopItems = list.FindAll(x => x.IsSale == true).ToArray();
            fieldItems = list.FindAll(x => x.IsOnField == true).ToArray();
            
            List<Item>? equippedItems = inventory.FindAll(x => x.IsEquipped);
            EventManager.Instance.PostEvent(EventType.eUpdateItem, null);

            // ItemManager에 Event Listener 등록
            EventManager.Instance.AddListener(EventType.eGetItem, this);
            EventManager.Instance.AddListener(EventType.eGameEnd, this);

        }

        public void ShowInventory(bool isBought = false, bool isEquipped = false)
        // 인벤토리 보기 관련 메서드
        {
            Console.WriteLine("[아이템 목록]");

            foreach (Item item in inventory)
            // 양식에 맞춰 콘솔에 아이템 정보 출력
            // 1. [E] {아이템 이름} | (양식 수정 예정)
            {
                if (isBought)
                {
                    Console.WriteLine($"");
                }
            }
            
        }

        public void GetFieldItem()
        {
            // 필드에 드랍된 아이템 줍줍
        }

        public void EquipItem()
        {
            // 장비 착용 관련 메서드
        }

        public void SellItem()
        {
            // 아이템 판매 관련 메서드
        }

        public void BuyItem()
        {
            // 아이템 구매 관련 메서드
        }

        public void ShowShop()
        {
            // 상점 관련 메서드
        }

        public void OnEvent(EventType type, object data)
        {
            // 게임 이벤트에 따른 인벤토리 기능 구현
            switch (type)
            {
                // case가 eGameEnd인 경우 ItemData를 저장
                case EventType.eGameEnd:
                    ItemData itemData = new ItemData();
                    itemData.inventory = inventory.Select(x => x.Name).ToArray();
                    itemData.equippedItem = inventory.FindAll(x => x.IsEquipped).Select(x => x.Name).ToArray();
                    itemData.fieldItem = fieldDisplay.Select(x => x.Name).ToArray();
                    itemData.saleItem = shopDisplay.Select(x => x.Name).ToArray();

                    Utilities.SaveFile(SaveType.ItemData, itemData);
                    break;
            }
        }
    }

    class Item
    // 아이템 자료 클래스
    {
        // { get; private set; } : 읽기 전용 프로퍼티, 외부에서 수정 불가
        public string Name { get; private set; }
        public ItemType Type { get; private set; }
        public int ATK { get; private set; }
        public int DEF { get; private set; }
        public string Comment { get; private set; }
        public int Cost { get; private set; }

        public bool IsEquipped { get; set; } // 장비 착용 여부
        public bool IsSale { get; private set; } // 상점에서 판매 가능한 지의 여부
        public bool IsOnField { get; private set; } // 던전에서 얻을 수 있는 지의 여부

        public Item(string name, int type, int atk, int def, string comment, int reqGold, bool isSale, bool isOnField)
        // Item 클래스 생성자
        {
            Name = name;
            Type = (ItemType)type;
            ATK = atk;
            DEF = def;
            Comment = comment;
            Cost = reqGold;
            IsSale = isSale;
            IsOnField = isOnField;
        }
    }
}
