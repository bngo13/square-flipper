using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RestartController : MonoBehaviour
{
    [SerializeField] private GameObject LevelLoader;

    private Preload _loader;

    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _loader = LevelLoader.GetComponent<Preload>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        // Detect player click using ray
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction);
        
        // Hit will never be null since player is limited to what they can see (background exists)
        if (hit.collider.IsUnityNull()) return;
        if (hit.collider.gameObject != this.gameObject) return;
        
        // Reload board on press
        _loader.ReloadBoard();
    }

    public void HideButton()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowButton()
    {
        this.gameObject.SetActive(true);
    }
}
