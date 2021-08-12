using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GridDisplay : MonoBehaviour
{
    #region Singleton Pattern
    public static GridDisplay instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    #endregion
    // display images
    [SerializeField] GameObject cellPrefab;
    [SerializeField] private Sprite bombIcon;
    [SerializeField] private Sprite flagIcon;
    [SerializeField] private Color hiddenBackgroundColor;
    [SerializeField] private Color revealedBackgroundColor;
    [SerializeField] private Color victoryBackgroundColor;
    [SerializeField] private Color lossBackgroundColor;
    [SerializeField] private Color[] numberColors = new Color[8];
    [SerializeField] private Image backgroundGridImage;
    [SerializeField] private Color backgroundGridColor;
    private int coconutIndexColor = 0;

    // grid related
    private CustomGrid<MineGridObject> grid;
    private GameObject[,] gridGameObjects;
    private int gridWidth;
    private int gridHeight;

    public void DisplayGrid(CustomGrid<MineGridObject> grid)
    {
        this.grid = grid;
        gridWidth = grid.GetWidth();
        gridHeight = grid.GetHeight();
        grid.OnGridValueChanged += (object sender, OnGridValueChangedEventArgs e) => UpdateGrid(e.x, e.y);

        float cellSize = grid.GetCellSize();
        gridGameObjects = new GameObject[gridWidth, gridHeight];
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Vector3 prefabPosition = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
                gridGameObjects[x, y] = Instantiate(cellPrefab, prefabPosition, Quaternion.identity, this.transform);
                UpdateGrid(x, y);
            }
        }

        // Displaying the background
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);

        backgroundGridImage.color = backgroundGridColor;
    }

    private void UpdateGrid(int x, int y)
    {
        MineGridObject mineGridObject = grid.GetGridObject(x, y);
        GameObject cell = gridGameObjects[x, y];
        Image hiddenBackgroundImage = cell.transform.GetChild(0).GetComponent<Image>();
        Image revealedBackgroundImage = cell.transform.GetChild(1).GetComponent<Image>();
        Image iconImage = cell.transform.GetChild(2).GetComponent<Image>();
        TextMeshProUGUI textMesh = cell.GetComponentInChildren<TextMeshProUGUI>();
        
        if (mineGridObject.IsHidden())
        {
            hiddenBackgroundImage.color = hiddenBackgroundColor;
            revealedBackgroundImage.color = Color.clear;    
            textMesh.text = "";
            if (mineGridObject.IsFlagged())
            {
                iconImage.sprite = flagIcon;
                iconImage.color = numberColors[coconutIndexColor];
                coconutIndexColor = (coconutIndexColor + 1) % 8;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.color = Color.clear;
            }
        }
        else
        {
            hiddenBackgroundImage.color = Color.clear;
            if (mineGridObject.HasFinished() && mineGridObject.HasBomb())
            {
                if (mineGridObject.HasWon())
                    revealedBackgroundImage.color = victoryBackgroundColor;
                else
                    revealedBackgroundImage.color = lossBackgroundColor;
            }
            else
            {
                revealedBackgroundImage.color = revealedBackgroundColor;
            }
            textMesh.text = "";
            if (mineGridObject.HasBomb())
            {
                iconImage.sprite = bombIcon;
                iconImage.color = Color.white;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.color = Color.clear;

                int number = (int)mineGridObject.GetCellContent();
                if (number != 0)
                {
                    textMesh.text = number.ToString();
                    textMesh.color = numberColors[number - 1];
                }
            }
        }
    }
    public void DestroyGrid()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Destroy(gridGameObjects[x, y]);
            }
        }

        backgroundGridImage.color = Color.clear;
    }
}
