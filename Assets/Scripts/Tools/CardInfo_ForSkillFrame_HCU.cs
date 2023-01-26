using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum ����
public enum SkillTiming     // ��ų�� �ߵ��Ǵ� ��
{
    turnStart,
    startBattle,
    buy,
    sell,
    reroll,
    attackBefore,
    attackAfter,
    kill,
    hit,
    enemyHit,
    death,
    summonAlly,
    turnEnd
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
}

public enum EffectTarget // Ÿ�� ���� - ��, ��, �Ʊ� ��
{
    none,
    self,
    unit,
    enemyUnit,
    allyUint,
    enemyFront,
    enemyBack,
    allyFront,
    allyBack
}

public enum TriggerCondition // �ߵ��Ǳ� ���� ���� - ex ) �� �ڸ��� �־���Ѵ�
{
    allyEmpty
}

public enum EffectType // ��ų ȿ��
{
    damage,     //������
    getGold,    //��� ȹ��
    changeHP,   //ü�� ����
    changeATK,  //���ݷ� ����
    changeATKandHP, //��ü ����
    summon,         //��ȯ
    changeDamage,   //������ ����
    attackTargeting,//���� Ÿ����(����׷�)
    ReduceHireCost, //����� ����
    ReduceShopLevelUpCost,  //����������� ����
    grantEXP,       //����ġ �ο�
    addHireUnit,    //��������߰�

}


#endregion
public partial class CardInfo : ScriptableObject
{
    [Header("Skill Data")]
    [SerializeField] internal SkillTiming skillTiming; 
    [SerializeField] internal TargetType targetType; 
    [SerializeField] internal EffectTarget target; // Ÿ�� ���� - ����, �Ʊ�, �ڽ� ��
    [SerializeField] internal int Min_target; // �ּ� Ÿ�� ��
    [SerializeField] internal int Max_target; // �ִ� Ÿ�� ��
    [SerializeField] internal TriggerCondition triggerCondition; //�ߵ� ���� - � ��Ȳ���� �ߵ��Ǵ°�(ex - �� �ڸ� ���� ��)
    [SerializeField] internal EffectType EffectType;
    [SerializeField] internal int value1; //��1
    [SerializeField] internal int value2; //��2
    [SerializeField] internal int triggerCount; // �ߵ�Ƚ��
    [SerializeField] internal int groupIndex; // �׷��ε���

    /*//858ed67e0d64b72429e8c773f1903334
    [SerializeField] internal int ID;
    [SerializeField] internal string objName;
    [SerializeField] internal int level;
    [SerializeField] internal int tier;
    [SerializeField] internal int tribe;
    [SerializeField] internal int hp;
    [SerializeField] internal int attackValue;
    [SerializeField] internal string skilltype;
    [SerializeField] internal string skill;
    [SerializeField] internal bool appear;
    public string GetSkillExplantion(int num)
    {
        string [] skillExplantion =  skill.Split(".");
        if (num == 1)
        {
            return $"Level 1: {skillExplantion[0]}";
        }
        else if (num == 2)
        {
            if (skillExplantion.Length==2) return $"Level 2:{skillExplantion[0]}";
            return $"Level 2:{skillExplantion[1]}";
        }
        else if (num == 3)
        {
            if (skillExplantion.Length == 2) return $"Level 3:{skillExplantion[0]}";
            return $"Level 3:{skillExplantion[2]}";
        }
        return null;
    }*/

}