//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    public void MenuButton() => EventManager.Trigger(EventEnum.BackToMenue);

    public void ResumeButton() => EventManager.Trigger(EventEnum.Resume, false);
    public void RetryButton() => EventManager.Trigger(EventEnum.Retry, false);
}