using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerMove
{
    public GameObject Square;
    public int Priority;

    public ComputerMove(GameObject square, int priority)
    {
        Square = square;
        Priority = priority;
    }
}

public class ComputerPlayer : MonoBehaviour
{
    public GameController gameController;
    private List<ComputerMove> possibleMoves = new List<ComputerMove>();
    public const int BASE_MOVE_PRIORITY = 1;
    public const int SMART_MOVE_PRIORITY = 2;
    public const int PREVENT_LOSS_MOVE_PRIORITY = 3;
    public const int WIN_MOVE_PRIORITY = 4;

    List<GameObject> availableSpaces = new List<GameObject>();
    List<GameObject> occupiedSpaces = new List<GameObject>();

    public void BeginComputerTurn()
    {
        possibleMoves.Clear();
        ReadGameBoard();
        MakeBestMove();
        
    }

    void ReadGameBoard()
    {   
        availableSpaces.Clear();
        occupiedSpaces.Clear();

        foreach (var space in gameController.buttonList)
        {   
            var text = space.GetComponentInChildren<Text>().text;
            if (string.IsNullOrEmpty(text))
            {
                availableSpaces.Add(space);
            } else 
            {
                occupiedSpaces.Add(space);
            }
        }

        EvaluateForWin();
        EvaluateForLoss();
        EvaluateRandomMove();
    }

    void CreateMove(GameObject square, int priority)
    {
        possibleMoves.Add(new ComputerMove(square, priority));
    }

    void EvaluateForLoss()
    {
        for (int i = 0; i < gameController.gridList.Count; i++)
        {

            // Checks each track (3 square line)
            // And determines if the human player has 2 occupied squares while 1 empty square remains in the track.
            if (TrackHasN(track: gameController.gridList[i], has: gameController.humanPlayer.gameMarker, n: 2) && TrackHasN(track: gameController.gridList[i], has: "", n: 1))
            {
                UnityEngine.Debug.Log("I can lose...");
                DebugHelpers.PrintTrackContents(gameController.gridList[i]);
                
                CreateMove(FindTrackSquareWith(track: gameController.gridList[i], ""), PREVENT_LOSS_MOVE_PRIORITY);
            }
        }

    }

    void EvaluateForWin()
    {
        for (int i = 0; i < gameController.gridList.Count; i++)
        {

            // Checks each track (3 square line)
            // And determines if the computer player has 2 occupied squares while 1 empty square remains in the track.
            if (TrackHasN(track: gameController.gridList[i], has: gameController.computerPlayer.gameMarker, n: 2) && TrackHasN(track: gameController.gridList[i], has: "", n: 1))
            {
                UnityEngine.Debug.Log("I can win!");
                DebugHelpers.PrintTrackContents(gameController.gridList[i]);
                CreateMove(FindTrackSquareWith(track: gameController.gridList[i], ""), WIN_MOVE_PRIORITY);
            }
        }
    }

    bool TrackHasN(List<GameObject> track, string has, int n)
    {

        return track.FindAll(gridSpace => gridSpace.GetComponentInChildren<Text>().text == has).Count == n;
    }

    GameObject FindTrackSquareWith(List<GameObject> track, string with)
    {
        return track.Find(gridSpace => gridSpace.GetComponentInChildren<Text>().text == with);
    }

    void EvaluateRandomMove()
    {
        var random = new System.Random();
        var randomIndex = random.Next(availableSpaces.Count);

        CreateMove(availableSpaces[randomIndex], BASE_MOVE_PRIORITY);
    }

    void MakeBestMove()
    {
        possibleMoves.OrderByDescending(move => move.Priority).ToList()[0].Square.GetComponent<GridSpace>().SetSpace();
    }
    
}
