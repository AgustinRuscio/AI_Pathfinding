using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject _pauseMenu;
    
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        EventManager.Subscribe(EventEnum.Pause, Pause);
        EventManager.Subscribe(EventEnum.Resume, Resume);
    }

    private void Pause(params object[] parameters)
    {
        Time.timeScale = 0f;
        _pauseMenu.SetActive(true);
    }

    private void Resume(params object[] parameters)
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(EventEnum.Pause, Pause);
        EventManager.Unsubscribe(EventEnum.Resume, Resume);
    }
}