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
    [SerializeField] List<GameObject> playerAttackList = new List<GameObject>();
    [SerializeField] List<GameObject> enemyAttackList = new List<GameObject>();

    bool isPlayerPreemptiveAlive = true; // player ���� ���� ����
    bool isEnemyPreemptiveAlive = true; // enemy ���� ���� ����
    bool isFirstAttack = true; // ���� �İ��� ���� bool ���� => true : Player ����
    bool isResurrection = true; // ��ȯ Ư���� ���� bool ����

    int playerTurnCount = 0; // Player Turn Count
    int enemyTurnCount = 0; // Enemy Turn Count
    int randomArrayNum = 0;

    // ���� master client�� gamemananger���� ������ ���� �迭�� ��ü ���� (�� ���� ���� �� ����)
    int[] exArray = new int[100];

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

    #region Player, EnemyList �ʱ�ȭ
    public void Init()
    {
        // Master Clinet�� �� ���帶�� �����ϴ� Random Array
        for (int i = 0; i < exArray.Length; i++)
        {
            exArray[i] = Random.Range(0, 3);
        }

        // player ���ݸ���Ʈ �߰�
        for (int i = 0; i < playerForwardUnits.Count; i++) { playerAttackList.Add(playerForwardUnits[i]); }
        for (int i = 0; i < playerBackwardUnits.Count; i++) { playerAttackList.Add(playerBackwardUnits[i]); }

        // enemy ���ݸ���Ʈ �߰�
        for (int i = 0; i < enemyForwardUnits.Count; i++) { enemyAttackList.Add(enemyForwardUnits[i]); }
        for (int i = 0; i < enemyBackwardUnits.Count; i++) { enemyAttackList.Add(enemyBackwardUnits[i]); }
    }
    #endregion

    #region ���� ���� 
    // �ΰ��� ���� ����
    public void AttackLogic()
    {
        // ���� ������ ���
        if (isFirstAttack) { PreemptiveAttack(); }

        // ������ ������ ���
        else if (!isFirstAttack) { SubordinatedAttack(); }

        else { Debug.Log("���� �İ��� �������� ����"); }
    }
    #endregion

    #region Player ���� ����
    // Player ���� ����
    public void PreemptiveAttack()
    {
        Debug.Log("platyer ����");

        while (playerAttackList.Count != 0 || enemyAttackList.Count != 0)
        //for (int i = 0; i < 6; i++) 
        {
            Debug.Log("���� ����");

            if (randomArrayNum == exArray.Length)
            {
                randomArrayNum = 0;
            }

            // ���� ������ ����ִ� ���
            if (isEnemyPreemptiveAlive)
            {
                if (playerAttackList.Count >= playerTurnCount)
                {
                    while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    // �÷��̾� ������ �� ���� ���� ���� ����
                    playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                    // �ǰ� ���� ������ ����, ���� ����Ʈ���� ����
                    enemyAttackList.Remove(enemyForwardUnits[exArray[randomArrayNum]]);
                    enemyForwardUnits[exArray[randomArrayNum]] = null;
                    enemyTurnCount--;

                    // ���ο� random num �ο�
                    // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                    randomArrayNum++;


                    // ���� ������ ������ ���
                    if (enemyForwardUnits.Count == 0) { isEnemyPreemptiveAlive = false; }

                    // 1�� ���ῡ ���� �� ���� ���� 
                    playerTurnCount++;

                    
                }

                else
                {
                    // player ���� ���� �ʱ�ȭ
                    playerTurnCount = 0;
                }

            }

            // ���� ������ ������ ���
            else if (!isEnemyPreemptiveAlive)
            {
                // �Ŀ��� ���� ������ ���·� ����
                if (playerAttackList.Count >= playerTurnCount)
                {
                    // �÷��̾� ������ �� �Ŀ� ���� ���� ����
                    playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                    // �ǰ� ���� ������ ����, �Ŀ� ����Ʈ���� ����
                    enemyAttackList.Remove(enemyBackwardUnits[exArray[randomArrayNum]]);
                    enemyBackwardUnits[exArray[randomArrayNum]] = null;

                    // ���ο� random num �ο�
                    randomArrayNum++;

                    // ���� ������ ������ ���
                    if (enemyBackwardUnits.Count == 0)
                    {
                        // �÷��̾� �¸�
                        BattleWin();
                    }
                }
            }

            // �÷��̾��� ������ ����ִ� ���
            if (isPlayerPreemptiveAlive)
            {
                if (enemyAttackList.Count >= enemyTurnCount)
                {
                    while (playerForwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    // �� ������ �÷��̾� ���� �� ������ �÷��̾� ����
                    enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                    // �ǰ� ���� ������ ����, ��������Ʈ���� ����
                    playerAttackList.Remove(playerForwardUnits[exArray[randomArrayNum]]);
                    playerForwardUnits[exArray[randomArrayNum]] = null;

                    // ���ο� random num �ο�
                    // ex) ���� �÷��̾��� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                    randomArrayNum++;

                    // �÷��̾��� ������ ������ ���
                    if (playerForwardUnits.Count == 0) { isPlayerPreemptiveAlive = false; }

                    // 1�� ���ῡ ���� �� ���� ����
                    enemyTurnCount++;
                }

                else
                {
                    // Enemy ���� ���� �ʱ�ȭ
                    enemyTurnCount = 0;
                }

            }

            // �÷��̾��� ������ ������ ���
            else if (!isEnemyPreemptiveAlive)
            {
                // �Ŀ��� ���ݰ����� ���·� ����
                if (enemyAttackList.Count >= enemyTurnCount)
                {
                    // �� ������ �÷��̾� ���� �� ������ �÷��̾� ����
                    enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                    // �ǰ� ���� ������ ����, �Ŀ� ����Ʈ���� ����
                    playerAttackList.Remove(playerBackwardUnits[exArray[randomArrayNum]]);
                    playerBackwardUnits[exArray[randomArrayNum]] = null;

                    // ���ο� random num �ο�
                    randomArrayNum++;

                    // �÷��̾��� ������ ������ ���
                    if (playerBackwardUnits.Count == 0)
                    {
                        // �÷��̾� �й�
                        BattleLose();
                    }
                }
            }

            else
            {
                Debug.Log("����/�Ŀ� �������� Ȯ�� �ʿ� rq_SSH");
            }
            Debug.Log("playerAttackList.Count : " + playerAttackList.Count);
            Debug.Log("enemyAttackList.Count : " + enemyAttackList.Count);
        }
    }
    #endregion

    #region Enemy ���� ����
    // Enemy ���� ����
    public void SubordinatedAttack()
    {

    }
    #endregion

    // �¸� ��
    private void BattleWin()
    {
        Debug.Log("�¸�");
        // �¸� ���� �߰�
    }

    // �й� ��
    private void BattleLose()
    {
        Debug.Log("�й�");
        // �й� ���� �߰�
    }

}
