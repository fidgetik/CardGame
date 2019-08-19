using UnityEngine;

public enum GameState
{
    Pause,
    Play
}

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    private GameState _state;
    
    public static GameController Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    public GameState State { get; set; }

    private void Awake()
    {
        _instance = this;
        _state = GameState.Play;
    }

    private void Update()
    {
        Time.timeScale = _state == GameState.Pause ? 0f : 1f;
    }
}
