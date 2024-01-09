using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    enum MapType
    {
        NONE,
        Dungeon
    }

    //추민규님 마을 <-> 던전or상태보기 구현
    internal class Map
    {
        private DungeonManager dungeonManager;
        private MonsterManager monsterManager;
        private MapType mapType = MapType.NONE;

        public Map()
        {
            dungeonManager = new DungeonManager();
            //EventManager.Instance.PostEvent(EventType.NONE);
        }

        public void DrawMap()
        {
            while (true)
            {
                switch (mapType)
                {
                    case MapType.Dungeon:
                        {
                            ShowDungeon();
                            break;
                        }
                    default:
                        {
                            ShowTown();
                            break;
                        }
                }
            }
        }

        private void ShowDungeon()
        {
            // 전투

            Console.WriteLine("Battle!!\n");

            // 몬스터 출력
            Random random = new Random();
            int monsterCount = random.Next(1, 5);
            MonsterType[] monsterTypes = new MonsterType[monsterCount];
            Monster[] monsters = new Monster[monsterCount];

            for (int i = 0; i < monsterCount; i++)
            {
                int rand = random.Next(0, 3);
                // 몬스터 생성
                if(rand == (int)MonsterType.Monster1)
                {
                    monsterTypes[i] = MonsterType.Monster1;
                }
                else if(rand == (int)MonsterType.Monster2)
                {
                    monsterTypes[i] = MonsterType.Monster2;
                }
                else
                {
                    monsterTypes[i] = MonsterType.Monster3;
                }
            }

            int j = 0;
            foreach (var monsterType in monsterTypes)
            {
                monsters[j] = new Monster(monsterType);
                Console.WriteLine($"Lv. {monsters[j].Lv} {monsters[j].Name} HP {monsters[j].Hp}");
                j++;
            }

            // 내 정보 출력
            Console.WriteLine("\n[내정보]");
            Console.WriteLine("Lv. 1  Chad (전사)");
            Console.WriteLine("HP 100/100\n");

            // 1. 공격
            Console.WriteLine("1. 공격");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">> ");

            string? str = Console.ReadLine();

            if (str != null && int.TryParse(str, out int a))
            {
                int type = int.Parse(str);

                if (type == 1)
                {
                    while (true)
                    {
                        Console.WriteLine("Battle!!\n");

                        // 몬스터 출력
                        j = 0;
                        foreach (var monsterType in monsterTypes)
                        {
                            monsters[j] = new Monster(monsterType);
                            Console.WriteLine($"{j + 1} Lv. {monsters[j].Lv} {monsters[j].Name} HP {monsters[j].Hp}");
                            j++;
                        }

                        // 내 정보 출력
                        Console.WriteLine("\n[내정보]");
                        Console.WriteLine("Lv. 1  Chad (전사)");
                        Console.WriteLine("HP 100/100\n");

                        Console.WriteLine("0. 취소\n");
                        Console.WriteLine("대상을 선택해주세요.");
                        Console.Write(">> ");

                        str = Console.ReadLine();

                        if (str != null && int.TryParse(str, out int b))
                        {
                            type = int.Parse(str);
                            if (type >= 1 && type <= monsterCount)
                            {
                                while (true)
                                {
                                    Console.WriteLine("Battle!!\n");

                                    // 몬스터 출력

                                    // 내 정보 출력
                                    Console.WriteLine("\n[내정보]");
                                    Console.WriteLine("Lv. 1  Chad (전사)");
                                    Console.WriteLine("HP 100/100\n");

                                    Console.WriteLine("0. 취소\n");
                                    Console.WriteLine("대상을 선택해주세요.");
                                    Console.Write(">> ");

                                    str = Console.ReadLine();

                                    if (str != null && int.TryParse(str, out int c))
                                    {
                                        type = int.Parse(str);

                                        // 플레이어가 선택한 몬스터에 대한 공격 결과
                                    }
                                    else
                                    {
                                        // 플레이어가 취소를 선택했을 때
                                        break;
                                    }

                                    // if(플레이어의 체력이 0이거나 || 몬스터의 상태가 모두 dead일 경우)
                                    // Battle!! - Result Victory
                                    // else
                                    // Battle!! - Result Lose

                                }
                            }
                        }
                        else break;     // 플레이어가 취소를 누를 경우
                    }
                }
            }
        }

        private void ShowTown()
        {
            while (true)
            {
                Console.WriteLine("스파르타 마을에 오신것을 환영합니다!");
                Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
                Console.WriteLine("2. 전투 시작\n");

                string? str = Console.ReadLine();

                if (str != null && int.TryParse(str, out int a))
                {
                    int type = int.Parse(str);

                    if (type > 0 && type < 3)
                    {
                        mapType = (MapType)(type - 1);
                        Console.Clear();
                        break;
                    }
                }
            }
        }
    }
}
