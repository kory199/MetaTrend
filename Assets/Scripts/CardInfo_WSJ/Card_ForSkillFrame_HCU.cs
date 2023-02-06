//using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public partial class Card : MonoBehaviour
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] List<Card> skillTarget;
    [SerializeField] Vector2 batchPos;
    [SerializeField] Vector2 targetPos;

    [SerializeField] GameObject curPos;

    //什迭 反引 淫恵 痕呪
    public int giveDamage = 0;
    public int takeDamage = 0;


    #region 什迭 反引 旋遂 淫恵 痕呪 乞製

    public void Attack(int damage, Card Target, bool isDirect, bool isFirst) // 切重戚 因維獣 硲窒馬澗 敗呪 // 爽澗 汽耕走, 凶険 企雌 // 送羨 因維戚劃 焼艦劃 (因維 託景凶 凶軒澗 依 / 什迭汽耕走稽 凶軒澗 依) // 湛展 姥歳(巷廃欠覗 号走)
    {
        if (cardInfo.skillTiming == SkillTiming.attackBefore) SkillActive(); // 因維 穿 反引 降疑
        Target.Hit(damage, this, isDirect, isFirst); // 走榎採斗 鎧亜 格研 凶軒畏陥澗 源戚醤
        if (cardInfo.skillTiming == SkillTiming.attackAfter) SkillActive(); // 因維 板 反引 降疑
    }

    public void Hit(int damage, Card Attacker, bool isDirect, bool isFirst) // 切重戚 杷維獣 硲窒鞠澗 敗呪 // 閤精 汽耕走, 劾 凶鍵 紫寓
    {
        if (isDirect && isFirst == true) // 坦製 送羨 因維聖 閤紹聖 凶幻 誓呪研 馬澗 依戚 誓雁 舛雁 展雁 杯雁 原競馬陥.
            Attacker.Hit(damage, this, true, false); // 艦亜 劾 送羨 凶携陥檎 蟹亀 格研 凶険 依戚陥.

        this.curHP -= damage;
        if (this.curHP <= 0)
        {
            if(Attacker.cardInfo.skillTiming == SkillTiming.kill)   Attacker.SkillActive(); // 鎧亜 宋醸澗汽 旋戚 坦帖獣 反引亜 赤陥檎 旋 反引 胡煽 降疑獣佃層陥.
            if (cardInfo.skillTiming == SkillTiming.death) SkillActive(); // 紫諺獣 反引 降疑
            Destroy(this.gameObject);
        }

        if (cardInfo.skillTiming == SkillTiming.hit) // 杷維獣 反引 降疑. 宋生檎 杷維獣 反引亜 降疑馬走 省澗陥.
        {
            GameMGR.Instance.Event_HitEnemy();
            SkillActive();
        } 
    }


    #endregion

    public void SetSkillTiming() // 什迭聖 情薦 降疑獣徹汗劃拭 魚虞辞 唖 季軒惟戚闘 戚坤闘拭 蓄亜獣佃層陥.
    {
        switch (cardInfo.skillTiming)
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

    public void SkillActive() // 什迭 反引 降疑 // FindTargetType 敗呪研 搭背 姥端旋昔 什迭 旋遂 企雌戚 舛背走壱 貝 戚板拭 降疑馬澗 惟 限陥壱 瑳 呪 赤澗 採歳旋昔 採歳
    {
        switch (cardInfo.effectType)
        {
            case EffectType.getGold:
                GameMGR.Instance.uiManager.goldCount += cardInfo.value1;
                break;
            case EffectType.damage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].Hit(cardInfo.value1, this, false, false);
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
                for (int i = 0; i < skillTarget.Count; i++)
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
                Card summonCard = Resources.Load<Card>($"Prefabs/{cardInfo.sumom_Unit}");
                //GameMGR.Instance.battleLogic.playerForwardUnits.Add(summonCard.gameObject);
                summonCard.transform.position = targetPos;
                break;
            case EffectType.reduceShopLevelUpCost:
                // 雌繊 傾婚穣 搾遂 姶社
                if(GameMGR.Instance.uiManager.shopMoney > 0)
                    GameMGR.Instance.uiManager.shopMoney -= cardInfo.value1;
                break;
            case EffectType.addHireUnit:
                // 壱遂亜管 政間 蓄亜
                GameMGR.Instance.spawner.SpecialMonster();
                break;
        }
    }
    public void FindTargetType() // 嬢恐 政莫税 企雌聖 達澗走拭 魚虞 叔楳馬澗 井酔亜 陥牽陥澗 源戚空 源戚空 源戚空 源戚空 源戚空 源
    {
        GameObject[] searchArea = new GameObject[6]; // 企雌 骨是亜 焼浦昔走 旋浦昔走拭 魚虞 姥歳馬食 眼澗 惟績神崎詮闘 痕呪

        
        switch (cardInfo.effectTarget) // 什迭 反引 旋遂 企雌拭 魚献 貼事 骨是 走舛
        {
            
            case EffectTarget.ally:
                searchArea = GameMGR.Instance.battleLogic.playerAttackArray;
                break;
            case EffectTarget.allyForward:
                searchArea = GameMGR.Instance.battleLogic.playerForwardUnits;
                break;
            case EffectTarget.allyBackward:
                searchArea = GameMGR.Instance.battleLogic.playerBackwardUnits;
                break;
            case EffectTarget.enemy:
                searchArea = GameMGR.Instance.battleLogic.enemyAttackArray;
                break;
            case EffectTarget.enemyForward:
                searchArea = GameMGR.Instance.battleLogic.enemyForwardUnits;
                break;
            case EffectTarget.enemyBackward:
                searchArea = GameMGR.Instance.battleLogic.enemyBackwardUnits;
                break;
            case EffectTarget.both:
                //Array.Resize<GameObject>(ref searchArea, 12);
                searchArea = new GameObject[12];
                for (int i = 0; i < GameMGR.Instance.battleLogic.playerAttackArray.Length; i++)
                {
                    searchArea[i] = (GameMGR.Instance.battleLogic.playerAttackArray[i]);
                }
                for (int i = 0; i < GameMGR.Instance.battleLogic.enemyAttackArray.Length; i++)
                {
                    searchArea[i+6] = (GameMGR.Instance.battleLogic.enemyAttackArray[i+6]);
                }
                break;
            case EffectTarget.none:
                break;
        }


        switch (cardInfo.targetType) // 姥端旋昔 因維 企雌 走舛 ( 端径戚 碍精, 因維径戚 株精, 穿伸 去去)
        {
            case TargetType.self:
                skillTarget.Add(this);
                break;

            case TargetType.empty: // 朔 因娃聖 達澗陥 = 社発獣
                bool isFind = false;
                for (int i = 0; i < 3; i++) // 蒋伸 伊紫
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
                    for (int i = 0; i < 3; i++) // 急伸 伊紫
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
                int random = Random.Range(0, 6);
                while (searchArea[random].GetComponent<Card>().curHP <= 0) // 宋精 焼浦戚 焼諌 凶猿走 沓棋葵聖 宜形
                {
                    random = Random.Range(0, 6);
                }
                //skillTarget.Add(GameMGR.Instance.battleLogic.)
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;
            case TargetType.randomExceptMe:
                random = Random.Range(0, 6);
                while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // 宋精 焼浦戚 焼諌 凶猿走 沓棋葵聖 宜形
                {
                    random = Random.Range(0, 6);
                }
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;

            case TargetType.front:      // 穿伸しじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじしじし
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
                    //企雌戚 蒸生糠稽 什迭 巷反 
                    skillTarget.Clear();
                }
                if (searchArea.Length == 0)
                {
                    skillTarget.Clear();
                }
                else // 廃誤戚虞亀 詞焼赤陥檎
                {
                    random = Random.Range(0, 3);
                    while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // 宋精 焼浦戚 焼諌 凶猿走 沓棋葵聖 宜形
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponent<Card>());
                }
                break;

            case TargetType.back:       // 板伸 ぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞしぞし
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
                    //企雌戚 蒸生糠稽 什迭 巷反 
                    skillTarget.Clear();
                }
                else // 廃誤戚虞亀 詞焼赤陥檎
                {
                    random = Random.Range(0, 3);
                    while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // 宋精 焼浦戚 焼諌 凶猿走 沓棋葵聖 宜形
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponent<Card>());
                }
                break;

            case TargetType.otherSide:
                searchArea = GameMGR.Instance.battleLogic.playerAttackArray;
                int myIndex = System.Array.IndexOf(searchArea, gameObject); // 蟹研 達澗 引舛 蟹拭惟稽 彊蟹澗 食楳 乞且稽益 拭琶稽益 覗継稽益 紫戚左益 焼戚情硲益
                searchArea = GameMGR.Instance.battleLogic.enemyAttackArray;
                skillTarget.Add(searchArea[myIndex].GetComponent<Card>());
                break;

            case TargetType.leastATK:
                // 亜舌 因維径戚 碍精 企雌聖 達焼虞焼焼焼た焼焼焼焼焼焼焼焼焼焼焼焼た焼焼綜困!醤焼焼焼た 降郊軒戚 帖人焼焼焼焼焼焼た
                int[] atkArray = new int[6];
                int leastAtk = -1;
                int validIndex = 0; // 政反葵戚 赤聖 凶原陥 臣虞亜澗 昔畿什 朝錘闘 痕呪
                for (int i = 0; i < 6; i++) // 亜舌 因維径戚 碍精 政間聖 達澗 引舛
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue; // 宋精 橿汐精 企雌拭辞 薦須廃陥.
                    if (leastAtk == -1) //焼巷依亀 蒸聖 凶拭澗 置段稽 級嬢紳 橿汐戚 葵聖 閤澗陥. 
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
                validIndex = 0; // 政反葵戚 赤聖 凶原陥 臣虞亜澗 昔畿什 朝錘闘 痕呪
                for (int i = 0; i < 6; i++) // 亜舌 因維径戚 碍精 政間聖 達澗 引舛
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
                    if (mostAtk == -1) //焼巷依亀 蒸聖 凶拭澗 置段稽 級嬢紳 橿汐戚 葵聖 閤澗陥. 
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
                validIndex = 0; // 政反葵戚 赤聖 凶原陥 臣虞亜澗 昔畿什 朝錘闘 痕呪
                for (int i = 0; i < 6; i++) // 亜舌 因維径戚 碍精 政間聖 達澗 引舛
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
                    if (leastHp == -1) //焼巷依亀 蒸聖 凶拭澗 置段稽 級嬢紳 橿汐戚 葵聖 閤澗陥. 
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
                validIndex = 0; // 政反葵戚 赤聖 凶原陥 臣虞亜澗 昔畿什 朝錘闘 痕呪
                for (int i = 0; i < 6; i++) // 亜舌 因維径戚 碍精 政間聖 達澗 引舛
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
                    if (mostHp == -1) //焼巷依亀 蒸聖 凶拭澗 置段稽 級嬢紳 橿汐戚 葵聖 閤澗陥. 
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

