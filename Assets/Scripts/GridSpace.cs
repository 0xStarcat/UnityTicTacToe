using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour
{   
    public const int CENTER_SQUARE_SCORE = 2;
    public const int CORNER_SQUARE_SCORE = 1;
    public const int SIDE_SQUARE_SCORE = 0;
    
    public Button button;
    public Text buttonText;
    public int Score;

    private GameController gameController;

    public void SetSpace() 
    {
        buttonText.text = gameController.GetCurrentPlayer().gameMarker;
        button.interactable = false;
        gameController.EndTurn();

    }

    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }
}