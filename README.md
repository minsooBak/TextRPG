# 데마시아스토리(DemaciaStory)
데마시아스토리(DemaciaStory)
## 프로젝트 소개
메이플 월드의 용사들이 소환사의 던전을 탐험하는 콘솔 앱 게임입니다.


## 개발 기간
24.01.09~24.01.16

### 멤버 구성
* 팀장(송상화) : 던전매니저 및 전투 구현

* 팀원(박민수) : 기초 베이스, 캐릭터 생성

* 팀원(정원우) : 몬스터매니저 및 맵 이동 구현

* 팀원(추민규) : 아이템매니저 및 인벤토리 구현

* 팀원(유시아) : 플레이어 구현


### 개발 환경
- 'Visual Studio 2022'
- 'C# Net.8.0'

## 주요 기능
* 필수 요구 사항
  * 게임 시작 화면
  * 상태 보기
  * 전투 시작
     
* 선택 요구 사항
  * 캐릭터 생성 기능
  * 직업 선택 기능
  * 스킬 기능
  * 치명타 기능
  * 회피 기능
  * 레벨업 기능
  * 보상 추가
  * 콘솔 꾸미기
  * 몬스터 종류 추가
  * 아이템 적용
  * 스테이지 추가
  * 게임 저장하기
      
* 기타 기능
  * 퀘스트 기능
  * 퀘스트 선택과 완료
  * 휴식
  * 인벤토리, 아이템 장착/해제
  * 상점, 아이템 구매/판매


## 팀원 별 구현한 기능
* 팀장(송상화)
  * DungeonManager의 Dungeon 클래스와 던전 몬스터 생성, 전투 기능 중 공격과 출력문을 구현.
  * MonsterManager의 GetExp, GetReward와 Monster 클래스의 Attack과 TakeDamage, ShowStats 부분을 구현.
  * Player 클래스에서 Attack과 TakeDamage, ShowStats을 구현.
    
* 팀원(박민수)
  * 게임의 전체적인 기반이 되는 기능을 구현.
  * Utilities 전체 구현. (정적클래스, 자주쓰는 메서드들을 모아둔 클래스 = 데이터 저장, 불러오기, 키입력받기, 폰트의 색깔을 바꾸고 출력 뒤 리셋, 이벤트데이터를 보낼때 타입과 데이터를 묶어주는 함수)
  * EventManager 전체 구현. (싱글톤클래스, 이벤트타입별로 들을 클래스들을 등록 및 이벤트 타입별에 맞는 클래스들에게 이벤트 발생 시 타입과 데이터 전달)
    
* 팀원(정원우)
  * SkillManager : 스킬 할당, 스킬 출력, 스킬 Json 파일 구현.
  * QuestManager : 퀘스트 할당, 퀘스트 출력 , 퀘스트 보상, 퀘스트 Json 파일 구현.
  * Map : 휴식 기능 , 퀘스트 기능 구현.
    
* 팀원(추민규)
  * ItemManager : 인벤토리 출력, 상점 출력, 장비 장착/해제, 아이템 구매/판매, 아이템 정보 저장, 아이템 Json 파일 구현.
  * Map : ShowInventory, ShowShop 및 게임 종료 기능 구현.
    
* 팀원(유시아)
  * Player : 전반적인 이벤트처리 코드 수정 및 정리, 플레이어의 직업 별 기초 스탯, 경험치 증가 구현.
  * CreatePlayer : 직업 선택 시 부가 설명 기능 구현.
