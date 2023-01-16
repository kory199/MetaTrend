using MongoDB.Driver;
using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public partial class Drag2D : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.11f);
    WaitForSeconds meltWait = new WaitForSeconds(0.1f);

    BoxCollider2D pol;
    MeshRenderer spriteRenderer;
    Vector2 pos;
    Vector2 battleZonePos;
    Vector2 meltPos;

    float timer = 0f;
    float distance = 10;
    private bool isClickBool = false;
    public bool isFreezen = false;
    bool isClickBattleMonster = false;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void Start()
    {
        spriteRenderer = GetComponent<MeshRenderer>();
        pol = GetComponent<BoxCollider2D>();
        pos = this.gameObject.transform.position;
    }

    Camera mainCam = null;
    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = mainCam.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition + Vector3.down;

        RaycastHit2D hit = Physics2D.Raycast(objPosition, Vector2.zero);

        // �巡�� �Ҷ� ���� ���̸� ���� �ؿ� ���� ��Ʋ���͸� �ٸ� ��ġ�� ����
        if (isClickBattleMonster == true)
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("BattleMonster"))
                {
                    if (hit.collider.name != this.gameObject.name)
                    {
                        GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                        hit.collider.gameObject.transform.position = vec.transform.position + Vector3.down;
                    }

                    else if (hit.collider.name == this.gameObject.name)
                    {
                        timer += Time.deltaTime;
                        Debug.Log(timer);
                        if (timer > 1f)
                        {
                            GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                            hit.collider.gameObject.transform.position = vec.transform.position + Vector3.down;
                        }
                    }
                }
                else
                    timer = 0f;
            }
        }
    }

    private void OnMouseDown()
    {
        isClickBool = false;
        pol.enabled = false;
        battleZonePos = pos;

        if (gameObject.CompareTag("BattleMonster"))
        {
            UIManager.Instance.sell.gameObject.SetActive(true);

            isClickBattleMonster = true;
        }
    }

    private void OnMouseUp()
    {
        isClickBool = true;
        pol.enabled = true;
        isClickBattleMonster = false;

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
            // ������ ī�带 ����� ��
            if (gameObject.CompareTag("FreezeCard"))
            {
                // ������ ī�带 ����� ������ ���� �� ���� ��ġ�� ���ư���
                // ��Ʈī��� �±� ������ �ǰ� ���� ��ġ�� ���ư��� �ٽ� ���� ���°� �ȴ�. 
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                }

                // ������� �� ��Ʋ���� ���� ���� �����ϰ� �ϴ� ����ó��
                if (UIManager.Instance.goldCount >= 3)
                {
                    if (collision.gameObject.CompareTag("BattleZone"))
                    {
                        meltPos = collision.gameObject.transform.position;
                        StartCoroutine(COR_BackMelt());
                    }
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
                    if (UIManager.Instance.goldCount >= 3)
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
        yield return wait;
        UIManager.Instance.sell.gameObject.SetActive(false);
    }
    // ���� ��ġ�� ������ �Լ�
    private IEnumerator COR_BackAgain()
    {
        yield return wait;

        if (CompareTag("BattleMonster"))
            transform.position = pos + Vector2.down;

        else if (CompareTag("Monster"))
            transform.position = pos;

        if (CompareTag("FreezeCard"))
        {
            transform.position = pos;
            gameObject.tag = "MeltCard";
        }
    }
    // ������ī��� �ٲٴ� �Լ�
    IEnumerator COR_BackCard()
    {
        yield return wait;
        gameObject.tag = "FreezeCard";
    }

    // ��ī�带 ���Ž� �� �Լ� ���� �ڸ��� ���ư� �ڸ��� bool���� �ٲ��� ���ŵȴ�.
    IEnumerator COR_BackMelt()
    {
        gameObject.tag = "MeltCard";
        this.transform.position = battleZonePos;
        yield return meltWait;
        gameObject.tag = "BattleMonster";
        pos = meltPos;
        this.gameObject.transform.position = pos + Vector2.down;
        spriteRenderer.sortingOrder = 3;
        UIManager.Instance.goldCount -= 3;
    }

    void SellButton()
    {
        UIManager.Instance.sell.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
