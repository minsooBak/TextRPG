using System.Text.RegularExpressions;

namespace TextRPG
{
    internal class CreatePlayer
    {
        //캐릭터 생성 및 출력
        public KeyValuePair<string, string> Create()
        {
            Utilities.AddLine("RPG_GAME에 오신것을 환영합니다!");
            Utilities.AddLine("이곳은 캐릭터 생성을 하는 곳 입니다.\n");
            Utilities.AddLine("1. 캐릭터 생성");
            Utilities.AddLine("\n0. 종료\n");
            Utilities.AddLine("원하시는 행동을 입력해주세요");
            Utilities.Add(">>");
            int key = Utilities.GetInputKey(0, 1);
            switch (key)
            {
                case 0:
                    {
                        Console.Clear();
                        return new KeyValuePair<string, string>();
                    }
                case 1:
                    {
                        Console.Clear();
                        string name = CreateName();
                        Console.Clear();
                        string job = CreateJob();
                        Console.Clear();
                        return new KeyValuePair<string, string>(name, job);
                    }
                default:
                    {
                        Console.Error.WriteLine("CreatePlayer InputKey Error");
                        return new KeyValuePair<string, string>();
                    }
            }
            
        }

        //이름 만들기
        string CreateName()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("캐릭터 생성 - 이름");
                Console.ResetColor();
                Console.WriteLine("이름을 정해주세요!\n");
                Console.WriteLine("당신의 이름은? [이름 생성 규칙 : 띄워쓰기 금지 / 10글자 이내]");
                Console.Write(">>");
                string? str = Console.ReadLine();
                bool isCheck = Regex.IsMatch(str, @"[^a-zA-Z0-9가-힣]");
                if (str != null && str.Length <= 10 && isCheck == false)
                {
                    return str;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 이름입니다!");
                    Console.WriteLine("===================================================");
                }
            }
        }

        //클래스 정하기
        string CreateJob()
        {
            Utilities.AddLine("직업을 정해주세요!\n");
            Utilities.AddLine("1. 전사");
            Utilities.AddLine("2. 마법사");
            Utilities.AddLine("3. 궁수");
            Utilities.AddLine("4. 도적");
            Utilities.AddLine("당신의 직업은?");
            Utilities.Add(">>");
            int key = Utilities.GetInputKey(1, 4, ConsoleColor.DarkYellow, "캐릭터 생성 - 직업");
            switch (key)
            {
                case 1:
                    Console.WriteLine("전사 기본스텟 : 체력 500, 마나 100, 공격력 300, 방어력 100");
                    Console.WriteLine("전직 완료. 마을로 입장합니다.");
                    Thread.Sleep(1500);
                    return "전사";
                case 2:
                    Console.WriteLine("마법사 기본 스텟 : 체력 200, 마나 500, 공격력 250, 방어력 50");
                    Console.WriteLine("전직 완료. 마을로 입장합니다.");
                    Thread.Sleep(1500);
                    return "마법사";
                case 3:
                    Console.WriteLine("궁수 기본 스텟 : 체력 250, 마나 300, 공격력 400, 방어력 80");
                    Console.WriteLine("전직 완료. 마을로 입장합니다.");
                    Thread.Sleep(1500);
                    return "궁수";
                case 4:
                    Console.WriteLine("도적 기본 스텟 : 체력 350, 마나 300, 공격력 450, 방어력 60");
                    Console.WriteLine("전직 완료. 마을로 입장합니다.");
                    Thread.Sleep(1500);
                    return "도적";
                default:
                    {
                        Console.Error.WriteLine("Player Job Input Error");
                        break;
                    }

            }
            return "";
        }
    }
}
