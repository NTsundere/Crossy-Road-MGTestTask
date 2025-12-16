using System;

public interface ISceneLoader
{
    void LoadScene(string name, Action onLoaded = null);
}
