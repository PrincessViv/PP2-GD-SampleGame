using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonFunctions : MonoBehaviour
{
    public void Resume()
    {
        GManager.instance.StateUnpaused();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GManager.instance.StateUnpaused();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Respawn() 
    {
        GManager.instance.playerScript.RespawnPlayer();
        GManager.instance.StateUnpaused();
    }
}
   

