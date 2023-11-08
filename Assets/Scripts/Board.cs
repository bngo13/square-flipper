using UnityEngine;

public class Board : MonoBehaviour
{   
    // Board Details
    private int _width;
    private int _height;
    
    // Board Placements
    private GameObject[,] _tileGrid; // Flippable Tiles
    private GameObject[,] _wallGrid; // Wall Tiles
    
    // Player Details
    private (int, int) _player;
    private int _tilesLeft;

    private void Awake()
    {
        // Init Board
        this._width = 20;
        this._height = 10;
        this._tilesLeft = this._width * this._height;

        // Init Board Objects
        this._tileGrid = new GameObject[this._width, this._height];
        this._wallGrid = new GameObject[this._width, this._height];
        
        // Init Player
        this._player = (new int(), new int()); // X, Y
        
    }

    public void InsertTiles(int col, int row, GameObject tile)
    {
        this._tileGrid[col, row] = tile;
    }
    
    public void InsertWall(int col, int row, GameObject tile)
    {
        this._wallGrid[col, row] = tile;
        this._tilesLeft -= 1;
    }

    public void InsertFlipped(int col, int row)
    {
        var tile = this._tileGrid[col, row];
        tile.SetActive(true);
        this._tilesLeft -= 1;
    }

    public void InsertPlayer(int col, int row)
    {
        this._player = (col, row);
    }

    public void FlipTile()
    {
        var tile = this._tileGrid[this._player.Item1, this._player.Item2];
        
        // Calculate Tiles Left
        if (tile.activeSelf) this._tilesLeft += 1;
        else this._tilesLeft -= 1;
        
        // Flip
        tile.SetActive(!tile.activeSelf);
    }

    public bool MovePlayer(int relativeCol, int relativeRow)
    {
        // Stop moving condition
        if (!CanMovePlayer(relativeCol, relativeRow)) return false;
        
        // Keep Moving
        this._player.Item1 += relativeCol;
        this._player.Item2 += relativeRow;
        return true;
    }

    public bool CanMovePlayer(int relativeCol, int relativeRow)
    {
        var potentialCol = this._player.Item1 + relativeCol;
        var potentialRow = this._player.Item2 + relativeRow;

        // Bound Check
        if (potentialCol >= this._width || potentialCol < 0 || potentialRow >= this._height || potentialRow < 0) 
            return false;
        
        // Wall Grid Check
        return !this._wallGrid[potentialCol, potentialRow];
    }

    public bool IsCleared()
    {
        // Win Condition is either all tiles filled or one tile left but player is on that tile
        if (this._tilesLeft <= 0) return true;
        if (this._tilesLeft != 1) return false;
        
        var tile = this._tileGrid[this._player.Item1, this._player.Item2];
        return !tile.activeSelf;
    }

    public void Clear()
    {
        foreach (var tile in this._tileGrid) Destroy(tile);
        foreach (var wall in this._wallGrid) Destroy(wall);
        Awake();
    }
}
