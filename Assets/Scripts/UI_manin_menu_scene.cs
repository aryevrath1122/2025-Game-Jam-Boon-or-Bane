using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_manin_menu_scene : MonoBehaviour
{
    public int start_scene;

    [Header("All Canvas")]
    [SerializeField] GameObject sceneloading_canvas;
    [SerializeField] GameObject mainmenu_canvas;
    [SerializeField] GameObject level_and_number_of_player_selection_canvas;
    [SerializeField] GameObject setting_canvas;
    [SerializeField] GameObject credit_canvas;

    [Header("setting canvas")]
    [SerializeField] GameObject controlls;
    [SerializeField] GameObject sound;

    [SerializeField] Slider loading_bar;

    [Header("button reference")]
    [SerializeField] GameObject level_selection_button_1;
    [SerializeField] GameObject main_menu_startbutton;
    [SerializeField] GameObject setting_button;
    [SerializeField] GameObject sound_setting_button;
    [SerializeField] GameObject countroller_setting_button;


    public void Start_Game()
    {
        mainmenu_canvas.SetActive(false);
        sceneloading_canvas.SetActive(true);
        StartCoroutine(Load_scene_async());
    }

    IEnumerator Load_scene_async()
    {
        AsyncOperation loadscene = SceneManager.LoadSceneAsync(start_scene);
        while (!loadscene.isDone)
        {
            float progressvalue = Mathf.Clamp01(loadscene.progress / 0.9f);
            loading_bar.value = progressvalue;
            yield return null;
        }
    }

    public void player_and_level_selection()
    {
        mainmenu_canvas.SetActive(false);
        level_and_number_of_player_selection_canvas.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(level_selection_button_1);
    }
    public void selection_back_tomenu()
    {
        mainmenu_canvas.SetActive(true);
        level_and_number_of_player_selection_canvas.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(main_menu_startbutton);
    }
    public void setting()
    {
        mainmenu_canvas.SetActive(false);
        setting_canvas.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(setting_button);
    }
    public void setting_back_to_Mainmenu()
    {
        mainmenu_canvas.SetActive(true);
        setting_canvas.SetActive(false);
    }
    public void credit()
    {
        mainmenu_canvas.SetActive(false);
        credit_canvas.SetActive(true);
    }
    public void credit_back_to_mainmenu()
    {
        mainmenu_canvas.SetActive(true);
        credit_canvas.SetActive(false);
    }
    public void Quit_Game()
    {
        Application.Quit();

    }

    public void controlls_visibility()
    {
        setting_canvas.SetActive(false);
        controlls.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(countroller_setting_button);
    }
    public void sound_changer()
    {
        sound.SetActive(true);
        setting_canvas.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(sound_setting_button);
    }

    public void controlls_back_to_setting()
    {
        setting_canvas.SetActive(true);
        controlls.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(setting_button);
    }

    public void sound_back_to_setting()
    {
        sound.SetActive(false);
        setting_canvas.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(setting_button);
    }

    public void levelselect(int scene_changing_number)
    {
        SceneManager.LoadScene(scene_changing_number);
    }

    public void random_level_select()
    {
        int randomnum = Random.Range(1, 3);
        levelselect(randomnum);
    }

}