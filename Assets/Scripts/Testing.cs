using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    private CustomGrid<StringGridObject> grid;

    private void Start()
    {
        grid = new CustomGrid<StringGridObject>(8, 4, 10f, new Vector3(-50, -20), (CustomGrid<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));

        grid.OnGridValueChanged += Grid_OnGridValueChanged; // subscribing to the event
    }

    private void Update()
    {
        Vector3 position = UtilsClass.GetMouseWorldPosition();
        
        if (Input.GetKeyDown(KeyCode.A)) { grid.GetGridObject(position).AddLetter("A"); }
        if (Input.GetKeyDown(KeyCode.B)) { grid.GetGridObject(position).AddLetter("B"); }
        if (Input.GetKeyDown(KeyCode.C)) { grid.GetGridObject(position).AddLetter("C"); }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { grid.GetGridObject(position).AddNumber("1"); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { grid.GetGridObject(position).AddNumber("2"); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { grid.GetGridObject(position).AddNumber("3"); }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetGridObject(UtilsClass.GetMouseWorldPosition())?.ToString());
        }
    }

    private void Grid_OnGridValueChanged(object sender, OnGridValueChangedEventArgs e)
    {
        Debug.LogFormat("Value of ({0}, {1}) changed", e.x, e.y);
    }
}

public class StringGridObject
{
    private CustomGrid<StringGridObject> grid;
    private int x;
    private int y;

    private string letters;
    private string numbers;

    public StringGridObject(CustomGrid<StringGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        letters = "";
        numbers = "";
    }
    public void AddLetter(string letter)
    {
        letters += letter;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void AddNumber(string number)
    {
        numbers += number;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return letters + "\n" + numbers;
    }
}
