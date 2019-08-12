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

public class Move
{   
    public List<Space> Track;
    public Space Space;
    public int Score;
    public string Type;

    public Move(List<Space> track, Space space, int score, string type)
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
    public const int LOSS_MOVE_SCORE = 0;

    public const string BASE_MOVE_TYPE = "BASE_MOVE";
    public const string CHECK_MOVE_TYPE = "CHECK_MOVE";
    public const string PREVENT_LOSS_MOVE_TYPE = "PREVENT_LOSS_MOVE";
    public const string WIN_MOVE_TYPE = "WIN_MOVE";

    public void BeginComputerTurn()
    {
 
    }


    // void EvaluateRandomMove()
    // {

    //     var random = new System.Random();
    //     var randomIndex = random.Next(availableSpaces.Count);

    //     CreateMove(availableSpaces[randomIndex], RANDOM_MOVE_PRIORITY, "random move");
    // }

    public static bool SpaceHasValue(Space space, string value)
    {
        return space.GameMarker == value;
    }

    bool TrackHasN(List<Space> track, string has, int n)
    {   // Determines if a 3 square track has N occurances of a value
        return track.FindAll(space => SpaceHasValue(space: space, value: has)).Count == n;
    }

    public GameBoard CreateMoveModel(int index, string playerMarker, List<string> spaces)
    {
        GameBoard clonedBoard = new GameBoard(spaces);
        clonedBoard.Spaces[index] = playerMarker;

        return clonedBoard;
    }

    public int CalculateDecisionScore(int index, string playerMarker)
    {
        string opponentMarker = gameController.GetOpponentGameMarker(playerMarker);
        GameBoard ownMoveModel = CreateMoveModel(index, playerMarker, gameController.gameBoard.Spaces);
        int ownMoveScore = EvaluateMove(ownMoveModel, index, playerMarker);
        int opponentPossibleMovesScore = 0;

        List<Space> availableSpaces = gameController.gameBoard.GetAvailableSpaces();
        
        foreach (var availableSpace in availableSpaces.FindAll(sp => sp.Id != index))
        {
            opponentPossibleMovesScore += EvaluateMove(CreateMoveModel(availableSpace.Id, opponentMarker, ownMoveModel.Spaces), availableSpace.Id, opponentMarker);
        }        

        return ownMoveScore - opponentPossibleMovesScore;
    }

    public int EvaluateMove(GameBoard clonedGameBoard, int index, string currentPlayerMarker)
    {
        // Calculates the score of a move 
        // based on its position in every track it occupies

        string opponentMarker = gameController.GetOpponentGameMarker(currentPlayerMarker);
        List<Move> scoreList = new List<Move>();

        if (MoveWillLose(clonedGameBoard, opponentMarker))
        {
            return LOSS_MOVE_SCORE;
        }

        // Clones game grid and adds the move into it
        foreach (var track in clonedGameBoard.GetTracks().FindAll(track => track.Exists(sp => sp.Id == index)))
        {            

            if (TrackHasN(track: track, has: currentPlayerMarker, n: 3))
            {   // Win
                scoreList.Add(new Move(track: track, space: clonedGameBoard.GetSpaces()[index], score: WIN_MOVE_SCORE, type: WIN_MOVE_TYPE));
                return scoreList.Sum(m => m.Score);
            }
            else if (TrackHasN(track: track, has: opponentMarker, n: 2))
            {   // prevent loss
                scoreList.Add(new Move(track: track, space: clonedGameBoard.GetSpaces()[index], score: PREVENT_LOSS_MOVE_SCORE, type: PREVENT_LOSS_MOVE_TYPE));
            }
            else if (TrackHasN(track: track, has: currentPlayerMarker, n: 2) && TrackHasN(track: track, has: "", n: 1))
            {   // check opponent
                scoreList.Add(new Move(track: track, space: clonedGameBoard.GetSpaces()[index], score: CHECK_MOVE_SCORE, type: CHECK_MOVE_TYPE));
            }
            else if (TrackHasN(track: track, has: "", n: 2))
            {
                scoreList.Add(new Move(track: track, space: clonedGameBoard.GetSpaces()[index], score: BASE_MOVE_SCORE, type: BASE_MOVE_TYPE));
            }
        }

        return scoreList.Sum(m => m.Score);
    }

    public bool MoveWillLose(GameBoard clonedGameBoard, string opponentMarker)
    {

        foreach (var track in clonedGameBoard.GetTracks())
        {

            if (TrackHasN(track: track, has: opponentMarker, n: 2) && TrackHasN(track: track, has: "", n: 1))
            {
                
                return true;
            }
        }

        return false;
    }
    
}
