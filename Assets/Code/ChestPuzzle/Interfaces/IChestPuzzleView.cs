using ChestPuzzle.Models;
using System;
using UnityEngine;

namespace ChestPuzzle.Interfaces
{
    public interface IChestPuzzleView
    {
        event Action<KeyColor, Vector2Int> OnKeyDraggedToLock;
        event Action OnViewClosed;

        void Show();
        void Hide();
        void Initialize(KeyColor lockColor, KeyColor[,] keyGrid);
        void UpdateKeysCounter(int collected, int required);
        void ShowKeyResult(KeyColor keyColor, bool isCorrect, Vector2Int gridPosition);
        void ShowWinScreen();
        void ShowError(string message);
    }
}