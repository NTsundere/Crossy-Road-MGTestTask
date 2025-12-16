using ChestPuzzle.Models;
using System;
using UnityEngine;

namespace ChestPuzzle.Interfaces
{
    public interface IKeyDragHandler
    {
        event Action<KeyColor, Vector2Int, Vector2> OnKeyDragged;
        event Action<KeyColor, Vector2Int> OnKeyDragStarted;
        event Action<KeyColor, Vector2Int> OnKeyDragEnded;

        bool IsDraggingEnabled { get; set; }
        KeyColor GetKeyColorAt(Vector2Int gridPosition);
        void MarkKeyAsUsed(Vector2Int gridPosition);
    }
}