using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameMGR : Singleton<GameMGR>
{
    // 나와 상대방의 매칭 정보를 확인할 수 있는 정수형 배열인 것이다. 0은 나고 1은 상대다.
    public int[] matching = new int[2];

    // 서로가 동일한 랜덤값을 가지기 위한 것이다.
    public int[] randomValue = new int[100];
    public Dictionary<int, List<Card>> playerList = new Dictionary<int, List<Card>>();

    public bool isBattleNow = false; // 현재 전투씬인지 비전투씬인지를 구분하는 불값.


    // 델리게이트 이벤트들의 총집합이라고 보면 되는 것입니다.
    public delegate void _callback_SkillTiming();
    public delegate void _callback_SkillTiming2(Card card);
    public event _callback_SkillTiming callbackEvent_TurnStart;
    public event _callback_SkillTiming callbackEvent_TurnEnd;
    public event _callback_SkillTiming2 callbackEvent_Buy;
    public event _callback_SkillTiming2 callbackEvent_Sell;
    public event _callback_SkillTiming callbackEvent_Reroll;
    public event _callback_SkillTiming callbackEvent_BattleStart;
    public event _callback_SkillTiming callbackEvent_BeforeAttack;
    public event _callback_SkillTiming callbackEvent_AfterAttack;
    public event _callback_SkillTiming callbackEvent_Kill;
    public event _callback_SkillTiming callbackEvent_Hit;
    public event _callback_SkillTiming2 callbackEvent_HitEnemy;
    public event _callback_SkillTiming callbackEvent_Death;
    public event _callback_SkillTiming callbackEvent_Summon;


    #region 델리게이트 함수 모음. 전역적으로 사용될 수 있는 것들만 쓴다.

    public void Event_TurnStart()
    {
        if (callbackEvent_TurnStart != null)
            callbackEvent_TurnStart();
        else
            Debug.Log("아니 지금 턴스타트가 비었다는 말인가");
    }
    public void Event_TurnEnd()
    {
        callbackEvent_TurnEnd();
    }
    public void Event_Buy(Card card)
    {
        Debug.Log(card.cardInfo.objName + "게임매니저에서 함수 실행");
        if (callbackEvent_Buy != null)
            callbackEvent_Buy(card);
    }
    public void Event_Sell(Card card)
    {
        if (callbackEvent_Sell != null)
            callbackEvent_Sell(card);
    }
    public void Event_Reroll()
    {
        if(callbackEvent_Reroll != null) callbackEvent_Reroll();
    }
    public void Event_BattleStart()
    {
        if (callbackEvent_BattleStart != null)
        {
            Debug.Log("전투시작시 불린다");
            callbackEvent_BattleStart();
        }
        else
            Debug.Log("아니 지금 전투시작시가 비었다는 말인가");
    }
    public void Event_Summon()
    {
        callbackEvent_Summon();
    }
    public void Event_HitEnemy(Card card)
    {
        if(callbackEvent_HitEnemy != null)
            callbackEvent_HitEnemy(card);
    }
    
    #endregion

}
