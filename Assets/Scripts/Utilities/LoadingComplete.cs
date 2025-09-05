using System;
using System.Collections;
using UnityEngine;

public class LoadingComplete : MonoBehaviour
{
    [SerializeField] private int waitTime;
    
    public static Action LoadingCompleteAction;

    private void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    IEnumerator LoadingRoutine()
    {
        yield return new WaitForSeconds(waitTime);
        LoadingCompleteAction?.Invoke();
    }
}
