using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class BattleLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> playerForwardUnits = new List<GameObject>();
    [SerializeField] List<GameObject> playerBackwardUnits = new List<GameObject>();
    [SerializeField] List<GameObject> enemyForwardUnits = new List<GameObject>();
    [SerializeField] List<GameObject> enemyBackwardUnits = new List<GameObject>();

    List<GameObject> playerAttackList = new List<GameObject>();
    List<GameObject> enemyAttackList = new List<GameObject>();

    bool isPlayerPreemptiveAlive = true; // player ���� ���� ����
    bool isEnemyPreemptiveAlive = true; // enemy ���� ���� ����

    bool isFirstAttack = true; // ���� �İ��� ���� bool ����
    bool isResurrection = true; // ��ȯ Ư���� ���� bool ����
    int playerTurnCount = 0; // Player Turn Count
    int enemyTurnCount = 0; // Enemy Turn Count
    int randomArrayNum = 0;

    // ���� master client�� gamemananger���� ������ ���� �迭�� ��ü ���� (�� ���� ���� �� ����)
    int[] exArray = new int[100];
    int randomNum = 0;

    int playerCurRound = 0;
    int enemyCurRound = 0;

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        AttackLogic();
    }

    public void Init()
    {
        // Master Clinet�� �� ���帶�� �����ϴ� Random Array
        for (int i = 0; i < exArray.Length; i++)
        {
            exArray[i] = Random.Range(0, 3);
        }

        // player ���ݸ���Ʈ �߰�
        for (int i = 0; i < playerForwardUnits.Count; i++)
        {
            playerAttackList.Add(playerForwardUnits[i]);
        }
        for (int i = 0; i < playerBackwardUnits.Count; i++)
        {
            playerAttackList.Add(playerBackwardUnits[i]);
        }

        // enemy ���ݸ���Ʈ �߰�
        for (int i = 0; i < enemyForwardUnits.Count; i++)
        {
            enemyAttackList.Add(enemyForwardUnits[i]);
        }
        for (int i = 0; i < enemyBackwardUnits.Count; i++)
        {
            enemyAttackList.Add(enemyBackwardUnits[i]);
        }


        /*
                // myPlayer Unit ����, �Ŀ� ���� ���� (0 ~ 2)
                for (int i = 0; i < UnitNum; i++)
                {
                    playerForwardUnits[i].GetComponent<AttackLogic>().Init(i);
                    playerBackwardUnits[i].GetComponent<AttackLogic>().Init(i);
                }

                // enemyPlayer Unit ����, �Ŀ� ���� ���� (3 ~ 5)
                for (int i = 0; i < UnitNum; i++)
                {
                    enemyForwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
                    enemyBackwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
                }
        */
    }

    // �ΰ��� ���� ����
    public void AttackLogic()
    {
        // ���� ������ ���
        if (isFirstAttack)
        {
            PreemptiveAttack();
        }

        // ������ ������ ���
        else if (!isFirstAttack)
        {
            SubordinatedAttack();
        }

        else
        {
            Debug.Log("���� �İ��� �������� ����");
        }
        // ���� �Ŀ� ���� �Ǵ� ����
    }

    // Player ���� ����
    public void PreemptiveAttack()
    {
        while (playerAttackList.Count == 0 || enemyAttackList.Count == 0)
        {
            // �÷��̾��� ������ ����ִ� ���
            if (isPlayerPreemptiveAlive)
            {
                if (playerAttackList.Count <= playerTurnCount)
                {
                    // �÷��̾� ������ �� ���� ���� �� ������ �� ����
                    playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                    // �ǰ� ���� ������ ����, ���� ����Ʈ���� ����
                    enemyForwardUnits.Remove(enemyForwardUnits[exArray[randomArrayNum]]);
                    enemyAttackList.Remove(enemyForwardUnits[exArray[randomArrayNum]]);

                    // ���ο� random num �ο�
                    // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                    randomArrayNum++;

                    // ���� ������ ������ ���
                    if (enemyForwardUnits.Count == 0)
                    {
                        isEnemyPreemptiveAlive = false;
                    }
                }

                else
                {
                    // player ���� ���� �ʱ�ȭ
                    playerTurnCount = 0;
                }

                // 1�� ���ῡ ���� �� ���� ����
                playerTurnCount++;
            }

            // �÷��̾��� ������ ������ ���
            else if (!isPlayerPreemptiveAlive)
            {
                // �Ŀ��� ���� ������ ���·� ����
            }

            // ���� ������ ����ִ� ���
            if (isEnemyPreemptiveAlive)
            {
                if (enemyAttackList.Count <= enemyTurnCount)
                {
                    // �� ������ �÷��̾� ���� �� ������ �� ����
                    enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                    // �ǰ� ���� ������ ����, ��������Ʈ���� ����
                    playerAttackList.Remove(playerForwardUnits[exArray[randomArrayNum]]);
                    playerForwardUnits.Remove(playerForwardUnits[exArray[randomArrayNum]]);

                    // ���ο� random num �ο�
                    // ex) ���� �÷��̾��� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                    randomArrayNum++;

                    // �÷��̾��� ������ ������ ���
                    if (playerForwardUnits.Count == 0)
                    {
                        isPlayerPreemptiveAlive = false;
                    }
                }

                else
                {
                    // Enemy ���� ���� �ʱ�ȭ
                    enemyTurnCount = 0;
                }

                // 1�� ���ῡ ���� �� ���� ����
                enemyTurnCount++;
            }

            // ���� ������ ������ ���
            else if(!isEnemyPreemptiveAlive)
            {
                // �Ŀ��� ���ݰ����� ���·� ����
            }

            else
            {
                Debug.Log("����/�Ŀ� �������� Ȯ�� �ʿ� rq_SSH");
            }

        }

    }

    // Enemy ���� ����
    public void SubordinatedAttack()
    {

    }
}
