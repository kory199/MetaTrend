using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSpwaner : MonoBehaviourPun
{
    bool isMyunit = true;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        // ���� �������� �޾ƿ� ���� ��ġ ����
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(0, i, 0 == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }

        // ��Ī�� ������ �������� �޾ƿ� ���� ��ġ ����
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(0, i, 0 == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }
    }
}
