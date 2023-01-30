using UnityEngine;

public class BattleZone : MonoBehaviour
{
    public bool isHere = false;
    [SerializeField] int myNum; // 상점 유닛 배치 순서 (0~5)

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BattleMonster"))
        {
            this.isHere = true;
            this.gameObject.tag = "FullZone";
            GameMGR.Instance.spawner.cardBatch[myNum] = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        this.isHere = false;

        if(collision.gameObject.CompareTag("BattleMonster"))
        {
            this.gameObject.tag = "BattleZone";
            GameMGR.Instance.spawner.cardBatch[myNum] = null;
        }
    }
}
