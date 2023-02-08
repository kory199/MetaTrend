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
    [SerializeField] internal int min_Target;
    [SerializeField] internal int max_Target;
    [SerializeField] internal TriggerCondition triggerCondition;
    [SerializeField] internal EffectType effectType;
    [SerializeField] internal int value1;
    [SerializeField] internal int value2;
    [SerializeField] internal int value3;
    [SerializeField] internal string sumom_Unit;
    [SerializeField] internal int num_Triggers;
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

}