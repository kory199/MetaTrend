using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public enum OrderType
{
    Buy,
    Sell,
    Start,
    Attack,
    Hit,
    Kill,
    Die,
    Alive,
    End
}

public class TurnSystem : MonoBehaviourPunCallbacks
{
    private bool isGameOver;

    [SerializeField] private GameObject Me;
    [SerializeField] private GameObject enemyDeck;

    [SerializeField] private GameObject[] playerCardPos;
    [SerializeField] private GameObject[] otherCardPos;

    public Player[] players = null;

    [SerializeField] public InputField inputField; // �г��� �Է�ĭ

    public int[] setRandom = new int[500]; // ������ ����� �������� �����Ѵ�. 

    public int userID = 0;
    /*    private TurnSystem instance = null;
        public TurnSystem Inst
        {
            get
            {
                if(instance == null)
                    instance = FindObjectOfType<TurnSystem>();
                return instance;
            }
        }*/

    #region ���� �ݹ� �Լ���

    public void SetName()
    {
        string myName = null;
        PhotonNetwork.NickName = myName;
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("�����ͼ����� ���� ����");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ����");


        PhotonNetwork.JoinRoom("test");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // ��ɼǿ� ���� ����
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20 };
        PhotonNetwork.CreateRoom("test", roomOptions);
        //PhotonNetwork.JoinRoom("test");
        Debug.Log("test�� ����");
    }



    public override void OnJoinedRoom() // �ڱ� �ڽŸ�
    {
        Debug.Log("test�� ����");

        // �÷��̾� ������ȣ ���� ( ���������� ������ �����ϱ� ���� )
        players = PhotonNetwork.PlayerList;
        Debug.Log("�� ������ �� : " + PhotonNetwork.PlayerList.Length);
        Debug.Log(players[0].ActorNumber);
        for (int i = 0; i < players.Length; i++)
        {
            PhotonNetwork.NickName = PhotonNetwork.PlayerList[i].ActorNumber.ToString();
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName) userID = i;  // �����г����� ��ġ�ϸ� ������ȣ�� �����Ѵ�.
        }
        /*for(int i = 0; i < players.Length; i++)
        {
            // ���� ��ȣ�� �ű��.
            if (players[i].ActorNumber == playerNum)
            {
                playerID = i;
                Debug.Log("�� ID�� " + i);
                playerNum++;
            }
        }*/

    }
    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer) // �ٸ� ��� �����
    {
        Debug.Log(newPlayer.NickName + " ����");
        Debug.Log("�� ������ �� : " + PhotonNetwork.PlayerList.Length);
    }

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(); // ���� �⺻���� ��� �� �����ͼ��� ����
    }

    private void Start()
    {
        //playerCardPos = new GameObject[6];
        //otherCardPos = new GameObject[6];
        GameLoop();
    }

    private void GameLoop()
    {

        BattleOrder(); // ������� ����

        if (!isGameOver)
            GoShop(); // �������� ��!
    }

    //List<Player> matchMan = null;
    Player[] matchMan = new Player[8]; // �ִ� 8���̰� �� �̻��� ���� ���� ������ �ϴ� �� ������ ����.

    [PunRPC]
    public void MatchingSetting()
    {
        // ��Ī�� ������Ŭ���̾�Ʈ ȥ�� �����Ͽ� �������� �����Ѵ�.

        Queue<Player> matchQueue = new Queue<Player>();

        for (int i = 0; i < setRandom.Length; i++)
        {
            setRandom[i] = Random.Range(0, 3);
        }
        int n = 0;
        // ť�� �÷��̾���� ������ ������� ����ִ´�.

        players = PhotonNetwork.PlayerList;
        Debug.Log("players.Length �� ���̴� :" + players.Length);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            n = Random.Range(0, PhotonNetwork.PlayerList.Length);
                if (matchQueue.Contains(players[n]))
                { i--; continue; }
                matchQueue.Enqueue(players[n]);
                Debug.Log("ť�� 1�� �߰�");
        }
        Debug.Log(matchQueue.Count);
        // �÷��̾���� ť�� �� ��������� �迭�� ������� �ִ´�.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            matchMan[i] = matchQueue.Dequeue();
            Debug.Log("ť���� 1�� ����");
        }

        // ��� ������ ������.
        Debug.Log(setRandom);
        Debug.Log(matchMan);
        photonView.RPC("Matching", RpcTarget.All, setRandom, matchMan);
        Debug.Log("RPC Matchig ȣ��");
        if(setRandom == null)
        {
            Debug.Log("setRandom�� ��");
        }
        if(matchMan == null)
        {
            Debug.Log("matchman�� ��");
        }
    }

    [PunRPC]
    public void Matching(int[] random, Player[] matchManAll)
    {
        if(random == null)
        Debug.Log("random �迭 �Ѱܹޱ� ����");
        if(matchManAll == null)
        Debug.Log("matchMan �迭 �Ѱܹޱ� ����");
        matchMan = matchManAll;
        //������ Ŭ���̾�Ʈ�� ���� ������ ��ġ�� �� �÷��̾���� 1:1�� ���� �Լ�
        Debug.Log("Matching �Լ� ����");
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            // �� ȥ�� �������� �ڵ� �¸�
            Debug.Log("�� ȥ�� �濡 ���ƹ��ȴ�");
        }

        else if (PhotonNetwork.PlayerList.Length == 2)
        {
            // 1 : 1 ����
            if (matchMan[0].NickName == PhotonNetwork.NickName)
            {
                // ���� ����
                Debug.Log("���� " + userID + " �г��� : " + matchMan[0].NickName + " ��� : " + matchMan[1].NickName);
                // �� ���� �迭1
            }
            else
            {
                // ���� �İ�
                Debug.Log("���� " + userID + " �г��� : " + matchMan[1].NickName + " ��� : " + matchMan[0].NickName);
                // �� ���� �迭
            }
        }

        // �÷��̾ ¦��, 4�� �̻��̶��
        else if (PhotonNetwork.PlayerList.Length % 2 == 0 && PhotonNetwork.PlayerList.Length > 3)
        {
            for (int i = 0; i < players.Length;)
            {
                if (matchMan[i].NickName == PhotonNetwork.NickName)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        // �� ����  = matchMan[i+1] 
                        Debug.Log("���� " + userID + " �г��� : " + matchMan[0].NickName + " ��� : " + matchMan[1].NickName);
                        // ���� ����
                    }
                    else
                    {
                        // �� ���� = matchMan[i-1]
                        Debug.Log("���� " + userID + " �г��� : " + matchMan[1].NickName + " ��� : " + matchMan[0].NickName);
                        // ���� �İ�
                    }
                }
            }
        }

        // �÷��̾ Ȧ��, 3�� �̻��̶��
        else if (PhotonNetwork.PlayerList.Length % 2 == 1 && PhotonNetwork.PlayerList.Length > 2)
        {
            for (int i = 0; i < players.Length;)
            {
                if (matchMan[i].NickName == PhotonNetwork.NickName)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        // �� ����  = matchMan[i+1]
                        Debug.Log("���� " + userID + " �г��� : " + matchMan[0].NickName + " ��� : " + matchMan[1].NickName);
                    }
                    else
                    {
                        // �� ���� = matchMan[i-1]
                        Debug.Log("���� " + userID + " �г��� : " + matchMan[1].NickName + " ��� : " + matchMan[0].NickName);
                    }
                }
            }
        }
    }

    private void GoShop()
    {
        // ���� ������ �̵�

    }

    private void BattleOrder()
    {
        // �нú�(���� ������� �׻� �ߵ�) // �Լ� X

        // ���� ����
        // if ���� �����̶��
        for (int i = 0; i < 6; i++)
        {
            // myDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
            // otherDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
        }
        // else ��밡 �����̶��
        for (int i = 0; i < 6; i++)
        {
            // otherDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
            // myDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
        }

        // �� ���� = �������� �̵�
        GoShop();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            photonView.RPC("MatchingSetting", RpcTarget.MasterClient);
        }
    }


}
