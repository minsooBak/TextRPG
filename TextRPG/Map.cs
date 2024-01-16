namespace TextRPG
{
    //맵의 이동 처리
    internal class Map
    {
        enum GameState
        {
            EndGame,
            PlayerInfo ,
            StartBattle,
            Inventory,
            Shop,
            Quest,
            Rest,
            NONE
        }

        Player player = new Player();
        DungeonManager dungeonManager;
        ItemManager itemManager = new ItemManager();
        QuestManager questManager = new QuestManager();

        private bool isGameEnd = false;
        private GameState gameState = GameState.NONE;
        public void DrawMap()
        {
            dungeonManager = new DungeonManager(player);
            while (!isGameEnd)
            {
                switch (gameState)
                {
                    case GameState.PlayerInfo:
                        ShowStatus();
                        break;
                    case GameState.StartBattle:
                        ShowBattle();
                        break;
                    case GameState.Inventory:
                        ShowInventory();
                        break;
                    case GameState.Shop:
                        ShowShop();
                        break;
                    case GameState.Quest:
                        ShowQuest();
                        break;
                    case GameState.EndGame: // 게임 종료
                        isGameEnd = true; // while문 종료
                        break;
                    case GameState.Rest:
                        ShowRest();
                        break;
                    default:
                        itemManager.Mode = 0; // 메인 화면 호출 시 Mode 기본값으로 변경
                        StartGame(); 
                        break;
                }
            }
            // 저장 후 게임 종료
            EndGame();
        }

        private void ShowRest()
        {
            Console.Clear();

            Utilities.TextColor("휴식하기\n", ConsoleColor.DarkYellow);
            Console.WriteLine("체력 + 100 , 마나 100 회복합니다.");
            Console.WriteLine("휴식하시려면 500 G 를 지불하세요");
            Console.WriteLine();

            Console.WriteLine("1. 휴식하기");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            int input = (int)Utilities.GetInputKey(0, 1);
            input--;
            switch (input)
            {
                case 0: // 
                    EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Rest, 500));
                    break;
                case -1:
                    break;
            }
            gameState = GameState.NONE;
        }

        private void EndGame()
        // 게임 종료 관련 메서드
        {
            Console.Clear();

            // Save_Data.json 파일에 저장하도록 ItemManager의 OnEvent() 호출
            EventManager.Instance.PostEvent(EventType.eGameEnd, "");

            Console.WriteLine("게임을 종료합니다. \n(Enter키를 눌러 진행하세요...)");
            Console.ReadLine();
        }

        private void ShowQuest()//퀘스트 목록을 보여준다.
        {
            bool isIn = true;
            int QuestCount;
            while (isIn)
            {
                Console.Clear();
                Utilities.TextColor("Quest!!\n", ConsoleColor.DarkYellow);
                bool isNull = questManager.ShowQuests(); //목록들 보여주고 퀘스트가 없다면 false 있다면 true

                Utilities.TextColorWithNoNewLine("0", ConsoleColor.DarkRed);
                Console.WriteLine(". 나가기");
                 
                if (isNull)//퀘스트가 있다면 
                {
                    Utilities.AddLine("원하시는 퀘스트를 선택해주세요.");
                    Utilities.Add(">>");
                    QuestCount = questManager.QuestCount;
                }
                else //퀘스트가 없다면
                {
                    Utilities.AddLine("원하시는 행동을 선택해주세요.");
                    Utilities.Add(">>");
                    QuestCount = 0;
                }
                 
                switch (Utilities.GetInputKey(0, QuestCount)) //퀘스트 노출된 만큼만 입력 받음
                {
                    case 0: //나가기 눌렀을 때 
                        isIn = false;
                        break;
                    case 1:
                        Console.Clear();
                        questManager.ShowQuest(0);//퀘스트 들어가서 보기
                        break;
                    case 2:
                        Console.Clear();
                        questManager.ShowQuest(1);
                        break;
                    case 3:
                        Console.Clear();
                        questManager.ShowQuest(2);
                        break;
                }
            }
            Console.Clear();
            gameState = GameState.NONE;//게임 상태를 마을로 다시 가기
        }

        public void StartGame()
        {
            Console.Clear();

            Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.");
            Console.WriteLine("이제 전투를 시작할 수 있습니다.");
            Console.WriteLine("");

            Console.WriteLine("1. 상태 보기");
            Console.Write("2. 전투 시작 (현재 진행 : ");
            Utilities.TextColorWithNoNewLine($"{player.dungeonStage + 1}", ConsoleColor.DarkRed);       // 현재 플레이어가 입장 가능한 스테이지 출력
            Console.WriteLine("층)");
            Console.WriteLine("3. 인벤토리 보기");
            Console.WriteLine("4. 상점 보기");
            Console.WriteLine("5. 퀘스트 보기");
            Console.WriteLine("6. 휴식하기");
            Console.WriteLine("");
            Console.WriteLine("0. 종료");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            switch ((GameState)Utilities.GetInputKey(0, 6))
            {
                case GameState.PlayerInfo: // 상태 보기
                    gameState = GameState.PlayerInfo;
                    break;
                case GameState.StartBattle: // 전투 시작
                    gameState = GameState.StartBattle;
                    break;
                case GameState.Inventory: // 인벤토리 보기
                    gameState = GameState.Inventory;
                    break;
                case GameState.Shop:
                    gameState = GameState.Shop;
                    break;
                case GameState.Quest:
                    gameState = GameState.Quest;
                    break;
                case GameState.EndGame: // 입력값이 0이라면 
                    gameState = GameState.EndGame; // gameState를 EndGame으로 변경
                    break;
                case GameState.Rest:
                    gameState = GameState.Rest;
                    break;
            }
        }

        private void ShowBattle()
        {
            // 던전 스테이지 선택.
            // 플레이어의 던전 클리어 여부에 따라 진입 가능한 던전 난이도가 높아집니다.
            dungeonManager.SelectDungeonStage(player.dungeonStage);

            // StartGame()으로 돌아가기
            gameState = GameState.NONE;
        }

        private void ShowStatus()
        {
            Console.Clear();

            player.PlayerStats();

            Console.WriteLine($"소지금 : {player.Gold} G");
            Console.WriteLine("");

            Console.WriteLine("0. 나가기");
            Console.WriteLine("");
            

            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">>");
            switch (Utilities.GetInputKey(0, 0))
            {
                case 0:
                    gameState = GameState.NONE; // StartGame()으로 돌아가기
                    break;
            }
        }

        private void ShowInventory()
        /* 인벤토리 UI, itemManager.Mode에 따라 다른 내용 출력
            mode=0 일 때: 기본 인벤토리 아이템 목록만 출력 (번호 없음)
            mode=1 일 때: 장착 여부와 함께 인벤토리 아이템 목록 출력 (장착 중인 아이템은 [E]로 표시)
        */
        {
            Console.Clear();

            Utilities.TextColor("인벤토리", ConsoleColor.Yellow);
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine("");

            itemManager.ShowInventory(itemManager.Mode);
            Console.WriteLine("");

            if (itemManager.Mode == 0) // Mode가 0일 때(기본 상태)
            {
                Console.WriteLine("1. 장착 관리");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("");

                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");
                switch (Utilities.GetInputKey(0, 1))
                {
                    case 0:
                        gameState = GameState.NONE; // StartGame()으로 돌아가기
                        break;
                    case 1:
                        itemManager.Mode = 1;
                        gameState = GameState.Inventory; // Mode를 1로 바꾸고 다시 ShowInventory() 호출
                        break;
                }
            }
            else if (itemManager.Mode == 1) // Mode가 1일 때(장착 관리 상태)
            {
                Console.WriteLine("0. 나가기");
                Console.WriteLine("");

                Console.WriteLine("장착할 아이템 번호를 입력해주세요.");
                Console.Write(">>");
                int itemNum = Utilities.GetInputKey(0, itemManager.GetInventorySize);
                switch (itemNum)
                {
                    case 0:
                        itemManager.Mode = 0;
                        gameState = GameState.NONE; // StartGame()으로 돌아가기
                        break;
                    default:
                        itemManager.EquipItem(itemNum); // 아이템 장착/해제
                        break;
                }
            }
        }

        private void ShowShop()
        /* 상점 UI, itemManager.Mode에 따라 다른 내용 출력
            mode=0 일 때: 기본 상점 아이템 목록만 출력 (번호 없음)
            mode=1 일 때: 번호와 함께 상점 아이템 목록 출력
            mode=2 일 때: 번호와 함께 '인벤토리' 판매 가능 아이템 목록 출력
        */
        {
            Console.Clear();

            Utilities.TextColor("상점", ConsoleColor.Yellow);
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine("");

            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.Gold} G");
            Console.WriteLine("");

            if (itemManager.Mode == 0) // Mode가 0일 때(기본 상태)
            {
                itemManager.ShowShop(itemManager.Mode);
                Console.WriteLine("");

                Console.WriteLine("1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("");

                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");
                switch (Utilities.GetInputKey(0, 2))
                {
                    case 0:
                        gameState = GameState.NONE; // StartGame()으로 돌아가기
                        break;
                    case 1:
                        itemManager.Mode = 1;
                        gameState = GameState.Shop; // Mode를 1로 바꾸고 다시 ShowShop() 호출
                        break;
                    case 2:
                        itemManager.Mode = 2; // Mode를 2로 바꾸고 다시 ShowShop() 호출
                        gameState = GameState.Shop;
                        break;
                }
            }
            else if (itemManager.Mode == 1) // Mode가 1일 때(아이템 구매 상태)
            {
                itemManager.ShowShop(itemManager.Mode);
                Console.WriteLine("");

                Console.WriteLine("0. 나가기");
                Console.WriteLine("");

                Console.WriteLine("구매할 아이템 번호를 입력해주세요.");
                Console.Write(">>");
                int itemNum = Utilities.GetInputKey(0, itemManager.GetShopDisplaySize);
                switch (itemNum)
                {
                    case 0:
                        itemManager.Mode = 0;
                        gameState = GameState.Shop; // ShowShop()-Mode0 으로 돌아가기
                        break;
                    default:
                        itemManager.BuyItem(itemNum, player.Gold); // 아이템 구매
                        break;
                }
            }
            else if (itemManager.Mode == 2) // Mode가 2일 때(아이템 판매 상태)
            {
                itemManager.ShowInventory(itemManager.Mode);
                Console.WriteLine("");

                Console.WriteLine("0. 나가기");
                Console.WriteLine("");

                Console.WriteLine("판매할 아이템 번호를 입력해주세요. (정가의 85%를 지급 받습니다.)");
                Console.Write(">>");
                int itemNum = Utilities.GetInputKey(0, itemManager.GetInventorySize);
                switch (itemNum)
                {
                    case 0:
                        itemManager.Mode = 0;
                        gameState = GameState.Shop; // ShowShop()-Mode0 으로 돌아가기
                        break;
                    default:
                        itemManager.SellItem(itemNum, player.Gold); // 아이템 판매
                        break;
                }
            }
        }
    }
}