using ChestPuzzle.Models;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : IPuzzleGenerator
{
    public KeyColor GenerateLockColor()
    {
        var colors = System.Enum.GetValues(typeof(KeyColor));
        return (KeyColor)colors.GetValue(Random.Range(0, colors.Length));
    }

    public KeyColor[,] GenerateKeyGrid(int size, KeyColor lockColor, int requiredKeys)
    {
        var grid = new KeyColor[size, size];
        var availablePositions = new List<Vector2Int>();

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                availablePositions.Add(new Vector2Int(x, y));

        for (int i = 0; i < requiredKeys; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            var pos = availablePositions[randomIndex];
            grid[pos.x, pos.y] = lockColor;
            availablePositions.RemoveAt(randomIndex);
        }

        var otherColors = GetOtherColors(lockColor);
        foreach (var pos in availablePositions)
        {
            grid[pos.x, pos.y] = otherColors[Random.Range(0, otherColors.Count)];
        }

        return grid;
    }

    private List<KeyColor> GetOtherColors(KeyColor excluded)
    {
        var result = new List<KeyColor>();
        foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
        {
            if (color != excluded) result.Add(color);
        }
        return result;
    }
}