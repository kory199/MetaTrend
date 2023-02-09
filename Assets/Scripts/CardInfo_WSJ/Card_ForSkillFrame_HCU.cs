using System.Collections.Generic;
using UnityEngine;

public partial class Card : MonoBehaviour
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] List<Card> skillTarget;
    [SerializeField] Vector2 batchPos;
    [SerializeField] Vector2 targetPos;

    [SerializeField] GameObject curPos;

    //��ų ȿ�� ���� ����
    public int giveDamage = 0;
    public int takeDamage = 0;



    public void Start()
    {
        // SetSkillTiming(); // ���� ��ųŸ�ֿ̹� ���� �̺�Ʈ�� �߰��ؾ��Ѵٸ� �߰��Ѵ�.
    }


    #region ��ų ȿ�� ���� ���� ���� ����

    public void Attack(int damage, Card Target, bool isDirect, bool isFirst) // �ڽ��� ���ݽ� ȣ���ϴ� �Լ� // �ִ� ������, ���� ��� // ���� �����̳� �ƴϳ� (���� ���ʶ� ������ �� / ��ų�������� ������ ��) // ùŸ ����(���ѷ��� ����)
    {
        if (cardInfo.skillTiming == SkillTiming.attackBefore) SkillActive(); // ���� �� ȿ�� �ߵ�
        Target.Hit(damage, this, isDirect, isFirst); // ���ݺ��� ���� �ʸ� �����ڴٴ� ���̾�
        if (cardInfo.skillTiming == SkillTiming.attackAfter) SkillActive(); // ���� �� ȿ�� �ߵ�
    }

    public void Hit(int damage, Card Attacker, bool isDirect, bool isFirst) // �ڽ��� �ǰݽ� ȣ��Ǵ� �Լ� // ���� ������, �� ���� ���
    {
        if (isDirect && isFirst == true) // ó�� ���� ������ �޾��� ���� ������ �ϴ� ���� ���� ���� Ÿ�� �մ� �����ϴ�.
            Attacker.Hit(damage, this, true, false); // �ϰ� �� ���� ���ȴٸ� ���� �ʸ� ���� ���̴�.

        this.curHP -= damage;
        if (this.curHP <= 0)
        {
            if (Attacker.cardInfo.skillTiming == SkillTiming.kill) Attacker.SkillActive(); // ���� �׾��µ� ���� óġ�� ȿ���� �ִٸ� �� ȿ�� ���� �ߵ������ش�.
            if (cardInfo.skillTiming == SkillTiming.death) SkillActive(); // ����� ȿ�� �ߵ�
            Destroy(this.gameObject);
        }

        if (cardInfo.skillTiming == SkillTiming.hit) // �ǰݽ� ȿ�� �ߵ�. ������ �ǰݽ� ȿ���� �ߵ����� �ʴ´�.
        {
            GameMGR.Instance.Event_HitEnemy();
            SkillActive();
        }
    }

    #endregion

    public void SetSkillTiming() // ��ų�� ���� �ߵ���Ű���Ŀ� ���� �� ��������Ʈ �̺�Ʈ�� �߰������ش�. �̺�Ʈ�� �������� �������ν� �̺�Ʈ�� �����ϸ� �ȿ� �߰��� ��� �Լ����� ����Ǳ� ������ ���������� ���Ǵ� �κп����� ����ϴ� ���� ���� ���� Ÿ�� �մ� �����ϴٰ� ���� �κ����� �κ��̶�� �� �� �ִ� �κ��̴�.
    {
        Debug.Log("SetSkillTiming �����ϴ� �Լ��� ���Դ�");
        switch (cardInfo.skillTiming)
        {
            case SkillTiming.turnStart:
                GameMGR.Instance.callbackEvent_TurnStart += SkillActive;
                break;
            case SkillTiming.turnEnd:
                GameMGR.Instance.callbackEvent_TurnEnd += SkillActive;
                break;
            case SkillTiming.buy:
                GameMGR.Instance.callbackEvent_Buy += SkillActive2;
                Debug.Log("���Ž�ȿ���ϱ� �̺�Ʈ�� �߰�");
                break;
            case SkillTiming.sell:
                GameMGR.Instance.callbackEvent_Sell += SkillActive2;
                break;
            case SkillTiming.reroll:
                GameMGR.Instance.callbackEvent_Reroll += SkillActive;
                break;
            /*case SkillTiming.attackBefore:
                GameMGR.Instance.callbackEvent_BeforeAttack += SkillActive;
                break;
            case SkillTiming.attackAfter:
                GameMGR.Instance.callbackEvent_AfterAttack += SkillActive;
                break;*/
            case SkillTiming.kill:
                GameMGR.Instance.callbackEvent_Kill += SkillActive;
                break;
            /*case SkillTiming.hit:
                GameMGR.Instance.callbackEvent_Hit += SkillActive;
                break;*/
            case SkillTiming.hitEnemy:
                GameMGR.Instance.callbackEvent_HitEnemy += SkillActive;
                break;
            case SkillTiming.death:
                GameMGR.Instance.callbackEvent_Death += SkillActive;
                break;
            case SkillTiming.battleStart:
                GameMGR.Instance.callbackEvent_BattleStart += SkillActive;
                break;
                /*case SkillTiming.summon:
                    GameMGR.Instance.callbackEvent_Summon += SkillActive;
                    break;*/
        }
    }

    public void SkillActive() // ��ų ȿ�� �ߵ� // FindTargetType �Լ��� ���� ��ü���� ��ų ���� ����� �������� �� ���Ŀ� �ߵ��ϴ� �� �´ٰ� �� �� �ִ� �κ����� �κ�
    {
        Debug.Log("Skill Active");
        FindTargetType();
        SkillEffect();
    }

    public void SkillActive2(Card card)
    {
        Debug.Log("Skill Active 2");
        //if (card != this) return;
        FindTargetType();
        SkillEffect();
        GameMGR.Instance.callbackEvent_Buy -= SkillActive2;
    }


    public void SkillEffect() // ��ų �ߵ��� ����Ǵ� ȿ��
    {
        Debug.Log("��ųȿ�� �ߵ�");
        switch (cardInfo.effectType)
        {
            case EffectType.getGold:
                GameMGR.Instance.uiManager.goldCount += cardInfo.GetValue(0, level);
                break;
            case EffectType.damage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].Hit(cardInfo.GetValue(1, level), this, false, false);
                }
                break;
            case EffectType.changeDamage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].giveDamage += cardInfo.GetValue(1, level);
                    skillTarget[i].takeDamage += cardInfo.GetValue(2, level);
                }
                break;
            case EffectType.changeATK:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].ChangeValue(CardStatus.Attack, cardInfo.GetValue(1, level));
                    //skillTarget[i].curAttackValue += cardInfo.value1;
                }
                break;
            case EffectType.changeHP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curHP += cardInfo.GetValue(1, level);
                }
                break;
            case EffectType.changeATKandHP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].GetComponentInChildren<Card>().curAttackValue += cardInfo.GetValue(1, level);
                    skillTarget[i].GetComponentInChildren<Card>().curHP += cardInfo.GetValue(2, level);
                }
                break;
            case EffectType.grantEXP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curEXP += cardInfo.GetValue(1, level);
                }
                break;
            case EffectType.summon:
                Card summonCard = Resources.Load<Card>($"Prefabs/{cardInfo.sumom_Unit}");
                //GameMGR.Instance.battleLogic.playerForwardUnits.Add(summonCard.gameObject);
                summonCard.transform.position = targetPos;
                break;
            case EffectType.reduceShopLevelUpCost:
                // ���� ������ ��� ����
                if (GameMGR.Instance.uiManager.shopMoney > 0)
                    GameMGR.Instance.uiManager.shopMoney -= cardInfo.GetValue(1, level);
                break;
            case EffectType.addHireUnit:
                // ��밡�� ���� �߰�
                GameMGR.Instance.spawner.SpecialMonster();
                break;
        }

        skillTarget.Clear();    // ��ų Ÿ�ٵ� �ѹ� �ʱ�ȭ������
    }

    public List<GameObject> searchArea = new List<GameObject>(); // ��� ������ �Ʊ����� ���������� ���� �����Ͽ� ��� ���ӿ�����Ʈ ����
    public void FindTargetType() // � ������ ����� ã������ ���� �����ϴ� ��찡 �ٸ��ٴ� ���̶� ���̶� ���̶� ���̶� ���̶� ��
    {
        searchArea.Clear();
        Debug.Log("Ÿ���� ã�´�");

        if (GameMGR.Instance.isBattleNow)
        {
            Debug.Log("����� ������ ����");
            switch (cardInfo.effectTarget) // ��ų ȿ�� ���� ��� ���� Ž�� ���� ����
            {
                case EffectTarget.ally:
                    for (int i = 0; i < GameMGR.Instance.battleLogic.playerAttackArray.Length; i++)
                    {
                        if (GameMGR.Instance.battleLogic.playerAttackArray[i] != null)
                        {
                            searchArea.Add(GameMGR.Instance.battleLogic.playerAttackArray[i]);
                        }
                    }
                    break;
                case EffectTarget.allyForward:
                    for (int i = 0; i < GameMGR.Instance.battleLogic.playerForwardUnits.Length; i++)
                    {
                        if (GameMGR.Instance.battleLogic.playerForwardUnits[i] != null)
                        {
                            searchArea.Add(GameMGR.Instance.battleLogic.playerForwardUnits[i]);
                        }
                    }
                    break;
                case EffectTarget.allyBackward:
                    for (int i = 0; i < GameMGR.Instance.battleLogic.playerBackwardUnits.Length; i++)
                    {
                        if (GameMGR.Instance.battleLogic.playerBackwardUnits[i] != null)
                        {
                            searchArea.Add(GameMGR.Instance.battleLogic.playerBackwardUnits[i]);
                        }
                    }
                    break;
                case EffectTarget.enemy:
                    for (int i = 0; i < GameMGR.Instance.battleLogic.enemyAttackArray.Length; i++)
                    {
                        if (GameMGR.Instance.battleLogic.enemyAttackArray[i] != null)
                        {
                            searchArea.Add(GameMGR.Instance.battleLogic.enemyAttackArray[i]);
                        }
                    }
                    break;
                case EffectTarget.enemyForward:
                    for (int i = 0; i < GameMGR.Instance.battleLogic.enemyForwardUnits.Length; i++)
                    {
                        if (GameMGR.Instance.battleLogic.enemyForwardUnits[i] != null)
                        {
                            searchArea.Add(GameMGR.Instance.battleLogic.enemyForwardUnits[i]);
                        }
                    }
                    break;
                case EffectTarget.enemyBackward:
                    for (int i = 0; i < GameMGR.Instance.battleLogic.enemyBackwardUnits.Length; i++)
                    {
                        if (GameMGR.Instance.battleLogic.enemyBackwardUnits[i] != null)
                        {
                            searchArea.Add(GameMGR.Instance.battleLogic.enemyBackwardUnits[i]);
                        }
                    }
                    break;
                case EffectTarget.both:
                    for (int i = 0; i < GameMGR.Instance.battleLogic.playerAttackArray.Length; i++)
                    {
                        searchArea.Add(GameMGR.Instance.battleLogic.playerAttackArray[i]);
                    }
                    for (int i = 0; i < GameMGR.Instance.battleLogic.enemyAttackArray.Length; i++)
                    {
                        searchArea.Add(GameMGR.Instance.battleLogic.enemyAttackArray[i]);
                    }
                    break;
                case EffectTarget.none:
                    break;
            }
        }
        else // �������� �ƴ϶�� ������ ��������
        {
            Debug.Log("����� ������ ����");
            switch (cardInfo.effectTarget) // ��ų ȿ�� ���� ��� ���� Ž�� ���� ����
            {
                case EffectTarget.ally:
                case EffectTarget.allyForward:
                case EffectTarget.allyBackward:
                case EffectTarget.both:
                    Debug.Log("������ �������� searchArea�� �߰�");
                    for (int i = 0; i < GameMGR.Instance.spawner.cardBatch.Length; i++)
                    {
                        if (GameMGR.Instance.spawner.cardBatch[i] != null)
                        {
                            Debug.Log(i + "��° ī���ġ�� �༮�� searchArea�� �߰�");
                            searchArea.Add(GameMGR.Instance.spawner.cardBatch[i]);
                        }
                    }
                    Debug.Log("ī���ġ ���� ���� �߰� �� ���� searchArea ���� :" + searchArea.Count);
                    break;
                case EffectTarget.none:
                    break;
            }
        }

        //=============================================================================================================================================================

        switch (cardInfo.targetType) // ��ü���� ���� ��� ���� ( ü���� ����, ���ݷ��� ����, ���� ���)
        {
            case TargetType.self:
                skillTarget.Add(this);
                break;

            case TargetType.empty: // �� ������ ã�´� = ��ȯ��
                bool isFind = false;
                for (int i = 0; i < 3; i++) // �տ� �˻�
                {
                    if (GameMGR.Instance.battleLogic.playerForwardUnits[i] == null)
                    {
                        targetPos = GameMGR.Instance.battleLogic.playerForwardUnits[i].transform.position;
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    for (int i = 0; i < 3; i++) // �޿� �˻�
                    {
                        if (GameMGR.Instance.battleLogic.playerBackwardUnits[i] == null)
                        {
                            targetPos = GameMGR.Instance.battleLogic.playerForwardUnits[i].transform.position;
                            break;
                        }
                    }
                }
                break;

            case TargetType.random:
                int random = Random.Range(0, searchArea.Count);
                while (searchArea[random].GetComponent<Card>().curHP <= 0) // ���� �Ʊ��� �ƴ� ������ �������� ����
                {
                    random = Random.Range(0, searchArea.Count);
                }
                //skillTarget.Add(GameMGR.Instance.battleLogic.)
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;
            case TargetType.randomExceptMe:
                if (searchArea.Count == 1) break;
                List<Card> targetArray = new List<Card>();
                Debug.Log(searchArea.Count);
                for (int i = 0; i < searchArea.Count; i++)
                {
                    if (searchArea[i] != null)
                    {
                        targetArray.Add(searchArea[i].GetComponentInChildren<Card>());

                    }
                }
                Debug.Log(targetArray.Count);
                int setMaxTarget = 0;
                if (int.Parse(cardInfo.maxTarget) == searchArea.Count)
                {
                    setMaxTarget = int.Parse(cardInfo.maxTarget) - 1;
                }
                else
                {
                    setMaxTarget = int.Parse(cardInfo.maxTarget);
                }
                for (int i = 0; i < setMaxTarget; i++)
                {
                    random = Random.Range(0, targetArray.Count);
                    if (targetArray[random].curHP <= 0 && targetArray[random] == this && !skillTarget.Contains(targetArray[random])) // ���� �Ʊ��� �ƴ� ������ �������� ����
                    {
                        i--;
                        continue;
                    }
                    skillTarget.Add(targetArray[random]);
                    Debug.Log("skillTarget�� Add��");

                }



                break;

            case TargetType.front:       // ������������������������������������������������������������������������������������������������������������������������������������������
                random = Random.Range(0, 3);
                bool isAllDead = true;
                for (int i = 0; i < 3; i++)
                {
                    if (searchArea[i].GetComponent<Card>().curHP >= 0)
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
                if (searchArea.Count == 0)
                {
                    skillTarget.Clear();
                }
                else // �Ѹ��̶� ����ִٸ�
                {
                    random = Random.Range(0, 3);
                    while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // ���� �Ʊ��� �ƴ� ������ �������� ����
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponent<Card>());
                }
                break;

            case TargetType.back:       // �Ŀ� ������������������������������������������������������������������������������������������������������������������������������������
                random = Random.Range(0, 3);
                isAllDead = true;
                for (int i = 3; i < 6; i++)
                {
                    if (searchArea[i].GetComponent<Card>().curHP >= 0)
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
                    random = UnityEngine.Random.Range(0, 3);
                    while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // ���� �Ʊ��� �ƴ� ������ �������� ����
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponent<Card>());
                }
                break;

            case TargetType.otherSide:
                int myPosNum = System.Array.IndexOf(GameMGR.Instance.battleLogic.playerAttackArray, this);
                if (GameMGR.Instance.battleLogic.enemyAttackArray[myPosNum] != null)
                {
                    skillTarget.Add(searchArea[myPosNum].GetComponent<Card>());
                }
                break;

            case TargetType.leastATK:
                // ���� ���ݷ��� ���� ����� ã�ƶ�ƾƾƤ��ƾƾƾƾƾƾƾƾƾƾƾƤ��ƾ��!�߾ƾƾƤ� �߹ٸ��� ġ�;ƾƾƾƾƾƤ�
                int[] atkArray = new int[6];
                int leastAtk = -1;
                int validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue; // ���� �༮�� ��󿡼� �����Ѵ�.
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
                    if (validIndex != 0) if (atkArray[validIndex] < atkArray[0]) leastAtk = i;
                }
                skillTarget.Add(searchArea[leastAtk].gameObject.GetComponent<Card>());
                break;

            case TargetType.mostATK:
                atkArray = new int[6];
                int mostAtk = -1;
                validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
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
                skillTarget.Add(searchArea[mostAtk].GetComponent<Card>());
                break;
            case TargetType.leastHP:
                int[] hpArray = new int[6];
                int leastHp = -1;
                validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
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
                skillTarget.Add(searchArea[leastHp].gameObject.GetComponent<Card>());
                break;
            case TargetType.mostHP:
                hpArray = new int[6];
                int mostHp = -1;
                validIndex = 0; // ��ȿ���� ���� ������ �ö󰡴� �ε��� ī��Ʈ ����
                for (int i = 0; i < 6; i++) // ���� ���ݷ��� ���� ������ ã�� ����
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
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
                skillTarget.Add(searchArea[mostHp].GetComponent<Card>());
                break;

            default:
                break;

        }
    }
}

