using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S3Stage : MonoBehaviour
{
    private Dictionary<Player, List<Card>> playedCardDict = new Dictionary<Player, List<Card>>();
    private List<Player> playerList = new List<Player>();
    private int i;
    //private Dictionary<Player,>;
    public void StartStage(Dictionary<Player, List<Card>> playerCardListDict)
    {
        playedCardDict = playerCardListDict;
        playerList = new List<Player>();
        StartCoroutine("S3CardTakeEffect");

    }
    IEnumerator S3CardTakeEffect()
    {
        for (int i = 0; i < playedCardDict.Count; i++)
        {
            playerList.Add(playedCardDict.ElementAt(i).Key);
        }
        List<Player> priorityList = playerList.OrderByDescending(x => x.Priority).ToList();
        while (playedCardDict[priorityList[i]].Count != 0)
        {
            for (i = 0; i < priorityList.Count; i++)
            {
                List<Card> playedCard = null;
                if (playedCardDict.TryGetValue(priorityList[i], out playedCard))
                {
                    if (playedCard.Count == 0)
                    {
                        playedCardDict.Remove(priorityList[i]);
                        break;
                    }
                    //Interact 
                    //Debug.Log(priorityList[i].name + " " + playerInteract[0].PlayerInteractType);

                    yield return new WaitForSeconds(1);
                    CardManager.Instance.CardTakeEffectClientRpc(priorityList[i].Id, EffectStage.S3);
                    playedCard.RemoveAt(0);
                    playedCardDict[priorityList[i]] = playedCard;
                }
            }
        }
        List<Player> dyingPlayers = new List<Player>();
        List<Player> alivePlayers = new List<Player>();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].HP <= 0)
            {
                dyingPlayers.Add(playerList[i]);
            }
            else
            {
                alivePlayers.Add(playerList[i]);
            }
        }
        if (dyingPlayers.Count == 0)
        {
            TurnbasedSystem.Instance.isDie.Value = false;
        }
        else
        {
            TurnbasedSystem.Instance.isDie.Value = true;
            PlayerManager.Instance.PlayerDying(dyingPlayers, alivePlayers);
        }
        TurnbasedSystem.Instance.CompleteStage(GameStage.S3);

    }
}
