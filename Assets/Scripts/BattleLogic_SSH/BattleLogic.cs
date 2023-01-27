using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] public List<GameObject> playerForwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> playerBackwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> enemyForwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> enemyBackwardUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> playerAttackList = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyAttackList = new List<GameObject>();

    private bool isPlayerPreemptiveAlive = true; // player ���� ���� ����
    private bool isEnemyPreemptiveAlive = true; // enemy ���� ���� ����
    private bool isFirstAttack = true; // ���� �İ��� ���� bool ���� => true : Player ����
    private bool isResurrection = true; // ��ȯ Ư���� ���� bool ����

    private int playerTurnCount = 0; // Player Turn Count
    private int enemyTurnCount = 0; // Enemy Turn Count
    private int randomArrayNum = 0;

    // ���� master client�� gamemananger���� ������ ���� �迭�� ��ü ���� (�� ���� ���� �� ����)
    private int[] exArray = new int[100];

    private int playerCurRound = 0;
    private int enemyCurRound = 0;

    

    #region Player, EnemyList �ʱ�ȭ
    public void Init()
    {
        // Master Clinet�� �� ���帶�� �����ϴ� Random Array

        // player ���ݸ���Ʈ �߰�
        for (int i = 0; i < playerForwardUnits.Count; i++)
        {
            playerAttackList.Add(playerForwardUnits[i]);
            playerAttackList.Add(playerBackwardUnits[i]);
        }

        // enemy ���ݸ���Ʈ �߰�
        for (int i = 0; i < enemyForwardUnits.Count; i++)
        {
            enemyAttackList.Add(enemyForwardUnits[i]);
            enemyAttackList.Add(enemyBackwardUnits[i]);
        }
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
        for (int i = 0; i < GameMGR.Instance.randomValue.Length; i++)
            exArray[i] = GameMGR.Instance.randomValue[i];
        Debug.Log("player ����");

        while (playerAttackList.Count != 0 || enemyAttackList.Count != 0)
        {
            Debug.Log("���� ����");

            // ���� ���� ������ �ִ� �迭 1���� ������ �� 0��°�� �ʱ�ȭ
            if (randomArrayNum == exArray.Length)
            {
                randomArrayNum = 0;
            }

            // [Player -> Enemy Attack] ���� ������ ����ִ� ���
            if (isEnemyPreemptiveAlive)
            {
                // �ǰ� ������ ���� ���ö����� randomArray ��ȸ
                while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                // ���� ������ �÷��̾ ���ö� ���� playerturnCount ����
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                }

                // �÷��̾� ������ �� ���� ���� ���� ����
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        break;
                    }
                    else
                    {
                        Debug.Log("enemyAttackList Ž����");
                    }
                }

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                enemyForwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // ���� ������ ������ ���
                if (enemyForwardUnits[0] == null && enemyForwardUnits[1] == null && enemyForwardUnits[2] == null)
                {
                    isEnemyPreemptiveAlive = false;
                }

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // ���� ������ ������ ���
            else if (!isEnemyPreemptiveAlive)
            {
                // �Ŀ��� ���� ������ ���·� ����
                while (enemyBackwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (playerAttackList.Count - 1 > playerTurnCount)
                {
                    playerTurnCount = 0;
                }

                Debug.Log("***playerTurnCount : " + playerTurnCount);
                // ���� ������ �÷��̾ ���ö� ���� playerTurnCount ����
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    if (playerTurnCount > playerAttackList.Count - 1)
                    {
                        playerTurnCount = 0;
                    }
                    Debug.Log("***playerTurnCount : " + playerTurnCount);
                }

                // �÷��̾� ������ �� �Ŀ� ���� ���� ����
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        break;
                    }

                    else
                    {
                        Debug.Log("enemyAttackList Ž����");
                    }
                }

                //�ǰ� ���� ������ �Ŀ� ����Ʈ���� ����
                enemyBackwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // ���� �Ŀ��� ������ ���
                if (enemyBackwardUnits[0] == null && enemyBackwardUnits[1] == null && enemyBackwardUnits[2] == null)
                {
                    // �÷��̾� �¸�
                    BattleWin();
                    break;
                }

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // [Enemy -> Player Attack] �÷��̾��� ������ ����ִ� ���
            if (isPlayerPreemptiveAlive)
            {
                while (playerForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (enemyAttackList.Count == enemyTurnCount)
                {
                    enemyTurnCount = 0;
                }

                else if (enemyAttackList.Count != enemyTurnCount)
                {
                    while (enemyAttackList[enemyTurnCount] == null)
                    {
                        enemyTurnCount++;
                    }
                }

                else
                {
                    Debug.Log("enemyTurnCount Ȯ�� �ʿ�");
                }

                // �� ������ �÷��̾� ���� �� ������ �÷��̾� ����
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        break;
                    }

                    else
                    {
                        Debug.Log("playerForwardUnits Ž����");
                    }
                }

                playerForwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) ���� �÷��̾��� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // �÷��̾��� ������ ������ ���
                if (playerForwardUnits[0] == null && playerForwardUnits[1] == null && playerForwardUnits[2] == null)
                {
                    isPlayerPreemptiveAlive = false;
                }

                // 1�� ���ῡ ���� �� ���� ����
                enemyTurnCount++;
            }

            // �÷��̾��� ������ ������ ���
            else if (!isPlayerPreemptiveAlive)
            {
                // �Ŀ��� ���� ������ ���·� ����

                while (playerBackwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (enemyTurnCount > enemyAttackList.Count - 1)
                {
                    enemyTurnCount = 0;
                }

                Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                while (enemyAttackList[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    if (enemyTurnCount > enemyAttackList.Count - 1)
                    {
                        enemyTurnCount = 0;
                    }
                    Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                }

                // �� ������ �÷��̾� �Ŀ� ���� ���� ����
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� �÷��̾� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        Debug.Log(playerBackwardUnits[exArray[randomArrayNum]] + " : null");
                        break;
                    }

                    else
                    {
                        Debug.Log("playerAttackList Ž����");
                    }
                }

                playerBackwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // �÷��̾��� �Ŀ��� ������ ���
                if (playerBackwardUnits[0] == null && playerBackwardUnits[1] == null && playerBackwardUnits[2] == null)
                {
                    // �÷��̾� �й�
                    BattleLose();
                    break;
                }

                // 1�� ���ῡ ���� �� ���� ���� 
                enemyTurnCount++;
                // player ���� ���� �ʱ�ȭ
                enemyTurnCount = 0;
            }

            else
            {
                Debug.Log("����/�Ŀ� �������� Ȯ�� �ʿ� rq_SSH");
            }
        }
    }
    #endregion

    #region Enemy ���� ����
    // Enemy ���� ����
    public void SubordinatedAttack()
    {
        Debug.Log("Enemy ����");

        while (playerAttackList.Count != 0 || enemyAttackList.Count != 0)
        {
            Debug.Log("���� ����");

            // ���� ���� ������ �ִ� �迭 1���� ������ �� 0��°�� �ʱ�ȭ
            if (randomArrayNum == exArray.Length)
            {
                randomArrayNum = 0;
            }

            // [Enemy -> Player Attack] �÷��̾��� ������ ����ִ� ���
            if (isPlayerPreemptiveAlive)
            {
                while (playerForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (enemyAttackList.Count == enemyTurnCount)
                {
                    enemyTurnCount = 0;
                }

                else if (enemyAttackList.Count != enemyTurnCount)
                {
                    while (enemyAttackList[enemyTurnCount] == null)
                    {
                        enemyTurnCount++;
                    }
                }

                else
                {
                    Debug.Log("enemyTurnCount Ȯ�� �ʿ�");
                }

                // �� ������ �÷��̾� ���� �� ������ �÷��̾� ����
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        break;
                    }

                    else
                    {
                        Debug.Log("playerForwardUnits Ž����");
                    }
                }

                playerForwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) ���� �÷��̾��� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // �÷��̾��� ������ ������ ���
                if (playerForwardUnits[0] == null && playerForwardUnits[1] == null && playerForwardUnits[2] == null)
                {
                    isPlayerPreemptiveAlive = false;
                }

                // 1�� ���ῡ ���� �� ���� ����
                enemyTurnCount++;
            }

            // �÷��̾��� ������ ������ ���
            else if (!isPlayerPreemptiveAlive)
            {
                // �Ŀ��� ���� ������ ���·� ����

                while (playerBackwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (enemyTurnCount > enemyAttackList.Count - 1)
                {
                    enemyTurnCount = 0;
                }

                Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                while (enemyAttackList[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    if (enemyTurnCount > enemyAttackList.Count - 1)
                    {
                        enemyTurnCount = 0;
                    }
                    Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                }

                // �� ������ �÷��̾� �Ŀ� ���� ���� ����
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� �÷��̾� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        Debug.Log(playerBackwardUnits[exArray[randomArrayNum]] + " : null");
                        break;
                    }

                    else
                    {
                        Debug.Log("playerAttackList Ž����");
                    }
                }

                playerBackwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // �÷��̾��� �Ŀ��� ������ ���
                if (playerBackwardUnits[0] == null && playerBackwardUnits[1] == null && playerBackwardUnits[2] == null)
                {
                    // �÷��̾� �й�
                    BattleLose();
                    break;
                }

                // 1�� ���ῡ ���� �� ���� ���� 
                enemyTurnCount++;
                // player ���� ���� �ʱ�ȭ
                enemyTurnCount = 0;
            }

            else
            {
                Debug.Log("����/�Ŀ� �������� Ȯ�� �ʿ� rq_SSH");
            }

            // [Player -> Enemy Attack] ���� ������ ����ִ� ���
            if (isEnemyPreemptiveAlive)
            {
                // �ǰ� ������ ���� ���ö����� randomArray ��ȸ
                while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                // ���� ������ �÷��̾ ���ö� ���� playerturnCount ����
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                }

                // �÷��̾� ������ �� ���� ���� ���� ����
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        break;
                    }
                    else
                    {
                        Debug.Log("enemyAttackList Ž����");
                    }
                }

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                enemyForwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // ���� ������ ������ ���
                if (enemyForwardUnits[0] == null && enemyForwardUnits[1] == null && enemyForwardUnits[2] == null)
                {
                    isEnemyPreemptiveAlive = false;
                }

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // ���� ������ ������ ���
            else if (!isEnemyPreemptiveAlive)
            {
                // �Ŀ��� ���� ������ ���·� ����
                while (enemyBackwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (playerAttackList.Count - 1 > playerTurnCount)
                {
                    playerTurnCount = 0;
                }

                Debug.Log("***playerTurnCount : " + playerTurnCount);
                // ���� ������ �÷��̾ ���ö� ���� playerTurnCount ����
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    if (playerTurnCount > playerAttackList.Count - 1)
                    {
                        playerTurnCount = 0;
                    }
                    Debug.Log("***playerTurnCount : " + playerTurnCount);
                }

                // �÷��̾� ������ �� �Ŀ� ���� ���� ����
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        break;
                    }

                    else
                    {
                        Debug.Log("enemyAttackList Ž����");
                    }
                }

                //�ǰ� ���� ������ �Ŀ� ����Ʈ���� ����
                enemyBackwardUnits[exArray[randomArrayNum]] = null;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // ���� �Ŀ��� ������ ���
                if (enemyBackwardUnits[0] == null && enemyBackwardUnits[1] == null && enemyBackwardUnits[2] == null)
                {
                    // �÷��̾� �¸�
                    BattleWin();
                    break;
                }

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }
        }
    }
    #endregion

    // �¸� ��
    private void BattleWin()
    {
        Debug.Log("Player Win");
        // �¸� ���� �߰�
    }

    // �й� ��
    private void BattleLose()
    {
        Debug.Log("Player Lose");
        // �й� ���� �߰�
    }
}
