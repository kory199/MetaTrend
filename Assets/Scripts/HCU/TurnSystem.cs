using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
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
                n = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);

                if(matchingList.Contains(n)){ i--; continue; } // �ߺ� ���� ó��

                if(curRound != 0)
                {
                    if (i % 2 != 0)
                    {
                        matchingList.Add(n);
                        if ((int)PhotonNetwork.PlayerList[matchingList[i]].CustomProperties["Opponent"] == (int)PhotonNetwork.PlayerList[matchingList[i-1]].CustomProperties["Number"])
                        {
                            Debug.Log($"{(int)PhotonNetwork.PlayerList[matchingList[i - 1]].CustomProperties["Opponent"]}�� ���� ��밡 {(int)PhotonNetwork.PlayerList[matchingList[i]].CustomProperties["Number"]}");
                            
                            //i--;
                            //continue;
                        }
                        matchingList.Remove(n);
                    }
                }

                matchingList.Add(n);
               
            }

            curRound++;

            Debug.Log($"��Ī ����Ʈ ī��Ʈ = {matchingList.Count}");

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
        public void Matching(int[] random, int[] num)
        {
            for (int i = 0; i < num.Length; i++)
            {
                matchMan[i] = PhotonNetwork.PlayerList[num[i]];
                Debug.Log(i + "��° ����" + matchMan[i]);
            }

            // ���� �İ� �迭�� ������ ��� �ش��ϴ� ��ũ��Ʈ
            /*
            // ���� �İ��� ���ư��鼭 ��Ī�Ѵٴ� ����.

            int arrayLength = 0;

            if (matchMan.Length % 2 == 0)
            {
                arrayLength = matchMan.Length / 2;
            }
            else
            {
                arrayLength = matchMan.Length / 2 + 1;
            }

            
            int[] first = new int[arrayLength]; // ���� �迭
            int[] after = new int[arrayLength]; // �İ� �迭

            for (int i = 0; i < first.Length; i++)
            {
                if(i % 2 == 0)
                first[i] = (int)matchMan[i].CustomProperties["Number"];
                else
                after[i] = (int)matchMan[i].CustomProperties["Number"];
            }

            int[] temp = new int[first.Length];

            //���� �İ� ������
            for (int i = 0; i < first.Length; i++)
            { 
                temp[i] = after[0];
                after[0] = after[i];
                after[i] = temp[i];
            }

            // �İ� ��ĭ�� �и���
            for (int i = 0; i < first.Length; i++)
            {
                if(i == first.Length - 1)
                {
                    temp[i] = after[i];
                    after[i] = after[0];
                    after[0] = temp[i];
                }
                else
                {
                    temp[i] = after[i];
                    after[i] = after[i + 1];
                    after[i + 1] = temp[i];
                }
            }

            //�߰��� ���� �� �ִ��� üũ
            for(int i = 0; i < first.Length; i++)
            {
                if (!playerList.Contains(first[i]))
                {
                    // first[i] �� �濡 �������� �ʴ´�.
                }
                else if (!playerList.Contains(after[i]))
                {
                    // after[i] �� �濡 �������� �ʴ´�.
                }
            }

            for(int i = 0; i < first.Length; i++)
            {
                Debug.Log($"{i}�� 1:1 = {first[i]} vs {after[i]}");
            }
            */

            //================================  ������ Ŭ���̾�Ʈ�� ���� ������ ��ġ�� �� �÷��̾���� 1:1�� ���� �Լ�  =====================================
            /*
            for(int i = 0; i < matchMan.Length; i++)
            {
                if (matchMan[i].NickName == PhotonNetwork.NickName) // ���� ���� �İ� ī��Ʈ�� üũ
                {
                    if (i % 2 == 0)
                    {
                        // ���� ���� üũ
                        firstCount++;
                        if (firstCount > 1)
                        {
                            // �������� ������ �İ�
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SetAttack", 2 } });
                            firstCount = 0;
                        }
                    }
                    else
                    {
                        // �İ� ���� üũ
                        afterCount++;
                        if (afterCount > 1)
                        {
                            // �������� ������ ����
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SetAttack", 1 } });
                            afterCount = 0;
                        }
                    }
                }
            }*/


            if (num.Length < 2)
            {
                Debug.Log("�� ȥ�� �濡 ���ƹ��ȴ�");
            }

            if (num.Length == 2)
            {
                Debug.Log("1:1");
                if (matchMan[0].NickName == PhotonNetwork.NickName)
                {
                    Debug.Log($"���� {matchMan[0].CustomProperties["Number"]}, ���� ����");
                }
                else
                {
                    Debug.Log($"���� {matchMan[1].CustomProperties["Number"]}, ���� �İ�");
                }
            }

            // �÷��̾ ¦��
            else if (num.Length % 2 == 0)
            {
                Debug.Log("1:1 & 1:1 ����");
                for (int i = 0; i < num.Length; i++)
                {
                    if (matchMan[i].NickName == PhotonNetwork.NickName)
                    {
                        if (i == 0 || i % 2 == 0)
                        {
                            Debug.Log("���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i + 1].CustomProperties["Number"] + " ���� ����");
                            string a = "���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i + 1].CustomProperties["Number"] + " ���� ����";
                            // ���� ����
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                            matchMan[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", (int)matchMan[i + 1].CustomProperties["Number"] } });
                        }
                        else if ( i % 2 != 0 && i < num.Length )
                        {
                            // �� ���� = matchMan[i-1]
                            Debug.Log("���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i - 1].CustomProperties["Number"] + " ���� �İ�");
                            string a = "���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i - 1].CustomProperties["Number"] + " ���� �İ�";
                            // ���� �İ�
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                            matchMan[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", (int)matchMan[i - 1].CustomProperties["Number"] } });
                        }
                    }
                }
            }

            // �÷��̾ Ȧ��, 3�� �̻��̶��
            else if (num.Length % 2 == 1 && num.Length > 2)
            {
                Debug.Log("1:1:1 ����");
                for (int i = 0; i < num.Length; i++)
                {
                    if (matchMan[i].NickName == PhotonNetwork.NickName)
                    {
                        if (i == 0 || i % 2 == 0)
                        {
                            if (i == num.Length - 1)
                            {
                                Debug.Log("������ " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i - 1].CustomProperties["Number"] + " ���� ����");
                                string a = "������ " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i - 1].CustomProperties["Number"] + " ���� ����";
                                // ���� ����
                                photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                                matchMan[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", (int)matchMan[i - 1].CustomProperties["Number"] } });
                            }
                            else
                            {
                                Debug.Log("���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i + 1].CustomProperties["Number"] + " ���� ����");
                                string a = "���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i + 1].CustomProperties["Number"] + " ���� ����";
                                // ���� �İ�
                                photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                                matchMan[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", (int)matchMan[i + 1].CustomProperties["Number"] } });
                            }
                        }
                        else
                        {
                            // �� ���� = matchMan[i-1]
                            Debug.Log("���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i - 1].CustomProperties["Number"] + " ���� �İ�");
                            string a = "���� " + userID + " �г��� : " + matchMan[i].CustomProperties["Number"] + " ��� : " + matchMan[i - 1].CustomProperties["Number"] + " ���� �İ�";
                            // ���� �İ�
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                            matchMan[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", (int)matchMan[i - 1].CustomProperties["Number"] } });
                        }
                    }
                }
            }
             // ��Ī ������
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

