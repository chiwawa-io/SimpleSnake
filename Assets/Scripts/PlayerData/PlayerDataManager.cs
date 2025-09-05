using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private int _bestScore;
    public int BestScore => _bestScore;
    
    public Action<int, string> OnError;
    
    [SerializeField] private NetworkManager networkManager; 

    public void LoadData()
    {
        networkManager.WebSocketCommandHandler.SendGetUserDataRequestCommand(OnDataLoadSuccess, OnDataLoadError);
    }

    public void CheckAndSaveScore(int score)
    {
        if (score > _bestScore)
        {
            _bestScore = score;
            
            SaveData();
        }
        else
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        networkManager.WebSocketCommandHandler.SendSetUserDataRequestCommand(_bestScore, OnDataSaveSuccess, OnDataSaveError);    
    }

    void OnDataSaveSuccess () {}

    void OnDataLoadSuccess(object response)
    {
        if (response != null) _bestScore = (int)response;
    }

    void OnDataSaveError(int code, string message)
    {
        OnError?.Invoke(code, message);
    }

    void OnDataLoadError(int code, string message)
    {
        _bestScore = 0;
        OnError?.Invoke(code, message);
    }
}
