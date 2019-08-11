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

    public List<GameObject> AvailableSpaces()
    {
        return Track.FindAll(space => ComputerPlayer.SpaceHasValue(space: space, value: "")).ToList();
    }
}

public class Move
{   
    public List<GameObject> Track;
    public GridSpace Space;
    public int Score;
    public string Type;

    public Move(List<GameObject> track, GridSpace space, int score, string type)
    {
        Track = track;
        Space = space;
        Score = score;
        Type = type;
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
    public const int BASE_MOVE_SCORE = 1;
    public const int POSSIBLE_MOVES = 9;
    public const int CHECK_MOVE_SCORE = BASE_MOVE_SCORE * POSSIBLE_MOVES * 2;
    public const int PREVENT_LOSS_MOVE_SCORE = BASE_MOVE_SCORE * POSSIBLE_MOVES * 3;
    public const int WIN_MOVE_SCORE = BASE_MOVE_SCORE * POSSIBLE_MOVES * POSSIBLE_MOVES;

    public const string BASE_MOVE_TYPE = "BASE_MOVE";
    public const string CHECK_MOVE_TYPE = "CHECK_MOVE";
    public const string PREVENT_LOSS_MOVE_TYPE = "PREVENT_LOSS_MOVE";
    public const string WIN_MOVE_TYPE = "WIN_MOVE";

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

    List<GameObject> GetAvailableSpaces()
    {   
        List<GameObject> availableSpaces = new List<GameObject>();
        foreach (var space in gameController.buttonList)
        {
            var text = space.GetComponentInChildren<Text>().text;
            if (string.IsNullOrEmpty(text))
            {
                availableSpaces.Add(space);
            }
        }

        return availableSpaces;
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
        // TODO - solve for opponent corner start.
        // If opponent can exploit 4 open tracks -
        // Force their hand into a side square - square that only benefits 1 open track.

        // Choose the best evaluated track (All open = 2, two open = 1, else = 0)
        // Choose the best space in that track (center = 2, corner = 1, side = 0)
        var random = new System.Random();
        List<EvaluatedTrack> bestTrackList = evaluatedTracks.GroupBy(track => track.Score) // Group tracks by score [[2], [1, 1], [0, 0]]
                                                            .Select(group => group.ToList())
                                                            .ToList()
                                                            .OrderByDescending(group => group[0].Score) // order groups by score
                                                            .ToList()[0];
        UnityEngine.Debug.Log("BTL: " + bestTrackList.Count);
        
        EvaluatedTrack bestTrack = bestTrackList.FindAll(track => track.AvailableSpaces().Count > 0) // With available spaces
                                                .OrderByDescending(track => track.AvailableSpaces()
                                                                                 .Max(space => space.GetComponent<GridSpace>().Score))
                                                                                 .ToList()[0]; // Track with the highest scored available square
        UnityEngine.Debug.Log("BEST TRACK: " + bestTrack.Score);
        foreach (var item in bestTrack.Track)
        {
            UnityEngine.Debug.Log(item);
            
        }
        
        
        var bestMoveList = bestTrack.Track.FindAll(space => SpaceHasValue(space: space, value: "")).OrderByDescending(space => space.GetComponent<GridSpace>().Score).ToList();

        if (bestMoveList.Count > 0)
        {
            CreateMove(bestMoveList[0], SMART_MOVE_PRIORITY, "smart move");
            UnityEngine.Debug.Log("BEST MOVE: " + bestMoveList[0].GetComponent<GridSpace>().Score);
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

    public static bool SpaceHasValue(GameObject space, string value)
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

    public bool Test()
    {
        return true;
    }

    public List<List<GameObject>> CreateMoveModel(GridSpace space)
    {
        List<List<GameObject>> clonedBoard = gameController.gridList.ConvertAll(el => el);

        foreach (var track in clonedBoard.FindAll(track => track.Exists(gridSpace => gridSpace.GetComponent<GridSpace>().id == space.id)))
        {

            var gridSpace = track.Find(gs => gs.GetComponent<GridSpace>().id == space.id).GetComponent<GridSpace>();

            gridSpace.AddGameMarker(gameController.GetCurrentPlayer().gameMarker);
        }

        return clonedBoard;

    }

    public int CalculateDecisionScore(GridSpace space, string currentPlayerMarker)
    {
        string opponentMarker = gameController.GetOpponentGameMarker(currentPlayerMarker);
        int ownMoveScore = EvaluateMove(CreateMoveModel(space), space, currentPlayerMarker).Sum(move => move.Score);
        int opponentPossibleMovesScore = 0;
        List<GameObject> availableSpaces = GetAvailableSpaces();
        
        foreach (var gridSpaceObject in availableSpaces.FindAll(sp => sp.GetComponent<GridSpace>().id != space.id))
        {
            GridSpace gridSpace = gridSpaceObject.GetComponent<GridSpace>();
            opponentPossibleMovesScore += EvaluateMove(CreateMoveModel(gridSpace), gridSpace, opponentMarker).Sum(m => m.Score);
        }        

        return ownMoveScore - opponentPossibleMovesScore;
    }

    public List<Move> EvaluateMove(List<List<GameObject>> clonedGameBoard, GridSpace space, string currentPlayerMarker)
    {
        // Calculates the score of a move 
        // based on its position in every track it occupies


        List<Move> scoreList = new List<Move>();
        string opponentMarker = gameController.GetOpponentGameMarker(currentPlayerMarker);

        // Clones game grid and adds the move into it
        foreach (var track in clonedGameBoard.FindAll(track => track.Exists(gridSpace => gridSpace.GetComponent<GridSpace>().id == space.id)))
        {
            
            var gridSpace = track.Find(gs => gs.GetComponent<GridSpace>().id == space.id).GetComponent<GridSpace>();
            

            if (TrackHasN(track: track, has: currentPlayerMarker, n: 3))
            {   // Win
                scoreList.Add(new Move(track: track, space: gridSpace, score: WIN_MOVE_SCORE, type: WIN_MOVE_TYPE));
            }
            else if (TrackHasN(track: track, has: opponentMarker, n: 2))
            {   // prevent loss
                scoreList.Add(new Move(track: track, space: gridSpace, score: PREVENT_LOSS_MOVE_SCORE, type: PREVENT_LOSS_MOVE_TYPE));
            }
            else if (TrackHasN(track: track, has: currentPlayerMarker, n: 2) && TrackHasN(track: track, has: "", n: 1))
            {   // check opponent
                scoreList.Add(new Move(track: track, space: gridSpace, score: CHECK_MOVE_SCORE, type: CHECK_MOVE_TYPE));
            }
            else if (TrackHasN(track: track, has: "", n: 2))
            {
                scoreList.Add(new Move(track: track, space: gridSpace, score: BASE_MOVE_SCORE, type: BASE_MOVE_TYPE));
            }
        }
        return scoreList;
    }
    
}
