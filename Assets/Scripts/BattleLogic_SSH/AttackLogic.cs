using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AttackLogic : Skill
{
    Vector2 playerPosition = Vector2.zero;
    Vector2 enemyPosition = Vector2.zero;

    public void UnitAttack(GameObject targetUnit)
    {
/*
        playerPosition = gameObject.transform.position;
        enemyPosition = targetUnit.transform.position;
*/
        Debug.Log(gameObject.name + " ����");
        Debug.Log(targetUnit.name + " �ǰ�");

        // enemy �ǰ� �� ���ƿ�
    }
}
