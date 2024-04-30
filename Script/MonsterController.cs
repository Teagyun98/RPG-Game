using System.Collections.Generic;
using UnityEngine;

// 몬스터 상태 Enum
public enum MonsterState
{
    Idle,
    Run,
    Attack,
    Death,
}

// 캐릭터에서 사용된 인터페이스를 가져와 사용
// 몬스터 Idle 클래스
public class MonsterIdle : CharState<MonsterController>
{
    MonsterController controller;

    private float patrolTime;

    public void OperateEnter(MonsterController sender) 
    {
        controller = sender;
        controller.Animator.SetBool("Idle", true);

        patrolTime = 0;
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Idle", false); }
    public void OperateUpdate(MonsterController sender) 
    {
        // 목표 캐릭터가 없는 경우 반환
        if (controller.target == null)
		{
            patrolTime += Time.deltaTime;

            // 목표 캐릭터가 없는 경우가 5초 이상이 되면 Run 상태로 변경
            if (patrolTime > 5)
                controller.Sm.SetState(controller.DicState[MonsterState.Run]);

            return;
        }


        // 목표 캐릭터가 공격 범위보다 멀리 있는 경우 Run 상태로 변경
        if (Vector2.Distance(controller.transform.position, controller.target.transform.position) > controller.AttackDistance)
            controller.Sm.SetState(controller.DicState[MonsterState.Run]);
        // 목표 캐릭터가 공격 범위에 있고 공격 딜레이가 0보다 작으면 Attack 아니면 Idle 상태로 변경
        else if (Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.AttackDistance)
        {
            controller.Sm.SetState(controller.DicState[controller.AttackDelay <= 0 ? MonsterState.Attack : MonsterState.Idle]);
        }

    }
    public void OperateFixedUpdate(MonsterController sender) { }
}

// 몬스터 Run 클래스
public class MonsterRun : CharState<MonsterController>
{
    MonsterController controller;

    Vector2 patrolPos;

    public void OperateEnter(MonsterController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Run", true);

        // 타겟이 없는 상태로 Run상태로 넘어왔다면 첫번째 캐릭터 주위의 위치를 목표로 순찰
        if (controller.target == null)
            patrolPos = (Vector2)GameManager.Instance.FirstChar().transform.position + new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Run", false); }
    public void OperateUpdate(MonsterController sender) 
    {
        // 목표 캐릭터가 공격 범위에 있고 공격 딜레이가 0보다 작으면 Attack 아니면 Idle 상태로 변경
        if (controller.target != null && Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.AttackDistance)
        {
            controller.Sm.SetState(controller.DicState[controller.AttackDelay <= 0 ? MonsterState.Attack : MonsterState.Idle]);
        }
    }
    public void OperateFixedUpdate(MonsterController sender) 
    {
        // 목표 캐릭터가 없으면 반환
        if (controller.target == null)
		{
            // 이동 방향과 맞게 몬스터 스케일 변경
            controller.ScaleInversion(controller.transform.position.x < patrolPos.x);

            // 목표로 1의 속도로 이동
            controller.transform.position = Vector2.MoveTowards(controller.transform.position, patrolPos, 1 * Time.fixedDeltaTime);

            if (Vector2.Distance(controller.transform.position, patrolPos) < 1)
                controller.Sm.SetState(controller.DicState[MonsterState.Idle]);

            return;
        }

        // 이동 방향과 맞게 몬스터 스케일 변경
        controller.ScaleInversion(controller.transform.position.x < controller.target.transform.position.x);

        // 목표로 1의 속도로 이동
        controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.target.transform.position, 1 * Time.fixedDeltaTime);
    }
}

// 몬스토 Attack 클래스
public class MonsterAttack : CharState<MonsterController>
{
    MonsterController controller;

    public void OperateEnter(MonsterController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Attack", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Attack", false); }
    public void OperateUpdate(MonsterController sender) { }
    public void OperateFixedUpdate(MonsterController sender) { }
}

// 몬스터 Death 클래스
public class MonsterDeath : CharState<MonsterController>
{
    MonsterController controller;

    public void OperateEnter(MonsterController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Death", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Death", false); }
    public void OperateUpdate(MonsterController sender) { }
    public void OperateFixedUpdate(MonsterController sender) { }
}

public class MonsterController : MonoBehaviour
{
    // 몬스터 상태들을 Dictionary 변수로 선언
    public Dictionary<MonsterState, CharState<MonsterController>> DicState { get; private set; }
    // StateMachine 변수
    public StateMachine<MonsterController> Sm { get; private set; }
    // 애니메이터 변수
    public Animator Animator { get; private set; }
    private MonsterUI ui;

    // 최대 HP
    public int MaxHp { get; private set; }
    // 현재 HP
    private float hp;
    // 공격력
    public int AttackPower { get; private set; }
    // 공격 딜레이
    public float AttackDelay { get; private set; }
    // 공격 사거리
    public float AttackDistance { get; private set; }

