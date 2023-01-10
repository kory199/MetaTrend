using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class BattleLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] myForwardUnits = null;
    [SerializeField] GameObject[] myBackwardUnits = null;
    [SerializeField] GameObject[] enemyForwardUnits = null;
    [SerializeField] GameObject[] enemyBackwardUnits = null;

    int UnitNum = 3; // ����, �Ŀ��� ���� ��    
    bool isFirstAttack = false; // ���� �İ��� ���� bool ����
    bool isResurrection = true; // ��ȯ Ư���� ���� bool ����
    int turnCount = 0; // Turn Count

    // ���� master client�� gamemananger���� ������ ���� �迭�� ��ü ����
    int[] exArray = new int[100];
    int randomNum = 0;

    int playerCurRound = 6;
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

        // myPlayer Unit ����, �Ŀ� ���� ���� (0 ~ 2)
        for (int i = 0; i < UnitNum; i++)
        {
            myForwardUnits[i].GetComponent<AttackLogic>().Init(i);

            myBackwardUnits[i].GetComponent<AttackLogic>().Init(i);
        }

        // enemyPlayer Unit ����, �Ŀ� ���� ���� (3 ~ 5)
        for (int i = 0; i < UnitNum; i++)
        {
            enemyForwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
            enemyBackwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
        }
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
        GameObject EnemyUnit = new GameObject();

        while (true)
        {
            // ������ ����ִ� ���
            if (myForwardUnits.Length != 0)
            {
                // �ǰ� Enemy Unit
                randomNum = exArray[turnCount];
                EnemyUnit = enemyForwardUnits[randomNum];
                myForwardUnits[turnCount].GetComponent<AttackLogic>().UnitAttack(turnCount, EnemyUnit);

                turnCount++;
            }

            // ������ ������ ���
            else if(myForwardUnits.Length == 0)
            {

            }
        }
    }

    // Enemy ���� ����
    public void SubordinatedAttack()
    {

    }

    // �ǰݴ��� ���� ����
    public void RemoveUnit(int hitUnitNum)
    {

    }

}
