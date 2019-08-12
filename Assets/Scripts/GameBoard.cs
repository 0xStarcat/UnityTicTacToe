using System.Collections;
using System.Collections.Generic;

public class Space
{   
    public int Id;
    public string GameMarker;

    public Space(int id, string gameMarker)
    {
        Id = id;
        GameMarker = gameMarker;
    }
}

public class GameBoard
{
    public GameController gameController;
    public List<string> Spaces;

    public GameBoard()
    {
        Spaces = new List<string>(new string[] { "", "", "", "", "", "", "", "", "" });
    }

    public GameBoard(List<string> spaces)
    {   
        Spaces = new List<string>();

        for (int i = 0; i < spaces.Count; i++)
        {
            Spaces.Add(spaces[i]);
        }
    }

    public void MakeMove(int index, string gameMarker)
    {
        Spaces[index] = gameMarker;

    }

    public List<List<Space>> GetTracks()
    {   
        List<List<Space>> trackList;
        Space[] row1 = { new Space(0, Spaces[0]), new Space(1, Spaces[1]), new Space(2, Spaces[2]) };
        Space[] row2 = { new Space(3, Spaces[3]), new Space(4, Spaces[4]), new Space(5, Spaces[5]) };
        Space[] row3 = { new Space(6, Spaces[6]), new Space(7, Spaces[7]), new Space(8, Spaces[8]) };
        Space[] column1 = { new Space(0, Spaces[0]), new Space(3, Spaces[3]), new Space(6, Spaces[6]) };
        Space[] column2 = { new Space(1, Spaces[1]), new Space(4, Spaces[4]), new Space(7, Spaces[7]) };
        Space[] column3 = { new Space(2, Spaces[2]), new Space(5, Spaces[5]), new Space(8, Spaces[8]) };
        Space[] diagonal1 = { new Space(0, Spaces[0]), new Space(4, Spaces[4]), new Space(8, Spaces[8]) };
        Space[] diagonal2 = { new Space(2, Spaces[2]), new Space(4, Spaces[4]), new Space(6, Spaces[6]) };

        trackList = new List<List<Space>>();
        trackList.Add(new List<Space>(row1));
        trackList.Add(new List<Space>(row2));
        trackList.Add(new List<Space>(row3));
        trackList.Add(new List<Space>(column1));
        trackList.Add(new List<Space>(column2));
        trackList.Add(new List<Space>(column3));
        trackList.Add(new List<Space>(diagonal1));
        trackList.Add(new List<Space>(diagonal2));

        return trackList;
    }

    public List<Space> GetAvailableSpaces()
    {   
        List<Space> spaces = new List<Space>();

        for (int i = 0; i < Spaces.Count; i++)
        {
            if (Spaces[i] == "")
            {
                spaces.Add(new Space(i, ""));
            }
        }
        
        return spaces;
    }

    public List<Space> GetSpaces()
    {
        List<Space> spaces = new List<Space>();

        for (int i = 0; i < Spaces.Count; i++)
        {
            spaces.Add(new Space(i, Spaces[i]));
        }

        return spaces;
    }
}