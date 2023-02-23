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

    public bool firstAttackTime = false;
    public bool secondAttackTime = false;

    [SerializeField] int firstNum = 0;  // �� �ε��� ���尪
    [SerializeField] int secondNum = 0;   // ��� �ε��� ���尪

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
    public bool CheckAllDead(GameObject[] array, bool isArray = true, int forwardAliveNum = 0)
    {
        for(int i = 0; i < 6; i++)
        {
            if (i >= 3) 
            {
                if (isArray) // array ���� �����̶��
                {
                    if (forwardAliveNum == 1)
                        firstForwardAlive = false;// 3������ �˻��ߴµ� ������ �ȉ�ٸ� ���� �����̴�.
                    else if (forwardAliveNum == 2)
                        secondForwardAlive = false;
                }
            }
            
            if (array[i] != null)
            {
                if (i < 3)
                {
                    if (isArray) // array ���� �����̶��
                    {
                        if (forwardAliveNum == 1)
                            firstForwardAlive = true;  // ���� �ִ°� �ɷȴµ� �ε����� 3���� �۴ٸ� ���� ����
                        else if (forwardAliveNum == 2)
                            secondForwardAlive = true;
                    }
                }
                return true;    // ����ִ�.
            }
        }
        return false;
    }

    public GameObject FindTarget(ref bool isForwardAlive, GameObject[] other)
    {
        if(isForwardAlive) // ��� ������ ����ִ� ���
        {
            Debug.Log("��� ������ ����ִٰ� ���̴� �κ��̴� : " + isForwardAlive);
            while (other[exArray[countNum]] == null) // ����� ã�� �� ����
            {
                Debug.Log(countNum);
                Debug.Log(exArray[countNum]);
                if (exArray[countNum] >= 3) // ���� ����ִµ� �޿� �����̸� �տ��� ����
                    exArray[countNum] -= 3;

                Debug.Log(exArray[countNum] + " : 3 �Ѿ 3 �� ��");
                if (other[exArray[countNum]] != null)   return other[exArray[countNum]];
                else
                    countNum++;
                if (countNum >= 200) Debug.Log("200");
            }  
        }

        else // ��� ������ �����ߴٸ�
        {
            Debug.Log("��� ������ ����ִٰ� ���̴� �κ��̴� : " + isForwardAlive);
            while (other[exArray[countNum]] == null) // ����� ã�� �� ����
            {
                Debug.Log(countNum);
                Debug.Log(exArray[countNum]);
                if (exArray[countNum] < 3) // ���� ����ִµ� �޿� �����̸� �տ��� ����
                    exArray[countNum] += 3;
                if (other[exArray[countNum]] != null) return other[exArray[countNum]];  // ���ϰ��� �ִٸ� ã��
                else
                    countNum++;
                if (countNum >= 200) Debug.Log("200");
            }
        }

        return other[exArray[countNum]];
    }

    void FindAttacker(GameObject[] array, ref int num, ref bool attackTime, bool myforwardAlive, GameObject[] me = null)
    {
        if (num > 5) num = 0;
        if (array[num] == null)  // ������ ������ ������ ���� ã�´�
        {
            for (int i = num; i < 6; i++)
            {
                if (array[i] != null)
                {
                    attackTime = true;
                    num = i;
                    return;
                }
            }

            //if (array.All(x => x == null)) { PlayerBattleWin(); }
            //else
            if (array[num] == null)
            {
                if (num > 5) num = 0;
                if (array[num] == null)
                {
                    for (int i = num; i < 6; i++)
                    {
                        if (array[i] != null)
                        {
                            attackTime = true;
                            num = i;
                            return;
                        }
                    }
                }
                else
                    attackTime = true;
            }
        }
        else
            attackTime = true;
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

        }

        // ���� ���� �ʱ�ȭ
        firstNum = 0;
        secondNum = 0;

        bool eventExist = GameMGR.Instance.Event_BattleStart();   // ���� ���۽� ��ų �ߵ� ����
        if (eventExist) yield return new WaitForSeconds(1f); // ��ų�� �ִٸ� ��ų �ߵ� �ð� ��� (���� �ʿ�)

        while(true)
        {
            FindAttacker(firstArray, ref firstNum, ref firstAttackTime, firstForwardAlive, firstArray);

            if (!CheckAllDead(firstArray, true, 1)) // ��� ã�� ���� �� ���� ���� ����ִٸ� ���� ����.
            {
                PlayerBattleLose();
                yield break;
            }

            if (!CheckAllDead(secondArray, true, 2)) // �����׾��µ� ���׾��ٸ� ���� �̱��. �̱�� ���� �Լ��� ������ ������ ������ ���� �ʿ�
            {
                PlayerBattleWin();
                yield break;
            }

            else // ���� �����ִ�
            {
                isWaitAttack = false;

                FindAttacker(firstArray, ref firstNum, ref firstAttackTime, firstForwardAlive, firstArray);

                yield return new WaitUntil(() => firstAttackTime);

                Debug.Log(firstArray[firstNum].name);
                // firstArray[i] �� FindTarget(secondForwardAlive, secondArray) �� ã�� ���� ����
                firstArray[firstNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(ref secondForwardAlive, secondArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;
                firstAttackTime = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

                firstNum++;

            }

            // ========== �ٸ� �� �� =============

            FindAttacker(secondArray, ref secondNum, ref secondAttackTime, secondForwardAlive, secondArray);

            if (!CheckAllDead(secondArray, true, 2))
            {
                PlayerBattleWin(); // ������ �̱��.
                yield break; 
            }


            if (!CheckAllDead(firstArray, true, 1)) // �����׾��µ� ���׾��ٸ� ���� �̱��. �̱�� ���� �Լ��� ������ ������ ������ ���� �ʿ�
            {
                PlayerBattleLose();
                yield break;
            }


            else // ���� �����ִ�
            {
                isWaitAttack = false;

                FindAttacker(secondArray, ref secondNum, ref secondAttackTime, secondForwardAlive, secondArray);

                yield return new WaitUntil(() => secondAttackTime);

                Debug.Log(secondArray[secondNum].name);  // ���ڱ� ���⼭ ���� ��������.
                // firstArray[i] �� FindTarget(secondForwardAlive, secondArray) �� ã�� ���� ����
                secondArray[secondNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(ref firstForwardAlive, firstArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;
                secondAttackTime = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

                secondNum++;
            }
            

        }

    }

    
       
 
}
