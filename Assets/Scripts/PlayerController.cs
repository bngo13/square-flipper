using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Visible Player Objects
    private SpriteRenderer _playerSprite;
    private GameObject _w, _a, _s, _d;
    
    // Board Callbacks
    private Board _board;
    private (int, int) _moveDir = (0, 0);
    private bool _isMovable = true;

    // Move Counting
    private int _currentMoveCount;
    void Awake()
    {
        // Get the control scheme around player
        _w = transform.Find("W").gameObject;
        _a = transform.Find("A").gameObject;
        _s = transform.Find("S").gameObject;
        _d = transform.Find("D").gameObject;
        
        // Get the player
        _playerSprite = this.gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetBoard(Board board)
    {
        this._board = board;
    }

    public void StartMoving(int hMove, int vMove)
    {
        // Set move direction
        this._moveDir = (hMove, vMove);
        
        // If you can't move, don't change sprite
        if (!this._board.CanMovePlayer(hMove, vMove)) return;
        
        // Count moves
        _currentMoveCount += 1;
        
        // Modify player sprite
        ToggleHints(false);
        this._playerSprite.color = new Color(1, 1, 1, 0.3f);
    }

    public bool Move()
    {
        if (!this._board.MovePlayer(this._moveDir.Item1, this._moveDir.Item2))
        {
            // If the player can't move, turn back on player sprite and reset movement
            ToggleHints(true);
            this._moveDir = (0, 0);
            return false;
        }
        
        // Keep moving
        this._board.FlipTile();
        this.gameObject.transform.Translate(this._moveDir.Item1, this._moveDir.Item2, 0);
        
        return true;
    }

    public void ToggleMove()
    {
        _isMovable = !_isMovable;
    }

    public int GetMoveCount()
    {
        return this._currentMoveCount;
    }

    private void ToggleHints(bool isActive)
    {
        _w.SetActive(isActive);
        _a.SetActive(isActive);
        _s.SetActive(isActive);
        _d.SetActive(isActive);
    }

    public bool CanMove()
    {
        return _isMovable;
    }

    public void Reset()
    {
        // Reset to preload state
        _board = null;
        _moveDir = (0, 0);
        _isMovable = true;
        _currentMoveCount = 0;
        ToggleHints(true);
        this._playerSprite.color = new Color(1, 1, 1, 1f);
    }
}
