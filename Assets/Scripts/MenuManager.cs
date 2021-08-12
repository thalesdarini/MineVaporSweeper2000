using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    #region Singleton Pattern
    public static MenuManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] GameObject interfaceCanvas;
    [SerializeField] GameObject welcomeCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] GameObject victoryCanvas;
    [SerializeField] GameObject coconutCanvas;
    // [SerializeField] GameObject gridCanvas;

    public void Start()
    {
        interfaceCanvas.SetActive(false);
        welcomeCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        victoryCanvas.SetActive(false);
        coconutCanvas.SetActive(false);
    }

    public void ExitWelcomeScreen()
    {
        SoundManager.PlaySound("mouse_click");
        welcomeCanvas.SetActive(false);
        interfaceCanvas.SetActive(true);
        coconutCanvas.SetActive(true);
    }

    public void GameWon()
    {
        SoundManager.PlaySound("victory");
        interfaceCanvas.SetActive(false);
        victoryCanvas.SetActive(true);
    }

    public void GameOver()
    {
        SoundManager.PlaySound("bomb");
        interfaceCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
    }

    public void ReestartGame()
    {
        SoundManager.PlaySound("mouse_click");
        Minesweeper.instance.DestroyGrid();
        interfaceCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        victoryCanvas.SetActive(false);
    }

    public void ExitGame()
    {
        SoundManager.PlaySound("mouse_click");
        Application.Quit();
    }
}
