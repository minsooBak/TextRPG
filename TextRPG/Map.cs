using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static TextRPG.Player;

namespace TextRPG
{
    //추민규님 마을 <-> 던전or상태보기 구현
    internal class Map
    {
        enum GameState
        {
            PlayerInfo = 1,
            StartBattle,
            Inventory,
            Shop,
            NONE
        }


        Player player = new Player();
        MonsterManager monsterManager = new MonsterManager();
        DungeonManager dungeonManager;
        ItemManager itemManager = new ItemManager();
        private bool isGameEnd = false;
        private GameState gameState = GameState.NONE;

        public void DrawMap()
        {
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
                    default:
                        StartGame();
                        break;
                }
            }
        }

        public void StartGame()
        {
            dungeonManager = new DungeonManager(player);
            Utilities.AddLine("스파르타 던전에 오신 여러분 환영합니다.");
            Utilities.AddLine("이제 전투를 시작할 수 있습니다.");
            Utilities.AddLine("");

            Utilities.AddLine("1. 상태 보기");
            Utilities.AddLine("2. 전투 시작");
            Utilities.AddLine("");

            //스킬 출력 예제
            //skillManager.ShowSkillList("전사");
            //skillManager.ShowSkillList("공허충");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            switch ((GameState)Utilities.GetInputKey(1, 2))
            {
                case GameState.PlayerInfo: // 상태 보기
                    gameState = GameState.PlayerInfo;
                    break;
                case GameState.StartBattle: // 전투 시작
                    gameState = GameState.StartBattle;
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
            EventManager.Instance.PostEvent(EventType.eClearMonsters);
            // StartGame()으로 돌아가기
            gameState = GameState.NONE;
        }

        private void ShowStatus()
        {
            Utilities.TextColor("상태 보기", ConsoleColor.Yellow);
            Utilities.AddLine("캐릭터의 정보가 표시됩니다.");
            Utilities.AddLine("");

            Utilities.AddLine($"Lv. {player.Level}");
            Utilities.AddLine($"{player.Name} (player.Class추가?)");
            Utilities.AddLine($"공격력 : {player.Atk}");
            Utilities.AddLine($"방어력 : {player.Def}");
            Utilities.AddLine($"체력 : {player.Hp}");
            Utilities.AddLine($"마나 : {player.Mp}");
            Utilities.AddLine($"소지금 : {player.Gold}");
            Utilities.AddLine("");

            Utilities.AddLine("0. 나가기");
            Utilities.AddLine("");
            

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            switch ((GameState)Utilities.GetInputKey(1, 1))
            {
                default:
                    gameState = GameState.NONE; // StartGame()으로 돌아가기
                    break;
            }
        }

        private void ShowInventory()
        {
            Utilities.TextColor("인벤토리", ConsoleColor.Yellow);
            Utilities.AddLine("보유 중인 아이템을 관리할 수 있습니다.");
            Utilities.AddLine("");

            itemManager.ShowInventory();
            Utilities.AddLine("");

            Utilities.AddLine("1. 장착 관리");
            Utilities.AddLine("0. 나가기");
            Utilities.AddLine("");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            switch ((GameState)Utilities.GetInputKey(1, 2))
            {
                default:
                    gameState = GameState.NONE; // StartGame()으로 돌아가기
                    break;
            }
        }

        private void ShowShop()
        {
            Utilities.TextColor("상점", ConsoleColor.Yellow);
            Utilities.AddLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Utilities.AddLine("");

            Utilities.AddLine("[보유 골드]");
            Utilities.AddLine($"{player.Gold} G");
            Utilities.AddLine("");

            itemManager.ShowShop();

            Utilities.AddLine("1. 아이템 구매");
            Utilities.AddLine("0. 나가기");
            Utilities.AddLine("");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            Utilities.Add(">>");
            switch ((GameState)Utilities.GetInputKey(1, 2))
            {
                default:
                    gameState = GameState.NONE; // StartGame()으로 돌아가기
                    break;
            }
    }
}