using ChestPuzzle.Interfaces;
using ChestPuzzle.Models;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace ChestPuzzle.Presenters
{
    public class ChestPuzzlePresenter : IInitializable, IDisposable
    {
        private readonly IChestPuzzleView _view;
        private readonly ChestPuzzleModel _model;
        private readonly IPuzzleGenerator _generator;
        private readonly PuzzleConfig _config;

        public ChestPuzzlePresenter(
            IChestPuzzleView view,
            ChestPuzzleModel model,
            IPuzzleGenerator generator,
            PuzzleConfig config)
        {
            _view = view;
            _model = model;
            _generator = generator;
            _config = config;
        }

        public void Initialize()
        {
            PuzzleActions.ChestTouched += StartPuzzle;
            _view.OnKeyDraggedToLock += HandleKeyDrag;
            _view.OnViewClosed += ClosePuzzle;

            _model.OnKeysCollectedChanged += UpdateKeysCounter;
            _model.OnPuzzleCompleted += HandlePuzzleComplete;
        }

        public void Dispose()
        {
            PuzzleActions.ChestTouched -= StartPuzzle;
            _view.OnKeyDraggedToLock -= HandleKeyDrag;
            _view.OnViewClosed -= ClosePuzzle;

            _model.OnKeysCollectedChanged -= UpdateKeysCounter;
            _model.OnPuzzleCompleted -= HandlePuzzleComplete;
        }

        private void StartPuzzle()
        {
            var lockColor = _generator.GenerateLockColor();
            var keyGrid = _generator.GenerateKeyGrid(_config.GridSize, lockColor, _config.RequiredKeys);

            _model.Initialize(lockColor, keyGrid, _config.RequiredKeys);
            _view.Initialize(lockColor, keyGrid);
            _view.Show();
        }

        private void HandleKeyDrag(KeyColor keyColor, Vector2Int gridPos)
        {
            bool isCorrect = _model.TryCollectKey(keyColor);
            _view.ShowKeyResult(keyColor, isCorrect, gridPos);
        }

        private void UpdateKeysCounter(int collected)
        {
            _view.UpdateKeysCounter(collected, _model.RequiredKeys);
        }

        private void HandlePuzzleComplete(bool success)
        {
            if (success)
            {
                _view.ShowWinScreen();
                _view.

                UniTask.Delay(2000).ContinueWith(() =>
                {
                    PuzzleActions.OnPuzzleSolved();
                });
            }
        }

        private void ClosePuzzle()
        {
            _model.Reset();
            _view.Hide();
        }
    }
}