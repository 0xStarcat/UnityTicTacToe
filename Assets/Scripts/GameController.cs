using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;
    public string gameMarker;
    [HideInInspector]
    public bool isComputer;
}

[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour
{
    public GameBoard gameBoard;
    public GameObject[] buttonList;
    public List<List<GameObject>> gridList;
    public GameObject gameOverPanel;
    public GameObject restartButton;
    public ComputerPlayer computerPlayerObject;
    
    public Player playerX;
    public Player playerO;

    public Player humanPlayer;
    public Player computerPlayer;

    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;

    private Player currentPlayer;
    private string gameOverText;
    private int moveCount;

    void Awake() {
        ConstructGrid();
        SetGameControllerReferenceOnButtons();
        RestartGame();

        gameBoard = new GameBoard();
    }

    ////
    // GET / SET
    ////

    public void MakeMove(int index, string gameMarker)
    {
        gameBoard.MakeMove(index, gameMarker);
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void SetSpaceAtIndex(int index, string gameMarker)
    {
        buttonList[index].GetComponent<GridSpace>().AddGameMarker(gameMarker);
    }

    public Player GetOpponent(Player player)
    {
        if (player.gameMarker == "X")
        {
            return playerO;
        } else if (player.gameMarker == "O")
        {
            return playerX;
        } else 
        {
            return null;
        }
    }

    public string GetOpponentGameMarker(string gameMarker)
    {
        if (gameMarker == "X")
        {
            return "O";
        } else if (gameMarker == "O") {
            return "X";
        } else {
            return "";
        }
    }

    ////
    // GAME STATES
    ////

    private void WaitingToPlay()
    {
        SetButtonsInteractable(false);
        SetChoosePlayerButtonsActive(true);
    }

    private void WaitingForPlayerMove()
    {
        if (currentPlayer.isComputer)
        {
            SetButtonsInteractable(false);
            computerPlayerObject.BeginComputerTurn();
        } else {
            SetButtonsInteractable(true);
        }
        
    }

    private void GameWon()
    {
        gameOverText = $"{currentPlayer.gameMarker} wins!";
    }

    private void GameTied()
    {
        gameOverText = "Game Tied!";
    }

    private void GameOver(bool tie)
    {
        if (tie) { GameTied(); } else { GameWon(); }

        DisplayGameOverPanel();
        SetButtonsInteractable(false);
        restartButton.SetActive(true);
    }

    ////
    //  GAME SETUP
    ////

    private void ConstructGrid()
    {
        GameObject[] row1 = { buttonList[0], buttonList[1], buttonList[2] };
        GameObject[] row2 = { buttonList[3], buttonList[4], buttonList[5] };
        GameObject[] row3 = { buttonList[6], buttonList[7], buttonList[8] };
        GameObject[] column1 = { buttonList[0], buttonList[3], buttonList[6] };
        GameObject[] column2 = { buttonList[1], buttonList[4], buttonList[7] };
        GameObject[] column3 = { buttonList[2], buttonList[5], buttonList[8] };
        GameObject[] diagonal1 = { buttonList[0], buttonList[4], buttonList[8] };
        GameObject[] diagonal2 = { buttonList[2], buttonList[4], buttonList[6] };

        gridList = new List<List<GameObject>>();
        gridList.Add(new List<GameObject>(row1));
        gridList.Add(new List<GameObject>(row2));
        gridList.Add(new List<GameObject>(row3));
        gridList.Add(new List<GameObject>(column1));
        gridList.Add(new List<GameObject>(column2));
        gridList.Add(new List<GameObject>(column3));
        gridList.Add(new List<GameObject>(diagonal1));
        gridList.Add(new List<GameObject>(diagonal2));
    }

    public void SetHumanSide(string humanSide)
    {   // Called from Player Button
        if (humanSide == "X") 
        { 
            currentPlayer = playerX; 
            SetPlayerTypes(playerX, playerO);
            SetPlayerColors(playerX, playerO); 
        } else if (humanSide == "O")
        {
            currentPlayer = playerO;
            SetPlayerTypes(playerO, playerX);
            SetPlayerColors(playerO, playerX); 
        }

        StartGame();
    }

    private void SetPlayerTypes(Player human, Player computer)
    {
        human.isComputer = false;
        computer.isComputer = true;
        humanPlayer = human;
        computerPlayer = computer;
    }

    private void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;

        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    private void SetGameControllerReferenceOnButtons() 
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponent<GridSpace>().SetGameController(this);
        }
    }

    ////
    // GAME MECHANIC ACTIONS
    ////

    private void StartGame()
    {
        SetButtonsInteractable(true);
        SetChoosePlayerButtonsActive(false);
    }
    
    public void EndTurn() { 
        moveCount++;
        CheckWin();
    }

    private void ChangeSides()
    {
        if (currentPlayer == playerX) 
        {
            currentPlayer = playerO;
            SetPlayerColors(newPlayer: playerO, oldPlayer: playerX);
        } else if (currentPlayer == playerO)
        {
            currentPlayer = playerX;
            SetPlayerColors(newPlayer: playerX, oldPlayer: playerO);
        }
    }

    private void CheckWin() {
        if (gridList.Exists(list => list.TrueForAll(space => space.GetComponentInChildren<Text>().text == currentPlayer.gameMarker)))
        {
            GameOver(tie: false);
        }else if (moveCount == 9)
        {
            GameOver(tie: true);
        } else 
        {
            ChangeSides();
            WaitingForPlayerMove();
        }

    }

    private void RestartGame()
    {
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        moveCount = 0;
        ClearButtonValues();
        WaitingToPlay();
    }

    ////
    //  GAME UI ACTIONS
    ////

    private void DisplayGameOverPanel()
    {
        gameOverPanel.GetComponentInChildren<Text>().text = gameOverText;
        gameOverPanel.SetActive(true);
    }

    private void SetButtonsInteractable(bool toggle)
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            if (buttonList[i].GetComponentInChildren<Text>().text == "")
            {   // Only execute on buttons that aren't already taken.
                buttonList[i].GetComponent<Button>().interactable = toggle;
            }
        }
    }

    private void SetChoosePlayerButtonsActive(bool toggle)
    {
        playerX.button.gameObject.SetActive(toggle);
        playerO.button.gameObject.SetActive(toggle);
    }

    private void ClearButtonValues()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInChildren<Text>().text = null;
        }
    }


}
