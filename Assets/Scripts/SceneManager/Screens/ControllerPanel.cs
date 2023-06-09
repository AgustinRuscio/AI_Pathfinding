//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ControllerPanel : AbstractScreen
{
    [SerializeField]
    private string sceneToLoad;

    public void BTN_Play() => SceneManager.LoadScene(sceneToLoad);   
}