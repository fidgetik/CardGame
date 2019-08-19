using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private List<GameObject> _playerCards;
    private bool _isAttacker;
    
    public List<GameObject> PlayerCards
    {
        get { return _playerCards; }
        set { _playerCards = value; }
    }

    public bool IsAttacker
    {
        get { return _isAttacker; }
        set { _isAttacker = value; }
    }

    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        _playerCards = new List<GameObject>();
        _instance = this;
    }

    public void WaitForYourTurn(bool yourTurn)
    {
        foreach (var card in _playerCards)
        {
            _isAttacker = yourTurn;
            card.GetComponent<CardController>().EnableToDrag = yourTurn;
        }
    }

    public void SendCardFromHand(CardController card)
    {
        _playerCards.Remove(card.gameObject);
    }   
}