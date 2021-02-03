using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Menu : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera menuVcam;
    private int scenenumb;
    private bool OnOffMenu;
    [SerializeField] private GameObject menu;

    private void Start()
    {
        scenenumb = SceneManager.GetActiveScene().buildIndex;
        if (scenenumb == 1)
        {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        OnOffMenu = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Resume()
    {
        PauseMenu(false, 1f, 9);
    }

    private void Update()
    {
        if (scenenumb == 1)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (OnOffMenu)
                {
                    PauseMenu(true, 0f, 12);
                    OnOffMenu = false;
                }
                else if (!OnOffMenu)
                {
                    PauseMenu(false, 1f, 9);
                    OnOffMenu = true;
                }
            }
        }
    }

    private void PauseMenu(bool state, float time, int priority)
    {
        menu.SetActive(state);
        Cursor.visible = state;
        if (state) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
        menuVcam.m_Priority = priority;
        Time.timeScale = time;
    }
}
