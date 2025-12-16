using ChestPuzzle.Models;

public interface IPuzzleGenerator
{
    KeyColor GenerateLockColor();
    KeyColor[,] GenerateKeyGrid(int size, KeyColor lockColor, int requiredKeys);
}