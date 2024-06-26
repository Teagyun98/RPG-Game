<div align="center">
<h2>[2024] RPG Game 🎮</h2>
</div>

## 목차
  - [개요](#개요) 
  - [필수 구현 요소](#필수-기능)
  - [추가 기능](#추가-기능)

## 개요
- 프로젝트 이름: Project_CA
- 개발 엔진 및 언어: Unity & C#
- 개발 기간 : 5일

## 필수 기능
|![GameStart](https://github.com/Teagyun98/Project_CA/assets/120551471/b48bcdbe-c21f-4541-9b4a-e019bc0b36b0)|![InGame1](https://github.com/Teagyun98/Project_CA/assets/120551471/c2e8ebcd-67be-4e6a-84c0-ea5753311a56)|![InGame2](https://github.com/Teagyun98/Project_CA/assets/120551471/28b85fb2-a07f-4dd9-b153-b71cdf9295a5)|
|:---:|:---:|:---:|
|시작 화면|플레이 화면|플레이 화면|

캐릭터 직업은 탱커, 근거리 딜러, 원거리 딜러, 힐러 총 4개의 직업이 있습니다.<br>
카메라는 첫번째 캐릭터를 추적하며 첫번째 캐릭터가 사망시 다음 캐릭터를 추적합니다.<br>
게임 실행시 캐릭터 배치 슬롯이 나오고 게임시작 버튼을 누르면 게임이 시작됩니다.<br>
캐릭터는 사망시 5초 후 부활하며 모든 캐릭터가 사망하면 게임오버가 되고 다시 캐릭터 배치 슬롯이 나옵니다.<br>
몬스터는 5초 주기로 첫번째 캐릭터 주위에 소환되며 캐릭터가 추적 사거리에 들어오면 추적을 시작하고 사거리에서 없어질 경우 추적을 멈춥니다.<br>
캐릭터와 몬스터는 서로의 공격 거리에 들어오면 공격을 시작하며 몬스터는 단일공격, 캐릭터는 단일공격과 스킬공격을 사용합니다.<br>
몬스터 관련 변동 수치, 캐릭터 관련 변동 수치는 GameManager Object에서 코드의 수정 없이 Inspector창에서 수정 가능합니다.<br>
게임 화면 하단에 캐릭터 HP UI가 있습니다.<br>
캐릭터와 몬스터는 전투 상황에 따라 애니메이션을 재생합니다.<br>

## 추가 기능
캐릭터는 몬스터 처치시 경험치를 획득하고 레벨업 할 수 있습니다. 레벨은 화면 하단의 UI를 통해 확인할 수 있습니다.<br>
캐릭터와 몬스터 상단에 따라다니는 체력 게이지가 있습니다.<br>
몬스터 처치시 Gold를 획득하며 이를 소모하여 캐릭터를 레벨업 시킬 수 있습니다. (캐릭터 레벨업시 공격력과 체력 증가) 레벨업은 필요 골드를 가지고 있으면 하단 UI에 버튼이 활성화 됩니다.<br>
몬스터는 추적에 실패하거나 근처에 캐릭터가 없으면 첫번째 캐릭터를 기준으로 주위로 순찰합니다.<br>
몬스터를 10마리 사냥시 붉은색 보스 몬스터가 등장하며 일반 몬스터보다 체력 10배, 공격력 2배, 사거리 2배 수치가 적용되어 있습니다.<br>
보스 몬스터를 처치시 스테이지가 넘어가며 몬스터의 체력이 스테이지 단계 당 10% 씩 강화 됩니다.<br>
스킬 발동시 상태이상 스킬일 경우 노란색 파티클 공격 스킬일 경우 붉은색 파티클, 회복 스킬일 경우 녹색 파티클이 활성화 됩니다.<br>



