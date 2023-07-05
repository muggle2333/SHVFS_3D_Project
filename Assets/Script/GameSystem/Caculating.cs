using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caculating : MonoBehaviour
{
    protected int academyMaxHP;
    protected int academyHPPerRound;
    protected int academyAttackRange;
    protected int academyAttackDamage;
    protected int academyDefense;
    protected int academyAPPerRound;

    protected int totalAcademyMaxHP;
    protected int totalAcademyHPPerRound;
    protected int totalAcademyAttackRange;
    protected int totalAcademyAttackDamage;
    protected int totalAcademyDefense;
    protected int totalAcademyAPPerRound;


    protected int cardAttackDamage;
    protected int cardDefense;
    protected int cardAttackRange;

    protected int cardDamage;
    protected int cardAP;
    protected int cardHP;
    protected int cardFreeMoveNum;

    protected int totalCardAttackDamage;
    protected int totalCardDefense;
    protected int totalCardAttackRange;
    
    protected int[] academyEffectNum = new int[6];
    protected AcademyBuffData AcademyBuffData;
    protected Card CardData;

    public static Caculating Instance;

    protected PlayerAcademyBuffcomponent playerAcademyBuffcomponent;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        playerAcademyBuffcomponent = FindObjectOfType<PlayerAcademyBuffcomponent>();   
    }
    public void DelataCardData (Card card,Player player)
    {
        totalCardAttackRange += card.playerDataEffect.visionRange;
        totalCardDefense += card.playerDataEffect.defence;
        totalCardAttackDamage += card.playerDataEffect.attack;

        academyEffectNum = card.academyEffectNum;
        for(int i = 0; i < 6; i++)
        {
            player.academyOwnedPoint[i] += academyEffectNum[i];
        }
        
        playerAcademyBuffcomponent.UpdatePlayerAcademyBuff(player);

        
        cardDamage = card.Damage;
        cardAP = card.playerDataEffect.actionPoint;
        cardHP = card.playerDataEffect.hp;
    
    }    

    public void CardDataInitialize(Player player)
    {
      
        cardAttackDamage = 0;
        cardAttackRange = 0;
        cardDefense = 0;

        cardDamage = 0;
        cardAP = 0;
        cardHP = 0;
        cardFreeMoveNum = 0;
        for (int i = 0; i < 6; i++)
        {
            player.academyOwnedPoint[i] -= academyEffectNum[i];
        }
        for (int i = 0; i < 6 ; i++)
        {
            academyEffectNum[i] = 0;
        }

        playerAcademyBuffcomponent.UpdatePlayerAcademyBuff(player);
    }
    public void AcademyBuff(Dictionary<AcademyType, AcademyBuffData> PlayerAcademyBuffDict,Player player)
    {
        for(int i = 0; i < 6; i++)
        {
            PlayerAcademyBuffDict.TryGetValue((AcademyType)(i + 1), out AcademyBuffData);
            academyMaxHP += AcademyBuffData.maxHp;
            academyHPPerRound += AcademyBuffData.hpPreRound;
            academyAttackRange += AcademyBuffData.attackRange;
            academyAttackDamage += AcademyBuffData.attackDamage;
            academyDefense += AcademyBuffData.defense;
            academyAPPerRound += AcademyBuffData.APPerRound;
        }

        totalAcademyMaxHP = academyMaxHP;
        totalAcademyHPPerRound = academyHPPerRound;
        totalAcademyAttackRange = academyAttackRange;
        totalAcademyAttackDamage = academyAttackDamage;
        totalAcademyDefense = academyDefense;
        totalAcademyAPPerRound = academyAPPerRound;

        academyMaxHP = 0;
        academyHPPerRound = 0;
        academyAttackRange = 0;
        academyAttackDamage = 0;
        academyDefense = 0;
        academyAPPerRound = 0;

        CalculatPlayerBaseData(player);
    }

    public void CalculatPlayerBaseData(Player player)
    {
        for (int i = 0; i < player.academyOwnedPoint.Length; i++)
        {
            player.academyOwnedPoint[i] = player.academyOwnedPoint[i] + academyEffectNum[i];
        }
        //FindObjectOfType<PlayerAcademyBuffcomponent>().UpdatePlayerAcademyBuff(player);

        player.MaxHP = 3 + totalAcademyMaxHP;
        player.AttackDamage = 1 + totalAcademyAttackDamage + totalCardAttackDamage;
        player.Range = 1 + totalAcademyAttackRange + totalCardAttackRange;
        player.Defence = totalAcademyDefense + totalCardDefense;
        player.ActionPointPerRound = 3 + totalAcademyAPPerRound;
    }

    public void CalaulatPlayerData(Player player)
    {
        player.HP += cardHP;

        if (player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }

        if (cardDamage > player.Defence)
        {
            player.HP -= (cardDamage - player.Defence);
        }
        player.CurrentActionPoint += cardAP;
    }

}
