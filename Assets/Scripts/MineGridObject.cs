using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellContent
{
    Bomb = -1,
    Blank,
    One, Two, Three, Four, Five, Six, Seven, Eight
};

public class MineGridObject
{
    // grid related variables
    private CustomGrid<MineGridObject> grid;
    private int x; // gridPosition
    private int y;

    // cell state variables
    private bool hidden;
    private bool flagged;
    private bool won;
    private bool finished;
    private CellContent cellContent;

    public MineGridObject(CustomGrid<MineGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        hidden = true;
        flagged = false;
        won = false;
        finished = false;
        cellContent = CellContent.Blank;
    }

    public void GetXY(out int x, out int y)
    {
        x = this.x;
        y = this.y;
    }

    public CellContent GetCellContent()
    {
        return cellContent;
    }

    public void PlantBomb()
    {
        cellContent = CellContent.Bomb;
    }

    public void IncrementBombCount()
    {
        if (cellContent != CellContent.Bomb)
        {
            cellContent = (CellContent)((int)cellContent + 1);
        }
    }

    public bool IsHidden()
    {
        return hidden;
    }

    public bool IsFlagged()
    {
        return flagged;
    }

    public bool HasWon()
    {
        return won;
    }

    public bool HasFinished()
    {
        return finished;
    }

    public bool HasBomb()
    {
        if (cellContent == CellContent.Bomb)
            return true;
        return false;
    }

    public bool RevealCell()
    {
        if (!flagged && hidden)
        {
            hidden = false;
            grid.TriggerGridObjectChanged(x, y);
            return true;
        }
        else
            return false;
    }

    public void ForceRevealCell(bool victory)
    {
        flagged = false;
        hidden = false;
        finished = true;
        won = victory;
        grid.TriggerGridObjectChanged(x, y);
    }

    public bool ToggleFlag()
    {
        if (hidden)
        {
            SoundManager.PlaySound("flag_cell");
            flagged = !flagged;
            grid.TriggerGridObjectChanged(x, y);
            return true;
        }
        else
            return false;
    }

    public void UnflagCell()
    {
        flagged = false;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        if (flagged)
            return ">";
        if (hidden)
            return "";
        

        switch (cellContent)
        {
            case CellContent.Bomb:
                return "BOMB";
            case CellContent.Blank:
                return "-";
            default:
                return ((int)cellContent).ToString();
        }
    }
}
