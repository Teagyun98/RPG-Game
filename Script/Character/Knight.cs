using UnityEngine;

public class Knight : CharController
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        // 초기 직업 설정
        status = GameManager.Instance.characterStatusList[0];
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
            if(skillDelay <= 0 && Vector2.Distance(transform.position, target.transform.position) < status.skillDistance)
			{
                // 스턴
                target.GetStun(1);
                GameManager.Instance.SetEffect(target.transform.position, "Yellow");
                target.GetDamage(status.attackPower, this);
                // 스킬 딜레이 초기화
                skillDelay = status.skillDelay;
            }
            else if(attackDelay <= 0 && Vector2.Distance(transform.position, target.transform.position) < status.attackDistance)
			{
                target.GetDamage(status.attackPower, this);
                // 공격 딜레이 초기화
                attackDelay = status.attackDelay;
            }
        }

        Sm.SetState(DicState[CharState.Idle]);
    }
}
