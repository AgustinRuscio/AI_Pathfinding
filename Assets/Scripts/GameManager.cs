//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject _pauseMenu;

    [SerializeField]
    private GameObject _gameOver;

    const string menuName = "Menue";

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Locked;

        EventManager.Subscribe(EventEnum.Pause, Pause);
        EventManager.Subscribe(EventEnum.Resume, Resume);
        EventManager.Subscribe(EventEnum.BackToMenue, BacktoMenue);
        EventManager.Subscribe(EventEnum.Retry, Retry);
        EventManager.Subscribe(EventEnum.GameOver, GameOver);
    }


    #region PauseMenue

    private void Pause(params object[] parameters)
    {
        Time.timeScale = 0f;
        _pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Resume(params object[] parameters)
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void BacktoMenue(params object[] parameters)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(menuName);
    }

    #endregion

    private void GameOver(params object[] parameters)
    {
        Time.timeScale = 0f;
        _gameOver.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Retry(params object[] parameters)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(EventEnum.Pause, Pause);
        EventManager.Unsubscribe(EventEnum.Resume, Resume);
        EventManager.Unsubscribe(EventEnum.BackToMenue, BacktoMenue);
        EventManager.Unsubscribe(EventEnum.Retry, Retry);
        EventManager.Unsubscribe(EventEnum.GameOver, GameOver);
    }
}