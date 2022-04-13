using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartReceiver : MonoBehaviour
{
    [SerializeField] UnityEngine.Events.UnityEvent OnGameStart;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuModel.gameStarted += GameStartReceived;
    }

    private void OnDestroy()
    {
        MainMenuModel.gameStarted -= GameStartReceived;
    }

    void GameStartReceived()
    {
        OnGameStart?.Invoke();
    }

    public void CutsceneCompleted()
    {
        SaveManager.SetGameState(1, true);
    }
}
