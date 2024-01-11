using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        }


        Player player = new Player();
        MonsterManager monsterManager = new MonsterManager();
        DungeonManager dungeonManager;
        
        public void StartGame()
        {
            dungeonManager = new DungeonManager(player);
            Utilities.AddLine("스파르타 던전에 오신 여러분 환영합니다.");
            Utilities.AddLine("이제 전투를 시작할 수 있습니다.");
            Utilities.AddLine("");

            Utilities.AddLine("1. 상태 보기");
            Utilities.AddLine("2. 전투 시작");
            Utilities.AddLine("");

            Utilities.AddLine("원하시는 행동을 입력해주세요.");
            switch ((GameState)Utilities.GetInputKey(1, 2, ConsoleColor.Yellow, ">>"))
            {
                case GameState.PlayerInfo: // 상태 보기

                    break;
                case GameState.StartBattle: // 전투 시작
                    ShowBattle();
                    break;
            }
        }

        private void ShowBattle()
        {
            // 던전 스테이지 선택.
            // 플레이어의 던전 클리어 여부에 따라 진입 가능한 던전 난이도가 높아집니다.
            // 현재는 SelectDungeonStage의 매개변수에 따라 난이도를 조절해주세요.
            dungeonManager.SelectDungeonStage(1);

            EventManager.Instance.PostEvent(EventType.eClearMonsters);
        }
    }
}

