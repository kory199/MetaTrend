using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    [SerializeField] CardInfo cardInfo;
    private SpriteRenderer spriteRenderer;
    private bool isTouch;


    /*�ڽ��� ������Ʈ �̸��� ���� ��ũ���ͺ� �����͸� �о�ͼ� �����Ѵ�
    ��������Ʈ �������� ���� ������ ����*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name}");
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(Resources.Load<Sprite>($"Sprites/{cardInfo.objName}"));
        Debug.Log(cardInfo.objName);
        spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/{cardInfo.objName}");
    }

    /*Ŀ���ҵ� ������ �� Ŭ���ϸ� ����Ǵ� �Լ�*/ 
    public void OnMouseDown()
    {
        if (GameMGR.Instance.customDeckShop.isJoinShop)
        {
            if (GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName))
            {
                if (isTouch)
                {
                    isTouch = false;
                    spriteRenderer.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    isTouch = true;
                    spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1);
                }
            }
            else Debug.Log("8���� ����");
        }
    }

}
