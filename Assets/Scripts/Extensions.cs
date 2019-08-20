using System;
using System.Collections.Generic;

public static class Extensions
{
    private static readonly Random Rnd = new Random();

    public static T PopRandom<T>(this List<T> self)
    {
        int i = Rnd.Next(self.Count);
        T val = self[i];
        self.RemoveAt(i);

        return val;
    }
}
