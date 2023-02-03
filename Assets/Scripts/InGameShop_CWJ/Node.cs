using UnityEngine;

public class Node : MonoBehaviour
{
    // �� Ÿ�Ͽ� ���Ͱ� �ִ���?
    public bool isNotSpawn = false;
    public SpriteRenderer mySprite = null;

    public bool isNotMonster = false;

    GameObject collisionObj;
    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            this.isNotSpawn = true;
            gameObject.tag = "FullZone";
            gameObject.transform.localScale = new Vector3(0.245f, 0.245f, 1);
            mySprite.sprite = Resources.Load<Sprite>("FrameFreeze");
        }

        if (collisionObj == null && collision.gameObject.CompareTag("Monster"))
        {
            collisionObj = collision.gameObject;
            gameObject.tag = "FullZone";
            mySprite.sprite = Resources.Load<Sprite>("FrameOn");
            isNotMonster = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "SelectRing";
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
            isNotMonster = false;
            collisionObj = null;

        }

        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "SelectRing";
            this.isNotSpawn = false;
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
        }
    }
}
