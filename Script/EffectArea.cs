using System.Collections.Generic;
using UnityEngine;

public class EffectArea : MonoBehaviour
{
    // 파티클을 생성할 위치
    [SerializeField] private Transform effectArea;
    // 파티클 프리펩
    [SerializeField] private ParticleSystem particle;

    private Dictionary<string, Color> effectColor;

    private void Start()
    {
        effectColor = new Dictionary<string, Color> 
        {
            { "Red", Color.red},
            { "Green", Color.green},
            { "Yellow", Color.yellow},
        };
    }

    // 파티클 활성화 함수
    public void Set(Vector3 pos, string color)
    {
        ParticleSystem effect = null;

        for(int i = 0; i< effectArea.childCount; i++)
        {
            if(effectArea.GetChild(i).gameObject.activeSelf == false)
            {
                effect = effectArea.GetChild(i).GetComponent<ParticleSystem>();

                effect.gameObject.SetActive(true);
                break;
            }    
        }

        if (effect == null)
            effect = Instantiate(particle, effectArea);

        effect.transform.position = pos;
        ParticleSystem.MainModule main = effect.main;
        main.startColor = effectColor[color];
    }
}
