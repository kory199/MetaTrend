using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using Unity.VisualScripting;

using Photon.Pun;

public partial class Card : MonoBehaviourPun
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] List<Card> skillTarget;
    [SerializeField] Vector2 batchPos;
    [SerializeField] Vector2 targetPos;

    [SerializeField] GameObject curPos;

    //��ų ȿ�� ���� ����
    public int giveDamage = 0;
    public int takeDamage = 0;

    
    public int shopBatchEmptyIndex = 0;  // ���� ��ġ �ε������� �����ϴ� ����

    public bool isMine; // �� ī�尡 ���� ������ ���� ������

    //��ų ������ �����ϴ� �迭 ( �� isMine ���� ���� ���� ������ �ٸ��� )
    GameObject[] myArea;
    GameObject[] myAreaFront;
    GameObject[] myAreaBack;
    GameObject[] enemyArea;
    GameObject[] enemyAreaFront;
    GameObject[] enemyAreaBack;

    public void Start()
    {
        // SetSkillTiming(); // ���� ��ųŸ�ֿ̹� ���� �̺�Ʈ�� �߰��ؾ��Ѵٸ� �߰��Ѵ�.
    }

    #region ��ų ȿ�� ���� ���� ���� ����

    public void Attack(int damage, Card Target, bool isDirect, bool isFirst) // �ڽ��� ���ݽ� ȣ���ϴ� �Լ� // �ִ� ������, ���� ��� // ���� �����̳� �ƴϳ� (���� ���ʶ� ������ �� / ��ų�������� ������ ��) // ùŸ ����(���ѷ��� ����)
    {
        if (cardInfo.skillTiming == SkillTiming.attackBefore) SkillActive(); // ���� �� ȿ�� �ߵ�
        GameMGR.Instance.audioMGR.BattleAttackSound(damage);
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
            GameMGR.Instance.objectPool.DestroyPrefab(gameObject.transform.parent.gameObject);
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
                Debug.Log("�Ͻ��۽� ȿ���ϱ� �̺�Ʈ�� �߰�");
                break;
            case SkillTiming.turnEnd:
                GameMGR.Instance.callbackEvent_TurnEnd += SkillActive;
                Debug.Log("�������ȿ���ϱ� �̺�Ʈ�� �߰�");
                break;
            case SkillTiming.buy:
                //GameMGR.Instance.callbackEvent_Buy += SkillActive2;
                Debug.Log("���Ž�ȿ���ϱ� �̺�Ʈ�� �߰�");
                break;
            case SkillTiming.sell:
                GameMGR.Instance.callbackEvent_Sell += SkillActive2;
                Debug.Log("�ǸŽ�ȿ���ϱ� �̺�Ʈ�� �߰�");
                break;
            case SkillTiming.reroll:
                GameMGR.Instance.callbackEvent_Reroll += SkillActive;
                Debug.Log("���ѽ�ȿ���ϱ� �̺�Ʈ�� �߰�");
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
        if (card != this) return;

        if (cardInfo.effectType == EffectType.summon)
        {
            for (int i = 0; i < cardInfo.GetNumTrigger(level); i++)
            {
                FindTargetType();
                SkillEffect();
            }
        }
        else
        {
            Debug.Log("Skill Active 2");
            FindTargetType();
            SkillEffect();
        }
    }


    public void SkillEffect() // ��ų �ߵ��� ����Ǵ� ȿ��
    {
        Debug.Log(skillTarget.Count + "��ųŸ�� ����");
        Debug.Log("��ųȿ�� �ߵ�");
        switch (cardInfo.effectType)
        {
            case EffectType.getGold:
                Debug.Log("��� ȹ�� ȿ�� �ߵ�");
                GameMGR.Instance.uiManager.goldCount += cardInfo.GetValue(0, level);
                break;
            case EffectType.damage:
                Debug.Log("������ ȿ�� �ߵ�");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].Hit(cardInfo.GetValue(1, level), this, false, false);
                }
                break;
            case EffectType.changeDamage:
                Debug.Log("������ ���� ȿ�� �ߵ�");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].giveDamage += cardInfo.GetValue(1, level);
                    skillTarget[i].takeDamage += cardInfo.GetValue(2, level);
                }
                break;
            case EffectType.changeATK:
                Debug.Log("���ݷ� ȿ�� �ߵ�");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].ChangeValue(CardStatus.Attack, cardInfo.GetValue(1, level), true);
                    Debug.Log(cardInfo.GetValue(1, level));
                    //skillTarget[i].curAttackValue += cardInfo.value1;
                }
                break;
            case EffectType.changeHP:
                Debug.Log("ü�� ȿ�� �ߵ�");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].ChangeValue(CardStatus.Hp, cardInfo.GetValue(1, level), true);
                }
                break;
            case EffectType.changeATKandHP:
                Debug.Log("���ݷ� ü�� ȿ�� �ߵ�");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                        skillTarget[i].ChangeValue(CardStatus.Attack, cardInfo.GetValue(1, level), true);
                        skillTarget[i].ChangeValue(CardStatus.Hp, cardInfo.GetValue(2, level), true);
                }
                break;
            case EffectType.grantEXP:
                Debug.Log("����ġ �ο� ȿ�� �ߵ�");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curEXP += cardInfo.GetValue(1, level);
                }
                break;
            case EffectType.summon:
                Debug.Log(cardInfo.sumom_Unit + "��ȯ ȿ�� �ߵ�");
                if (targetPos == Vector2.zero) break; //���࿡ ��ĭ�� ���ٸ� ��ȯ�� ��������
                GameObject summonCard = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{cardInfo.sumom_Unit}"), targetPos + new Vector2(0, -0.6f), Quaternion.identity);
                summonCard.transform.GetChild(0).tag = "BattleMonster";
                summonCard.transform.localScale = summonCard.transform.localScale * 2;

                GameMGR.Instance.spawner.cardBatch[shopBatchEmptyIndex] = summonCard;
                //summoncard �̸� ����� ����
                Debug.Log(summonCard.name);

                //GameMGR.Instance.battleLogic.playerForwardUnits.Add(summonCard.gameObject);
                Debug.Log(targetPos + ": ��ȯ�� �� �����Ǿ��ִ� Ÿ������");
                //summonCard.transform.position = targetPos;
                break;
            case EffectType.reduceShopLevelUpCost:
                Debug.Log("���� ���� ��� ���� ȿ�� �ߵ�");
                // ���� ������ ��� ����
                if (GameMGR.Instance.uiManager.shopMoney > 0)
                    GameMGR.Instance.uiManager.shopMoney -= cardInfo.GetValue(1, level);
                GameMGR.Instance.uiManager.ChangeShopLevelUpCost(GameMGR.Instance.uiManager.shopMoney); //  ��밪 ���� ���
                break;
            case EffectType.addHireUnit:
                Debug.Log("��밡�� ���� �߰� ȿ�� �ߵ�");
                // ��밡�� ���� �߰�
                GameMGR.Instance.spawner.SpecialMonster();
                break;
        }
    }

    //==============================================================================================================================================================
    //==========================================               �� Ư �� �� �� �� �� ��                ================================================================
    //==============================================================================================================================================================

    public void CheckTriggerCondition(Card target = null)
    {
        if(cardInfo.triggerCondition != 0) // Ư�� �ߵ������� �ִ� ���
        {
            switch(cardInfo.triggerCondition)
            {
                case TriggerCondition.allyEmpty:
                    for(int i = 0; i < 6; i++)
                    {
                        if (GameMGR.Instance.spawner.cardBatch[i] == null)
                        {
                            curAttackValue = cardInfo.GetValue(1, level);
                            curHP = cardInfo.GetValue(2, level);
                            break;
                        }
                    }
                    break;
                case TriggerCondition.damageEcess:
                    if(curAttackValue > target.curHP)
                    {
                        int excessDamage = curAttackValue - target.curHP;
                        FindTargetType(false);
                        List<GameObject> curExistBatch = new List<GameObject>();
                        for(int i = 0; i < enemyArea.Length; i++)
                        {
                            if (enemyArea[i] != null)
                            {
                                curExistBatch.Add(enemyArea[i]);
                            }
                        }
                        if (curExistBatch.Count == 0) break;

                        int curTargetNum = Random.Range(0, curExistBatch.Count);

                        Attack(excessDamage, curExistBatch[curTargetNum].GetComponentInChildren<Card>(), false, false);
                    }
                    break;
                case TriggerCondition.losePlayerHP:
                    int curLoseLife = 20 - (int)PhotonNetwork.LocalPlayer.CustomProperties["Life"];
                    curHP += curLoseLife * cardInfo.GetValue(1, level);
                    break;
            }
        }
    }

    //==============================================================================================================================================================

    public List<GameObject> searchArea = new List<GameObject>(); // ��� ������ �Ʊ����� ���������� ���� �����Ͽ� ��� ���ӿ�����Ʈ ����
    public void FindTargetType(bool isBaseOnDB = true) // � ������ ����� ã������ ���� �����ϴ� ��찡 �ٸ��ٴ� ���̶� ���̶� ���̶� ���̶� ���̶� ��
    {
        searchArea.Clear();
        skillTarget.Clear();
        Debug.Log("Ÿ���� ã�´�");
        // ������ ��ų ���� ���� ���� �������� ��� ���� ���������� ���� ����ִ� ��찡 �ٸ� ��츦 ���ϴ� ����� �� �� �ִ� ���


        if (isMine)
        {
            myArea = GameMGR.Instance.battleLogic.playerAttackArray;
            myAreaFront = GameMGR.Instance.battleLogic.playerForwardUnits;
            myAreaBack = GameMGR.Instance.battleLogic.playerBackwardUnits;

            enemyArea = GameMGR.Instance.battleLogic.enemyAttackArray;
            enemyAreaFront = GameMGR.Instance.battleLogic.enemyForwardUnits;
            enemyAreaBack = GameMGR.Instance.battleLogic.enemyBackwardUnits;
        }
        else
        {
            enemyArea = GameMGR.Instance.battleLogic.playerAttackArray;
            enemyAreaFront = GameMGR.Instance.battleLogic.playerForwardUnits;
            enemyAreaBack = GameMGR.Instance.battleLogic.playerBackwardUnits;

            myArea = GameMGR.Instance.battleLogic.enemyAttackArray;
            myAreaFront = GameMGR.Instance.battleLogic.enemyForwardUnits;
            myAreaBack = GameMGR.Instance.battleLogic.enemyBackwardUnits;
        }

        if(isBaseOnDB)  SetTargetType();    // �������� ��� ã��
    }

    //===============================================================================================================================================================
    //======================================     ����� ã�� ���� ���� �� ��ü���� ����� Ư¡�� �����ϴ� �κ�         =====================================================
    //===============================================================================================================================================================
    private void SetTargetType()
    {
            if (GameMGR.Instance.isBattleNow)
            {
                switch (cardInfo.effectTarget) // ��ų ȿ�� ���� ��� ���� Ž�� ���� ����
                {
                    case EffectTarget.ally:
                        Debug.Log("�Ʊ�");
                        for (int i = 0; i < myArea.Length; i++)
                        {
                            if (myArea[i] != null)
                            {
                                searchArea.Add(myArea[i]);
                            }
                        }
                        break;
                    case EffectTarget.allyForward:
                        Debug.Log("�Ʊ�����");
                        for (int i = 0; i < myAreaFront.Length; i++)
                        {
                            if (myAreaFront[i] != null)
                            {
                                searchArea.Add(myAreaFront[i]);
                            }
                        }
                        break;
                    case EffectTarget.allyBackward:
                        Debug.Log("�Ʊ��Ŀ�");
                        for (int i = 0; i < myAreaBack.Length; i++)
                        {
                            if (myAreaBack[i] != null)
                            {
                                searchArea.Add(myAreaBack[i]);
                            }
                        }
                        break;
                    case EffectTarget.enemy:
                        Debug.Log("����");
                        for (int i = 0; i < enemyArea.Length; i++)
                        {
                            if (enemyArea[i] != null)
                            {
                                searchArea.Add(enemyArea[i]);
                            }
                        }
                        break;
                    case EffectTarget.enemyForward:
                        Debug.Log("������");
                        for (int i = 0; i < enemyAreaFront.Length; i++)
                        {
                            if (enemyAreaFront[i] != null)
                            {
                                searchArea.Add(enemyAreaFront[i]);
                            }
                        }
                        break;
                    case EffectTarget.enemyBackward:
                        Debug.Log("���Ŀ�");
                        for (int i = 0; i < enemyAreaBack.Length; i++)
                        {
                            if (enemyAreaBack[i] != null)
                            {
                                searchArea.Add(enemyAreaBack[i]);
                            }
                        }
                        break;
                    case EffectTarget.both:
                        Debug.Log("��ü");
                        for (int i = 0; i < myArea.Length; i++)
                        {
                            searchArea.Add(myArea[i]);
                        }
                        for (int i = 0; i < enemyArea.Length; i++)
                        {
                            searchArea.Add(enemyArea[i]);
                        }
                        break;
                    case EffectTarget.none:
                        break;
                }

                Debug.Log("����� ������ ����");

            }
            else // �������� �ƴ϶�� ������ ��������  // ���������� ������ ���� ���� �ش����� �ʰ� �ȴ�. 
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
                        Debug.Log("�ƹ� ��� ����");
                        break;
                }
            }
        


        //=============================================================================================================================================================

        switch (cardInfo.targetType) // ��ü���� ���� ��� ���� ( ü���� ����, ���ݷ��� ����, ���� ���)
        {

            case TargetType.self:
                skillTarget.Add(this);
                Debug.Log("����� ���ڽ�");
                break;

            case TargetType.empty: // �� ������ ã�´� = ��ȯ��
                bool isFind = false;
                targetPos = Vector2.zero; //��ġ�� �ʱ�ȭ
                Debug.Log("����� ��ĭ");
                if (GameMGR.Instance.isBattleNow)    // ���� ���°� �������� ���
                {
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
                                isFind = true;
                                break;
                            }
                        }
                        if(!isFind)
                        {
                            // ƨ�ܳ����� �ִϸ��̼� ����ϸ鼭 ��򰡿� ��ȯ�ߴٰ� ����
                        }
                    }
                }
                else    // ���� ���°� �������� ���
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (GameMGR.Instance.spawner.cardBatch[i] == null)
                        {
                            targetPos = GameMGR.Instance.spawner.shopBatchPos[i].transform.position;
                            Debug.Log(targetPos + ": Ÿ������");
                            shopBatchEmptyIndex = i; // ã�� ��ĭ �ε��� ���
                            isFind = true;
                            break;
                        }
                    }

                    if(!isFind)
                    {
                        // ƨ�ܳ����� �ִϸ��̼� ����ϸ鼭 ��򰡿� ��ȯ�ߴٰ� ����
                    }
                }
                break;

            case TargetType.random:
                Debug.Log("����� ����");
                int random = Random.Range(0, searchArea.Count);

                List<Card> targetArray1 = new List<Card>();
                for (int i = 0; i < searchArea.Count; i++)
                {
                    if (searchArea[i] != null)
                    {
                        targetArray1.Add(searchArea[i].GetComponentInChildren<Card>());
                    }
                }

                for (int i = 0; i < cardInfo.GetMaxTarget(cardInfo.level); i++)
                {
                    random = Random.Range(0, targetArray1.Count);
                    Debug.Log(random);
                    Debug.Log(targetArray1[random]);
                    if (skillTarget.Contains(targetArray1[random])) // ���� �Ʊ��� �ƴ� ������ �������� ����
                    {
                        i--;
                        continue;
                    }
                    skillTarget.Add(targetArray1[random]);
                    if (targetArray1.Count < cardInfo.GetMaxTarget(cardInfo.level)) break;
                    Debug.Log("skillTarget�� Add��");
                }

                //skillTarget.Add(GameMGR.Instance.battleLogic.)
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;
            case TargetType.randomExceptMe:
                Debug.Log("����� �� ������ ����");
                //if (searchArea.Count == 1) break;
                List<Card> targetArray = new List<Card>();
                Debug.Log(searchArea.Count);
                for (int i = 0; i < searchArea.Count; i++)
                {
                    if (searchArea[i] != null && searchArea[i] != this.transform.parent.gameObject)
                    {
                        targetArray.Add(searchArea[i].GetComponentInChildren<Card>());
                    }
                }
                Debug.Log(targetArray.Count);

                for (int i = 0; i < cardInfo.GetMaxTarget(cardInfo.level); i++)
                {
                    random = Random.Range(0, targetArray.Count);
                    Debug.Log(random);
                    Debug.Log(targetArray[random]);
                    if (skillTarget.Contains(targetArray[random])) // ���� �Ʊ��� �ƴ� ������ �������� ����
                    {
                        i--;
                        continue;
                    }
                    skillTarget.Add(targetArray[random]);
                    if (targetArray.Count < cardInfo.GetMaxTarget(cardInfo.level)) break;
                    Debug.Log("skillTarget�� Add��");
                }
                break;

            case TargetType.front:      // ����������������������������������������������������������������������������������������������������������������������������������������
                Debug.Log("����� ����");
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
                Debug.Log("����� �Ŀ�");
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
                Debug.Log("����� �� �ݴ���");
                int myPosNum = System.Array.IndexOf(GameMGR.Instance.battleLogic.playerAttackArray, this);
                if (GameMGR.Instance.battleLogic.enemyAttackArray[myPosNum] != null)
                {
                    skillTarget.Add(searchArea[myPosNum].GetComponent<Card>());
                }
                break;

            case TargetType.leastATK:
                Debug.Log("����� �ּҰ���");
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
                Debug.Log("����� �ִ����");
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
                Debug.Log("����� �ּ�ü��");
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
                Debug.Log("����� �ִ�ü��");
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

