using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackSound : MonoBehaviour
{
    private void OnMouseUp()
    {
        // ��� ������ ������ �Ҹ�
        if (gameObject.CompareTag("BackImage"))
        {
            GameMGR.Instance.audioMGR.SoundMouseClick();
            Debug.Log("Ŭ������ ����");
        }
    }
}
