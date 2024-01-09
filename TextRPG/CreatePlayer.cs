﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextRPG
{
    internal class CreatePlayer
    {
        public string Create()
        {
            while (true)
            {
                Utilities.AddLine("RPG_GAME에 오신것을 환영합니다!");
                Utilities.AddLine("이곳은 캐릭터 생성을 하는 곳 입니다.\n");
                Utilities.AddLine("1. 캐릭터 생성");
                Utilities.AddLine("\n0. 종료\n");
                Utilities.AddLine("원하시는 행동을 입력해주세요");
                Utilities.Add(">>");
                int key = Utilities.GetInputKey(0, 1);
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
                Utilities.TextColor("캐릭터 생성 - 이름", ConsoleColor.DarkYellow);
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