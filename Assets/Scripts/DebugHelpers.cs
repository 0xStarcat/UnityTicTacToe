using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DebugHelpers
{
    public static void PrintTrackContents(List<GameObject> track)
    {
        foreach (var gridSpace in track)
        {
            UnityEngine.Debug.Log(gridSpace.GetComponentInChildren<Text>().text);
        }
    }
}