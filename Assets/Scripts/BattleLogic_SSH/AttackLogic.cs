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

    public void UnitAttack(int playerUnitNum, GameObject targetUnit)
    {
        // BattleLogic Script���� ȣ���� player unit�� ��ġ�ϴ� ��� ����
        if (attackOrder == playerUnitNum)
        {
            playerPosition = gameObject.transform.position;
            enemyPosition = targetUnit.transform.position;

            // enemy �ǰ� �� ���ƿ�
            gameObject.transform.position = Vector2.Lerp(playerPosition, enemyPosition, 0.5f);
            gameObject.transform.position = Vector2.Lerp(enemyPosition, playerPosition, 0.5f);
        }
    }
}
