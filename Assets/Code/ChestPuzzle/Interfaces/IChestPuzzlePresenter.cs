using System;
using Unity.VisualScripting;

namespace ChestPuzzle.Interfaces
{
    public interface IChestPuzzlePresenter : IInitializable, IDisposable
    {
        void StartPuzzle();
        void ClosePuzzle();
        void ResetPuzzle();

        bool IsActive { get; }
        int CollectedKeys { get; }
        int RequiredKeys { get; }
    }
}