    // 목표 캐릭터
    public CharController target;

    private float stun;
    private bool boss;

    private void Awake()
    {
        // 애니메이터 초기화
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 몬스터 상태들 생성
        CharState<MonsterController> idle = new MonsterIdle();
        CharState<MonsterController> run = new MonsterRun();
        CharState<MonsterController> attack = new MonsterAttack();
        CharState<MonsterController> death = new MonsterDeath();

        // 몬스터 상태들 저장
        DicState = new Dictionary<MonsterState, CharState<MonsterController>>
        {
            { MonsterState.Idle, idle},
            { MonsterState.Run, run},
            { MonsterState.Attack, attack},
            { MonsterState.Death, death}
        };

        // StateMachine 초기화
        Sm = new StateMachine<MonsterController>(this, DicState[MonsterState.Idle]);
    }

    private void Update()
    {
        if (GameManager.Instance.GameOver)
            return;

        // 상태이상 중에는 반환
        if (stun > 0)
		{
            stun -= Time.deltaTime;
            return;
        }

        if (ui != null)
            ui.SetColor(false);

        // 공격 딜레이
        if (AttackDelay > 0)
            AttackDelay -= Time.deltaTime;

        // 목표 캐릭터 가져오기
        target = GameManager.Instance.NearChar(transform.position);

        // StateMachine Update문
        Sm.DoOperateUpdate();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GameOver)
            return;

        // 상태이상 중에는 반환
        if (stun > 0)
            return;

        // StateMachine FixedUpdate문
        Sm.DoOperateFixedUpdate();
    }

    // 공격 애니메이션 이벤트 함수
    public void AttackEnd()
    {
        // Idle 상태로 변환
        Sm.SetState(DicState[MonsterState.Idle]);
        // 공격 딜레이 초기화
        AttackDelay = GameManager.Instance.monsterAttackDelay;

        // 목표 캐릭터가 공격 범위에 있고 공격 딜레이가 0보다 작으면 Attack 아니면 Idle 상태로 변경
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < AttackDistance)
            target.GetDamage(AttackPower);
    }

    // 몬스터 스탯 세팅 함수
    public void SetStatus(bool boss = false)
	{
        this.boss = boss;

        stun = 0;

        MaxHp = GameManager.Instance.monsterHp;
        hp = MaxHp;
        AttackPower = GameManager.Instance.monsterAttackPower;
        AttackDelay = GameManager.Instance.monsterAttackDelay;
        AttackDistance = GameManager.Instance.mosterAttackDistance;

        // 스테이지에 따라 몬스터 스텟 가산
        MaxHp += MaxHp / 10 * GameManager.Instance.Stage;
        hp += hp / 10 * GameManager.Instance.Stage;
        AttackPower += AttackPower / 10 * GameManager.Instance.Stage;

        // 보스 몬스터일 경우 추가 스텟
        if(boss)
		{
            MaxHp *= 10;
            hp *= 10;
            AttackPower *= 2;
            AttackDistance *= 2;
		}

        // 보스 몬스터 색구분
        transform.GetComponent<SpriteRenderer>().color = boss ? new Color32(255, 0, 0, 255) : new Color32(255, 255, 255, 255);
    }

    // 몬스터 스케일 변환 함수
    public void ScaleInversion(bool check)
	{
        if ((check && transform.localScale.x < 0) || (!check && transform.localScale.x > 0))
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

    // 몬스터가 공격 받을 때 사용되는 함수
    public void GetDamage(float damage, CharController charController)
    {
        // 공격한 캐릭터 레벨당 데미지 가산
        damage += damage / 10 * (charController.Level - 1);
        hp -= damage;

        if (ui == null)
        {
            // 처음 몬스터 생성시 HpBar가 없지만 피격시 생성
            ui = GameManager.Instance.GetMonsterUI(this);
        }

        ui.SetSlider(hp / MaxHp);

        // 데미지를 받고 hp가 0보다 적어지면 Death 상태로 변환
        if (hp <= 0)
		{
            Sm.SetState(DicState[MonsterState.Death]);
            charController.SetExp(1);
            GameManager.Instance.SetGold(boss ? 100 : 10);
            GameManager.Instance.stageExp++;

            if (boss)
                GameManager.Instance.NextStage();

            if (ui != null)
            {
                ui.SetMonster(null);
                ui.gameObject.SetActive(false);
                ui = null;
            }
        }
    }

    // 몬스터 부활 함수
    public void Resurrection(bool boss = false)
    {
        SetStatus(boss);
        Sm.SetState(DicState[MonsterState.Idle]);
    }

    // 스턴 함수
    public void GetStun(float time)
	{
        stun += time;

        if (ui != null)
            ui.SetColor(true);
	}
}
