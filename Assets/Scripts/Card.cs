using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField] public CardInfo cardInfo;
    public TextMeshPro hpText;
    public TextMeshPro atkText;
    public TextMeshPro levelText;
    public int level;
    public int curAttackValue;
    public int curHP;


    /*�ڽ��� ������Ʈ �̸��� ���� ��ũ���ͺ� �����͸� �о�ͼ� �����Ѵ�
    ��������Ʈ �������� ���� ������ ����*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name.Replace("(Clone)","")}");
        hpText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
        atkText = transform.GetChild(0).GetChild(3).GetComponent<TextMeshPro>();
        levelText = transform.GetChild(0).GetChild(5).GetComponent<TextMeshPro>();
        hpText.text = cardInfo.hp.ToString();
        atkText.text = cardInfo.attackValue.ToString();
        level = 1;
        levelText.text = level.ToString();
    }
    private void Awake()
    {
        SetMyInfo(name);
    }

}
