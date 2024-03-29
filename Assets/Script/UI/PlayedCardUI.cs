using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayedCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text SelfPlayedCard;
    [SerializeField] private TMP_Text EnemyPlayedCard;
    [SerializeField] private Image selfTitle;
    [SerializeField] private Image enemyTitle;
    [SerializeField] private Sprite blueTitleSprite;
    [SerializeField] private Sprite redTitleSprite;
    private bool canStart=false;
    public void InitializePlayedCardUI()
    {
        if (GameplayManager.Instance.currentPlayer.Id == PlayerId.RedPlayer)
        {
            selfTitle.sprite = redTitleSprite;
            enemyTitle.sprite = blueTitleSprite;
        }
        else
        {
            selfTitle.sprite = blueTitleSprite;
            enemyTitle.sprite = redTitleSprite;
        }
        //Invoke("UnLock", 3);
        UnLock();
    }
    private void Update()
    {
        if (canStart)
        {
            if (GameplayManager.Instance.currentPlayer.Id == PlayerId.RedPlayer)
            {
                SelfPlayedCard.text = CardManager.Instance.redPlayerPlayedCards.Count.ToString();
                if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
                {
                    EnemyPlayedCard.text = "--";
                }
                else
                {
                    EnemyPlayedCard.text = CardManager.Instance.bluePlayerPlayedCards.Count.ToString();
                }
            }
            else
            {
                SelfPlayedCard.text = CardManager.Instance.bluePlayerPlayedCards.Count.ToString();
                if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
                {
                    EnemyPlayedCard.text = "--";
                }
                else
                {
                    EnemyPlayedCard.text = CardManager.Instance.redPlayerPlayedCards.Count.ToString();
                }
            }
        }
    }
    private void UnLock()
    {
        canStart = true;
        
    }
}
