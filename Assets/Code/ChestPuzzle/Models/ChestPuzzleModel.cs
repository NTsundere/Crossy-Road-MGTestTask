using System;

namespace ChestPuzzle.Models
{
    public class ChestPuzzleModel
    {
        public KeyColor LockColor { get; private set; }
        public KeyColor[,] KeyGrid { get; private set; }
        public int KeysCollected { get; private set; }
        public int RequiredKeys { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsCompleted { get; private set; }

        public event Action<int> OnKeysCollectedChanged;
        public event Action<bool> OnPuzzleCompleted;

        public void Initialize(KeyColor lockColor, KeyColor[,] keyGrid, int requiredKeys)
        {
            LockColor = lockColor;
            KeyGrid = keyGrid;
            RequiredKeys = requiredKeys;
            KeysCollected = 0;
            IsActive = true;
            IsCompleted = false;
        }

        public bool TryCollectKey(KeyColor keyColor)
        {
            if (!IsActive || IsCompleted) return false;

            bool isCorrect = keyColor == LockColor;

            if (isCorrect)
            {
                KeysCollected++;
                OnKeysCollectedChanged?.Invoke(KeysCollected);

                if (KeysCollected >= RequiredKeys)
                {
                    IsCompleted = true;
                    IsActive = false;
                    OnPuzzleCompleted?.Invoke(true);
                }
            }

            return isCorrect;
        }

        public void Reset()
        {
            IsActive = false;
            IsCompleted = false;
            KeysCollected = 0;
        }
    }
}