using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelLoader
{
    public static List<string[]> LoadLevels(IEnumerable<TextAsset> levels)
    {
        return levels.Select(level => level.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)).ToList();
    }
}
