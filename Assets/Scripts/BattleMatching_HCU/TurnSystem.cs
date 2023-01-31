using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Hashtable = ExitGames.Client.Photon.Hashtable; // �̰� ������ �������� �ʼ������� ���� �𸥴ٴ� ���� �а��� ����


public class TurnSystem : MonoBehaviourPunCallbacks
{
    private bool isGameOver;
    private int []  readyCount = new int[2];

    [SerializeField] private GameObject Me;
    [SerializeField] private GameObject enemyDeck;

    [SerializeField] private GameObject[] playerCardPos;
    [SerializeField] private GameObject[] otherCardPos;

    public Player[] players = null;

    [SerializeField] public InputField inputField; // �г��� �Է�ĭ

    public int[] setRandom = new int[100]; // ������ ����� �������� �����Ѵ�. 

    public Player[] savePlayers = null;

    public int matchingDone;


    public int userID = 0;

    // �÷��̾� ������ ����
    public int[] life = null;
    public int myLife = 10;
    [SerializeField] public int startLife = 10;

    // ���� ���� ����
    public bool isWin = false;
    public bool isLose = false;

    // ���� ���� ī��Ʈ
    public int firstCount = 0;
    public int afterCount = 0;

    // ���� ī��Ʈ
    public int curRound = 0;

    // �� �� ī��Ʈ ( �� �÷��̾���� �� ���� �����´�. �迭�� �ε��� = �ش� �÷��̾� �ε��� )
    public int[] deckCount = new int[8];

    // ���� ���� ����
    public bool[] isFirst = new bool[8];
    public int[] firstPoint = new int[8]; // �� �÷��̾���� ���� ����. �ش� ������ ���� �÷��̾ �������� ������.

    // ���� ���� �������� �÷��̾� ����
    public List<int> leavePlayerList;


    // �����Ͱ� ������ �ִ� ���� ��Ī ��� ��� �迭
    [SerializeField] public int[] prevOpponent = new int[8];

    #region ���� �ݹ� �Լ���

