using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerDataManager playerDataManager;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject gameComponents;
    
    private int _currentScore;

    public static Action<int> OnGameOver;
    public static Action<int, string> OnError;
    
    private int _errorCode;
    private string _errorMessage;

    private void OnEnable() 
    {
        Player.UpdateScore += UpdateScore;
        GameUI.GameStart += GameStart;
    }

    private void OnDisable()
    {
        Player.UpdateScore -= UpdateScore;
    }

    void GameStart()
    {
        LevelBegin();
        gameComponents.SetActive(true);
    }

    void LevelBegin()
    {
        networkManager.WebSocketCommandHandler.SendLevelBeginRequestCommand(0,OnLevelBeginSuccess, OnLevelBeginError);
    }

    void OnLevelBeginSuccess() {}

    void OnLevelBeginError(int code,string msg)
    {
        OnError?.Invoke(code, msg);
        _errorCode = code;
        _errorMessage = msg;
    }
    
    void UpdateScore(int lengthOfSnake, Vector2 position)
    {
        _currentScore +=  lengthOfSnake*100;
    }

    private void GameOver()
    {
        OnGameOver?.Invoke(_currentScore);
        LevelEnd();
    }
    void LevelEnd()
    {
        networkManager.WebSocketCommandHandler.SendLevelEndRequestCommand(0, _currentScore, OnLevelEndSuccess, OnLevelEndError);
    }

    void OnLevelEndSuccess()
    {
        playerDataManager.CheckAndSaveScore(_currentScore);
    }

    void OnLevelEndError(int code, string msg)
    {
        OnError?.Invoke(code, msg);
        _errorCode = code;
        _errorMessage = msg;
    }
}
