using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WinPanelControl : MonoBehaviour
{
    [SerializeField] private List<GameObject> texts;

    [SerializeField] private GameObject fullResetObject;

    [SerializeField] private GameObject levelResetObject;

    [SerializeField] private GameObject gameCallback;
    
    private Preload _game;
    private Camera _camera;
    private RestartController _resetObject;
    private readonly List<TextMeshProUGUI> _textList = new();

    // Start is called before the first frame update
    public void Init()
    {
        _camera = Camera.main;
        _game = gameCallback.GetComponent<Preload>();
        _resetObject = levelResetObject.GetComponent<RestartController>();
        
        foreach (var text in texts)
        {
            _textList.Add(text.GetComponent<TextMeshProUGUI>());
        }
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        // Detect mouse click with ray
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction);

        // Hit will never be null since player is limited to what they can see (background exists)
        if (hit.collider.IsUnityNull()) return;
        if (hit.collider.gameObject != fullResetObject.gameObject) return;
        
        _game.ResetGame();
        HideScreen();
    }

    public void LoadMoves(List<int> moveList) 
    {
        // Load move count into respective text entry
        for (var i = 0; i < moveList.Count; i++)
        {
            _textList[i].text = moveList[i].ToString();
        }
    }

    private void HideScreen()
    {
        this.gameObject.SetActive(false);
        _resetObject.ShowButton();
    }

    public void ShowScreen()
    {
        this.gameObject.SetActive(true);
        _resetObject.HideButton();
    }
}
