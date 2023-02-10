using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum ����
public enum SkillTiming     // ��ų�� �ߵ��Ǵ� ��
{
    turnStart,
    battleStart,
    buy,
    sell,
    reroll,
    attackBefore,
    attackAfter,
    kill,
    hit,
    hitEnemy,
    death,
    summon,
    turnEnd
}
public enum EffectTarget // Ÿ�� ���� - ��, ��, �Ʊ� ��
{
    none,

    both,
    enemy,
    ally,
    enemyBackward,
    enemyForward,
    allyBackward,
    allyForward,

}

public enum TargetType // ��ų�� � Ÿ���� �븮����(ex - ���ݷ� ���� ����)
{
    none,
    all,
    allExceptMe,
    random,
    randomExceptMe,
    mostATK,
    leastATK,
    leastHP,
    mostHP,
    front,
    back,
    near,
    otherSide,
    forward,
    backward,
    self,
    empty,

}

public enum TriggerCondition // �ߵ��Ǳ� ���� ���� - ex ) �� �ڸ��� �־���Ѵ�
{
    allyEmpty,
    damageEcess,
    losePlayerHP,
}


public enum EffectType // ��ų ȿ��
{
    damage,     //������
    getGold,    //��� ȹ��
    changeHP,   //ü�� ����
    changeATK,  //���ݷ� ����
    changeATKandHP, //��ü ����
    changeDamage,     //������ ���� value1�� �ִ� ������, value2�� �޴� ������
    summon,         //��ȯ
    reduceShopLevelUpCost,  //����������� ����
    addHireUnit,    //��������߰�
    grantEXP,       //����ġ �ο�
    crossChangeHPandATK, // ���ݷ°� ü�� �¹ٲ�
    oneShotKill,    //����
    attackTargeting,//���� Ÿ����(����׷�)

}
#endregion


[CreateAssetMenu(fileName = "new CardData", menuName = "ScriptableObjects/CardData")]
public class CardInfo : ScriptableObject
{
    //858ed67e0d64b72429e8c773f1903334
    [SerializeField] internal int ID;
    [SerializeField] internal string objName;
    [SerializeField] internal int level;
    [SerializeField] internal int tier;
    [SerializeField] internal int hp;
    [SerializeField] internal int atk;
    [SerializeField] internal string description;
    [SerializeField] internal SkillTiming skillTiming;
    [SerializeField] internal TargetType targetType;
    [SerializeField] internal EffectTarget effectTarget;
    [SerializeField] internal string minTarget;
    [SerializeField] internal string maxTarget;
    [SerializeField] internal TriggerCondition triggerCondition;
    [SerializeField] internal EffectType effectType;
    [SerializeField] internal string value1;
    [SerializeField] internal string value2;
    [SerializeField] internal string value3;
    [SerializeField] internal string sumom_Unit;
    [SerializeField] internal string num_Triggers;
    [SerializeField] internal int duration;
    [SerializeField] internal string appear;

    public string GetSkillExplantion(int num)
    {
        string [] skillExplantion =  description.Split(".");
        if (num == 1)
        {
            return $"Lv 1: {skillExplantion[0]}";
        }
        else if (num == 2)
        {
            if (skillExplantion.Length==1) return $"";
            return $"Lv 2:{skillExplantion[1]}";
        }
        else if (num == 3)
        {
            if (skillExplantion.Length == 1) return $"";
            return $"Lv 3:{skillExplantion[2]}";
        }
        return null;
    }
    public int GetMaxTarget(int level)
    {
        string[] Target = maxTarget.Split(".");
        if (level == 1)
        {
            return int.Parse(Target[0]);
        }
        else if (level == 2)
        {
            if (Target.Length == 1) return int.Parse(Target[0]);
            return int.Parse(Target[1]);
        }
        else if (level == 3)
        {
            if (Target.Length == 1) return int.Parse(Target[0]);
            return int.Parse(Target[2]);
        }
        return 0;

    }
    public int GetNumTrigger(int level)
    {
        string[] numTrigger = num_Triggers.Split(".");
        if (level == 1)
        {
            return int.Parse(numTrigger[0]);
        }
        else if (level == 2)
        {
            if (numTrigger.Length == 1) return int.Parse(numTrigger[0]);
            return int.Parse(numTrigger[1]);
        }
        else if (level == 3)
        {
            if (numTrigger.Length == 1) return int.Parse(numTrigger[0]);
            return int.Parse(numTrigger[2]);
        }
        return 0;

    }
    public int GetValue(int valueNum, int level)
    {
        string[] value=null;
        if (valueNum == 1)
        {
            value = value1.Split(".");
        }
        else if (valueNum == 2)
        {
            value = value2.Split(".");
        }
        else
        {
            value = value3.Split(".");
        }
        if (level == 1)
        {
            return int.Parse(value[0]);
        }
        else if (level == 2)
        {
            if (value.Length == 1) return int.Parse(value[0]);
            return int.Parse(value[1]);
        }
        else if (level == 3)
        {
            if (value.Length == 1) return int.Parse(value[0]);
            return int.Parse(value[2]);
        }
        return 0;
    }

}