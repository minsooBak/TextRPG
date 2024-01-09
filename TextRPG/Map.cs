using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    //추민규님 마을 <-> 던전or상태보기 구현
    internal class Map
    {
        enum GameState
        {
            PlayerInfo,
            StartBatlle
        }

        public void StartGame()
        {
            while (true) 
            {
                Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.");
                Console.WriteLine("이제 전투를 시작할 수 있습니다.");
                Console.WriteLine();

                Console.WriteLine("1. 상태 보기");
                Console.WriteLine("2. 전투 시작");
                Console.WriteLine();

                Console.WriteLine("원하시는 행동을 입력해주세요.");
                switch ((GameState)Utilities.GetInputKey(1, 2, ConsoleColor.Yellow, ">>"))
                {
                    case GameState.PlayerInfo:
                        break;
                    case GameState.StartBatlle:
                        break;
                }
            }
        }
    }
}
