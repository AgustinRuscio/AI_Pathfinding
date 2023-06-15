using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenue : MonoBehaviour
{
    public void MenuButton() => EventManager.Trigger(EventEnum.BackToMenue);

    public void ResumeButton() => EventManager.Trigger(EventEnum.Resume, false);
}
