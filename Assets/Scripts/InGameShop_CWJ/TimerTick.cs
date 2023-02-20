using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TimerTick : MonoBehaviour
{
    public Sprite[] sprites; // �̹��� �迭
    public Image image; // �̹��� ������Ʈ
    WaitForSeconds waittimer = new WaitForSeconds(0.3f);

    private int currentSprite = 0; // ���� �̹��� �ε���

    void Start()
    {
        StartCoroutine(AnimateSprite()); // �ڷ�ƾ ����
    }

    IEnumerator AnimateSprite()
    {
        while (true)
        {
            image.sprite = sprites[currentSprite]; // �̹��� ����
            currentSprite++; // �ε��� ����

            if (currentSprite >= sprites.Length)
            {
                currentSprite = 0; // �迭 ���� �����ϸ� ó������ ���ư�
            }

            if (GameMGR.Instance.uiManager.isTimerFast == false)
            {
                Debug.Log("Ÿ�̸� ���ο�");
                yield return waittimer; // 0.3�� ���
            }

            else if (GameMGR.Instance.uiManager.isTimerFast == true)
            {
                Debug.Log(GameMGR.Instance.uiManager.isTimerFast);
                Debug.Log("Ÿ�̸� �н�Ʈ");
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
