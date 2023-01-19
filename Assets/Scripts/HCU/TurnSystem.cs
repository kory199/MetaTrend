using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using ExitGames.Client.Photon;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.Experimental.AI;
//using Hashtable = ExitGames.Client.Photon.Hashtable; // �̰� ������ �������� �ʼ������� ���� �𸥴ٴ� ���� �а��� ����

namespace hcu
{
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

        // �� �� ī��Ʈ ( �� �÷��̾���� �� ���� �����´� )
        public int[] deckCount;

        // 

        #region ���� �ݹ� �Լ���

        public void SetName()
        {
            string myName = null;
            PhotonNetwork.NickName = myName;
        }

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
        public override void OnJoinedLobby()
        {
            Debug.Log("�κ� ���� ����");
            PhotonNetwork.JoinRoom("test");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // ��ɼǿ� ���� ����
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20 };
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            PhotonNetwork.CreateRoom("test", roomOptions);
            Debug.Log("test�� ����");

            photonView.RPC("FailedJoinRoom", RpcTarget.All, PhotonNetwork.NickName);
        }

        [PunRPC]
        public void FailedJoinRoom(string name)
        {
            Debug.Log(name + "�� �� �����µ� �����ߴ�");
        }

        [PunRPC]
        public void FailedConnectMaster(string name)
        {
            Debug.Log(name + "�� �����ͼ��� ���ῡ �����ߴ�");
        }

        private void OnFailedToConnectToMasterServer()
        {
            photonView.RPC("FailedConnectMaster", RpcTarget.All, PhotonNetwork.NickName);
        }
        //Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        public override void OnJoinedRoom() // �ڱ� �ڽŸ�
        {
            Debug.Log("test�� ����");
            players = PhotonNetwork.PlayerList;
            Debug.Log("�� ������ �� : " + PhotonNetwork.PlayerList.Length);
            Debug.Log(players[0].ActorNumber);
            for (int i = 0; i < players.Length; i++)
            {
                PhotonNetwork.PlayerList[i].NickName = (PhotonNetwork.PlayerList[i].ActorNumber - 1).ToString();
                if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName)
                {
                    ExitGames.Client.Photon.Hashtable myCustomProperty = new ExitGames.Client.Photon.Hashtable();
                    myCustomProperty = PhotonNetwork.LocalPlayer.CustomProperties;
                    PhotonNetwork.PlayerList[i].CustomProperties["Number"] = i;
                    PhotonNetwork.PlayerList[i].CustomProperties["Life"] = startLife;
                    PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Number", $"{i}" } });
                    PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{startLife}" } });
                    PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", -1 } });
                    userID = (int)PhotonNetwork.PlayerList[i].CustomProperties["Number"];  // �����г����� ��ġ�ϸ� ������ȣ�� �����Ѵ�.
                    Debug.Log($" ��ȣ �� �� ���� {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
                    Debug.Log($" ������ �� ���� {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
                    PhotonNetwork.SetPlayerCustomProperties(myCustomProperty);
                }
                Debug.Log($"�� ������ȣ�� {(int)PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) // �ٸ� ��� �����
        {
            Debug.Log(newPlayer.NickName + " ����");
            Debug.Log("�� ������ �� : " + PhotonNetwork.PlayerList.Length);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            matchingList.Remove((int)otherPlayer.CustomProperties["Number"]);
            Debug.Log(otherPlayer + "�� ������");
            if (otherPlayer.IsInactive) Debug.Log(" IsInactive");
            else Debug.Log("NotInactive");

            Debug.Log("���� Ŀ����������Ƽ ��ȣ" + (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
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
            Debug.Log($"{targetPlayer} �� {changedProps}�� ����Ǿ���");
        }
        #endregion

        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings(); // ���� �⺻���� ��� �� �����ͼ��� ����
        }

        private void Start()
        {
            GameLoop();
        }

        private void GameLoop()
        {
            //BattleOrder(); // ������� ����

            if (!isGameOver)
                GoShop(); // �������� ��!
        }

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
            // �÷��̾� ������ ���� (�迭 ����� ���� ����)
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
        }

        [SerializeField] List<int> matchingList = new List<int>();
        [SerializeField] List<int> matchingList2 = new List<int>();
        [SerializeField] List<int> matchingList3 = new List<int>();

        [PunRPC]
        public void MatchingSetting()
        {
            //���� ������ ���� - �� ������������ ���� ���� �޾ƿ´�
            for (int i = 0; i < setRandom.Length; i++)
            {
                setRandom[i] = UnityEngine.Random.Range(0, 3);
            }

            int n = 0;

            matchingList.Clear(); //  �÷��̾� ����Ʈ �ѹ� �ʱ�ȭ �� ���ش�.
            // �ߺ���Ī ���� ��ũ��Ʈ
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("���� " + i);
                n = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);

                if(matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])|| matchingList2.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])) { i--; continue; } // �ߺ� ���� ó��

                if (curRound != 0 && PhotonNetwork.PlayerList.Length>2)
                {
                    if (i % 2 != 0)
                    {
                        if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]))
                        {
                            Debug.Log($"{(int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]}�� �������");

                            i--;
                            continue;
                        }
                        else
                        {
                            Debug.Log("3 " + matchingList[0]);
                            matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                            Debug.Log("4 " + matchingList[1]);
                            matchingList2.Add(matchingList[0]);
                            matchingList2.Add(matchingList[1]);
                            matchingList.Clear();
                            if (matchingList2.Count >= PhotonNetwork.PlayerList.Length)
                            {
                                if (matchingList2.Count % 2 != 0)
                                {
                                    matchingList3=matchingList2; // ��α⸦ ������ ������ ���� ������ �����ϱ� ���� ������ ��Ī����Ʈ ������Ʈ ��Ÿ �׽�Ʈ ���� �ް� ���� 3

                                    matchingList3.Remove(matchingList3[matchingList3.Count - 1]);
                                    int a = UnityEngine.Random.Range(0, matchingList3.Count);
                                    int clone = matchingList3[a];
                                    matchingList2.Add(clone);
                                    for(int j = 0; j < matchNum.Length; j++)
                                    {
                                        matchNum[j] = matchingList2[j];
                                    }
                                    matchingList2.Clear();
                                    matchingList3.Clear();
                                    photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, true); // Null�� �ߴ� ����?}
                                    return;

                                }
                                else 
                                {
                                    matchNum = new int[matchingList2.Count];
                                    Debug.Log(matchNum.Length);
                                    for (int j = 0; j < matchNum.Length; j++)
                                    {
                                        matchNum[j] = matchingList2[j];
                                    }
                                    matchingList2.Clear();
                                    photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, false); // Null�� �ߴ� ����?}
                                    return;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("����� " + (int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                        matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                    }
                }
                else
                {
                    matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                }
            }

            curRound++;

            // �÷��̾���� ť�� �� ��������� �迭�� ������� �ִ´�.
            matchNum = new int[matchingList.Count];
            Debug.Log(matchNum.Length);
            for (int i = 0; i < matchNum.Length; i++)
            {
                matchNum[i] = matchingList[i];
            }

            photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum); // Null�� �ߴ� ����?
        }

        [PunRPC] // �� �÷��̾���� ��Ī�� ���������� �Ǿ����� Ȯ���ϱ� ���� RPC �Լ�
        public void ShowDebug(string a)
        {
            Debug.Log(a);
        }

        [PunRPC]
        public void Matching(int[] random, int[] num,bool clone)
        {

            if (clone) // Ŭ�� ������
            {
                Debug.Log("1:1 & 1:1 ����");
                for (int i = 0; i < num.Length; i++)
                {
                    if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == num[i] && i < num.Length - 1) // �� ��ȣ�� ��ġ�ϴ����� �迭 ��ȣ�� ������(Ŭ��) �� �ƴ� ��쿡��
                    {
                        if (i == 0 || i % 2 == 0)
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i+1] } });
                            Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i+1] );
                            string a = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i + 1];
                            // ���� ����
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                            
                        }
                        else if ( i % 2 != 0 && i < num.Length )
                        {
                            // �� ���� = matchMan[i-1]
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                            Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1]);
                            string a = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1];
                            // ���� �İ�
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                        }
                    }
                    else if(i < num.Length - 1)
                    {

                    }
                }
            }

            else if (!clone) // Ŭ�� ������ 01 23 45
            {
                Debug.Log("1:1 & 1:1 ����");
                for (int i = 0; i < num.Length; i++)
                {
                    if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == num[i])
                    {
                        if (i == 0 || i % 2 == 0)
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i + 1] } });
                            Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i + 1]);
                            string a = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i + 1];
                            // ���� ����

                            // ���� ¦�� ��� +1 Ȧ��

                            
                            SceneManager.LoadScene(1);


                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);

                        }
                        else if (i % 2 != 0 && i < num.Length)
                        {
                            // �� ���� = matchMan[i-1]
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                            Debug.Log("���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1]);
                            string a = "���� " + userID + " �г��� : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " ��� : " + num[i - 1];
                            // ���� �İ�

                            // ���� Ȧ�� ��� -1¦��

                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                        }
                    }
                }
            }

        }

        private void GoShop()
        {
            // ���� ������ �̵�
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


        private void Update()
        {
            //if(PhotonNetwork.IsMasterClient)
            if (Input.GetKeyDown(KeyCode.S))
            {
/*                if(PhotonNetwork.IsMasterClient)
                {
                    MatchingSetting();
                }*/
                photonView.RPC("MatchingSetting", RpcTarget.MasterClient);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                photonView.RPC("StartSetting", RpcTarget.MasterClient);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                // photonView.RPC("LifeManager", RpcTarget.MasterClient);
                LifeManager();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                isWin = true;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                isWin = false;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                LifeDown();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                LifeUp();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    Debug.Log($"{PhotonNetwork.PlayerList[i].NickName}");
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

}

