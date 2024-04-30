using UnityEngine;

public class Thief : CharController
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        status = GameManager.Instance.characterStatusList[1];
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
                // 근처의 몬스터들에게 스킬 공격
                foreach (MonsterController monster in GameManager.Instance.GetDistanceMonsters(transform.position, status.skillDistance))
                {
                    monster.GetDamage(status.attackPower, this);
                    GameManager.Instance.SetEffect(monster.transform.position, "Red");
                }
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
