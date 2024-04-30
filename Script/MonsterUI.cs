using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    // 몬스터 HpBar
    [SerializeField] private Slider hpBar;
    // HpBar 이미지
    [SerializeField] private Image fill;

    // 따라다닐 몬스터
    private MonsterController monster;

    private void FixedUpdate()
    {
        // 몬스터 위에 따라다니기
        if(monster)
            transform.position = monster.transform.position + new Vector3(0, 0.5f, 0);
    }

    public void SetMonster(MonsterController controller)
    {
        monster = controller;
    }

    public void SetSlider(float value)
    {
        hpBar.value = value;
    }

    // 스턴 상태일 때는 노란색으로 변함
    public void SetColor(bool stun)
    {
        if (stun)
            fill.color = new Color32(255, 255, 0, 255);
        else
            fill.color = new Color32(255, 0, 0, 255);
    }
}
