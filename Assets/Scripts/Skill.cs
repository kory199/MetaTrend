using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillType;
    
    public virtual void AttackSkill() // ���ݽ�
    {

    }
    public virtual void HitSkill() // �ǰݽ�
    {     
          
    }     
    public virtual void DeathSkill() // �����
    {     
          
    }     
    public virtual void KillSkill() // óġ��
    {      
           
    }      
    public virtual void BuySkill() // ���Ž�
    {      
           
    }      
    public virtual void SellSkill() // �ǸŽ�
    {      
           
    }      
    public virtual void GameStartSkill() //���� ���۽� 
    {     
           
    }      
    public virtual void StoreExitSkill() //
    {

    }
    /////////
    public void GetGold(int value)
    {

    }
    public void ThrowMissile()
    {

    }
    public void UnitCreate()
    {

    }
    public void AddStatus(Card card,string key)
    {

    }

}