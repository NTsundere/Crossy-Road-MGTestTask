using ChestPuzzle.Models;

namespace ChestPuzzle.Interfaces
{
    public interface ILockView
    {
        void SetColor(KeyColor color);
        void PlaySuccessAnimation();
    }
}