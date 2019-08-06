using System;
using System.Collections;
using System.Collections.Generic;

public static class ListHelpers
{

    // Deprecated in favor of List.FindAll with Count
    public static bool TrueForExactlyN<T>(List<T> list, int n, Func<T, bool> f1)
    {
        var trueCount = 0;

        for (int i = 0; i < list.Count; i++)
        {            
            if (f1(list[i]))
            {                
                trueCount++;
            }
        }

        return trueCount == n;
    }
}
    
