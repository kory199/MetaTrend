using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batch : MonoBehaviourPun
{
    Dictionary<int, List<Card>> playerList = new Dictionary<int, List<Card>>();
    List<Card> cardList;
    // ������ ��ġ ������ ���� ����
    [PunRPC]
    public void SetBatch(int playerNum, Card card)
    {
        cardList = null;
        Card instance = Resources.Load<Card>($"Prefabs/{card.name}");
        bool listCheck = playerList.TryGetValue(playerNum, out cardList);
        if (listCheck == false)
        {
            cardList = new List<Card>();
        }
        instance.ChangeValue(CardStatus.Hp, card.curHP);
        instance.ChangeValue(CardStatus.Attack, card.curAttackValue);
        instance.ChangeValue(CardStatus.Level, card.level);
        instance.ChangeValue(CardStatus.Exp, card.curEXP);
        cardList.Add(instance);
    }
    public List<Card> GetBatch(int num)
    {
        cardList = null;
        bool listCheck = playerList.TryGetValue(num, out cardList);
        return cardList;
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

        if (myCard == true)
        {
            // unitCard.transform.position = GameMGR.Instance.spawner.cardBatch[cardNum];
        }

        else if(myCard == false)
        {
            // unitCard.transform.position = GameMGR.Instance.spawner.emnycardBatch[cardNum];
        }

        else
        {
            Debug.Log("CreateBatch : myCard �� Ȯ���ʿ�");
        }
        return unitCard;
    }
}
