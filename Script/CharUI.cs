using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharUI : MonoBehaviour
{
	// 담당 캐릭터
	[SerializeField] private CharController character;
	[Header("Under UI")]
	// 레벨
	[SerializeField] private TextMeshProUGUI level;
	// Hp 게이지
	[SerializeField] private Slider hpBar;
	// 경험치 게이지
	[SerializeField] private Slider expBar;
	// 스킬 쿨타임 체크 슬라이더
	[SerializeField] private Image skillSlider;
	// 레벨업 버튼
	[SerializeField] private Button levelUpBtn;
	// 레벨업 버튼 텍스트
	[SerializeField] private TextMeshProUGUI levelUpBtnText;
	[Header("Follow HpBar")]
	// 캐릭터 위를 따라다니는 Hp 게이지
	[SerializeField] private Slider followHpBar;

	private void FixedUpdate()
	{
		// 따라다니는 게이지
		followHpBar.transform.position = character.transform.position + new Vector3(0,2,0);
	}

	private void Update()
	{
		hpBar.value = character.hp / character.status.hp;
		followHpBar.value = hpBar.value;
		level.text = $"Lv.{character.Level}";
		expBar.value = (float)character.Exp / (10 + character.Level);
		skillSlider.fillAmount = character.skillDelay / character.status.skillDelay;

		// 레벨업에 필요한 골드가 있고 캐릭터가 사망하지 않았을 때 레벨업 버튼이 보이도록 함
		if (GameManager.Instance.Gold >= 100 + (character.Level - 1) * 10 && character.Sm.CurState != character.DicState[CharState.Die])
        {
            levelUpBtn.gameObject.SetActive(true);
			levelUpBtnText.text = $"Level Up!<br>{100 + (character.Level - 1) * 10} Gold";

        }
        else
            levelUpBtn.gameObject.SetActive(false);
    }

	// 레벨업 함수
	public void LevelUp()
	{
		if (GameManager.Instance.Gold < 100 + (character.Level - 1) * 10)
			return;

		GameManager.Instance.SetGold(-(100 + (character.Level - 1) * 10));
		character.SetExp(10 + character.Level);
	}

}
