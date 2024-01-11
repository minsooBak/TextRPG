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
            StartBattle
        }

        MonsterManager monsterManager = new MonsterManager();
        DungeonManager dungeonManager = new DungeonManager();
        Player player = new Player();

        public void StartGame()
        {
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
            dungeonManager.GetPlayer(player);

            EventManager.Instance.PostEvent(EventType.eMakeMonsters);      // 몬스터 생성하기

            dungeonManager.Encounter(monsterManager.dungeonMonsters);
        }
    }
}
