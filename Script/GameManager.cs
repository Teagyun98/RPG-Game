using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 캐릭터 기본 스탯 클래스
[Serializable]
public class CharacterStatus
{
    public string name;
    public int hp;
    public int attackPower;
    public int attackDistance;
    public float attackDelay;
    public float skillDistance;
    public float skillDelay;
}

public class GameManager : MonoBehaviour
{
    // 싱글톤
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance == null ? null : instance; }
        private set { instance = value; }
    }

    // 몬스터 기초값
    [Header("GameSetting\n\nMoster")]
    [SerializeField] private MonsterController goblin;
    public float monsterSpawnTime;
    public int monsterMaxActiveCount;
    public int monsterHp;
    public int monsterAttackPower;
    public int monsterAttackDelay;
    public float mosterAttackDistance;
    public float mosterSearchRange;
    // 캐릭터 기초값
    [Header("Character")]
    public float characterSpawnTime;
    public List<CharacterStatus> characterStatusList;

    [Header("GameInfo")]
    // 캐릭터 리스트
    [SerializeField] private List<CharController> charList;
    // 몬스터 스폰 Transform
    [SerializeField] private Transform monsterArea;
    [SerializeField] private Button gameStartBtn;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI goldText;
    // 몬스터 UI
    [SerializeField] private Transform monsterUIArea;
    [SerializeField] private MonsterUI monsterUI;
    [SerializeField] private EffectArea effectArea;

    public bool GameOver { get; private set; }
    public int Stage { get; private set; }
    public int stageExp;
    public int Gold { get; private set; }

    private void Awake()
    {
        // 싱글톤
        if (instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 게임 시작시에는 LandscapeLeft로 고정 하지만 게임 중에는 화면 회전이 가능하도록 변경
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToLandscapeLeft = true;

        GameOver = true;
        // 고블린 소환 함수
        StartCoroutine(SpawnGoblin());
    }

    private void FixedUpdate()
    {
        // 카메라가 첫번째 캐릭터를 따라 이동
        if (FirstChar() != null)
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position,FirstChar().transform.position + new Vector3(0,0,-10), 2);
    }

    // 가까이 있는 캐릭터 반환 함수
    public CharController NearChar(Vector2 pos)
	{
        // 캐릭터 리스트에서 죽지 않은 캐릭터 중 인자르 받은 위치에서 몬스터 탐색범위 안의 가장 가까운 캐릭터 반환
        CharController result = null;

        foreach(CharController character in charList)
		{
            if (Vector2.Distance(pos, character.transform.position) < mosterSearchRange && character.Sm.CurState != character.DicState[CharState.Die])
			{
                if (result == null || Vector2.Distance(result.transform.position, pos) > Vector2.Distance(character.transform.position, pos))
                    result = character;
			}
		}

        return result;
	}

    // 가까이 있는 몬스터 반환 함수
    public MonsterController NearMonster(Vector2 pos, float range) 
    {
        // MonsterArea에서 살아있는 몬스터 중 인자로 받은 위치와 탐색 범위 안에서 가장 가까운 몬스터 반환
        MonsterController result = null;
        MonsterController monster;

        for (int i = 0; i< monsterArea.childCount; i++)
        {
            monster = null;

            if(monsterArea.GetChild(i).GetComponent<MonsterController>() && monsterArea.GetChild(i).gameObject.activeSelf == true)
            {
                monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

                if ( monster.Sm != null && monster.Sm.CurState == monster.DicState[MonsterState.Death])
                    monster = null;
            }

            // 첫번째 캐릭터는 탐색 범위와 상관없이 몬스터를 쫒는다.
            if(monster != null && (range == 0 || Vector2.Distance(pos, monster.transform.position) <= range))
            {
                if(result == null || Vector2.Distance(pos, result.transform.position) > Vector2.Distance(pos, monster.transform.position))
                    result = monster;
            }
        }

        return result;
    }

    // 캐릭터 리스트에서 살아있는 첫번째 캐릭터 반환
    public CharController FirstChar()
    {
        CharController result = null;

        for(int i = 0; i < charList.Count; i++) 
        {
            if (charList[i].Sm.CurState != charList[i].DicState[CharState.Die])
            {
                result = charList[i];
                break;
            }
        }

        return result;
    }

    // 고블린 스폰 코루틴
    public IEnumerator SpawnGoblin()
    {
        int activeMonsterNum = 0;

        // 활동중인 몬스터 수 확인
        for(int i = 0; i < monsterArea.childCount; i++)
		{
            MonsterController monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

            if (monster != null && monster.gameObject.activeSelf == true && monster.Sm.CurState != monster.DicState[MonsterState.Death])
                activeMonsterNum++;
		}

        if(GameOver == false && activeMonsterNum < 5)
		{
            MonsterController _goblin = null;

            // 풀링
            for (int i = 0; i < monsterArea.childCount; i++)
            {
                MonsterController monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

                if (monster.Sm.CurState == monster.DicState[MonsterState.Death] || monster.gameObject.activeSelf == false)
                {
                    _goblin = monster;

                    // 비활성화된 고블리 활성화
                    if (_goblin.gameObject.activeSelf == false)
                        _goblin.gameObject.SetActive(true);

                    // 스테이지 경험치를 확인하여 보스 생성 여부 확인
                    bool boss = false;

                    if(stageExp > 10)
					{
                        stageExp -= 10;
                        boss = true;
					}

                    // 기존 오브젝트를 이용할 경우 보스로 부활시키기
                    _goblin.Resurrection(boss);
                    break;
                }
            }

            if (_goblin == null)
			{
                _goblin = Instantiate(goblin, monsterArea);

                // 보스 여부 확인
                bool boss = false;

                if (stageExp > 10)
                {
                    stageExp -= 10;
                    boss = true;
                }

                // 새로 생성한 몬스터일 경우 스텟을 세팅하며 보스로 생성
                _goblin.SetStatus(boss);
            }

            _goblin.transform.position = FirstChar().transform.position + new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));

        }

        yield return new WaitForSeconds(monsterSpawnTime);

        StartCoroutine(SpawnGoblin());
    }

    // 캐릭터가 모두 사망시 게임 오버
    public void CheckGameOver()
	{
        foreach(CharController character in charList)
		{
            if(character.Sm.CurState != character.DicState[CharState.Die])
			{
                GameOver = false;
                return;
			}
		}

        GameOver = true;
        gameStartBtn.gameObject.SetActive(true);
	}

    // 살아있는 캐릭터 중 최대체력 대비 체력이 가장 적은 캐릭터 반환
    public CharController GetLowHpChar(Vector2 pos , float distance)
	{
        CharController result = null;

        foreach (CharController character in charList)
		{
            if (character.hp < character.status.hp && Vector2.Distance(pos, character.transform.position) <= distance && character.Sm.CurState != character.DicState[CharState.Die])
                result = result == null || result.hp / result.status.hp > character.hp / character.status.hp ? character : result;
		}

        return result;
	}

    // 살아있는 몬스터 중 인자로 받은 위치에서 범위 내 모든 몬스터를 반환
    public List<MonsterController> GetDistanceMonsters(Vector2 pos, float distance)
	{
        List<MonsterController> list = new List<MonsterController>();

        for (int i = 0; i < monsterArea.childCount; i++)
        {
            MonsterController monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

            if (monster.Sm.CurState != monster.DicState[MonsterState.Death] && Vector2.Distance(pos, monster.transform.position) <= distance)
                list.Add(monster);
        }

        return list;
	}

    public void GameStart()
	{
        // Game에 사용되는 변수들 초기화
        GameOver = false;
        Stage = 1;
        stageText.text = $"Stage{Stage}";
        stageExp = 0;
        Gold = 0;
        goldText.text = $"Gold : {Gold}";

        // 몬스터 비활성화
        for (int i =  0; i < monsterArea.childCount; i++)
		{
            monsterArea.GetChild(i).gameObject.SetActive(false);
		}

        // 몬스터 UI 비활성화
        for(int i = 0; i< monsterUIArea.childCount; i++)
        {
            monsterUIArea.GetChild(i).gameObject.SetActive(false);
        }

        // 캐릭터 초기화
        for(int i = 0; i<charList.Count; i++)
		{
            charList[i].transform.position = new Vector3(i, 0, 0);
            charList[i].ResetChar();
		}

        gameStartBtn.gameObject.SetActive(false);
	}

    // 스테이지 등반 함수
    public void NextStage()
	{
        Stage++;
        stageText.text = $"Stage{Stage}";
	}

    // 골드 획득 함수
    public void SetGold(int amount)
    {
        Gold += amount;

        goldText.text = $"Gold : {Gold}";
    }

    // 몬스터 UI를 반환해주는 함수
    public MonsterUI GetMonsterUI(MonsterController controller)
    {
        MonsterUI result = null;

        for(int i = 0; i< monsterUIArea.childCount; i++) 
        {
            MonsterUI ui = monsterUIArea.GetChild(i).GetComponent<MonsterUI>();

            if(ui.gameObject.activeSelf == false)
            {
                ui.gameObject.SetActive(true);
                result = ui;
                break;
            }    
        }

        if(result == null)
            result = Instantiate(monsterUI, monsterUIArea);

        result.SetMonster(controller);

        return result;
    }

    // 스킬 효과 함수
    public void SetEffect(Vector3 pos, string color)
    {
        effectArea.Set(pos, color);
    }
}