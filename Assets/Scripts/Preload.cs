using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preload : MonoBehaviour
{
    // Board Setup
    private const int BoardWidth = 20;
    private const int BoardHeight = 10;
    private Board _board;
    private int _currentLevel;
    
    // Levels
    private List<string[]> _levels;
    
    // Imports
    [SerializeField] private GameObject fillerObject;
    [SerializeField] private GameObject wallObject;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private List<TextAsset> levelObjectList;
    
    // Player
    private GameObject _player;
    private PlayerController _playerController;
    private SpriteRenderer _playerSprite;
    private bool _playerIsMoving;
    private (int, int) _moveDirection = (0, 0);
    
    // End Screen Rendering
    private readonly List<int> _moveCounts = new();
    [SerializeField] private GameObject winScreen;
    private WinPanelControl _winControl;

    void Start()
    {
        // Get Win Panel Controls
        _winControl = winScreen.GetComponent<WinPanelControl>();
        _winControl.Init();
        
        // Add Board to Preload
        _board = this.gameObject.AddComponent<Board>();
        
        // Init Player
        _player = playerObject;
        _playerController = this._player.GetComponent<PlayerController>();
        _playerSprite = _player.GetComponent<SpriteRenderer>();
        
        StartBoard();
    }
    private void StartBoard()
    {
        // Init Board
        InitializeBoard();
        
        // Get Next Level
        var nextLevel = LoadLevels(_currentLevel);
        if (!nextLevel)
        {
            Win();
        }
        
        // Init Player Movement
        this._playerIsMoving = false;
        this._moveDirection = (0, 0);
    }

    private void InitializeBoard()
    {
        // Add a new tile for every single tile on screen
        for (var i = 0; i < BoardWidth; i++) 
        {
            for (var j = 0; j < BoardHeight; j++)
            {
                var newTile = Instantiate(fillerObject, new Vector3(i - 9.5f, j - 4.5f, 0), Quaternion.identity);
                newTile.SetActive(false);
                
                // Update changes in board
                _board.InsertTiles(i, j, newTile);
            }
        }
    }

    private bool LoadLevels(int levelIndex)
    {
        // Get Levels
        _levels = LevelLoader.LoadLevels(levelObjectList); 
        
        // Check for next level
        if (levelIndex >= _levels.Count)
        {
            return false;
        }
        
        // Get current level
        var level = _levels[levelIndex];
        for (var i = 0; i < level.Length; i++)
        {
            var str = level[i];
            
            // Load level based on character per line
            for (var j = 0; j < str.Length; j++)
            {
                switch (str[j])
                {
                    case 'W': // Create a wall
                    {
                        var wall = Instantiate(wallObject, new Vector3(i - 9.5f, j - 4.5f, 0), 
                            Quaternion.identity);
                        _board.InsertWall(i, j, wall);
                        break;
                    }
                    case 'P': // Create player character
                    {
                        // NEW
                        this._player.transform.position = new Vector3(i - 9.5f, j - 4.5f, 0);
                        _playerController.SetBoard(_board);
                        _board.InsertPlayer(i, j);
                        
                        // OLD
                        /*
                        this._player = Instantiate(playerObject, new Vector3(i - 9.5f, j - 4.5f, 0), 
                            Quaternion.identity);
                        _playerController = this._player.AddComponent<PlayerController>();
                        _playerController.SetBoard(_board);
                        this._playerSprite = _player.GetComponent<SpriteRenderer>();
                        _board.InsertPlayer(i, j);
                        */
                        break;
                    }
                    case '1':
                    {
                        this._board.InsertFlipped(i, j);
                        break;
                    }
                }
            }
        }
        return true;
    }
    
    void FixedUpdate()
    {
        if (this._board.IsCleared())
        {
            // Change to next level if board is clear
            this._currentLevel += 1;
            this._moveCounts.Add(this._playerController.GetMoveCount());
            ReloadBoard();
            
            // Pause player movement to prevent bleed
            this._playerController.ToggleMove();
            StartCoroutine(PausePlayer());
            return;
        }

        if (!this._playerIsMoving)
        {
            DetectMove();
            return;
        }

        KeepMoving();
    }

    private IEnumerator PausePlayer()
    {
        yield return new WaitForSeconds(0.4f);
        this._playerController.ToggleMove();
    }

    private void DetectMove()
    {
        if (!this._playerController.CanMove()) return;
        var hMoveDir = Input.GetAxis("Horizontal");
        var vMoveDir = Input.GetAxis("Vertical");

        if ((hMoveDir > -.15 && hMoveDir < .15) && (vMoveDir > -.15 && vMoveDir < .15)) return; // Delayed Movement Detection for precision
        //if (hMoveDir == 0 && vMoveDir == 0) return; // Fast Pressing
        
        // Reflect movement on player
        this._playerIsMoving = true;
        this._moveDirection = (hMoveDir < 0 ? -1 : hMoveDir == 0 ? 0 : 1, vMoveDir < 0 ? -1 : vMoveDir == 0 ? 0 : 1);
        
        this._playerController.StartMoving(_moveDirection.Item1, _moveDirection.Item2);
    }

    private void KeepMoving()
    {
        if (!this._playerController.CanMove()) return;
        if (this._playerController.Move()) return;
        
        // Stop player movement when player can't move
        this._playerIsMoving = false;
        this._moveDirection = (0, 0);
        this._playerSprite.color = new Color(1, 1, 1, 1f);
    }

    public void ReloadBoard()
    {
        /* OLD
        Destroy(this._player);
        */
        this._playerController.Reset();
        this._board.Clear();
        StartBoard();
    }

    private void Win()
    {
        // Update Moves
        _winControl.LoadMoves(_moveCounts);
        _winControl.ShowScreen();
        
        _player.SetActive(false);
    }

    public void ResetGame()
    {
        _player.SetActive(true);
        
        // Clear level count
        this._currentLevel = 0;
        
        // Clear move count
        this._moveCounts.Clear();
        
        ReloadBoard();
        
        // Freeze player to prevent bleed
        this._playerController.ToggleMove();
        StartCoroutine(PausePlayer());
    }
}
