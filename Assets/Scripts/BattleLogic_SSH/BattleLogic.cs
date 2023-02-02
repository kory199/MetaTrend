using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Unity.VisualScripting;
using System.Data;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] public List<GameObject> playerForwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> playerBackwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> enemyForwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> enemyBackwardUnits = new List<GameObject>();

    [SerializeField] public GameObject[] _playerForwardUnits = new GameObject[3];
    [SerializeField] public GameObject[] _playerBackwardUnits = new GameObject[3];
    [SerializeField] public GameObject[] _enemyForwardUnits = new GameObject[3];
    [SerializeField] public GameObject[] _enemyBackwardUnits = new GameObject[3];

    [SerializeField] public List<GameObject> playerAttackList = new List<GameObject>(); //�ܺο��� �����ϱ� ���� attackList 2�� �ۺ����� ����.
    [SerializeField] public List<GameObject> enemyAttackList = new List<GameObject>();

    private bool isPlayerPreemptiveAlive = true; // player ���� ���� ����
    private bool isEnemyPreemptiveAlive = true; // enemy ���� ���� ����
    private bool isFirstAttack = true; // ���� �İ��� ���� bool ���� => true : Player ����
    private bool isResurrection = true; // ��ȯ Ư���� ���� bool ����

    private int playerTurnCount = 0; // Player Turn Count
    private int enemyTurnCount = 0; // Enemy Turn Count

    private int randomArrayNum = 0;
    private int isPlayerAliveCount = 0;
    private int isEnemyAliveCount = 0;


    // ���� master client�� gamemananger���� ������ ���� �迭�� ��ü ���� (�� ���� ���� �� ����)
    private int[] exArray = new int[100];

    private int playerCurRound = 0;
    private int enemyCurRound = 0;

    #region PlayerList �ʱ�ȭ
    public void InitPlayerList()
    {
        Debug.Log("PlayerList �ʱ�ȭ");
        // Master Clinet�� �� ���帶�� �����ϴ� Random Array

        // player ���ݸ���Ʈ �߰�
        if (playerForwardUnits.Count != 0)
        {
            for (int i = 0; i < playerForwardUnits.Count; i++)
            {
                playerAttackList.Add(playerForwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("���� ������ Player ������ ����");
        }

        if (playerBackwardUnits.Count != 0)
        {
            for (int i = 0; i < playerBackwardUnits.Count; i++)
            {
                playerAttackList.Add(playerBackwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("���� ������ Player �Ŀ��� ����");
        }
    }
    #endregion

    #region EnemyList �ʱ�ȭ
    public void InitEnemyList()
    {
        // enemy ���ݸ���Ʈ �߰�
        if (enemyForwardUnits.Count != 0)
        {
            for (int i = 0; i < enemyForwardUnits.Count; i++)
            {
                enemyAttackList.Add(enemyForwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("���ݰ����� Enemy ������ ����");
        }

        if (enemyBackwardUnits.Count != 0)
        {
            for (int i = 0; i < enemyBackwardUnits.Count; i++)
            {
                enemyAttackList.Add(enemyBackwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("���� ������ Enemy �Ŀ��� ����");
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

    public void AliveUnit()
    {
        if (playerForwardUnits.Count == 0)
        {
            isPlayerPreemptiveAlive = false;
        }

        if (enemyForwardUnits.Count == 0)
        {
            isEnemyPreemptiveAlive = false;
        }
    }

    #region Player ���� ����
    // Player ���� ����
    public void PreemptiveAttack()
    {
        // ���� ��ġ���� Ȯ��
        AliveUnit();

        for (int i = 0; i < GameMGR.Instance.randomValue.Length; i++) exArray[i] = GameMGR.Instance.randomValue[i];
        Debug.Log("player ����");

        while (playerAttackList.Count != 0 || enemyAttackList.Count != 0)
        {
            Debug.Log("Player�� ���� ����");

            // ���� ���� ������ �ִ� �迭 1���� ������ �� 0��°�� �ʱ�ȭ
            if (randomArrayNum == exArray.Length) { randomArrayNum = 0; }

            // [Player -> Enemy Attack] ���� ������ ����ִ� ���
            if (isEnemyPreemptiveAlive)
            {
                // �ǰ� ������ ���� ���ö����� randomArray ��ȸ
                while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isEnemyAliveCount = 0;

                    for (int i = 0; i < enemyForwardUnits.Count; i++)
                    {
                        if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }

                        if (isEnemyAliveCount == enemyForwardUnits.Count)
                        {
                            isEnemyPreemptiveAlive = false;
                            break;
                        }
                    }

                    if (isEnemyPreemptiveAlive == false)
                    {
                        break;
                    }
                }

                if (playerAttackList.Count - 1 < playerTurnCount)
                {
                    playerTurnCount = 0;
                }

                // ���� ������ �÷��̾ ���ö� ���� playerturnCount ����
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerTurnCount > playerAttackList.Count - 1) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackList.Count; i++)
                    {
                        if (playerAttackList[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackList.Count) { break; }
                }

                Debug.Log("Player Attack Unit name : " + playerAttackList[playerTurnCount].name);
                Debug.Log("Enemy forward hit unit : " + enemyForwardUnits[exArray[randomArrayNum]].name);
                // �÷��̾� ������ �� ���� ���� ���� ����
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ����Ʈ���� ����
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        enemyForwardUnits[exArray[randomArrayNum]] = null;
                        Debug.Log("Delet Enemy list");
                        break;
                    }
                    else { Debug.Log("enemyAttackList Ž����"); }
                }

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // ���� ������ ���
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == enemyAttackList.Count)
                        {
                            isEnemyAliveCount = 0;
                            BattleWin();
                            break;
                        }
                    }
                }
                isEnemyAliveCount = 0;

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // ���� ������ ������ ���
            // �Ŀ��� ���� ������ ���·� ����
            else if (!isEnemyPreemptiveAlive)
            {
                while (enemyBackwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (playerAttackList.Count - 1 > playerTurnCount)
                {
                    playerTurnCount = 0;
                }

                // ���� ������ �÷��̾ ���ö� ���� playerTurnCount ����
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;

                    if (playerTurnCount > playerAttackList.Count - 1)
                    {
                        playerTurnCount = 0;
                    }

                    for (int i = 0; i < playerAttackList.Count; i++)
                    {
                        if (playerAttackList[i] == null)
                        {
                            isPlayerAliveCount++;
                        }
                    }

                    if (isPlayerAliveCount == playerAttackList.Count)
                    {
                        break;
                    }
                }

                Debug.Log("player attack unit : " + playerAttackList[playerTurnCount].name);
                Debug.Log("enemy backward hit unit : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                // �÷��̾� ������ �� �Ŀ� ���� ���� ����
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ����Ʈ���� ����
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        enemyBackwardUnits[exArray[randomArrayNum]] = null;
                        Debug.Log("Delet enemy unit");
                        break;
                    }

                    else
                    {
                        Debug.Log("enemyAttackList Ž����");
                    }
                }

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // ���� ������ ���
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == enemyAttackList.Count)
                        {
                            isEnemyAliveCount = 0;
                            BattleWin();
                            break;
                        }
                    }
                }

                isEnemyAliveCount = 0;

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // [Enemy -> Player Attack] �÷��̾��� ������ ����ִ� ���
            if (isPlayerPreemptiveAlive)
            {
                // �ǰ� ������ Player�� ���ö����� ���� �� �� ����
                while (playerForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;

                    isPlayerAliveCount = 0;

                    for (int i = 0; i < playerForwardUnits.Count; i++)
                    {
                        if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }

                        if (isPlayerAliveCount == playerForwardUnits.Count)
                        {
                            isPlayerPreemptiveAlive = false;
                            break;
                        }
                    }

                    if (isEnemyPreemptiveAlive == false)
                    {
                        break;
                    }
                }

                if (enemyAttackList.Count - 1 < enemyTurnCount)
                {
                    enemyTurnCount = 0;
                }

                while (enemyAttackList[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    isEnemyAliveCount = 0;

                    if (enemyTurnCount > enemyAttackList.Count - 1)
                    {
                        enemyTurnCount = 0;
                    }

                    for (int i = 0; i < enemyAttackList.Count; i++)
                    {
                        if (enemyAttackList[i] == null)
                        {
                            isEnemyAliveCount++;
                        }
                    }

                    if (isEnemyAliveCount == enemyAttackList.Count)
                    {
                        break;
                    }
                }

                Debug.Log("enemy attack unit : " + enemyAttackList[enemyTurnCount].name);
                Debug.Log("player forward hit Unit : " + playerForwardUnits[exArray[randomArrayNum]].name);
                // �� ������ �÷��̾� ���� �� ������ �÷��̾� ����
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        playerForwardUnits[exArray[randomArrayNum]] = null;
                        Debug.Log("Delet player unit");
                        break;
                    }

                    else
                    {
                        Debug.Log("playerForwardUnits Ž����");
                    }
                }

                // ���ο� random num �ο�
                // ex) ���� �÷��̾��� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // �÷��̾� ������ ������ ���
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == playerAttackList.Count)
                        {
                            isPlayerAliveCount = 0;
                            BattleLose();
                            break;
                        }
                    }
                }
                isPlayerAliveCount = 0;

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
                }

                Debug.Log("enemy attack unit : " + enemyAttackList[enemyTurnCount].name);
                Debug.Log("player backward hit unit : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                // �� ������ �÷��̾� �Ŀ� ���� ���� ����
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� �÷��̾� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        playerBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }

                    else
                    {
                        Debug.Log("playerAttackList Ž����");
                    }
                }


                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;


                // �÷��̾� ������ ������ ���
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == playerAttackList.Count)
                        {
                            isPlayerAliveCount = 0;
                            BattleLose();
                            break;
                        }
                    }
                }
                isPlayerAliveCount = 0;

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
        // ���� ��ġ���� Ȯ��
        AliveUnit();

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
