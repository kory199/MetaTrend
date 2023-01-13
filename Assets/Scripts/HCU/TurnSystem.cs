using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System;
using System.Linq;
//using Hashtable = ExitGames.Client.Photon.Hashtable; // 이게 구버전 한정인지 필수인지는 나도 모른다는 것이 학계의 점심


public class TurnSystem : MonoBehaviourPunCallbacks
{
    private bool isGameOver;

    [SerializeField] private GameObject Me;
    [SerializeField] private GameObject enemyDeck;

    [SerializeField] private GameObject[] playerCardPos;
    [SerializeField] private GameObject[] otherCardPos;

    public Player[] players = null;

    [SerializeField] public InputField inputField; // 닉네임 입력칸

    public int[] setRandom = new int[500]; // 공격할 대상을 랜덤으로 지정한다. 

    public Player[] savePlayers = null;

    public int matchingDone;

    public int userID = 0;

    // 플레이어 라이프 관련
    public int[] life = null;
    public int myLife = 10;
    [SerializeField] public int startLife = 10;

    // 전투 승패 관련
    public bool isWin = true;

    #region 포톤 콜백 함수들

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
        Debug.Log("로비에 입장 성공");
        PhotonNetwork.JoinRoom("test");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // 룸옵션에 대한 정보
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20 };
        roomOptions.IsOpen= true;
        roomOptions.IsVisible = true;
        PhotonNetwork.CreateRoom("test", roomOptions);
        Debug.Log("test룸 생성");

        photonView.RPC("FailedJoinRoom", RpcTarget.All, PhotonNetwork.NickName);
    }

    [PunRPC]
    public void FailedJoinRoom(string name)
    {
        Debug.Log(name + "가 방 들어오는데 실패했다"); 
    }

    [PunRPC]
    public void FailedConnectMaster(string name)
    {
        Debug.Log(name + "가 마스터서버 연결에 실패했다");
    }

    private void OnFailedToConnectToMasterServer()
    {
        photonView.RPC("FailedConnectMaster", RpcTarget.All, PhotonNetwork.NickName);
    }
    //Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
    public override void OnJoinedRoom() // 자기 자신만
    {
        Debug.Log("test룸 입장");
        players = PhotonNetwork.PlayerList;
        Debug.Log("방 접속자 수 : " + PhotonNetwork.PlayerList.Length);
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
                userID = (int)PhotonNetwork.PlayerList[i].CustomProperties["Number"];  // 고유닉네임이 일치하면 고유번호를 지정한다.
                Debug.Log($" 번호 값 잘 들어갔니 {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
                Debug.Log($" 라이프 잘 들어갔니 {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
                PhotonNetwork.SetPlayerCustomProperties(myCustomProperty);
            } 
            Debug.Log($"내 지정번호는 {(int)PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // 다른 사람 입장시
    {
        Debug.Log(newPlayer.NickName + " 입장");
        Debug.Log("방 접속자 수 : " + PhotonNetwork.PlayerList.Length);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer + "가 나갔다");
        if (otherPlayer.IsInactive) Debug.Log(" IsInactive");
        else Debug.Log("NotInactive");

        Debug.Log("커스텀프로퍼티 번호" + (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("유저번호"+userName[i]);
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == userName[i])
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { {"Number", $"{userName[i]}"} }); 
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { {"Life", $"{userLife[i]}"} });
                Debug.Log($"나는 {PhotonNetwork.LocalPlayer.NickName}, 나의 번호는 {PhotonNetwork.LocalPlayer.CustomProperties["Number"]}, 나의 체력은 {PhotonNetwork.LocalPlayer.CustomProperties["Life"]}");
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log($"{targetPlayer} 의 {changedProps}가 변경되었다");
    }

    #endregion

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(); // 포톤 기본세팅 사용 및 마스터서버 연결
    }

    private void Start()
    {
        GameLoop();
    }

    private void GameLoop()
    {
        BattleOrder(); // 순서대로 전투

        if (!isGameOver)
            GoShop(); // 상점으로 가!
    }

    //List<Player> matchMan = null;
    Player[] matchMan = new Player[8]; // 최대 8인이고 그 이상을 넘을 수는 없으니 일단 이 값으로 지정.
    int[] matchNum;
    int[] userName;
    int[] userLife;

    [PunRPC]
    public void StartSetting() // 게임 시작시 '최초' 한번만 실행되는 함수
    {
        userName = new int[PhotonNetwork.PlayerList.Length];
        userLife = new int[PhotonNetwork.PlayerList.Length];
        // 플레이어 라이프 지정
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].CustomProperties["Number"] = i;
            PhotonNetwork.PlayerList[i].CustomProperties["Life"] = startLife; // 서버 전체에서 동기화해줄 커스텀프로퍼티
            userName[i]= i;
            userLife[i]= startLife;
            
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log($"포톤 플레이어 {i} 번째 번호는 {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
            Debug.Log($"포톤 플레이어 {i} 번째의 체력은 {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
        }
    }




    [PunRPC]
    public void MatchingSetting()
    {
        Queue<int> matchNumQueue = new Queue<int>();

        //공격 랜덤값 지정 - 매 스테이지마다 랜덤 값을 받아온다
        for (int i = 0; i < setRandom.Length; i++) 
        {
            setRandom[i] = UnityEngine.Random.Range(0, 3);
        }
        int n = 0;
        // 큐에 플레이어들을 랜덤한 순서대로 집어넣는다.

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            n = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);
            if(matchNumQueue.Contains(n)) { i--; continue; }
            matchNumQueue.Enqueue(n);
            Debug.Log("큐에 " + n + " 추가");
        }
        Debug.Log(matchNumQueue.Count);
        // 플레이어들을 큐에 다 집어넣으면 배열에 순서대로 넣는다.
        matchNum = new int[PhotonNetwork.PlayerList.Length];
        for (int i = 0; i <matchNum.Length; i++)
        {
            matchNum[i] = matchNumQueue.Dequeue();
          //  Debug.Log("큐에서 1개 꺼냄");
        }

        photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum); // Null이 뜨는 이유?
    }

    [PunRPC] // 각 플레이어들의 매칭이 정상적으로 되었는지 확인하기 위한 RPC 함수
    public void ShowDebug(string a)
    {
        Debug.Log(a);
    }

    [PunRPC]
    public void Matching(int[] random, int[] num)
    {
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            matchMan[i] = PhotonNetwork.PlayerList[num[i]];
        }
        //마스터 클라이언트가 대진 설정을 마치고 각 플레이어들을 1:1로 묶는 함수
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            Debug.Log("나 혼자 방에 남아버렸다");
        }

        // 플레이어가 짝수
        else if (PhotonNetwork.PlayerList.Length % 2 == 0)
        {
            Debug.Log("1:1 & 1:1 시작");
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (matchMan[i].NickName == PhotonNetwork.NickName)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        // 내 상대는  = matchMan[i+1] 
                        Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i + 1].NickName + " 내가 선공");
                        string a = "나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i + 1].NickName;
                        // 내가 선공
                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                    }
                    else
                    {
                        // 내 상대는 = matchMan[i-1]
                        Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i - 1].NickName + " 내가 후공");
                        string a = "나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i - 1].NickName + " 내가 후공";
                        // 내가 후공
                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                    }
                }
            }
        }

        // 플레이어가 홀수, 3명 이상이라면
        else if (PhotonNetwork.PlayerList.Length % 2 == 1 && PhotonNetwork.PlayerList.Length > 2)
        {
            Debug.Log("1:1:1 시작");
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (matchMan[i].NickName == PhotonNetwork.NickName)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        if (i == PhotonNetwork.PlayerList.Length - 1)
                        {
                            Debug.Log("마지막 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i - 1].NickName + " 내가 선공");
                            string a = "마지막 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i - 1].NickName + " 내가 선공";
                            // 내가 선공
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                        }
                        else
                        {
                            Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i + 1].NickName + " 내가 선공");
                            string a = "나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i + 1].NickName + " 내가 선공";
                            // 내가 후공
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                        }
                    }
                    else
                    {
                        // 내 상대는 = matchMan[i-1]
                        Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i - 1].NickName + " 내가 후공");
                        string a = "나는 " + userID + " 닉네임 : " + matchMan[i].NickName + " 상대 : " + matchMan[i - 1].NickName + " 내가 후공";
                        // 내가 후공
                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                    }
                }
            }
        }

    }



    private void GoShop()
    {
        // 상점 씬으로 이동
    }

    private void BattleOrder()
    {
        // 패시브(순서 상관없이 항상 발동) // 함수 X

        // 전투 시작
        // if 내가 선공이라면
        for (int i = 0; i < 6; i++)
        {
            // myDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
            // otherDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
        }
        // else 상대가 선공이라면
        for (int i = 0; i < 6; i++)
        {
            // otherDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
            // myDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
        }

        // 턴 종료 = 상점으로 이동
        GoShop();
    }


    private void Update()
    {
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
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
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
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log($"{i} 번호의 유저 닉네임은 {PhotonNetwork.PlayerList[i].NickName}"); // 1
                Debug.Log($"{i}의 커스텀라이프 라이프값 : {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
            }
        }
    }

    

}
