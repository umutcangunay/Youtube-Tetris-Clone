using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static int gridWidth = 10;    // "Grid" defines the area (in units) that tetrominos can move inside.
    public static int gridHeight = 20;
    
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public int scoreOneLine = 40;    // For the scoring system in the game
    public int scoreTwoLines = 100;
    public int scoreThreeLines = 300;
    public int scoreFourLines = 1200;
    private int numberOfRowsThisTurn = 0;

    public Text hudScore;
    public static int currentScore;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnNextTetromino();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        UpdateUI();
    }

    public void UpdateUI()
    {
        hudScore.text = currentScore.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            switch (numberOfRowsThisTurn)
            {
                case 1: ClearedOneLine(); break;
                case 2: ClearedTwoLines(); break;
                case 3: ClearedThreeLines(); break;
                case 4: ClearedFourLines(); break;
            }

            numberOfRowsThisTurn = 0;    // Resets the variable after calculation
        }
    }

    public void ClearedOneLine()
    {
        currentScore += scoreOneLine;
    }

    public void ClearedTwoLines()
    {
        currentScore += scoreTwoLines;
    }

    public void ClearedThreeLines()
    {
        currentScore += scoreThreeLines;
    }

    public void ClearedFourLines()
    {
        currentScore += scoreFourLines;
    }
    
    public bool CheckIsAboveGrid(Tetromino tetromino)    // Checks if a tetromino is above the grid
    {
        for (int x = 0; x < gridWidth; ++x)    // Iterate through x values (row)
        {
            foreach (Transform mino in tetromino.transform)    // Get Transform component for each coordinate of tetromino
            {
                Vector2 pos = Round(mino.position);

                if (pos.y > gridHeight - 1)    // If any y coordinate of a tetromino is above the grid, return true
                {
                    return true;
                }
            }
            
        }

        return false;
    }
    public bool IsFullRowAt(int y)    // Checks if the row at position y is full
    {
        for (int x = 0; x < gridWidth; ++x)    // Iterating through all x positions inside the row
        {
            if (grid[x, y] == null)    // Checks if a spot in the row is empty
            {
                return false;
            }
        }

        numberOfRowsThisTurn += 1;    // Increments the variable for scoring purposes
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }    // Deletes the full row at position y

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }    // Moves the row at position y down by 1 unit

    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }    // Iterates MoveRowDown(int y) for all columns

    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);    // Deletes the current row
                MoveAllRowsDown(y + 1);    // Moves the upper row 1 unit down, replacing the deleted row
                --y;
            }
        }
    }    // Deletes the row

    public void UpdateGrid(Tetromino tetromino)    // Updates grid dimensions whenever the method is called
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (pos.y < gridHeight)
            {
                grid[(int) pos.x, (int) pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int) pos.x, (int) pos.y];
        }
    }

    public void SpawnNextTetromino()    // Spawns the next tetromino when the previous one lands
    {
        GameObject nextTetromino = (GameObject) Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)),
            new Vector2(5.0f, 20.0f), Quaternion.identity);
    }
    
    public bool CheckIsInsideGrid(Vector2 pos)    // Checks if the current tetromino is inside the grid
    {
        return ((int) pos.x >= 0 && (int) pos.x < gridWidth && (int) pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetromino()    // Get random tetromino prefab names
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/TetrominoT";

        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/TetrominoT";
                break;
            case 2:
                randomTetrominoName = "Prefabs/TetrominoLong";
                break;
            case 3:
                randomTetrominoName = "Prefabs/TetrominoS";
                break;
            case 4:
                randomTetrominoName = "Prefabs/TetrominoZ";
                break;
            case 5:
                randomTetrominoName = "Prefabs/TetrominoSquare";
                break;
            case 6:
                randomTetrominoName = "Prefabs/TetrominoL";
                break;
            case 7:
                randomTetrominoName = "Prefabs/TetrominoJ";
                break;
        }
        
        return randomTetrominoName;

    }

    public void GameOver()    // Loads the "GameOver" scene
    {
        SceneManager.LoadScene("GameOver");
    }
}
