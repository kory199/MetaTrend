using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    public GameObject[] firstForward = null;
    public GameObject[] firstBackward = null;
    public GameObject[] secondForward = null;
    public GameObject[] secondBackward = null;

    public GameObject[] firstArray = new GameObject[6];
    public GameObject[] secondArray = new GameObject[6];

    public bool firstForwardAlive = true;
    public bool secondForwardAlive = true;

    int countNum = 0; // ���� �迭�� �ε��� ī���� �뵵

    private void Start2()
    {
        Init2();
        PhotonNetwork.FetchServerTimestamp();
    }

    private void Init2()
    {
        exArray = new int[200];
        playerForwardUnits = new GameObject[3];
        playerBackwardUnits = new GameObject[3];
        enemyForwardUnits = new GameObject[3];
        enemyBackwardUnits = new GameObject[3];
    }

    // ���� ���� + ���� ���� üũ
    public bool CheckAllDead(GameObject[] array, bool forwardAlive)
    {
        for(int i = 0; i < 6; i++)
        {
            if (i >= 3) forwardAlive = false; // 3������ �˻��ߴµ� ������ �ȉ�ٸ� ���� �����̴�.
            if (array[i] != null)
            {
                if (i < 3) forwardAlive = true; // ���� �ִ°� �ɷȴµ� �ε����� 3���� �۴ٸ� ���� ����
                return true;
            }
        }
        return false;
    }

    public GameObject FindTarget(bool isForwardAlive, GameObject[] other)
    {
        if(isForwardAlive) // ��� ������ ����ִ� ���
        {
            while(other[exArray[countNum]] == null) // ����� ã�� �� ����
            {
                if (exArray[countNum] >= 3) // ���� ����ִµ� �޿� �����̸� �տ��� ����
                    exArray[countNum] -= 3;
                if (other[exArray[countNum]] != null) break; // ������ �ִٸ� ã��
                countNum++;
            }  
        }

        else // ��� ������ �����ߴٸ�
        {
            while (other[exArray[countNum]] == null) // ����� ã�� �� ����
            {
                if (exArray[countNum] < 3) // ���� ����ִµ� �޿� �����̸� �տ��� ����
                    exArray[countNum] += 3;
                if (other[exArray[countNum]] != null) break; // ���ϰ��� �ִٸ� ã��
                countNum++;
            }
        }

        return other[exArray[countNum]];
    }

    IEnumerator InBattleLogic(bool playerFirst)
    {
        AliveUnit();

        // �÷��̾ �������̸� player first
        if (playerFirst)
        {
            firstArray = playerAttackArray;
            firstForward = playerForwardUnits;
            firstBackward = playerBackwardUnits;

            secondArray = enemyAttackArray;
            secondForward = enemyForwardUnits;
            secondBackward = enemyBackwardUnits;

            firstForwardAlive = isPlayerPreemptiveAlive;
            secondForwardAlive = isEnemyPreemptiveAlive;
        }
        // ��밡 �����̸� enemy first
        else
        {
            firstArray = enemyAttackArray;
            firstForward = enemyForwardUnits;
            firstBackward = enemyBackwardUnits;

            secondArray = playerAttackArray;
            secondForward = playerForwardUnits;
            secondBackward = playerBackwardUnits;

            firstForwardAlive = isEnemyPreemptiveAlive;
            secondForwardAlive = isPlayerPreemptiveAlive;
        }

        bool eventExist = GameMGR.Instance.Event_BattleStart();   // ���� ���۽� ��ų �ߵ� ����
        if (eventExist) yield return new WaitForSeconds(1f); // ��ų�� �ִٸ� ��ų �ߵ� �ð� ��� (���� �ʿ�)

        int myNum = 0;  // �� �ε��� ���尪
        int enemyNum = 0;   // ��� �ε��� ���尪

        // �� ���� ����
        while (true)
        {
            if (firstArray[myNum] == null)  // ������ ������ ������ ���� ã�´�
            {
                if (myNum > 5) myNum = 0;
                for (int i = myNum; i < 6; i++)
                {
                    if (firstArray[myNum] != null) break;
                    myNum++;
                }
            }

            if (!CheckAllDead(secondArray, secondForwardAlive)) // �����׾��µ� ���׾��ٸ� ���� �̱��. �̱�� ���� �Լ��� ������ ������ ������ ���� �ʿ�
            {
                PlayerBattleWin();
                yield break;
            }

            else // ���� �����ִ�
            {
                isWaitAttack = false;
                // firstArray[i] �� FindTarget(secondForwardAlive, secondArray) �� ã�� ���� ����
                firstArray[myNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(secondForwardAlive, secondArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

            }

            // ========== �ٸ� �� �� =============

            if (secondArray[enemyNum] == null)  // ������ ������ ������ ���� ã�´�
            {
                if (enemyNum > 5) enemyNum = 0;
                for (int i = enemyNum; i < 6; i++)
                {
                    if (firstArray[enemyNum] != null) break;
                    enemyNum++;
                }
            }

            if (!CheckAllDead(firstArray, firstForwardAlive)) // �����׾��µ� ���׾��ٸ� ���� �̱��. �̱�� ���� �Լ��� ������ ������ ������ ���� �ʿ�
            {
                PlayerBattleLose();
                yield break;
            }

            else // ���� �����ִ�
            {
                isWaitAttack = false;
                // firstArray[i] �� FindTarget(secondForwardAlive, secondArray) �� ã�� ���� ����
                secondArray[enemyNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(firstForwardAlive, firstArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

            }
        }
    }
       
 
}
