using UnityEngine;

public class TileMap : MonoBehaviour
{
    private void Update()
    {
        Vector2 camPos = Camera.main.transform.position;
        Vector2 myPos = transform.position;

        // 각 타일이 캐릭터의 위치와 자신의 위치를 비교해 재배치 될 수 있도록 함
        if((camPos.x < myPos.x && myPos.x - camPos.x > 30) || (camPos.x > myPos.x && camPos.x - myPos.x > 30) || (camPos.y < myPos.y && myPos.y - camPos.y > 30) || (camPos.y > myPos.y && camPos.y - myPos.y > 30))
        {
            float diffX = camPos.x - myPos.x;
            float diffY = camPos.y - myPos.y;
            float dirX = diffX < 0 ? -1 : 1;
            float dirY = diffY < 0 ? -1 : 1;
            diffX = Mathf.Abs(diffX);
            diffY = Mathf.Abs(diffY);

            if (diffX > diffY)
                transform.Translate(Vector3.right * dirX * 60);
            else
                transform.Translate(Vector3.up * dirY * 60);
        }
    }
}
