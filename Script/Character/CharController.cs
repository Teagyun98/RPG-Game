using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 상태 Enum
public enum CharState
{
    Idle,
    Walk,
    Casting,
    Attack,
    Hurt,
    Die,
    Victory,
}

// 캐릭터 State Machine 패턴에 사용될 인터페이스
public interface CharState<T>
{
    // 캐릭터가 특정 상태에 진입 했을 때 실행될 함수
    void OperateEnter(T sender);
    // 특정 상태에 있을 때 실행될 Update문
    void OperateUpdate(T sender);
    // 특정 상태에 있을 때 실행될 FixedUpdate문
    void OperateFixedUpdate(T sender);
    // 특정 상태에서 빠져나갈 때 실행될 함수
    void OperateExit(T sender);
}

// 캐릭터 Idle상태
public class CharIdle : CharState<CharController>
{
    private CharController controller;

    // 상태에 진입할 때 해당 애니메이션의 bool 값을 true로 설정한다.
    public void OperateEnter(CharController sender)
	{
        controller = sender;

        controller.Animator.SetBool("Idle", true);
	}

    public void OperateUpdate(CharController sender) 
    {
        // 첫번째 캐릭터 가져오기
        CharController firstChar = GameManager.Instance.FirstChar();

        // 자신이 첫번째 캐릭터고 목표 몬스터가 있으면 Walk 상태로 변경, 자신이 첫번째 캐릭터가 아니고 첫번째 캐릭터가 Idle 상태가 아니면 Walk 상태로 변경
        if (((firstChar == controller && controller.target != null) || (firstChar != controller && firstChar.Sm.CurState != firstChar.DicState[CharState.Idle])))
            controller.Sm.SetState(controller.DicState[CharState.Walk]);

        // 공격이나 스킬범위에 몬스터가 있고 딜레이가 지났을때 Casting 상태로 넘어간다.
        if (controller.target != null)
        {
            if (Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.status.attackDistance)
                controller.Sm.SetState(controller.DicState[controller.attackDelay <= 0 ? CharState.Casting : CharState.Idle]);
            else if (Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.status.skillDistance && controller.skillDelay <= 0)
                controller.Sm.SetState(controller.DicState[CharState.Casting]);
        }
    }
    public void OperateFixedUpdate(CharController sender) { }

    // 해당 상태에서 빠져나갈 때 bool 값을 false로 설정한다.
    public void OperateExit(CharController sender) 
    {
        controller.Animator.SetBool("Idle", false);
    }
}

// 캐릭터 Walk 상태
public class CharWalk : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Walk", true);
    }
    public void OperateUpdate(CharController sender) 
    {
        // 공격이나 스킬범위에 몬스터가 있고 딜레이가 지났을때 Casting 상태로 넘어간다.
        if (controller.target != null)
        {
            if (Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.status.attackDistance)
                controller.Sm.SetState(controller.DicState[controller.attackDelay <= 0 ? CharState.Casting : CharState.Idle]);
            else if (Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.status.skillDistance && controller.skillDelay <= 0)
                controller.Sm.SetState(controller.DicState[CharState.Casting]);
        }
    }

    public void OperateFixedUpdate(CharController sender) 
    {
        // 주위에 몬스터가 없는 경우
        if (controller.target == null)
        {
            // 첫 번째 캐릭터가 Target이 없는 상태에서 Walk 상태로 넘어가지 않음
            // 넘어온 시점에서 첫번째 캐릭터가 아님은 전재로 첫번째 캐릭터 위치를 가져옴
            Vector2 targetPos = GameManager.Instance.FirstChar().transform.position;

            // 캐릭터 이동방향에 맞게 바라보도록 Scale 조정
            controller.ScaleInversion(controller.transform.position.x < targetPos.x);

            // 목표 방향으로 2의 속도로 이동
            controller.transform.position = Vector2.MoveTowards(controller.transform.position, targetPos, 2 * Time.fixedDeltaTime);
        }
        else
        {
            // 캐릭터 이동방향에 맞게 바라보도록 Scale 조정
            controller.ScaleInversion(controller.transform.position.x < controller.target.transform.position.x);

            // 몬스터 방향으로 이동
            controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.target.transform.position, 2 * Time.fixedDeltaTime);
        }
    }

    public void OperateExit(CharController sender) 
    {
        controller.Animator.SetBool("Walk", false);
    }
}

// 캐릭터 Casting 상태
public class CharCasting : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Casting", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Casting", false);
    }
}

// 캐릭터 Attack 상태
public class CharAttack : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Attack", true);

        if(controller.target != null)
            // 캐릭터 공격방향에 맞게 바라보도록 Scale 조정
            controller.ScaleInversion(controller.transform.position.x < controller.target.transform.position.x);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Attack", false);
    }
}

// 캐릭터 Hurt 상태
public class CharHurt : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Hurt", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Hurt", false);
    }
}

// 캐릭터 Die 상태
public class CharDie : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Die", true);

        controller.Die();
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Die", false);
    }
}

// 캐릭터 Victory 상태
public class CharVictory : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Victory", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Victory", false);
    }
}

// StateMachine 클래스
public class StateMachine<T>
{
    // 컨트롤러가 저장될 제네릭 변수
    private T m_sender;

    // 현재 상태 변수
    public CharState<T> CurState { get; set; }

    // 클래스 생성시 컨트롤러와 기본상태 설정
    public StateMachine(T sender, CharState<T> state)
    {
        m_sender = sender;
        SetState(state);
    }

