using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public partial class Drag2D : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.1f);

    Card card;
    BoxCollider2D pol;
    MeshRenderer spriteRenderer;
    Vector2 pos;
    Vector2 selectZonePos;
    Vector2 meltPos;
    Vector3 monsterPos = new Vector3(0, -0.6f, 0);

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
        card = GetComponent<Card>();

        this.selectZonePos = this.transform.position;
    }

    Camera mainCam = null;

    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = mainCam.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(objPosition, Vector2.zero);

        if (CompareTag("BattleMonster")) transform.position = objPosition + Vector3.down;
        else transform.position = objPosition + monsterPos;

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
        UpdateOutline(true);
        isClickBool = false;
        pol.enabled = false;

        GameMGR.Instance.uiManager.OnEnter_Set_SkillExplantion(false, Vector3.zero);
        GameMGR.Instance.uiManager.SetisExplantionActive(true);

        if (gameObject.CompareTag("BattleMonster"))
        {
            GameMGR.Instance.uiManager.sell.gameObject.SetActive(true);

            isClickBattleMonster = true;
        }
    }

    private void OnMouseUp()
    {
        UpdateOutline(false);
        isClickBool = true;
        pol.enabled = true;
        isClickBattleMonster = false;
        GameMGR.Instance.uiManager.SetisExplantionActive(false);

        if (this.gameObject.CompareTag("Monster"))
        {
            StartCoroutine(COR_BackAgain());
        }

        if(this.gameObject.CompareTag("FreezeCard"))
        {
            StartCoroutine(COR_BackAgain());
        }

        if (this.gameObject.CompareTag("BattleMonster"))
        {
            StartCoroutine(COR_SellButton());
            StartCoroutine(COR_BackAgain());
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
                    gameObject.tag = "Monster";
                    StartCoroutine(COR_BackAgain());
                }

                // ������� �� ��Ʋ���� ���� ���� �����ϰ� �ϴ� ����ó��
                if (GameMGR.Instance.uiManager.goldCount >= 3)
                {
                    if (collision.gameObject.CompareTag("BattleZone"))
                    {
                        meltPos = collision.gameObject.transform.position;
                        StartCoroutine(COR_BackMelt());
                    }
                }
            }

            if (gameObject.CompareTag("Monster"))
            {
                if (gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster"))
                {
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        ShopCardLevelUp(collision);
                    }
                }
            }

            if (gameObject.CompareTag("BattleMonster"))
            {
                if (gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster"))
                {
                    CardLevelUp(collision);
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
                    gameObject.tag = "FreezeCard";
                }
                // ���Ͱ� ��Ʋ ���� ������ ��尡 ���� �ǰ� ��Ʋ���� �±׷� �ٲ��
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        spriteRenderer.sortingLayerName = "SellTXT";
                        gameObject.tag = "BattleMonster";
                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        pos = collision.gameObject.transform.position;
                        Vector2 monTras = gameObject.transform.localScale;
                        gameObject.transform.localScale = monTras * 2;
                    }
                }
            }
        }
    }

    // ī�� ������ 
    void CardLevelUp(Collider2D collision)
    {
        int colAttack = collision.GetComponent<Card>().curAttackValue;
        int colHP = collision.GetComponent<Card>().curHP;
        int attack = card.curAttackValue;
        int hP = card.curHP;
        int plusAttack = 0;
        int plusHp = 0;

        if (colAttack > attack)
        {
            plusAttack = colAttack;
        }
        else if (colAttack <= attack)
        {
            plusAttack = attack;
        }

        if (colHP > hP)
        {
            plusHp = colHP;
        }
        else if (colHP <= hP)
        {
            plusHp = hP;
        }

        collision.GetComponent<Card>().ChangeValue(CardStatus.Attack, plusAttack + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Hp, plusHp + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Exp, 1);

        if (collision.gameObject.transform.position.y > gameObject.transform.position.y)
        {
            CombineCard(collision);
        }
    }

    void ShopCardLevelUp(Collider2D collision)
    {
        int colAttack = collision.GetComponent<Card>().curAttackValue;
        int colHP = collision.GetComponent<Card>().curHP;
        int attack = card.curAttackValue;
        int hP = card.curHP;
        int plusAttack = 0;
        int plusHp = 0;

        if (colAttack > attack)
        {
            plusAttack = colAttack;
        }
        else if (colAttack <= attack)
        {
            plusAttack = attack;
        }

        if (colHP > hP)
        {
            plusHp = colHP;
        }
        else if (colHP <= hP)
        {
            plusHp = hP;
        }

        collision.GetComponent<Card>().ChangeValue(CardStatus.Attack, plusAttack + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Hp, plusHp + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Exp, 1);

        Destroy(this.gameObject);
    }

    // �뺴 ����
    void CombineCard(Collider2D collision)
    {
        transform.position = collision.gameObject.transform.position;
        Destroy(collision.gameObject);
    }

    // �ǸŹ�ư ON OFF
    IEnumerator COR_SellButton()
    {
        yield return new WaitForSeconds(0.12f);
        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);
        //  GameMGR.Instance.uiManager.cardPannel.gameObject.SetActive(false);
    }

    // ���� ��ġ�� ������ �Լ�
    private IEnumerator COR_BackAgain()
    {
        yield return wait;

        if (CompareTag("BattleMonster") || CompareTag("BattleMonster2") || CompareTag("BattleMonster3"))
            transform.position = pos + Vector2.down;

        else if (CompareTag("Monster"))
        {
            transform.position = selectZonePos;
        }

        else if (CompareTag("FreezeCard"))
        {
            transform.position = selectZonePos;
        }
    }

    // ��ī�带 ���Ž�
    IEnumerator COR_BackMelt()
    {
        yield return wait;
        gameObject.tag = "BattleMonster";
        pos = meltPos;
        this.gameObject.transform.position = pos + Vector2.down;
        spriteRenderer.sortingOrder = 3;
        GameMGR.Instance.uiManager.goldCount -= 3;
        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
    }
}
