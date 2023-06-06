using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenue : MonoBehaviour
{
    const string menuName = "Menue";

    public void MenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuName);
    }

    public void ResumeButton() => EventManager.Trigger(EventEnum.Resume);
}