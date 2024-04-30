using UnityEngine;

public class Priest : CharController
{
    private CharController lowHpChar;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        status = GameManager.Instance.characterStatusList[3];
        hp = status.hp;
    }

    public override void Update()
    {
        base.Update();

        // 최대 체력이 아닌 체력 비율이 가장 적은 캐릭터 가져오기
        lowHpChar = GameManager.Instance.GetLowHpChar(transform.position, status.skillDistance);

        // 공격과 별개로 회복 스킬 사용
        if (lowHpChar != null && skillDelay <= 0 && (Sm.CurState == DicState[CharState.Idle] || Sm.CurState == DicState[CharState.Walk]))
            Sm.SetState(DicState[CharState.Casting]);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void AttackEnd()
    {
        // 일반공격과 스킬공격 구분
        if (lowHpChar != null && skillDelay <= 0 && Vector2.Distance(transform.position, lowHpChar.transform.position) < status.skillDistance)
        {
            // 체력이 낮은 아군 회복
            lowHpChar.hp += (status.attackPower + (status.attackPower/10*Level-1) )* 2.5f;
            GameManager.Instance.SetEffect(lowHpChar.transform.position, "Green");
            // 스킬 딜레이 초기화
            skillDelay = status.skillDelay;
            Sm.SetState(DicState[CharState.Idle]);
            return;
        }

        if (target != null)
        {
            if (attackDelay <= 0 && Vector2.Distance(transform.position, target.transform.position) < status.attackDistance)
            {
                target.GetDamage(status.attackPower, this);
                // 공격 딜레이 초기화
                attackDelay = status.attackDelay;
            }
        }

        Sm.SetState(DicState[CharState.Idle]);
    }
}
