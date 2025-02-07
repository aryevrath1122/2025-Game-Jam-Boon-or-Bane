using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pause_option : MonoBehaviour
{
    public GameObject pause_canvas;
    public GameObject cnuntroles_panel_canvas;
    public bool paueing;

    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            paueing = true;
        }
        if (paueing)
        {
            pause_canvas.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (!paueing)
        {
            pause_canvas.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void making_pause_false()
    {
        paueing = false;
    }
    public void backtomainmenu(int mainmenunum)
    {
        SceneManager.LoadScene(mainmenunum);
    }
    public void cuntrolspanel()
    {
        pause_canvas.SetActive(false);
        cnuntroles_panel_canvas.SetActive(true);
    }
    public void back_to_mainmenu()
    {
        pause_canvas.SetActive(true);
        cnuntroles_panel_canvas.SetActive(false);
    }
    public void quitting_the_game()
    {
        Application.Quit();
    }
}