    public override void OnConnectedToMaster()
    {
        photonView.RPC("OnConnectMaster", RpcTarget.All, PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    [PunRPC]
    public void OnConnectMaster(string name)
    {
        Debug.Log(name);
    }

    private void OnFailedToConnectToMasterServer()
    {
        photonView.RPC("FailedConnectMaster", RpcTarget.All, PhotonNetwork.NickName);
    }
    //Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // �÷��̾ ���� �� ���� �÷��̾��� �������� ��� ����Ʈ������ �����ؾ��Ѵ�. ���� �÷��̾��� Ŀ����������Ƽ�� ã�������� �� NULL
        //matchingList.Remove((int)otherPlayer.CustomProperties["Number"]);
        //exitPlayerList.Add((int)otherPlayer.CustomProperties["Nunber"]);
        //if (PhotonNetwork.PlayerList.Length == 2) 
        
            cloneOpponent = -1;
            cloneOpponentsOpponent = -1;
        Debug.Log(otherPlayer + "�� ������");
        if (otherPlayer.IsInactive) Debug.Log(" IsInactive");
        else Debug.Log("NotInactive");

        Debug.Log("���� Ŀ����������Ƽ ��ȣ" + (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);

        //�濡�� ���� �÷��̾�� �ι� �ٽ� �� �濡 ���� ������ ���� ���̴�. ���� ���� �߰�(���� ���� ���Ŀ�)

        //int a = (int)otherPlayer.CustomProperties["Number"]; // ����Ʈ�󿡼� �������� ��.

        /*for (int i = 0; i < userName.Length; i++)
        {
            Debug.Log("������ȣ" + userName[i]);
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == userName[i])
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Number", $"{userName[i]}" } });
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{userLife[i]}" } });
                Debug.Log($"���� {PhotonNetwork.LocalPlayer.NickName}, ���� ��ȣ�� {PhotonNetwork.LocalPlayer.CustomProperties["Number"]}, ���� ü���� {PhotonNetwork.LocalPlayer.CustomProperties["Life"]}");
            }
        }*/
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }
    #endregion

    //List<Player> matchMan = null;
    Player[] matchMan = new Player[8]; // �ִ� 8���̰� �� �̻��� ���� ���� ������ �ϴ� �� ������ ����.
    int[] matchNum;
    int[] prevMatchNum = new int[8];
    int[] userName;
    int[] userLife;

    [PunRPC]
    public void StartSetting() // ���� ���۽� '����' �ѹ��� ����Ǵ� �Լ�
    {
        userName = new int[PhotonNetwork.PlayerList.Length];
        userLife = new int[PhotonNetwork.PlayerList.Length];
        // �÷��̾� ������ȣ �� ������ ����. ���� �����ϴ� �������� �� �÷��̾��Ʈ �迭�� �ε����� ������ȣ ����. ����������� ���� ���ͳѹ� �ұ�Ģ ����
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].CustomProperties["Number"] = i;
            PhotonNetwork.PlayerList[i].CustomProperties["Life"] = startLife; // ���� ��ü���� ����ȭ���� Ŀ����������Ƽ
            userName[i] = i;
            userLife[i] = startLife;
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log($"���� �÷��̾� {i} ��° ��ȣ�� {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
            Debug.Log($"���� �÷��̾� {i} ��°�� ü���� {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
        }

        prevOpponent = new int[8];
    }

    [SerializeField] List<int> matchingList = new List<int>();
    [SerializeField] List<int> matchingListReal = new List<int>();
    [SerializeField] int cloneOpponent = -1;
    [SerializeField] int cloneOpponentsOpponent = -1;
    int c = 0; // ���� üũ ����׿� �ӽ� ����

    //=========================================================================================================================
    // ������ �����ϴ� �Լ�
    public void SetFirstAttack(int me, int you)
    {
        int myDeckCount = GameMGR.Instance.batch.GetBatch(me).Count;
        int youDeckCount = GameMGR.Instance.batch.GetBatch(you).Count;
        // ������ �������� �� ����� �� ���� �켱 �� ��
        if (myDeckCount > youDeckCount)
        {
            isFirst[me] = true;
            isFirst[you] = false;
        }

        // ���� �� ���� ���ٸ�
        else
        {
            int randomPoint = UnityEngine.Random.Range(0, 10);
            int randomPoint2 = UnityEngine.Random.Range(0, 10);
            firstPoint[me] += randomPoint;
            firstPoint[you] += randomPoint2;
        }
    }
    //=========================================================================================================================

    [PunRPC]
    public void MatchingSetting() // �����Ͱ� �� ���帶�� �����ϴ� ���� ����
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //���� ������ ���� - �� ������������ ���� ���� �޾ƿ´�
        for (int i = 0; i < setRandom.Length; i++)
        {
            setRandom[i] = UnityEngine.Random.Range(0, 3);
        }

        int n = -1; // �÷��̾��Ʈ �迭�� ���� �ε��� ��

        matchingList.Clear(); //  �÷��̾� ����Ʈ �ѹ� �ʱ�ȭ �� ���ش�.
        matchingListReal.Clear(); // ���� �÷��̾� ����Ʈ�� �ʱ�ȭ�Ѵ�.

        // �ߺ���Ī ���� ��ũ��Ʈ
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (cloneOpponent != -1)
            {
                matchingList.Add(cloneOpponent);
                for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
                {
                    if ((int)PhotonNetwork.PlayerList[j].CustomProperties["Number"] == cloneOpponent)
                    {
                        cloneOpponentsOpponent = (int)PhotonNetwork.PlayerList[j].CustomProperties["Opponent"];
                    }
                }
                cloneOpponent = -1;
                continue;
            }
            else
            {
                n = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length); // ���� �ε��� �� ����

                if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]) || matchingListReal.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])) { i--; continue; } // �ߺ� ���� ó��
            }
            if (curRound != 0 && PhotonNetwork.PlayerList.Length > 2)
            {
                if (matchingList.Count != 0)
                {
                    if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]) || cloneOpponentsOpponent == (int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])
                    {
                        Debug.Log($"{(int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]}�� �������");

                        // ���ǽ� 6,7���� ���
                        if ((PhotonNetwork.PlayerList.Length == 6 && matchingListReal.Count == 4) || (PhotonNetwork.PlayerList.Length == 7 && matchingListReal.Count == 4))  // 6���� �� ���� �� 4���� ��Ī �Ǿ��� ��
                        {
                            Debug.Log("6�� �Ǵ� 7���̶� ����� ���´�");
                            if (matchingListReal.Contains(prevOpponent[matchingListReal[0]]) && matchingListReal.Contains(prevOpponent[matchingListReal[1]]))
                            {
                                // 6�� ���Դµ� �� 4���̼� ���� ������ �����ٸ� 3,4�� ��Ī ����
                                matchingListReal.RemoveRange(2, 2);
                            }
                        }

                        else if (PhotonNetwork.PlayerList.Length == 8 && matchingListReal.Count == 6)
                        {
                            if (matchingListReal.Contains(prevOpponent[matchingListReal[0]]) && matchingListReal.Contains(prevOpponent[matchingListReal[1]]) && matchingListReal.Contains(prevOpponent[matchingListReal[2]]) && matchingListReal.Contains(prevOpponent[matchingListReal[3]]) && matchingListReal.Contains(prevOpponent[matchingListReal[4]]) && matchingListReal.Contains(prevOpponent[matchingListReal[5]]))
                            {
                                matchingListReal.Clear(); // 6�� ���Դµ� 6�� �� ������ �����ٸ� ����
                                matchingList.Clear();
                            }
                        }

                        c++;
                        if (c < 1000)
                        {
                            i--;
                            continue;
                        }
                        else
                        {
                            Debug.Log("���ѹݺ���");
                            c = 0;
                        }
                    }
                    else
                    {
                        matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);

                        Debug.Log(matchingList.Count);
                        matchingListReal.Add(matchingList[0]);
                        matchingListReal.Add(matchingList[1]);
                        matchingList.Clear();

                        if (matchingListReal.Count >= PhotonNetwork.PlayerList.Length)
                        {

                            matchNum = new int[matchingListReal.Count];
                            Debug.Log(matchNum.Length);
                            for (int j = 0; j < matchNum.Length; j++)
                            {
                                matchNum[j] = matchingListReal[j];
                            }

                            photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, false); // Null�� �ߴ� ����?}
                            curRound++;
                            return;
                        }
                        else
                        {
                            i = -1;
                            continue;
                        }
                    }
                }
                else
                {
                    Debug.Log("����� " + (int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                    matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                    if (matchingListReal.Count >= PhotonNetwork.PlayerList.Length - 1 && PhotonNetwork.PlayerList.Length % 2 != 0) // Ȧ�� + �Ѹ��� �� ¦ã����
                    {
                        int a = UnityEngine.Random.Range(0, matchingListReal.Count);

                        while ((int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"] == matchingListReal[a])
                        {
                            a = UnityEngine.Random.Range(0, matchingListReal.Count);
                        }
                        matchingList.Add(matchingListReal[a]); // Ŭ�� ���� �ڵ� by SJ @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                        cloneOpponent = matchingList[0];
                        matchingListReal.Add(matchingList[0]);
                        matchingListReal.Add(matchingList[1]);
                        matchingList.Clear();
                        matchNum = new int[matchingListReal.Count];
                        for (int j = 0; j < matchNum.Length; j++)
                        {
                            matchNum[j] = matchingListReal[j];
                        }
                        Debug.Log("matchList Cur Count :" + matchingList.Count);

                        photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, true); // Null�� �ߴ� ����?}
                        curRound++;
                        return;
                    }

                }
            }
            else // ù ���忡�� ���� �� �ִ� �κ�
            {
                matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
            }
        }

        if (PhotonNetwork.PlayerList.Length % 2 == 0)
        {
            matchingListInput(false);
        }
        else
        {
            for (int i = 0; i < matchingList.Count; i++)
            {
                matchingListReal.Add(matchingList[i]); // Debug �� ��
            }
            matchingListReal.RemoveAt(matchingList.Count - 1);
            cloneOpponent = matchingList[matchingList.Count - 1];
            matchingList.Add(matchingListReal[UnityEngine.Random.Range(0, matchingListReal.Count)]);

            matchingListInput(true);
        }
        curRound++;
    }
    //=========================================================================================================================

    public void matchingListInput(bool isClone)
    {
        matchNum = new int[matchingList.Count];
        for (int i = 0; i < matchNum.Length; i++)
        {

            matchNum[i] = matchingList[i];
        }

        Debug.Log("prevOpponent ���� : " + prevOpponent.Length);

        photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, isClone);
    }


    [PunRPC] // �� �÷��̾���� ��Ī�� ���������� �Ǿ����� Ȯ���ϱ� ���� RPC �Լ�
    public void ShowDebug(string debugMatching)
    {
        Debug.Log(debugMatching);
    }

    [PunRPC] // ������ Ŭ���̾�Ʈ�� ������ ����ǥ�� �� ���õ��� �޾ƿͼ� ���� �� �ڽŰ� �ڽ��� ��븦 ã�� �Լ�
    public void Matching(int[] random, int[] num, bool clone)
    {
        if (PhotonNetwork.IsMasterClient)   // ������ Ŭ���̾�Ʈ�� ���� ������ ���
        {
            Debug.Log("prevOpponent ���� : " + prevOpponent.Length);
            for (int i = 0; i < num.Length; i++)
            {
                if (clone && i == num.Length - 1) break; // ���� Ŭ���� �ִٸ� �迭 ������ ���� Ŭ���̹Ƿ� ���� ������ ������� �ʴ´�.
                if (i % 2 == 0)
                {
                    prevOpponent[num[i]] = num[i + 1];
                    Debug.Log($"{prevOpponent[num[i]]} �� ���� {num[i + 1]}");
                }
                else
                {
                    prevOpponent[num[i]] = num[i - 1];
                    Debug.Log($"{prevOpponent[num[i]]} �� ���� {num[i - 1]}");
                }
            }
        }


        Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        Debug.Log(num.Length);
        Debug.Log(num[0]);
        Debug.Log(num[1]);
        if (clone) // Ŭ�� ������
        {
            Debug.Log("Ŭ�� �ִ� Ȧ��");
            for (int i = 0; i < num.Length - 1; i++)
            {
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == num[i]) // �� ��ȣ�� ��ġ�ϴ����� �迭 ��ȣ�� ������(Ŭ��) �� �ƴ� ��쿡��
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i + 1] } });
                        Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i + 1]);
                        string matchingDebug = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i + 1];
                        // ���� ����

                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i + 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1);

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);

                    }
                    else if (i % 2 != 0 && i < num.Length)
                    {
                        // �� ���� = matchMan[i-1]
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                        Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1]);
                        string matchingDebug = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1];
                        // ���� �İ�

                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i - 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1);

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);
                    }
                }
            }
        }

        else if (!clone) // Ŭ�� ������ 01 23 45
        {
            Debug.Log("Ŭ�о��� ¦��");
            for (int i = 0; i < num.Length; i++)
            {
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == num[i])
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i + 1] } });
                        Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i + 1]);
                        string matchingDebug = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i + 1];
                        // ���� ����

                        // ���� ¦�� ��� +1 Ȧ��

                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i + 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1);

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);

                    }
                    else if (i % 2 != 0 && i < num.Length)
                    {
                        // �� ���� = matchMan[i-1]
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                        Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1]);
                        string matchingDebug = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1];
                        // ���� �İ�

                        // ���� Ȧ�� ��� -1¦��
                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i - 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1); // ���� ������ �̵�

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);
                    }
                }
            }

        }
        StartCoroutine(COR_DelayMove());

    }
    IEnumerator COR_DelayMove()
    {
        yield return new WaitForSeconds(1f);
        Camera.main.gameObject.transform.position = new Vector3(20, 0, -10);
        GameMGR.Instance.uiManager.storePannel.SetActive(false);
        GameMGR.Instance.batch.UnitPlacement();
        GameMGR.Instance.battleLogic.AttackLogic();
    }
    /*
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

        GoShop();
    }
    */
    [PunRPC]
    public void MatchingReady()
    {
        readyCount[0]++;
        if (PhotonNetwork.IsMasterClient)
        {
            if (readyCount[0] >= PhotonNetwork.PlayerList.Length)
            {
                photonView.RPC("MatchingSetting", RpcTarget.MasterClient);
            }
        }
    }

    public void LifeDown()
    {
        myLife--;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{myLife}" } });
    }

    public void LifeUp()
    {
        myLife++;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{myLife}" } });
    }

    //[PunRPC]
    public void LifeManager()
    {
        //if(!isWin)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log($"{i} ��ȣ�� ���� �г����� {PhotonNetwork.PlayerList[i].NickName}"); // 1
                Debug.Log($"{i}�� Ŀ���� �ѹ��� : {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}"); // 1
                Debug.Log($"{i}�� Ŀ���Ҷ����� �������� : {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
            }
        }
    }

}



