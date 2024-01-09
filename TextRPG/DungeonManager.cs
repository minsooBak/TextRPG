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
        // 임시 플레이어 정보
        public int PlayerHp = 100;
        public DungeonManager() 
        {
            // 몬스터 정보 받아오기
            // 플레이어 정보 받아오기
        }

        // 플레이어 혹은 몬스터가 상대를 공격했을 때 결과
        // 결과 : 
        // 누가 공격했는지
        // 누구를 공격했는지, 준 데미지 표기

        // 공격 받은 객체의 HP 상태 출력, 반환


        public void OnEvent(EventType type, object data)
        {
            throw new NotImplementedException();
        }
    }
}
