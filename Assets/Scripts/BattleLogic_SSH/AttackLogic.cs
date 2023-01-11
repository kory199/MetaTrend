using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLogic : MonoBehaviour
{
    [SerializeField] public int attackOrder = 0;

    Vector2 playerPosition = Vector2.zero;
    Vector2 enemyPosition = Vector2.zero;

    public void Init(int order)
    {
        // �÷��̾� ���� ����
        attackOrder = order;
    }

    public void UnitAttack(GameObject targetUnit)
    {
        playerPosition = gameObject.transform.position;
        enemyPosition = targetUnit.transform.position;

        Debug.Log(gameObject.name + " ����");
        Debug.Log(targetUnit.name + " �ǰ�");

        // enemy �ǰ� �� ���ƿ�
    }
}
