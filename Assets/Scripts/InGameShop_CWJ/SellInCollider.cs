using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellInCollider : MonoBehaviour
{
    [SerializeField] GameObject sell = null;
    [SerializeField] GameObject[] specialZone = null;
    Collider2D[] specialColl = null;


    private void Start()
    {
        specialColl = new Collider2D[specialZone.Length];

        for (int i = 0; i < 2; i++)
        {
            specialColl[i] = specialZone[i].GetComponent<Collider2D>();
        }

        for (int i = 0; i < 2; i++)
        {
            specialColl[i].enabled = false;
        }
    }

    public void SellOn()
    {
        if (sell.activeSelf == true)
        {
            Debug.Log("���� ����ư�� Ʈ���");
            for (int i = 0; i < 2; i++)
            {
                specialColl[i].enabled = false;
            }
        }

        if (sell.activeSelf == false)
        {
            Debug.Log("���� ����ư�� ������");
            for (int i = 0; i < 2; i++)
            {
                specialColl[i].enabled = true;
            }
        }
    }
}
