using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] private Vector2[] playerStartPoint = new Vector2[2];
    //public Player playerRed;
    //public Player playerBlue;
    public List<Player> playerList = new List<Player>();
    public Player currentPlayer;
    public GameplayUI gameplayUI;
    private ControlStage controlStage;
    private MoveStage moveStage;
    private AttackStage attackStage;
    public DiscardStage discardStage;
    private S2Stage s2Stage;
    private S3Stage s3Stage;
    private S4Stage s4Stage;

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
        

        controlStage = GetComponent<ControlStage>();
        discardStage = GetComponent<DiscardStage>();
        s2Stage = GetComponent<S2Stage>();
        moveStage = GetComponent<MoveStage>();
        s3Stage = GetComponent<S3Stage>();
        attackStage = GetComponent<AttackStage>();
        s4Stage = GetComponent<S4Stage>();
    }
    private void Start()
    {
        InitializePlayerClientRpc();
        CardManager.Instance.InitializeCardManager();
        
    }
    private void Update()
    {
        //if (playerRed.HP <= 0 || playerBlue.HP <= 0)
        //{
        //    TurnbasedSystem.Instance.Pause();
        //}
        //if (TurnbasedSystem.Instance.CurrentGameStage != GameStage.S1)
        //{
        //    UIManager.Instance.ShowGridObjectUI(false, null);
        //}
    }
    #region initializePlayer funcation ABANDON
    //can't use
    public void InitializePlayer()
    {
        //connectedClientsList only accessable to the server
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerList.Add(client.PlayerObject.GetComponent<Player>());
        }
        for(int i =0;i< playerList.Count;i++)
        {
            playerList[i].transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].trueGrid = playerList[i].currentGrid;
            playerList[i].GetComponent<PlayerInteractionComponent>().RefreshLinePath();
        }
        InitializePlayerClientRpc();
    }
    #endregion
    [ClientRpc]
    private void InitializePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        playerList =FindObjectsOfType<Player>().ToList<Player>();
        playerList = playerList.OrderBy(a=>a.Id).ToList();
        for (int i = 0; i < playerList.Count; i++)
        {
            //playerList[i].transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            //playerList[i].currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            //playerList[i].GetComponent<PlayerInteractionComponent>().RefreshLinePath();
            GridObject targetGridObject = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            PlayerManager.Instance.InitializePlayerStartPoint(playerList[i],targetGridObject);

        }
        int currentPlayrId = (int)NetworkManager.Singleton.LocalClientId;
        currentPlayer = playerList[currentPlayrId];
        GridObject currentGridObject = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[currentPlayrId].x, (int)playerStartPoint[currentPlayrId].y);
        GridVfxManager.Instance.UpdateVfxAcademy(currentGridObject);
        UIManager.Instance.UpdatePlayerDataUI(currentPlayer);
        
    }
    public void SetCurrentPlayer(Player player)
    {
        currentPlayer = player;

    }

    public List<Player> GetPlayer()
    {
        return playerList;
    }
    public void UpdateSelectPlayer(Player player)
    {
        this.currentPlayer = player;
        UIManager.Instance.UpdatePlayerDataUI(this.currentPlayer);
    }
    public void ShowGirdObjectData(Transform gridTrans)
    {
        GridObject selectedGridObject = GridManager.Instance.GetSelectedGridObject(gridTrans.position);
        if (selectedGridObject == null) return;
        UIManager.Instance.ShowGridObjectUI(true,gridTrans);
        PlayerManager.Instance.UpdateGridAuthorityData(currentPlayer, selectedGridObject);

    }

    public void StartControlStage()
    {
        BackUpControlStageClientRpc();
        controlStage.StartStage();
    }
    [ClientRpc]
    public void BackUpControlStageClientRpc()
    {
        GridManager.Instance.BackupGrid();
        foreach (var player in playerList)
        {
            PlayerManager.Instance.BackupPlayerData(player);
            player.UpdateDataPerTurn();
        }
    }
    public void StartDiscardStage()
    {
        ResetControlStageClientRpc();
        discardStage.StartStage();
    }
    [ClientRpc]
    public void ResetControlStageClientRpc()
    {
        GridManager.Instance.ResetGrid();
        //foreach (var player in playerList)
        //{
        //    PlayerManager.Instance.ResetPlayerDateAfterControlStage(player);
        //}
        UIManager.Instance.ShowGridObjectUI(false, null);
    }
    public void StartS2Stage()
    {
        s2Stage.StartStage(FindObjectOfType<CardManager>().playedCardDict);
    }

    public void StartMoveStage()
    {
        RefreshDataClientRpc();
        moveStage.StartStage(controlStage.playerInteractDict);
    }

    [ClientRpc]
    public void RefreshDataClientRpc()
    {
        foreach (var player in playerList)
        {
            PlayerManager.Instance.ResetPlayerDateAfterControlStage(player);
        }
    }

    public void StartS3Stage()
    {
        s3Stage.StartStage(FindObjectOfType<CardManager>().playedCardDict);
    }

    public void StartAttackStage()
    {
        attackStage.StartStage();
    }

    public void StartS4Stage()
    {
        s4Stage.StartStage(FindObjectOfType<CardManager>().playedCardDict);
    }

    public int GetWinner()
    {
        List<Player> dyingPlayers = new List<Player>();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].isDying.Value)
            {
                dyingPlayers.Add(playerList[i]);
            }
        }
        if (dyingPlayers.Count == 1)
        {
            return dyingPlayers[0].Id == PlayerId.RedPlayer ? 0 : 1;
        }
        else if (dyingPlayers.Count == 2)
        {
            dyingPlayers.OrderByDescending(a => a.HP);
            if (dyingPlayers[0].HP != dyingPlayers[1].HP)
            {
                return dyingPlayers[0].Id == PlayerId.RedPlayer ? 1 : 0;
            }
            else{
                int player0LandCount=0;
                int player1LandCount=0;
                for (AcademyType i = AcademyType.YI; i < AcademyType.FA; i++)
                {
                    player0LandCount += dyingPlayers[0].OwnedLandDic[i].Count;
                    player1LandCount += dyingPlayers[1].OwnedLandDic[i].Count;
                }
                if (player0LandCount == player1LandCount)
                {
                    return 2;
                }else
                {
                    return player0LandCount > player1LandCount ? 1 : 0;
                }
            }
        }
        return 0;
    }
}
