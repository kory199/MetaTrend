using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;

public partial class Card : MonoBehaviour
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] List<Card> skillTarget;
    [SerializeField] Vector2 TargetPos;

    [SerializeField] GameObject curPos;

    //��ų ���� ����
    [SerializeField] GameObject AllyCamp;   // �Ʊ� ����
    [SerializeField] GameObject EnemyCamp;   // �� ����

    //��ų ȿ�� ���� ����
    public int giveDamage = 0;
    public int takeDamage = 0;

    public void Start()
    {
        AllyCamp = GameObject.Find("PlayerUnit");   // �Ʊ� ����
        EnemyCamp = GameObject.Find("EnemyUnit");   // �� ����
    }

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

    public void Hit(int damage) // �ڽ��� �ǰݽ� ȣ��Ǵ� �Լ�
    {
        if(cardInfo.skillTiming == SkillTiming.hit)
        {
            SkillActive();
        }
        this.curHP -= damage;
        if (this.curHP <= 0) Destroy(this.GameObject());
    }

    public void SkillActive() // ��ų ȿ�� �ߵ�
    {
       switch(cardInfo.effectType)
        {
            case EffectType.getGold:
                GameMGR.Instance.uiManager.goldCount += cardInfo.value1;
                break;
            case EffectType.damage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].Hit(cardInfo.value1);
                }
                break;
            case EffectType.changeDamage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].giveDamage += cardInfo.value1;
                    skillTarget[i].takeDamage += cardInfo.value2;
                }
                break;
            case EffectType.changeATK:
                for(int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curAttackValue += cardInfo.value1;
                }
                break;
            case EffectType.changeHP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curHP += cardInfo.value1;
                }
                break;
            case EffectType.changeATKandHP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curAttackValue += cardInfo.value1;
                    skillTarget[i].curHP += cardInfo.value2;
                }
                break;
            case EffectType.grantEXP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curEXP += cardInfo.value1;
                }
                break;
            case EffectType.summon:
                
                break;
            case EffectType.reduceHireCost:
                // ���� ����� ����
                break;
            case EffectType.reduceShopLevelUpCost:
                // ���� ������ ��� ����
                break;
            case EffectType.addHireUnit:
                // ��밡�� ���� �߰�
                break;
        }
    }
    public void FindTargetType() // � ������ ����� ã������ ���� �����ϴ� ��찡 �ٸ��ٴ� ���̶� ���̶� ���̶� ���̶� ���̶� ��
    {
        GameObject searchArea; // ��� ������ �Ʊ����� ���������� ���� �����Ͽ� ��� ���ӿ�����Ʈ ����
        if (cardInfo.effectTarget == EffectTarget.ally)
            searchArea = AllyCamp; // �Ʊ� ����
        else if (cardInfo.effectTarget == EffectTarget.none) //����� ���ٸ� Ÿ������ų�ߵ��� ���� ���̴�.
            return;
        else
            searchArea = EnemyCamp; // ���� ����


        switch (cardInfo.targetType)
        {
            case TargetType.self:
                skillTarget.Add(this);
                break;

            case TargetType.empty: // �� ������ ã�´� = ��ȯ��
                for(int i = 0; i < 3; i++)
                {
                    if (GameMGR.Instance.battleLogic.playerForwardUnits[i] == null)
                    {
                        TargetPos = GameMGR.Instance.battleLogic.playerForwardUnits[i].transform.position;
                        Card summonCard = Resources.Load<Card>($"Prefabs/{cardInfo.summonName}");
                        GameMGR.Instance.battleLogic.playerForwardUnits.Add(summonCard.gameObject);
                    }
                }
                break;

            case TargetType.random:
                int random = Random.Range(0, 6);
                while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0) // ���� �Ʊ��� �ƴ� ������ �������� ����
                {
                    random = Random.Range(0, 6);
                }
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;

            case TargetType.randomExceptMe:
                random = Random.Range(0, 6);
                while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0 && searchArea.transform.GetChild(random).gameObject == this) // ���� �Ʊ��� �ƴ� ������ �������� ����
                {
                    random = Random.Range(0, 6);
                }
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;

            case TargetType.front:
                random = Random.Range(0, 3);
                bool isAllDead = true;
                for(int i = 0; i < 3; i++)
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP >= 0)
                    {
                        isAllDead = false;
                        break;
                    }
                }
               if(isAllDead)
                {
                    //����� �����Ƿ� ��ų ��ȿ 
                    skillTarget.Clear();
                }
                else // �Ѹ��̶� ����ִٸ�
                {
                    random = Random.Range(0, 3);
                    while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0 && searchArea.transform.GetChild(random).gameObject == this) // ���� �Ʊ��� �ƴ� ������ �������� ����
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea.transform.GetChild(random).gameObject.GetComponent<Card>());
                }
                break;

            case TargetType.back:
                random = Random.Range(3, 6);
                isAllDead = true;
                for (int i = 3; i < 6; i++)
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP >= 0)
                    {
                        isAllDead = false;
                        break;
                    }
                }
                if (isAllDead)
                {
                    //����� �����Ƿ� ��ų ��ȿ 
                    skillTarget.Clear();
                }
                else // �Ѹ��̶� ����ִٸ�
                {
                    random = Random.Range(3, 6);
                    while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0 && searchArea.transform.GetChild(random).gameObject == this) // ���� �Ʊ��� �ƴ� ������ �������� ����
                    {
                        random = Random.Range(3, 6);
                    }
                    skillTarget.Add(searchArea.transform.GetChild(random).gameObject.GetComponent<Card>());
                }
                break;
            case TargetType.otherSide:
                if(searchArea.transform.GetChild(transform.parent.GetSiblingIndex()).gameObject.GetComponent<Card>() != null)
                {
                    skillTarget.Add(searchArea.transform.GetChild(transform.parent.GetSiblingIndex()).gameObject.GetComponent<Card>());
                }
                break;

            case TargetType.leastATK:
                // ���� ���ݷ��� ���� ����� ã�ƶ�ƾƾƤ��ƾƾƾƾƾƾƾƾƾƾƾƤ��ƾ��!�߾ƾƾƤ� �߹ٸ��� ġ�;ƾƾƾƾƾƤ�
                int[] atkArray = new int[6];
                int leastAtk = -1;
                int validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue; // ���� �༮�� ��󿡼� �����Ѵ�.
                    if (leastAtk == -1) //�ƹ��͵� ���� ������ ���ʷ� ���� �༮�� ���� �޴´�. 
                    { 
                        leastAtk = i;
                        atkArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    else
                    {
                        atkArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    if(validIndex != 0)  if(atkArray[validIndex] < atkArray[0]) leastAtk = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(leastAtk).gameObject.GetComponent<Card>());
                break;

            case TargetType.mostATK:
                atkArray = new int[6];
                int mostAtk = -1;
                validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue;
                    if (mostAtk == -1) //�ƹ��͵� ���� ������ ���ʷ� ���� �༮�� ���� �޴´�. 
                    {
                        mostAtk = i;
                        atkArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    else
                    {
                        atkArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    if (validIndex != 0) if (atkArray[validIndex] > atkArray[0]) mostAtk = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(mostAtk).gameObject.GetComponent<Card>());
                break;
            case TargetType.leastHP:
                int[] hpArray = new int[6];
                int leastHp = -1;
                validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue;
                    if (leastHp == -1) //�ƹ��͵� ���� ������ ���ʷ� ���� �༮�� ���� �޴´�. 
                    {
                        leastHp = i;
                        hpArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    else
                    {
                        hpArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    if (validIndex != 0) if (hpArray[validIndex] < hpArray[0]) leastHp = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(leastHp).gameObject.GetComponent<Card>());
                break;
            case TargetType.mostHP:
                hpArray = new int[6];
                int mostHp = -1;
                validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue;
                    if (mostHp == -1) //�ƹ��͵� ���� ������ ���ʷ� ���� �༮�� ���� �޴´�. 
                    {
                        mostHp = i;
                        hpArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    else
                    {
                        hpArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    if (validIndex != 0) if (hpArray[validIndex] > hpArray[0]) mostHp = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(mostHp).gameObject.GetComponent<Card>());
                break;

            default:
                break;

        }
    }
}

