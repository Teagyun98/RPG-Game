using UnityEngine;

public class Archer : CharController
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        status = GameManager.Instance.characterStatusList[2];
        hp = status.hp;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void AttackEnd()
    {
        if (target != null)
        {
            // 일반공격과 스킬공격 구분
            if (skillDelay <= 0 && Vector2.Distance(transform.position, target.transform.position) < status.skillDistance)
            {
                // 멀리 있는 몬스터 스킬 공격
                target.GetDamage(status.attackPower * 2.5f, this);
                GameManager.Instance.SetEffect(target.transform.position, "Red");
                // 스킬 딜레이 초기화
                skillDelay = status.skillDelay;
            }
            else if (attackDelay <= 0 && Vector2.Distance(transform.position, target.transform.position) < status.attackDistance)
            {
                target.GetDamage(status.attackPower, this);
                // 공격 딜레이 초기화
                attackDelay = status.attackDelay;
            }
        }

        Sm.SetState(DicState[CharState.Idle]);
    }
}
