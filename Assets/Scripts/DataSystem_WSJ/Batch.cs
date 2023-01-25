using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batch : MonoBehaviourPun
{
    Dictionary<int, List<Card>> playerList = new Dictionary<int, List<Card>>();
    List<Card> cardList;

    Transform[] myCardPosition = null;
    Transform[] enemyCardPosition = null;

    bool isMinePlayerNum = true;

    // �ѽ��� ���� ���� ������ ���� �߰� �ڵ� - HCU *������
    int tempHp = 0;
    int tempAtk = 0;
    int tempExp = 0;
    int tempLevel = 0;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        unitPlacement();
    }

    public void Init()
    {
        GameObject temporaryPlayerObjects = GameObject.Find("PlayerPosition");
        GameObject temporaryEnemyObjects = GameObject.Find("EnemyPosition");
        myCardPosition = temporaryPlayerObjects.transform.GetComponentsInChildren<Transform>();
        enemyCardPosition = temporaryEnemyObjects.transform.GetComponentsInChildren<Transform>();
    }

    // ������ ��ġ ������ ���� ���� *������
    [PunRPC]
    public void SetBatch(int playerNum, Card card, int tempHp, int tempAtk, int tempExp, int tempLevel)
    {
        cardList = null;
        Card instance = Resources.Load<Card>($"Prefabs/{card.name}");
        bool listCheck = playerList.TryGetValue(playerNum, out cardList);
        if (listCheck == false)
        {
            cardList = new List<Card>();
        }
        instance.ChangeValue(CardStatus.Hp, card.curHP + tempHp);
        instance.ChangeValue(CardStatus.Attack, card.curAttackValue + tempAtk);
        instance.ChangeValue(CardStatus.Exp, card.curEXP + tempExp);
        instance.ChangeValue(CardStatus.Level, card.level + tempLevel);
        cardList.Add(instance);
    }

    public List<Card> GetBatch(int playerNum)
    {
        cardList = null;
        bool listCheck = playerList.TryGetValue(playerNum, out cardList);
        return cardList;
    }
    public void unitPlacement()
    {
        // ���� ��ġ ����
        // ���� �İ� ����
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[0], i, GameMGR.Instance.matching[0] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }

        // ��Ī�� ������ �������� �޾ƿ� ���� ��ġ ����
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[1], i, GameMGR.Instance.matching[1] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }
    }

    // ��Ʋ�� ���� ��ġ
    /// <summary>
    ///  playerNum : Photon Customproperties ������ȣ, 
    ///  cardNum : ī�� ��ġ ����, 
    ///  myCard : ���� ī�� ����
    /// </summary>
    /// <param name="CreateBatch"></param>
    public Card CreateBatch(int playerNum, int cardNum, bool myCard = true)
    {
        List<Card> cardList = null;
        playerList.TryGetValue(playerNum, out cardList);
        Card unitCard = GameObject.Instantiate<Card>(cardList[cardNum]);

        // player Unit ��ġ ����
        if (myCard == true)
        {
            unitCard.transform.position = myCardPosition[cardNum + 1].position;
        }

        // enemy Unit ��ġ ����
        else if (myCard == false)
        {
            unitCard.transform.position = enemyCardPosition[cardNum + 1].position;
        }

        else
        {
            Debug.Log("CreateBatch : myCard �� Ȯ���ʿ�");
        }
        return unitCard;
    }


}
