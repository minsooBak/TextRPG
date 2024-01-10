using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    //송상화님 던전 구현 
    // 전투 결과 구현하기
    internal class DungeonManager : IListener
    {
        public DungeonManager() 
        {
            // 몬스터 정보 받아오기
            // 플레이어 정보 받아오기
        }


        // 플레이어 혹은 몬스터가 상대를 공격했을 때 결과
        // 결과 : 
        // 누가 공격했는지
        // 누구를 공격했는지, 준 데미지 표기

        // 주는 데미지 결정
        // Hp 변화가 있을 시 Event.Type(eHpChange)로 postevent 해주기

        // 공격 받은 객체의 HP 상태 출력, 반환
        public void ShowBattle(Monster monster, Player player, bool isPlayerTurn)
        {
            Console.Clear();
            int playerHp = 100;     // 임시 플레이어 체력
            int playerAtk = 10;     // 임시 플레이어 공격력

            double getDamage;
            int damage;

            Console.WriteLine("Battle!!\n");

            if (isPlayerTurn)
            {
                getDamage = playerAtk / 100.0 * 10;
                damage = new Random().Next(playerAtk - (int)Math.Ceiling(getDamage), playerAtk + (int)Math.Ceiling(getDamage) + 1);

                Console.WriteLine("Chad 의 공격!");
                Console.WriteLine($"Lv.{monster.Lv} {monster.Name} 을(를) 맞췄습니다. [데미지 : {damage}]\n");

                Console.WriteLine($"Lv.{monster.Lv} {monster.Name}");
                Console.Write($"{monster.Hp} -> ");

                monster.Hp -= damage;
                if (monster.Hp <= 0) { monster.isDead = true; }

                Console.WriteLine($"{(monster.isDead ? "Dead" : monster.Hp)}");
            }
            else
            {
                damage = monster.Attack();
                Console.WriteLine($"Chad 을(를) 맞췄습니다. [데미지 : {damage}]\n");

                Console.WriteLine($"Lv.1 Chad");
                Console.Write($"{playerHp} -> {playerHp -= damage}");
            }

            Console.WriteLine("\n0. 다음\n");
            if (Utilities.GetInputKey(0, 0, ConsoleColor.Yellow, ">> ") == 0)
            {
                Console.Clear();
                return;
            }
        }

        public void ShowResult(int deadCounter, int monster)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Battle!! - Result\n");
            Console.ResetColor();

            if(deadCounter >= monster)
            {
                Console.WriteLine("Victory\n");

                Console.WriteLine($"던전에서 몬스터 {monster}마리를 잡았습니다\n");

                Console.WriteLine("Lv.1 Chad\n HP 100 -> 남은 체력\n");

                Console.WriteLine("0. 다음\n>> ");

                if (Utilities.GetInputKey(0, 0) == 0) return;
            }
            else
            {
                Console.WriteLine("You Lose\n");

                Console.WriteLine("Lv.1 Chad\n HP 100 -> 0\n");

                Console.WriteLine("0. 다음\n>> ");

                if (Utilities.GetInputKey(0, 0) == 0) return;
            }
        }

        public void OnEvent(EventType type, object data)
        {
            throw new NotImplementedException();
        }
    }
}
