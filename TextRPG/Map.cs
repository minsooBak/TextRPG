
namespace TextRPG
{
    //맵의 이동 처리
    internal class Map
    {
        enum GameState
        {
            NONE,
            PlayerInfo ,
            StartBattle,
            Inventory,
            Shop,
            Quest,
            EndGame
        }

        Player player = new Player();
        DungeonManager dungeonManager;
        //MonsterManager monsterManager = new MonsterManager();
        ItemManager itemManager = new ItemManager();
        QuestManager questManager = new QuestManager();

        private bool isGameEnd = false;
        private GameState gameState = GameState.NONE;
        public void DrawMap()
        {
            //Tuple<ePlayerType, 10>
            //EventManager.Instance.PostEvent(EventType.Player, new KeyValuePair<ePlayerType, Tuple<int, int>>(ePlayerType.Stats, new Tuple<int, int>(10, 10)));

            //EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.HP, -10));
            //EventManager.Instance.PostEvent(EventType.Player, Utilities.EventPair(ePlayerType.Stats,new int[] { 300,200}));
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
                        isGameEnd = true;
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

        private void EndGame()
        {
            Console.Clear();

            // Save_Data.json 파일에 저장
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
            Console.WriteLine("2. 전투 시작");
            Console.WriteLine("3. 인벤토리 보기");
            Console.WriteLine("4. 상점 보기");
            Console.WriteLine("5. 퀘스트 보기");
            Console.WriteLine("");
            Console.WriteLine("0. 종료");

            //스킬 출력 예제
            //skillManager.ShowSkillList("전사");
            //skillManager.ShowSkillList("공허충");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            switch ((GameState)Utilities.GetInputKey(0, 5))
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
                case GameState.NONE: // 입력값이 0이라면 게임 종료
                    gameState = GameState.EndGame;
                    break;
            }
        }

        private void ShowBattle()
        {
            // 던전 스테이지 선택.
            // 플레이어의 던전 클리어 여부에 따라 진입 가능한 던전 난이도가 높아집니다.
            // 현재는 SelectDungeonStage의 매개변수에 따라 난이도를 조절해주세요.
            dungeonManager.SelectDungeonStage(dungeonManager.dungeonStage);

            // 플레이어의 HP가 0이 되면 게임 종료하기
            // if(player.Hp <= 0) isGameEnd = true;

            // 전투 종료 후 몬스터 리스트 초기화
            EventManager.Instance.PostEvent(EventType.eClearMonsters, "");
            // StartGame()으로 돌아가기
            gameState = GameState.NONE;
        }

        private void ShowStatus()
        {
            Console.Clear();

            Utilities.TextColor("상태 보기", ConsoleColor.Yellow);
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine("");

            Console.WriteLine($"Lv. {player.Level}");
            Console.WriteLine($"{player.Name} ({player.Class})");
            Console.WriteLine($"공격력 : {player.ATK}");
            Console.WriteLine($"방어력 : {player.DEF}");
            Console.WriteLine($"체력 : {player.Health}");
            Console.WriteLine($"마나 : {player.MP}");
            Console.WriteLine($"소지금 : {player.Gold}");
            Console.WriteLine("");

            Console.WriteLine("0. 나가기");
            Console.WriteLine("");
            

            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">>");
            switch ((GameState)Utilities.GetInputKey(0, 0))
            {
                case GameState.NONE:
                    gameState = GameState.NONE; // StartGame()으로 돌아가기
                    StartGame();
                    break;
            }
        }

        private void ShowInventory()
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
                        itemManager.EquipItem(itemNum);
                        break;
                }
            }
        }

        private void ShowShop()
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
                        itemManager.Mode = 2;
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
                        itemManager.BuyItem(itemNum, player.Gold);
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
                        itemManager.SellItem(itemNum, player.Gold);
                        break;
                }
            }
        }
    }
}