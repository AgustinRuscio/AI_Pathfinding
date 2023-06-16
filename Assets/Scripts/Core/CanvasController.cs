//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public void MenuButton() => EventManager.Trigger(EventEnum.BackToMenu);
    public void ResumeButton() => EventManager.Trigger(EventEnum.Resume, false);
    public void RetryButton() => EventManager.Trigger(EventEnum.Retry, false);
}