    // 상태 변경 함수
    public void SetState(CharState<T> state)
    {
        // 컨트롤러가 없으면 반환
        if (m_sender == null)
            return;

        // 현재 상태와 변경될 상태가 같으면 반환
        if (CurState == state)
            return;

        // 현재 상태가 있으면 빠져나오는 함수 실행
        if (CurState != null)
            CurState.OperateExit(m_sender);

        // 현재 상태 변경
        CurState = state;

        // 현재 상태 진입 함수 실행
        if (CurState != null)
            CurState.OperateEnter(m_sender);
    }

    // 현재 상태에서 사용될 Update 함수
    public void DoOperateUpdate()
    {
        if (m_sender == null)
            return;

        CurState.OperateUpdate(m_sender);
    }

    // 현재 상태에서 사용될 FixedUpdate 함수
    public void DoOperateFixedUpdate()
    {
        if (m_sender == null)
            return;

        CurState.OperateFixedUpdate(m_sender);
    }
}

public class CharController : MonoBehaviour
{
    // 상태들을 저장할 변수를 Dictionary로 선언
    public Dictionary<CharState, CharState<CharController>> DicState { get; private set; }
    // StateMachine 변수
    public StateMachine<CharController> Sm { get; private set; }

    // 캐릭터 직업에 따라 바뀔 스탯을 가져올 변수
    public CharacterStatus status;
    // 캐릭터 애니메이터 변수
    public Animator Animator { get; private set; }
    // 가까이 있는 몬스터 저장 변수
    public MonsterController target;

    // 공격 딜레이 저장 변수
    public float attackDelay;
    public float skillDelay;
    public float hp;
    public int Level { get; private set; }
    public int Exp { get; private set; }

    public virtual void Awake()
    {
        // 애니메이터 초기화
        Animator = GetComponent<Animator>();
    }

    public virtual void Start()
	{
        // 캐릭터 상태들 생성
        CharState<CharController> idle = new CharIdle();
        CharState<CharController> walk = new CharWalk();
        CharState<CharController> casting = new CharCasting();
        CharState<CharController> attack = new CharAttack();
        CharState<CharController> hurt = new CharHurt();
        CharState<CharController> die = new CharDie();
        CharState<CharController> victory = new CharVictory();

        // Dictionary값으로 Enum과 짝지어 저장
        DicState = new Dictionary<CharState, CharState<CharController>>
		{
			{ CharState.Idle, idle },
			{ CharState.Walk, walk },
			{ CharState.Casting, casting },
			{ CharState.Attack, attack },
			{ CharState.Hurt, hurt },
			{ CharState.Die, die },
			{ CharState.Victory, victory }
		};

        // StateMachine 생성 및 초기 상태 설정
        Sm = new StateMachine<CharController>(this, DicState[CharState.Idle]);

        ResetChar();
    }

    public virtual void Update()
	{
        if (GameManager.Instance.GameOver)
            return;

        // 공격 딜레이
        if (attackDelay > 0)
            attackDelay -= Time.deltaTime;

        // 스킬 딜레이
        if (skillDelay > 0)
            skillDelay -= Time.deltaTime;

        // 가까이 있는 몬스터 불러오기 첫번째 캐릭터는 탐색 범위와 상관없이 가져올 수 있음
        target = GameManager.Instance.NearMonster(transform.position, GameManager.Instance.FirstChar() == this ? 0 : status.attackDistance);

        // StateMachine Update문 실행
        Sm.DoOperateUpdate();
	}

    public virtual void FixedUpdate()
	{
        if (GameManager.Instance.GameOver)
            return;
        // StateMachine FixedUpdate문 실행
        Sm.DoOperateFixedUpdate();
	}

    // 캐스팅 애니메이션이 끝날 때 실행되는 이벤트 함수
    public void CastingEnd()
    {
        // 공격 상태로 변경
        Sm.SetState(DicState[CharState.Attack]);
    }

    // 피격 모션이 끝났을 때 남은 hp를 확인하고 상태 변경
    public void HurtEnd()
	{
        if (hp <= 0)
            Sm.SetState(DicState[CharState.Die]);
        else
            Sm.SetState(DicState[CharState.Idle]);
	}

    // 스케일 방향 변환 함수
    public void ScaleInversion(bool check)
    {
        if ((check && transform.localScale.x > 0) || (!check && transform.localScale.x < 0))
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // 데미지를 받으면 피격 상태로 변경
    public void GetDamage(float damage)
	{
        hp -= damage;
        Sm.SetState(DicState[CharState.Hurt]);
	}

    // 캐릭터가 사망시 게임오버 여부를 확인하고 부활 코르틴 실행
    public void Die()
	{
        GameManager.Instance.CheckGameOver();

        StartCoroutine(Resurrection());
	}

    // 캐릭터 부활
    public IEnumerator Resurrection()
    {
        yield return new WaitForSeconds(5);

        // 이미 게임오버 이거나 사망 상태가 아닐 시 부활 안함
        if (GameManager.Instance.GameOver == false && Sm.CurState == DicState[CharState.Die])
		{
            hp = status.hp;
            Sm.SetState(DicState[CharState.Idle]);
        }
    }

    // 캐릭터 스텟 초기화
    public void ResetChar()
	{
        hp = status.hp;
        Sm.SetState(DicState[CharState.Idle]);
        target = null;

        // 공격 딜레이 초기화
        attackDelay = 0;
        skillDelay = 0;

        // 레벨, 경험치 초기화
        Level = 1;
        Exp = 0;
    }

    // 경험치 획득 함수
    public void SetExp(int num)
	{
        Exp += num;

        if(Exp >= 10+Level)
		{
            Level++;
            //레벨업시 체력 회복 및 가산
            hp = status.hp + status.hp/10*(Level-1);
            Exp -= 10 + Level;
		}
	}
}
