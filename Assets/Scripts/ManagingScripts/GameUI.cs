using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("States")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject leaderboardScreen;
    
    [Header("Game Time")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI addedScoreText;
    [SerializeField] private List<GameObject> lifeUI;
    
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private TextMeshProUGUI yourScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI timer;    
    
    [Header("Error Panel")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorCodeText;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private GameObject errorPanelButton;
    
    [Space]
    [SerializeField] private PlayerDataManager playerDataManager;

    private int _score;
    
    private bool _isGameOver;

    
    public enum GameState
    {
        Loading,
        MainMenu,
        Game,
        Leaderboard
    }

    private GameState _gameState;

    public static Action GameStart;
    
    void OnEnable()
    {
        GameManager.OnGameOver += GameOver;
        GameManager.OnError += OnError;
        Player.UpdateScore += AddedScoreNumber;
        Player.EventMessages += PrintMessage;
        Player.UpdateLife += UpdateLife;
        InactivityDetector.UpdateTimers += UpdateTimer;
        InactivityDetector.ForceStart += LoadGame;
        LoadingComplete.LoadingCompleteAction += LoadMainMenu;
    }
    
    void OnDisable()
    {
        GameManager.OnGameOver -= GameOver;
        GameManager.OnError -= OnError;
        Player.UpdateScore -= AddedScoreNumber;
        Player.EventMessages -= PrintMessage;
        Player.UpdateLife -= UpdateLife;
        InactivityDetector.UpdateTimers -= UpdateTimer;
        InactivityDetector.ForceStart += LoadGame;
        LoadingComplete.LoadingCompleteAction -= LoadMainMenu;
    }

    public void StateSwitchButtonClick(string action)
    {
        TurnOffEveryScreen();
        
        switch (action)
        {
           case "Loading":
               loadingScreen.SetActive(true);
               _gameState = GameState.Loading;
               break;
           case "MainMenu":
               mainMenuScreen.SetActive(true);
               _gameState = GameState.MainMenu;
               break;
           case "Game":
               gameScreen.SetActive(true);
               _gameState = GameState.Game;
               GameStart?.Invoke();
               break;
           case "Leaderboard":
               leaderboardScreen.SetActive(true);
               _gameState = GameState.Leaderboard;
               break;
           default:
               mainMenuScreen.SetActive(true);
               _gameState = GameState.MainMenu;
               break;
        }
    }

    public void LoadMainMenu()
    {
        TurnOffEveryScreen();
        
        mainMenuScreen.SetActive(true);
        _gameState = GameState.MainMenu;
    }

    private void LoadGame()
    {
        TurnOffEveryScreen();
        
        gameScreen.SetActive(true);
        _gameState = GameState.Game;
    }

    private void UpdateScoreText(int score)
    {
        _score += score;
        scoreText.text = _score.ToString("D10");
    }

    private void UpdateLife(int lives)
    {
        if (lifeUI.Count >0)
        {
            var heart = lifeUI[^1];
            lifeUI.Remove(heart);
            heart.SetActive(false);
        }
    }
    
    private void AddedScoreNumber(int bodyLength, Vector2 position)
    {
        UpdateScoreText(bodyLength*100);
        
        addedScoreText.gameObject.SetActive(true);
        addedScoreText.gameObject.transform.position = position;
        
        if (bodyLength>0)
            addedScoreText.text = "+" + (bodyLength*100);
        else
            addedScoreText.text = "" + (bodyLength*100);
        StartCoroutine(WaitAndTurnOff(addedScoreText.gameObject));
    }
    private void PrintMessage(string message, Vector2 position)
    {
        addedScoreText.gameObject.SetActive(true);
        addedScoreText.gameObject.transform.position = position;
        addedScoreText.text = message;
        StartCoroutine(WaitAndTurnOff(addedScoreText.gameObject));
    }
    
    private void GameOver(int score)
    {
        gameOverText.SetActive(true);
        StartCoroutine(WaitAndTurnOff(gameOverText, 2, true, score));
        
        _isGameOver = true;
    }

    private void OnError(int code, string message)
    {
        errorPanel.SetActive(true);
        errorPanelButton.GetComponent<Button>().Select();
        errorCodeText.text = code.ToString();
        errorMessageText.text = message;
    }

    private void UpdateTimer(int timeLeft)
    {
      if (_isGameOver) timer.text = timeLeft.ToString();
    }

    private void TurnOffEveryScreen()
    {
        loadingScreen.SetActive(false);
        gameScreen.SetActive(false);
        mainMenuScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
    }
    
    IEnumerator WaitAndTurnOff(GameObject obj, float time = 0.3f, bool reset = false, int score = 0)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);

        if (reset)
        {
            gameOverPanel.SetActive(true);
            restartButton.GetComponent<Button>().Select();
            yourScoreText.text = score.ToString("D10");
            bestScoreText.text = playerDataManager.BestScore.ToString("D10");
        }
    }
}
