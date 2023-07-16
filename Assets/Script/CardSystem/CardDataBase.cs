using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static CardDataBase Instance;
    public static Dictionary<AcademyType, List<CardSetting>> AllCardListDic = new Dictionary<AcademyType, List<CardSetting>>();
    public Dictionary<AcademyType, List<CardSetting>> AllTopCardListDic = new Dictionary<AcademyType, List<CardSetting>>();
    public List<CardSetting> AllCardList = new List<CardSetting>();
    public List<Card> cards = new List<Card>();
    public Card card;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
        for (int i=0;i<=(int)AcademyType.FA;i++)
        {
            if(i==0)
            {
                //Debug.Log(Resources.LoadAll<CardSetting>("Cards/BasicCards").Length);
                AllCardListDic.Add((AcademyType)i, new List<CardSetting>(Resources.LoadAll<CardSetting>("Cards/BasicCards")));
                continue;
            }
            AllCardListDic.Add((AcademyType)i, new List<CardSetting>(Resources.LoadAll<CardSetting>("Cards/EventCards/"+ ((AcademyType)i).ToString())));
            AllTopCardListDic.Add((AcademyType)i, new List<CardSetting>(Resources.LoadAll<CardSetting>("Cards/TopCards/" + ((AcademyType)i).ToString())));
        }
        AllCardList = new List<CardSetting>(Resources.LoadAll<CardSetting>("Cards"));
        
    }
    public void Start()
    {
        for (int i = 0; i < AllCardList.Count; i++)
        {
            card.cardSetting = AllCardList[i];
            cards.Add(card);
        }
    }
}
