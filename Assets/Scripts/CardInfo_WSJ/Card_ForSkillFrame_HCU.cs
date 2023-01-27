using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public partial class Card : MonoBehaviour
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] Card skillTarget;

    [SerializeField] GameObject curPos;

    public void SetSkillTiming() // ��ų�� ���� �ߵ���Ű���Ŀ� ���� �� �̺�Ʈ�� �߰������ش�.
    {
        switch(cardInfo.skillTiming)
        {
            case SkillTiming.turnStart:
                GameMGR.Instance.callbackEvent_TurnStart += SkillActive;
                break;
            case SkillTiming.turnEnd:
                GameMGR.Instance.callbackEvent_TurnEnd += SkillActive;
                break;
            case SkillTiming.buy:
                GameMGR.Instance.callbackEvent_Buy += SkillActive;
                break;
            case SkillTiming.sell:
                GameMGR.Instance.callbackEvent_Sell += SkillActive;
                break;
            case SkillTiming.reroll:
                GameMGR.Instance.callbackEvent_Reroll += SkillActive;
                break;
            case SkillTiming.attackBefore:
                GameMGR.Instance.callbackEvent_BeforeAttack += SkillActive;
                break;
            case SkillTiming.attackAfter:
                GameMGR.Instance.callbackEvent_AfterAttack += SkillActive;
                break;
            case SkillTiming.kill:
                GameMGR.Instance.callbackEvent_Kill += SkillActive;
                break;
            case SkillTiming.hit:
                GameMGR.Instance.callbackEvent_Hit += SkillActive;
                break;
            case SkillTiming.hitEnemy:
                GameMGR.Instance.callbackEvent_HitEnemy += SkillActive;
                break;
            case SkillTiming.death:
                GameMGR.Instance.callbackEvent_Death += SkillActive;
                break;
            case SkillTiming.battleStart:
                GameMGR.Instance.callbackEvent_BattleStart += SkillActive;
                break;
            case SkillTiming.summon:
                GameMGR.Instance.callbackEvent_Summon += SkillActive;
                break;

        }
    }

    public void SkillActive()
    {
       switch(cardInfo.effectType)
        {
            case EffectType.getGold:
                GameMGR.Instance.uiManager.goldCount += cardInfo.value1;
                break;
            case EffectType.damage:
                //cardInfo.value1;
                break;
            case EffectType.changeATK:
                
                break;

        }
    }

    public void CheckEffectTarget() // ��ų ���� ���
    {
        switch(cardInfo.effectTarget)
        {
            
            case EffectTarget.self:
                skillTarget = this;
                break;
            case EffectTarget.allyUnit:
                int random = Random.Range(0, 6);
                if (transform.parent.gameObject.name == "Store") // ���� ���� ��ġ�� ������ ���
                {
                    skillTarget = transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>();
                }
                else if (transform.parent.gameObject.name == "Battle") // ���� ���� ��ġ�� �������� ���
                {
                    skillTarget = transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>();
                }
                //skillTarget = 
                break;
            case EffectTarget.allyFront:
                break;
            case EffectTarget.allyBack:
                break;
            case EffectTarget.enemyUnit:
                break;
            case EffectTarget.enemyFront:
                break;
            case EffectTarget.enemyBack:
                break;
            case EffectTarget.unitAll: // �Ǿƽĺ� ���ϰ� �� �� ���ݴ������ ��� �ؾǹ����� ���
                break;
        }
    }

    public void FindTargetType() // � ������ ����� ã������ ���� �����ϴ� ��찡 �ٸ��ٴ� ���̶� ���̶� ���̶� ���̶� ���̶� ����-�߾�~ ��-��! ���̾�~
    {
        switch(cardInfo.targetType)
        {
            case TargetType.leastATK:
                // ���� ���ݷ��� ���� ����� ã�ƶ�ƾƾƤ��ƾƾƾƾƾƾƾƾƾƾƾƤ��ƾ��!�߾ƾƾƤ� �߹ٸ��� ġ�;ƾƾƾƾƾƤ�
                break;
        }
    }
}

