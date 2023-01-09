using System.Collections;
using UnityEngine;

public class Drag2D : MonoBehaviour
{
    PolygonCollider2D pol;
    SpriteRenderer spriteRenderer;
    Vector2 pos;
    Vector2 battleZonePos;

    float distance = 10;
    private bool isClickBool = false;
    public bool isFreezen = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pol = GetComponent<PolygonCollider2D>();
        pos = this.gameObject.transform.position;
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;
    }

    private void OnMouseDown()
    {
        isClickBool = false;
        // spriteRenderer.color = Color.red;
        pol.enabled = false;
        battleZonePos = pos;

        if (gameObject.CompareTag("BattleMonster"))
        {
            UIManager.Instance.sell.gameObject.SetActive(true);
        }
    }

    private void OnMouseUp()
    {
        isClickBool = true;
        pol.enabled = true;

        if (this.gameObject.CompareTag("Monster") || this.gameObject.CompareTag("BattleMonster") || this.gameObject.CompareTag("FreezeCard"))
        {
            StartCoroutine(COR_BackAgain());
        }
        if (UIManager.Instance.sell.activeSelf == true)
        {
            StartCoroutine(COR_SellButton());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isClickBool == true)
        {
            // ��� �ִ� ������Ʈ�� ��Ʋ���Ϳ� ������ ��ġ�� ���� �ٲ�
            if (collision.gameObject.CompareTag("BattleMonster"))
            {
                collision.gameObject.transform.position = pos;
            }

            // ������ ī�带 ����� ��
            if (gameObject.CompareTag("FreezeCard"))
            {
                // ������ ī�带 ����� ������ ���� �� ���� ��ġ�� ���ư���
                // ��Ʈī��� �±� ������ �ǰ� ���� ��ġ�� ���ư��� �ٽ� ���� ���°� �ȴ�. 
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                    gameObject.tag = "MeltCard";
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isClickBool == true)
        {
            // ��Ʋ ���͸� ����� ��
            if (gameObject.CompareTag("BattleMonster"))
            {
                // �Ǹ�
                if (collision.gameObject.CompareTag("Sell"))
                {
                    UIManager.Instance.goldCount += 1;
                    SellButton();
                }

                // ��� �ִ� ������Ʈ�� ��Ʋ���� ������ ������Ʈ ��ġ�� ����
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    pos = collision.gameObject.transform.position;
                }
            }

            // ���͸� ����� ��
            if (gameObject.CompareTag("Monster"))
            {
                // ����� ������ ������ī��� �±׸� �ٲ� �� ���� ��ġ�� ������.
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                    StartCoroutine(COR_BackCard());
                }
                // ���Ͱ� ��Ʋ ���� ������ ��尡 ���� �ǰ� ��Ʋ���� �±׷� �ٲ��
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    if (UIManager.Instance.goldCount > 0)
                    {
                        spriteRenderer.sortingOrder = 3;
                        gameObject.tag = "BattleMonster";
                        UIManager.Instance.goldCount -= 3;
                        pos = collision.gameObject.transform.position;
                    }
                }
            }
        }
    }

    // �ǸŹ�ư ON OFF
    IEnumerator COR_SellButton()
    {
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.sell.gameObject.SetActive(false);
    }
    // ���� ��ġ�� ������ �Լ�
    private IEnumerator COR_BackAgain()
    {
        yield return new WaitForSeconds(0.11f);
        transform.position = pos;
    }
    // ������ī��� �ٲٴ� �Լ�
    IEnumerator COR_BackCard()
    {
        yield return new WaitForSeconds(0.11f);
        gameObject.tag = "FreezeCard";
    }

    void SellButton()
    {
        Debug.Log("????????????");
        UIManager.Instance.sell.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
