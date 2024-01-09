using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextRPG
{
    internal class CreatePlayer
    {
        void TextColor(string str, ConsoleColor color1, ConsoleColor color2)
        {
            Console.ForegroundColor = color1;
            Console.WriteLine(str);
            Console.ForegroundColor = color2;
        }

        int GetInputKey(int min, int max)
        {
            while (true)
            {
                string? str = Console.ReadLine();
                if (str != null && int.TryParse(str, out int a))
                {
                    int key = int.Parse(str);
                    if (key > min && key < max)
                        return key;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다!");
                        Console.WriteLine("===================================================");
                    }
                }else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다!");
                    Console.WriteLine("===================================================");
                }
            }
        }

        public string Create()
        {
            while (true)
            {
                Console.WriteLine("RPG_GAME에 오신것을 환영합니다!");
                Console.WriteLine("이곳은 캐릭터 생성을 하는 곳 입니다.\n");
                Console.WriteLine("1. 캐릭터 생성");
                Console.WriteLine("\n0. 종료\n");
                Console.WriteLine("원하시는 행동을 입력해주세요");
                Console.Write(">>");
                int key = GetInputKey(0, 1);
                if (key == 0)
                {
                    Console.Clear();
                    return "";
                }
                else if (key == 1)
                {
                    Console.Clear();
                    string name = CreateName();
                    Console.Clear();
                    string job = CreateJob();
                    Console.Clear();
                    return name + " " + job;
                }
                else
                {
                    Console.Error.WriteLine("CreatePlayer InputKey Error");
                }
            }
        }
        string CreateName()
        {
            while (true)
            {
                TextColor("캐릭터 생성 - 이름", ConsoleColor.DarkYellow, ConsoleColor.Gray);
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

        string CreateJob()
        {
            while (true)
            {
                TextColor("캐릭터 생성 - 직업", ConsoleColor.DarkYellow, ConsoleColor.Gray);
                Console.WriteLine("직업을 정해주세요!\n");
                Console.WriteLine("1. 전사");
                Console.WriteLine("2. 마법사");
                Console.WriteLine("3. 궁수");
                Console.WriteLine("4. 도적");
                Console.WriteLine("당신의 직업은?");
                Console.Write(">>");
                int key = GetInputKey(1, 4);
                switch (key)
                {
                    case 1: return "전사";
                    case 2: return "마법사";
                    case 3: return "궁수";
                    case 4: return "도적";
                    default:
                        {
                            Console.Error.WriteLine("Player Job Input Error");
                            break;
                        }
                }
                break;
            }
            return "";
        }
    }
}
