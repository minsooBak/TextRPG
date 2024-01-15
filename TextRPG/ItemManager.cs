using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    enum ItemType
    {
        Weapon,
        Armor
    }

    struct SaveData
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

        public int Mode { get; set;}

        public ItemManager() 
        // ItemManager 생성자 : 
        {
            Mode = 0;

            List<Item>? list = (List<Item>)Utilities.LoadFile(LoadType.Item);
            items = list.ToArray();

            inventory = [];
            shopDisplay = [];
            fieldDisplay = [];

            foreach (Item item in items)
            {
                if (item.IsOnField)
                    fieldDisplay.Add(item);//필드 아이템 저장
                if (item.IsSale)
                    shopDisplay.Add(item);//상점 아이템 저장
            }

            //// 인벤토리 저장 파일 불러오기
            //SaveData? saveData = (SaveData?)Utilities.LoadFile(LoadType.SaveData);
            //if (data != null)
            //// 아이템 정보를 data에서 읽어와서 inventory및 기타 배열에 할당하기
            //{
            //    foreach (string item in data.Value.inventory)
            //    {
            //        Item _item = list.Find(x => x.Name == item);

            //        inventory.Add(_item);
            //    }
            //    foreach (string item in data.Value.equippedItem)
            //    {
            //        Item _item = list.Find(x => x.Name == item);
            //        _item.IsEquipped = true;
            //    }
            //    foreach (string item in data.Value.saleItem)
            //    {
            //        Item _item = list.Find(x => x.Name == item);
            //        shopDisplay.Add(_item);
            //    }
            //    foreach (string item in data.Value.fieldItem)
            //    {
            //        Item _item = list.Find(x => x.Name == item);
            //        fieldDisplay.Add(_item);
            //    }
            //}

            List<Item>? equippedItems = inventory.FindAll(x => x.IsEquipped);
            EventManager.Instance.PostEvent(EventType.eUpdateItem, "");

            // ItemManager에 Event Listener 등록
            EventManager.Instance.AddListener(EventType.eGetFieldItem, this);
            EventManager.Instance.AddListener(EventType.eGameEnd, this);

        }

        public void ShowInventory(int mode = 0)
        // 인벤토리 보기 관련 메서드
        {
            Console.WriteLine("[아이템 목록]");

            int count = 0;
            foreach (Item item in inventory)
            // 양식에 맞춰 콘솔에 아이템 정보 출력
            // 1. [E] {아이템 이름}    | {공격력 or 방어력} {추가 스탯}  | {설명}          |
            {
                count++;
                Console.Write("-");
                if (mode >= 1)
                {
                if (item.IsEquipped)
                    Console.Write($"{count} [E] ");
                else
                    Console.Write($"{count} [-] ");
                }
                Console.WriteLine($"{item.Name} | " +
                    $"{((item.ATK > 0) ? ("공격력" + $" +{item.ATK} | ") : "")} " +
                    $"{((item.DEF > 0) ? ("방어력" + $" +{item.DEF} | ") : "")} " +
                    $"{item.Description}  | " +
                    // mode가 2이면 아이템 옆에 정가 뜨게 하기
                    $"{((mode == 2) ? $"정가: {item.Cost} G" : "" )}");
            }
            
        }

        public void ShowShop(int mode = 0)
        {
            Console.WriteLine("[아이템 목록]");

            int count = 0;
            foreach (Item item in shopDisplay)
            // 양식에 맞춰 콘솔에 아이템 정보 출력
            // 1. {아이템 이름}    | {공격력 or 방어력} {추가 스탯}  | {설명} | {가격} G
            {
                count++;
                Console.Write("-");
                if (mode == 1) // mode가 1이면 아이템 옆에 숫자 뜨게 하기
                    Console.Write($"[{count}] ");
                
                Console.Write($"{item.Name} | " +
                    $"{((item.ATK > 0) ? ("공격력" + $" +{item.ATK} |  ") : "")}" +
                    $"{((item.DEF > 0) ? ("방어력" + $" +{item.DEF} |  ") : "")}" +
                    $"{item.Description}  | ");
                if (item.IsSale)
                    Console.WriteLine($"{item.Cost} G");
                else
                    Console.WriteLine("[구매 완료]");
            }
            
        }

        public void BuyItem(int itemNum, int myWallet)
        {
            // 아이템 구매 관련 메서드
            Item item = shopDisplay[itemNum - 1];
            if (item.IsSale == false)
            {
                Console.WriteLine("이미 구매한 아이템입니다.");
                Console.ReadLine();
                return;
            }
            else
            {
                if (item.Cost > myWallet)
                {
                    Console.WriteLine("소지금이 부족합니다.");
                    Console.ReadLine();
                    return;
                }
                else
                {
                    item.IsSale = false;
                    inventory.Add(item);
                    Console.WriteLine($"{item.Name}을 구매했습니다.");
                    Console.ReadLine();

                    // EventManager로 골드 변경 이벤트 전달
                    EventManager.Instance.PostEvent(EventType.eUpdateGold, -item.Cost);
                }
            }
        }

        public void SellItem(int itemNum, int myWallet)
        {
            // 아이템 판매 관련 메서드
            Item item = inventory[itemNum - 1];
            item.IsSale = true;
            if (item.IsEquipped) // 장비 중인 아이템을 판매하는 경우
            {
                item.IsEquipped = false;
                // EventManager로 스탯 변경 이벤트 전달
                EventManager.Instance.PostEvent(EventType.eUpdateStat, item);
            }

            // EventManager로 골드 변경 이벤트 전달
            int resultGold = (int) (item.Cost * 0.85);
            EventManager.Instance.PostEvent(EventType.eUpdateGold, resultGold);

            inventory.Remove(item);
            Console.WriteLine($"{item.Name}을 판매했습니다. (판매 금액: {resultGold} G)");
            Console.ReadLine();

        }
        public Item[] GetItems()
        {
            return items;
        }

        public void GetFieldItem(Item? data = null)
        {
            // 필드에 드랍된 아이템 줍줍
            if (data != null)
            {
                fieldDisplay.Add(data);

                inventory.Add(data); //인벤토리에 아이템 추가
                Console.WriteLine($"{data.Name}을 획득했습니다.");
            
            }
            else
            {
                Console.WriteLine($"아무 아이템도 얻지 못했습니다.");
            }
        }

        public void DropItem(int itemNum)
        {
            // 아이템 버리기(인벤토리에서 삭제)
            Item item = inventory[itemNum - 1];
            inventory.Remove(item);
            Console.WriteLine($"{item.Name}을 버렸습니다.");

            if (item.IsEquipped) // 버린 아이템이 장비중인 경우
            {
                // EventManager로 스탯 변경 이벤트 전달
                EventManager.Instance.PostEvent(EventType.eUpdateStat, item);
            }
        }
        public void EquipItem(int itemNum)
        {
            Item item = inventory[itemNum - 1];
            if (item.IsEquipped)//장착 중이면
            {
                item.IsEquipped = false;
                Console.WriteLine($"{item.Name}을 해제했습니다.");
                Console.ReadLine();
            }
            else//아니라면
            {
                item.IsEquipped = true;
                Console.WriteLine($"{item.Name}을 착용했습니다.");
                Console.ReadLine();
            }
            // EventManager로 스탯 변경 이벤트 전달
            EventManager.Instance.PostEvent(EventType.eUpdateStat, item);
        }

        public void OnEvent<T>(EventType type, T data)
        {
            var d = data as KeyValuePair<EventType, Item>?;
            // 게임 이벤트에 따른 인벤토리 기능 구현
            if(d != null)
            {
                switch (d.Value.Key)
                {
                    case EventType.eGameEnd:
                    {
                        SaveData itemData = new SaveData();
                        itemData.inventory = inventory.Select(x => x.Name).ToArray();
                        itemData.equippedItem = inventory.FindAll(x => x.IsEquipped).Select(x => x.Name).ToArray();
                        itemData.saleItem = shopDisplay.Select(x => x.Name).ToArray();
                        itemData.fieldItem = fieldDisplay.Select(x => x.Name).ToArray();

                        //Utilities.SaveFile(SaveType.ItemData, itemData); //기존 것
                        Utilities.SaveFile(SaveType.SaveData, itemData);

                            break;
                    }
                    case EventType.eGetFieldItem: 
                    {
                        foreach (var item in items) //전체 아이템 목록에서 하나 씩 아이템을 꺼내서
                        {
                            if (item.Name.Equals(data.ToString())) // 아이템의 이름과  매개변수data의 이름이 같으면
                            {
                                GetFieldItem(item);//아이템을 인벤토리에 넣는다. 얻었는지 못 얻었는지 출력문 
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        //    if(type == EventType.eGameEnd)
        //     {
        //         SaveData itemData = new SaveData();
        //         itemData.inventory = inventory.Select(x => x.Name).ToArray();
        //         itemData.equippedItem = inventory.FindAll(x => x.IsEquipped).Select(x => x.Name).ToArray();
        //         itemData.saleItem = shopDisplay.Select(x => x.Name).ToArray();
        //         itemData.fieldItem = fieldDisplay.Select(x => x.Name).ToArray();

        //         Utilities.SaveFile(SaveType.ItemData, itemData);
        //         break;

        //         // case가 eGetItem인 경우 필드 아이템 획득
        //         case EventType.eGetFieldItem:
        //             foreach (var item in items)
        //             {
        //                 if (item.Name.Equals(data.ToString()))
        //                 {
        //                     GetFieldItem(item);
        //                     break;
        //                 }
        //             }
        //             break;
        //     }

            var a = data as KeyValuePair<eItemType, Item>?;
            var b = data as KeyValuePair<EventType, int>?;

            if(a != null)
            {
                switch (a.Value.Key)
                {
                    case eItemType.eGetFieldItem:
                        {
                            GetFieldItem(a.Value.Value);//수정할 부분
                            break;
                        }                               
                }
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
        public string Description { get; private set; }
        public int Cost { get; private set; }

        public bool IsEquipped { get; set; } // 장비 착용 여부
        public bool IsSale { get; set; } // 상점에서 판매 가능한 지의 여부
        public bool IsOnField { get; private set; } // 던전에서 얻을 수 있는 지의 여부

        public Item(string name, int type, int atk, int def, string description, int cost, bool isSale, bool isOnField)
        // Item 클래스 생성자
        {
            Name = name;
            Type = (ItemType)type;
            ATK = atk;
            DEF = def;
            Description = description;
            Cost = cost;
            IsSale = isSale;
            IsOnField = isOnField;
        }
    }
}
