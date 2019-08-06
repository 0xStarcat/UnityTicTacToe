using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour
{
    public Button button;
    public Text buttonText;
    public string playerSide;

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