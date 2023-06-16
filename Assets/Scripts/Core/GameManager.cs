//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject _pauseMenuCanvas;

    [SerializeField]
    private GameObject _gameOverCanvas;

    [SerializeField]
    private GameObject _winCanvas;

    const string menuName = "Menu";

    private bool b_stopCamera;


    public bool GameOverCheck { get { return b_stopCamera; } private set { } }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Locked;

        EventManager.Subscribe(EventEnum.Pause, Pause);
        EventManager.Subscribe(EventEnum.Resume, Resume);
        EventManager.Subscribe(EventEnum.BackToMenu, BacktoMenu);
        EventManager.Subscribe(EventEnum.Retry, Retry);
        EventManager.Subscribe(EventEnum.GameOver, GameOver);
        EventManager.Subscribe(EventEnum.WinCondition, Win);
    }


    #region PauseMenue

    private void Pause(params object[] parameters)
    {
        Time.timeScale = 0f;
        _pauseMenuCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Resume(params object[] parameters)
    {
        Time.timeScale = 1f;
        _pauseMenuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void BacktoMenu(params object[] parameters)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(menuName);
    }

    #endregion


    #region WIN_LOST_CONDITION

    private void GameOver(params object[] parameters)
    {
        Time.timeScale = 0f;
        b_stopCamera = true;

        _gameOverCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Win(params object[] parameters)
    {
        Time.timeScale = 0f;
        b_stopCamera = true;

        _winCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Retry(params object[] parameters)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    private void OnDestroy()
    {
        EventManager.Unsubscribe(EventEnum.Pause, Pause);
        EventManager.Unsubscribe(EventEnum.Resume, Resume);
        EventManager.Unsubscribe(EventEnum.BackToMenu, BacktoMenu);
        EventManager.Unsubscribe(EventEnum.Retry, Retry);
        EventManager.Unsubscribe(EventEnum.GameOver, GameOver);
        EventManager.Unsubscribe(EventEnum.WinCondition, Win);
    }
}