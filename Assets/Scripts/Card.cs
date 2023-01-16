using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField] CardInfo cardInfo;
    public TextMeshPro hpText;
    public TextMeshPro atkText;
    public TextMeshPro levelText;
    public int level;


    /*�ڽ��� ������Ʈ �̸��� ���� ��ũ���ͺ� �����͸� �о�ͼ� �����Ѵ�
    ��������Ʈ �������� ���� ������ ����*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name}");
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
