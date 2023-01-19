using MongoDB.Driver;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    // �� Ÿ�Ͽ� ���Ͱ� �ִ���?
    public bool isNotSpawn = false;



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            this.isNotSpawn = true;

            gameObject.tag = "Rect";
        }

        if (collision.gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "Rect";
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "SelectRing";
        }

        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "SelectRing";
            this.isNotSpawn = false;
        }
    }
}
