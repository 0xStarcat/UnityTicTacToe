﻿using System;
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
    public int id;
    public string Value;

    private GameController gameController;

    public void SetSpace() 
    {
        button.interactable = false;
        gameController.MakeMove(id, gameController.GetCurrentPlayer().gameMarker);
        AddGameMarker(gameController.GetCurrentPlayer().gameMarker);
        gameController.EndTurn();
    }

    public void AddGameMarker(string gameMarker)
    {
        buttonText.text = gameMarker;
        Value = gameMarker;
    }

    public void SetGameController(GameController controller)
    {

        gameController = controller;
        id = Array.IndexOf(gameController.buttonList, this.gameObject);
    }
}