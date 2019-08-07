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
    public string Comment;

    public ComputerMove(GameObject square, int priority, string comment)
    {
        Square = square;
        Priority = priority;
        Comment = comment;
    }
}

public class EvaluatedTrack
{
    public List<GameObject> Track;
    public int Score;

    public EvaluatedTrack(List<GameObject> track, int score)
    {
        Track = track;
        Score = score;
    }
}

public class ComputerPlayer : MonoBehaviour
{
    public GameController gameController;
    private List<ComputerMove> possibleMoves = new List<ComputerMove>();
    public const int RANDOM_MOVE_PRIORITY = 0;
    public const int SMART_MOVE_PRIORITY = 1;
    public const int PREVENT_LOSS_MOVE_PRIORITY = 99;
    public const int WIN_MOVE_PRIORITY = 100;

    List<GameObject> availableSpaces = new List<GameObject>();
    List<GameObject> occupiedSpaces = new List<GameObject>();
    List<EvaluatedTrack> evaluatedTracks = new List<EvaluatedTrack>();

    public void BeginComputerTurn()
    {
        

        EvaluateBoard();
        EvaluateAllPossibleMoves();
        MakeBestMove();
        
    }

    void EvaluateAllPossibleMoves()
    {   
        possibleMoves.Clear();
        EvaluateForWin();
        EvaluateForLoss();
        EvaluateSmartMove();
        EvaluateRandomMove();
    }

    void CreateMove(GameObject square, int priority, string comment)
    {
        possibleMoves.Add(new ComputerMove(square, priority, comment));
    }

    void EvaluateBoard()
    {   // identifies all available & occupied spaces
        availableSpaces.Clear();
        occupiedSpaces.Clear();
        evaluatedTracks.Clear();

        foreach (var space in gameController.buttonList)
        {
            var text = space.GetComponentInChildren<Text>().text;
            if (string.IsNullOrEmpty(text))
            {
                availableSpaces.Add(space);
            }
            else
            {
                occupiedSpaces.Add(space);
            }
        }

        EvaluateTracks();
    }

    void EvaluateTracks()
    {
        for (int i = 0; i < gameController.gridList.Count; i++)
        {
            if (TrackHasN(track: gameController.gridList[i], has: "", n: 3))
            {
                evaluatedTracks.Add(new EvaluatedTrack(track: gameController.gridList[i], score: 2));
            } else if (TrackHasN(track: gameController.gridList[i], has: "", n: 2))
            {
                evaluatedTracks.Add(new EvaluatedTrack(track: gameController.gridList[i], score: 1));
            } else
            {
                evaluatedTracks.Add(new EvaluatedTrack(track: gameController.gridList[i], score: 0));
            }
        }
    }

    void EvaluateForWin()
    {
        for (int i = 0; i < gameController.gridList.Count; i++)
        {

            // Checks each track (3 square line)
            // And determines if the computer player has 2 occupied squares while 1 empty square remains in the track.
            // And moves into the empty square to win.
            if (TrackHasN(track: gameController.gridList[i], has: gameController.computerPlayer.gameMarker, n: 2) && TrackHasN(track: gameController.gridList[i], has: "", n: 1))
            {
                // UnityEngine.Debug.Log("I can win!");
                // DebugHelpers.PrintTrackContents(gameController.gridList[i]);
                CreateMove(FindTrackSquareWithValue(track: gameController.gridList[i], value: ""), WIN_MOVE_PRIORITY, "winning move");
            }
        }
    }

    void EvaluateForLoss()
    {
        for (int i = 0; i < gameController.gridList.Count; i++)
        {

            // Checks each track (3 square line)
            // And determines if the human player has 2 occupied squares while 1 empty square remains in the track.
            // And moves into the empty square to prevent loss.
            if (TrackHasN(track: gameController.gridList[i], has: gameController.humanPlayer.gameMarker, n: 2) && TrackHasN(track: gameController.gridList[i], has: "", n: 1))
            {
                // UnityEngine.Debug.Log("I can lose...");
                // DebugHelpers.PrintTrackContents(gameController.gridList[i]);
                
                CreateMove(FindTrackSquareWithValue(track: gameController.gridList[i], value: ""), PREVENT_LOSS_MOVE_PRIORITY, "preventing loss");
            }
        }

    }

    void EvaluateSmartMove()
    {
        // Choose the best evaluated track (All open = 2, two open = 1, else = 0)
        // Choose the best space in that track (center = 2, corner = 1, side = 0)

        var bestTrack = evaluatedTracks.OrderByDescending(track => track.Score).ToList()[0];
        UnityEngine.Debug.Log("BEST: " + bestTrack.Score);
        
        var bestMoveList = bestTrack.Track.FindAll(space => SpaceHasValue(space: space, value: "")).OrderByDescending(space => space.GetComponent<GridSpace>().Score).ToList();

        if (bestMoveList.Count > 0)
        {
            CreateMove(bestMoveList[0], SMART_MOVE_PRIORITY, "smart move");
        }
    }

    void EvaluateRandomMove()
    {

        var random = new System.Random();
        var randomIndex = random.Next(availableSpaces.Count);

        CreateMove(availableSpaces[randomIndex], RANDOM_MOVE_PRIORITY, "random move");
    }

    GameObject FindTrackSquareWithValue(List<GameObject> track, string value)
    {
        return track.Find(gridSpace => SpaceHasValue(space: gridSpace, value: value));
    }

    bool SpaceHasValue(GameObject space, string value)
    {
        return space.GetComponentInChildren<Text>().text == value;
    }

    bool TrackHasN(List<GameObject> track, string has, int n)
    {   // Determines if a 3 square track has N occurances of a value
        return track.FindAll(gridSpace => SpaceHasValue(space: gridSpace, value: has)).Count == n;
    }

    bool TrackHasGteN(List<GameObject> track, string has, int n)
    {   // Determines if a 3 square track has greater than or equal to >= N occurances of a value
        return track.FindAll(gridSpace => SpaceHasValue(space: gridSpace, value: has)).Count >= n;
    }

    void MakeBestMove()
    {
        var bestMove = possibleMoves.OrderByDescending(move => move.Priority).ToList()[0];
        UnityEngine.Debug.Log(bestMove.Comment);
        
        bestMove.Square.GetComponent<GridSpace>().SetSpace();
    }
    
}
