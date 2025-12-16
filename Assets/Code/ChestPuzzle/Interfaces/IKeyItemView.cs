using ChestPuzzle.Models;
using System;
using UnityEngine;

namespace ChestPuzzle.Interfaces
{
    public interface IKeyItemView
    {
        event Action<IKeyItemView> OnDragStarted;
        event Action<IKeyItemView, Vector2> OnDragEnded;

        KeyColor Color { get; }
        Vector2Int GridPosition { get; }
        void SetActive(bool active);
        void PlayCollectAnimation();
        void PlayErrorAnimation();
    }
}