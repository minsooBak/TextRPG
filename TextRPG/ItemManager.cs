namespace TextRPG
{
    enum ItemType //아이템 종류
    {
        Weapon,
        Armor
    }

    struct SaveData //저장할 데이터 구조
    {
        public string[] inventory;
        public string[] equippedItem;
        public string[] saleItem;
    }

    internal class ItemManager : IListener
    {
        // Save_Data.json에서 불러온 읽기 전용 아이템 목록
        readonly Item[] items;
        readonly Item[] fieldItems;
        readonly Item[] shopItems;

        // 게임에서 실제로 사용되는 아이템 목록
        List<Item> inventory; // 인벤토리
        List<Item> shopDisplay; // 상점에 노출되는 아이템 목록(IsSale이 true인 아이템)
        List<Item> fieldDisplay; 
        /* fieldDisplay는 IsOnField가 true인 아이템 리스트로,
         * 단순히 Item_Data.json 파일에서 상점에 노출되는 아이템과 구분하기 위해 만들어진 리스트입니다.
         * IsSale이 true면서 IsOnField를 true로 설정해서 상점에 노출되면서 던전에서도 사용 가능한 아이템을 만들 수 있고,
         * IsSale이 false면서 IsOnField를 true로 설정해서 상점에 노출되지 않으면서 던전에서 사용 가능한 아이템을 만들 수 있습니다.
        */



        // 리스트 크기를 반환하는 프로퍼티
        public int GetInventorySize { get { return inventory.Count; } }
        public int GetShopDisplaySize { get { return shopDisplay.Count; } }
        public int GetFieldDisplaySize { get { return fieldDisplay.Count; } }

        // ShowInventory, ShowShop 메서드에서 사용할 모드(Map.cs에서 사용함)
        public int Mode { get; set;}

        public ItemManager() 
        // ItemManager 생성자 : 
        {
            Mode = 0;

            List<Item>? list = Utilities.LoadFile<List<Item>>(LoadType.Item);


            items = list.ToArray();

            // 배열 초기화
            inventory = [];
            shopDisplay = [];
            fieldDisplay = [];

            // shopDisplay, fieldDisplay에 각 조건에 맞는 아이템 추가
            foreach (Item item in items)
            {
                if (item.IsOnField)
                    fieldDisplay.Add(item); //필드 아이템 저장
                if (item.IsSale)
                    shopDisplay.Add(item); //상점 아이템 저장
            }

            // 인벤토리 저장 파일 불러오기
            SaveData? saveData = Utilities.LoadFile<SaveData?>(LoadType.SaveData);

            // 저장된 파일이 null이 아니라면 저장된 정보 할당하기
            if (saveData != null)
            {
                foreach (string saveitem in saveData.Value.inventory)
                    // 인벤토리 아이템 목록 가져오기
                {
                    Item _item = list.Find(x => x.Name == saveitem);
                    inventory.Add(_item);
                }
                foreach (string saveitem in saveData.Value.equippedItem)
                    // 장착 여부 가져오기
                {
                    Item _item = list.Find(x => x.Name == saveitem);
                    _item.IsEquipped = true;
                }
                foreach (string saveitem in saveData.Value.saleItem)
                    // 판매 여부 가져오기
                {
                    Item _item = list.Find(x => x.Name == saveitem);
                    _item.IsSale = false;
                }

            }


            // ItemManager에 Event Listener 등록
            EventManager.Instance.AddListener(EventType.Item, this);
            EventManager.Instance.AddListener(EventType.eGameEnd, this);
        }

        public void ShowInventory(int mode = 0)
        /* 인벤토리 아이템 목록 표시 관련 메서드
            mode=0 일 때: -{아이템 이름}    | {공격력 or 방어력} {추가 스탯}  | {설명} |
            mode=1 일 때: -{번호} [E] {아이템 이름}    | {공격력 or 방어력} {추가 스탯}  | {설명} |
            mode=2 일 때: -{번호} [E] {아이템 이름}    | {공격력 or 방어력} {추가 스탯}  | {설명} | {가격} G
        */
        {
            Console.WriteLine("[아이템 목록]");

            int count = 0;
            foreach (Item item in inventory)
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
                    $"{((mode == 2) ? $"정가: {item.Cost} G" : "" )}");
            }
            
        }

        public void ShowShop(int mode = 0)
        /* 상점 아이템 목록 표시 관련 메서드
            mode=0 일 때: -{아이템 이름}    | {공격력 or 방어력} {추가 스탯}  | {설명} | {가격} G
            mode=1 일 때: -[{번호}] {아이템 이름}    | {공격력 or 방어력} {추가 스탯}  | {설명} | {가격} G
        */
        {
            Console.WriteLine("[아이템 목록]");

            int count = 0;
            foreach (Item item in shopDisplay)
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
        // 아이템 구매 관련 메서드(아이템 번호, 소지금)
        {
            Item item = shopDisplay[itemNum - 1]; // 상점에서 아이템 선택

            // IsSale(판매 가능 여부) 검사
            if (item.IsSale == false) // 팔렸으면
            {
                Console.WriteLine("이미 구매한 아이템입니다. \n(Enter키를 눌러 진행하세요...)");
                Console.ReadLine();
                return;
            }
            else // 안 팔렸으면
            {
                // 소지금 검사
                if (item.Cost > myWallet) // 소지금이 부족하면
                {
                    Console.WriteLine("소지금이 부족합니다. \n(Enter키를 눌러 진행하세요...)");
                    Console.ReadLine();
                    return;
                }
                else // 소지금이 충분하면
                {
                    item.IsSale = false; // 아이템 팔렸다고 표시
                    inventory.Add(item); // 인벤토리에 아이템 추가

                    Console.WriteLine($"{item.Name}을 구매했습니다. \n(Enter키를 눌러 진행하세요...)");
                    Console.ReadLine();

                    // EventManager로 Player에 아이템 가격만큼 소지금 차감하는 이벤트 전달
                    EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Gold, -item.Cost));
                }
            }
        }

        public void SellItem(int itemNum, int myWallet)
        // 아이템 판매 관련 메서드
        {
            Item item = inventory[itemNum - 1]; // 인벤토리에서 아이템 선택

            item.IsSale = true; // 아이템 상점에서 팔기 가능으로 바꿔줌

            if (item.IsEquipped)
            // 장비 중인 아이템을 판매하는 경우
            {
                item.IsEquipped = false; // 장착 해제

                // EventManager로 아이템 정보만큼 스탯 차감 이벤트 전달
                item.ATK *= -1;
                item.DEF *= -1;
                EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Stats, item));
            }

            // EventManager로 골드 변경 이벤트 전달
            int resultGold = (int) (item.Cost * 0.85); // 정가의 85% 계산
            EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Gold, resultGold));


            inventory.Remove(item); // 인벤토리에서 아이템 삭제

            Console.WriteLine($"{item.Name}을 판매했습니다. (판매 금액: {resultGold} G) \n(Enter키를 눌러 진행하세요...)");
            Console.ReadLine();

        }

        public void GetFieldItem(Item? data = null)
        // 던전에서 아이템 얻기 관련 메서드, 단순 인벤토리 아이템 추가용
        {
            if (data != null) // 매개변수로 아이템이 넘어오면
            {
                inventory.Add(data); //인벤토리에 아이템 추가
                Console.WriteLine($"{data.Name}을 획득했습니다.");
            }
            else
            {
                Console.WriteLine($"아무 아이템도 얻지 못했습니다. \n(Enter키를 눌러 진행하세요...)");
                Console.ReadLine();
            }
        }

        public void EquipItem(int itemNum)
        {
            // 인벤토리에서 아이템 장착/해제 관련 메서드

            Item item = inventory[itemNum - 1]; // 인벤토리에서 아이템 선택


            if (item.IsEquipped) // 장착 중이면 해제
            {
                item.IsEquipped = false; // 장착 해제
                Console.WriteLine($"{item.Name}을 해제했습니다. \n(Enter키를 눌러 진행하세요...)");
                Console.ReadLine();

                // EventManager로 아이템 정보만큼 스탯 차감 이벤트 전달
                item.ATK *= -1;
                item.DEF *= -1;
                EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Stats, item));
            }
            else // 아니라면 (아이템 장착)
            {
                item.IsEquipped = true; // 장착
                Console.WriteLine($"{item.Name}을 착용했습니다. \n(Enter키를 눌러 진행하세요...)");
                Console.ReadLine();

                // EventManager로 아이템 정보만큼 스탯 증가 이벤트 전달
                EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Stats, item));

                // QuestManager에 아이템 장착 이벤트 전달
                EventManager.Instance.PostEvent(EventType.Quest, Utilities.EventPair(eQuestType.Item, inventory[itemNum - 1].Name));//장착하면 아이템 이름으로 이벤트 발생
            }
        }

        //public void DropItem(int itemNum)
        //// 인벤토리에서 아이템 삭제 기능, 판매와는 다르게 그냥 버리는 것 (현재 안 쓰임)
        //{
        //    Item item = inventory[itemNum - 1]; // 인벤토리에서 아이템 선택

        //    inventory.Remove(item); // 인벤토리에서 아이템 삭제
        //    Console.WriteLine($"{item.Name}을 버렸습니다.");

        //    if (item.IsEquipped) // 버린 아이템이 장비중인 경우
        //    {
        //        // EventManager로 아이템 정보만큼 스탯 차감 이벤트 전달
        //        item.ATK *= -1;
        //        item.DEF *= -1;
        //        EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Stats, item));
        //    }
        //}

        public void OnEvent<T>(EventType type, T data)
        {
            // 받는 이벤트의 type에 따른 분기
            if (type == EventType.Item) // 'Item' 이벤트 관련
            {
                var d = data as KeyValuePair<eItemType, String>?;

                // 게임 이벤트에 따른 인벤토리 기능 구현
                if (d != null)
                {
                    switch (d.Value.Key)
                    {
                        case eItemType.eGameEnd:
                            {
                                break;
                            }
                        case eItemType.eGetFieldItem:
                            {
                                foreach (var item in items) //전체 아이템 목록에서 하나 씩 아이템을 꺼내서
                                {
                                    //if (item.Name.Equals(data.ToString)) // 아이템의 이름과  매개변수data의 이름이 같으면
                                    if (item.Name.Equals(d.Value.Value))
                                    {
                                        GetFieldItem(item);//아이템을 인벤토리에 넣는다. 얻었는지 못 얻었는지 출력문 
                                        break;
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            else if (type == EventType.eGameEnd) // 게임 종료 이벤트 관련
            // 게임이 종료 될 때 인벤토리, 장착 아이템, 판매 아이템을 저장
            {
                SaveData itemData = new SaveData(); // 저장할 데이터 구조 생성

                itemData.inventory = inventory.Select(x => x.Name).ToArray(); // 인벤토리 아이템 목록 저장
                itemData.equippedItem = inventory.FindAll(x => x.IsEquipped).Select(x => x.Name).ToArray(); // 장착 아이템 목록 저장
                itemData.saleItem = inventory.FindAll(x => x.IsSale == false).Select(x => x.Name).ToArray(); // 판매 아이템 목록 저장

                // Save_Data.json 파일에 저장
                Utilities.SaveFile(SaveType.SaveData, itemData);
            }
        }
    }

    class Item
    // 아이템 자료 클래스
    {
        // { get; private set; } : 읽기 전용 프로퍼티, 외부에서 수정 불가

        /* 아이템 변수 설명
         * Name : 아이템 이름
         * Type : 아이템 종류 (0이면 무기, 1이면 방어구)
         * ATK : 공격력
         * DEF : 방어력
         * Description : 아이템 설명
         * Cost : 아이템 가격
         * 
         * IsEquipped : 장비 착용 여부
         * IsSale : 상점에서 판매 가능한 지의 여부
         * IsOnField : 던전 아이템으로 분류할 건지의 여부
         */
        public string Name { get; private set; }
        public ItemType Type { get; private set; }
        public int ATK { get; set; } // 음수로 변경해야 할 때도 있어서 set은 public으로 설정
        public int DEF { get; set; } // 음수로 변경해야 할 때도 있어서 set은 public으로 설정
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
