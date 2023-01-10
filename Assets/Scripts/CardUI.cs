using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] CardInfo cardInfo;
    private Image image;
    private bool isTouch;

    /*�ڽ��� ������Ʈ �̸��� ���� ��ũ���ͺ� �����͸� �о�ͼ� �����Ѵ�
    ��������Ʈ �������� ���� ������ ����*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name}");
        image = GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>($"Sprites/{cardInfo.objName}");
    }

    /*Ŀ���ҵ� ������ �� Ŭ���ϸ� ����Ǵ� �Լ�*/ 
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameMGR.Instance.customDeckShop.isJoinShop)
        {
                if (!isTouch && GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName) < 8)
                {

                Debug.Log(GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName));
                    isTouch = true;
                    image.color = new Color(0.3f, 0.3f, 0.3f, 1);
                }
                else if (isTouch)
                {
                    isTouch = false;
                    image.color = new Color(1, 1, 1, 1);
                }
            
        }
    }                      
    public void ClearClick()
    {
        isTouch = false;
        image.color = new Color(1, 1, 1, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
}
