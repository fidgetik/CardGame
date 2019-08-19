using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField] private Text _infoPlayer1, _infoPlayer2;

    private readonly string [] _templatesText = {"Time for Player1", "Time for Player2", "Your turn", "Defence!", "Abandon defense"};
    
    private static Hud _instance;

    public static Hud Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void YourTurn(bool firstPlayer)
    {
        var temp = firstPlayer ? _infoPlayer1 : _infoPlayer2;
        temp.text = _templatesText[2];
    }

    public void TimeForPlayer(bool firstPlayer)
    {
        if (firstPlayer)
        {
            _infoPlayer2.text = _templatesText[0];
            YourTurn(true);
        }
        else
        {
            _infoPlayer1.text = _templatesText[1];
            YourTurn(false);
        }
    }

    public void Defence()
    {
        _infoPlayer1.text = _infoPlayer2.text = _templatesText[3];
    }

    public void AbandonDefence()
    {
        _infoPlayer1.text = _infoPlayer2.text = _templatesText[4];
    }
    
}
