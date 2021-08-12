using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
};

public class Minesweeper : MonoBehaviour
{
    #region Singleton Pattern
    public static Minesweeper instance;

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
    [SerializeField] private Transform gridCanvasTransform;

    // Grid related
    private int bombsQuantity;
    private int gridWidth;
    private int gridHeight;
    private static CustomGrid<MineGridObject> grid;

    // Control of the game state
    enum GameState
    {
        NotStarted,
        Started,
        GameOver, 
        Victory
    }
    private GameState gameState;
    private int currentFlags;
    private int hiddenCells;

    // Cached variables
    [SerializeField] private FlagCounter flagCounter;

    private void Start()
    {
        grid = null;
        gameState = GameState.NotStarted;
        SetDifficulty(Difficulty.Easy);
    }

    private void Update()
    {
        Vector3 position = UtilsClass.GetMouseWorldPosition();

        if (gameState == GameState.NotStarted)
        {
            if (Input.GetMouseButtonDown(0))
                InitializeGrid(position); // if the grid exists and the position is valid, then the grid is initialized and the first cell is opened
        }
        else if (gameState == GameState.Started)
        {
            MineGridObject clickedCell = grid.GetGridObject(position);
            if (clickedCell == null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                OpenCell(clickedCell);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ToggleFlag(clickedCell);
            }
        }
        else // gameState == GameState.GameOver || GameState.Victory
        {

        }
    }

    public void CreateGrid()
    {
        SoundManager.PlaySound("mouse_click");
        // if grid already exists, destroy it before creating another one
        if (grid != null)
        {
            gameState = GameState.NotStarted;
            GridDisplay.instance.DestroyGrid();
        }
        float cellSize = 10f;
        Vector3 gridPosition = gridCanvasTransform.position;

        grid = new CustomGrid<MineGridObject>(gridWidth, gridHeight, cellSize, gridPosition, (CustomGrid<MineGridObject> g, int x, int y) => new MineGridObject(g, x, y));
        GridDisplay.instance.DisplayGrid(grid);

        // initialize variables that control the state of the game
        hiddenCells = gridWidth * gridHeight;
        currentFlags = bombsQuantity;
        flagCounter.UpdateText(currentFlags);
    }

    // Initializes grid making sure it doesn't contain a bomb in the starting cell
    public void InitializeGrid(Vector3 startPos)
    {
        if (grid == null) // the grid has to be created first
            return;

        MineGridObject startingCell = grid.GetGridObject(startPos);
        if (startingCell == null) // not a valid position
            return;
        startingCell.GetXY(out int xStartPos, out int yStartPos); // getting the corresponding indexes of the starting cell

        // Planting the bombs and initializing all other cells
        for (int i = 0; i < bombsQuantity; i++)
        {
            // Get a random position that doesn't have a bomb in order to plant one
            int xPos, yPos;
            bool validPos = false;
            while (!validPos)
            {
                xPos = Random.Range(0, gridWidth);
                yPos = Random.Range(0, gridHeight);
                if (xPos == xStartPos && yPos == yStartPos) // can't plant a bomb in the starting cell
                    continue;
                MineGridObject randomCell = grid.GetGridObject(xPos, yPos);
                if (randomCell.GetCellContent() != CellContent.Bomb) // else -> it will try other position
                {
                    // Plant the bomb
                    randomCell.PlantBomb();
                    // Increment neighbours
                    for (int x = xPos - 1; x <= (xPos + 1); x++)
                    {
                        for (int y = yPos - 1; y <= (yPos + 1); y++)
                        {
                            // this will increment the cell bomb count only if the cell position is a valid one and the cell doesn't contain a bomb
                            grid.GetGridObject(x, y)?.IncrementBombCount();
                        }
                    }
                    // Next iteration
                    validPos = true;
                }
            }
        }
        OpenCell(startingCell);
        gameState = GameState.Started;
    }

    private void OpenCell(MineGridObject cell)
    {
        if (cell.RevealCell()) // if it was revealed, check what is there
        {
            SoundManager.PlaySound("reveal_cell");
            CellContent revealedContent = cell.GetCellContent();
            if (revealedContent == CellContent.Blank)
            {
                cell.GetXY(out int x, out int y);
                OpenAdjacentCells(x, y);
            }
            else if (revealedContent == CellContent.Bomb)
            {
                EndGame();
            }
            hiddenCells--;
            CheckWinCondition();
        }
    }

    private void OpenAdjacentCells(int x, int y)
    {
        // Call the recursive function in all neighbours
        for (int i = x - 1; i <= (x + 1); i++)
        {
            for (int j = y - 1; j <= (y + 1); j++)
            {
                if (i == x && j == y)
                    continue; // don't call it for itself
                OpenAdjacentCellsRecursive(i, j);
            }
        }
    }

    private void OpenAdjacentCellsRecursive(int x, int y)
    {
        MineGridObject currentCell = grid.GetGridObject(x, y);
        if (currentCell == null || !currentCell.IsHidden() || currentCell.IsFlagged())
            return;
        if (currentCell.RevealCell())
            hiddenCells--;
        CellContent revealedContent = currentCell.GetCellContent();
        if (revealedContent != CellContent.Blank)
            return;
        for (int i = x - 1; i <= (x + 1); i++)
        {
            for (int j = y - 1; j <= (y + 1); j++)
            {
                if (i == x && j == y)
                    continue; // don't call it for itself
                OpenAdjacentCellsRecursive(i, j);
            }
        }
    }

    private void ToggleFlag(MineGridObject cell)
    {
        if (cell.ToggleFlag())
        {
            if (cell.IsFlagged())
            {
                currentFlags--;
            }
            else
            {
                currentFlags++;
            }
            flagCounter.UpdateText(currentFlags);
            //if (currentFlags == 0)
                //CheckWinCondition();
        }
    }
    private void CheckWinCondition()
    {
        if (gameState != GameState.GameOver)
        {
            if (hiddenCells == bombsQuantity)
            {
                Debug.Log("You won");
                gameState = GameState.Victory;
                RevealBombs(true);
                MenuManager.instance.GameWon();
            }
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over");
        gameState = GameState.GameOver;
        RevealBombs(false);

        MenuManager.instance.GameOver();
    }

    public void DestroyGrid()
    {
        flagCounter.UpdateText(0);
        if (grid != null)
        {
            gameState = GameState.NotStarted;
            GridDisplay.instance.DestroyGrid();
        }
    }

    // Reveal all bombs if game is either won or lost (the win condition is passed to this function)
    private void RevealBombs(bool victory)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                MineGridObject mineGridObject = grid.GetGridObject(x, y);
                if (mineGridObject.HasBomb())
                    mineGridObject.ForceRevealCell(victory); // reveal cell even if it is flagged and signal that the game has ended
                //grid.GetGridObject(x, y).ForceRevealCell(victory);
            }
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        SoundManager.PlaySound("mouse_click");
        switch (difficulty)
        {
            case Difficulty.Easy:
                bombsQuantity = 10;
                gridHeight = 9;
                gridWidth = 9;
                break;
            case Difficulty.Normal:
                bombsQuantity = 40;
                gridHeight = 16;
                gridWidth = 16;
                break;
            case Difficulty.Hard:
                bombsQuantity = 99;
                gridHeight = 20;
                gridWidth = 24;
                break;
        }
    }
}
