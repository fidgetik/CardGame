using System.Collections.Generic;
using UnityEditor.Experimental.U2D;
using UnityEngine;
using UnityEngine.UI;

public class TableController : MonoBehaviour
{
    [SerializeField] private PlayerController _player1, _player2;
    [SerializeField] private Transform[] _borders;
    [SerializeField] private Transform _defencePosition;

    private static TableController _instance;

    private List<CardController> _attacker, _defender, _cards;
    private Hud _hud;

    private bool _checkCard = false;
    private float _speedForSlide = 300f;

    #region Properties

    public Transform[] Borders
    {
        get { return _borders; }
        set { _borders = value; }
    }

    public List<CardController> Attacker
    {
        get { return _attacker; }
        set { _attacker = value; }
    }

    public List<CardController> Defender
    {
        get { return _defender; }
        set { _defender = value; }
    }

    public static TableController Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    public PlayerController Player1
    {
        get { return _player1; }
        set { _player1 = value; }
    }

    public PlayerController Player2
    {
        get { return _player2; }
        set { _player2 = value; }
    }

    public bool CheckCard
    {
        get { return _checkCard; }
        set { _checkCard = value; }
    }

    #endregion

    private void Awake()
    {
        _instance = this;
        _cards = new List<CardController>();
        _attacker = new List<CardController>();
        _defender = new List<CardController>();
    }

    private void Start()
    {
        _hud = Hud.Instance;
    }

    public void SmallerCardOnHands()
    {
        var minimal1 = 0;
        var minimal2 = 0;

        minimal1 = SmallerCardOrNo(_player1.PlayerCards);
        minimal2 = SmallerCardOrNo(_player2.PlayerCards);

        if (minimal1 < minimal2)
        {
            ChangeText(true);
            _player1.IsAttacker = true;
            _player2.IsAttacker = false;
        }
        else
        {
            ChangeText(false);
            _player1.IsAttacker = false;
            _player2.IsAttacker = true;
        }
    }

    private int SmallerCardOrNo(List<GameObject> playerCards)
    {
        var minimal = 8;
        foreach (var card in playerCards)
        {
            var cardController = card.GetComponent<CardController>();

            if (CheckTrump(cardController))
            {
                if (minimal > (int) cardController.CurrentValue)
                    minimal = (int) cardController.CurrentValue;
            }
        }

        return minimal;
    }

    private void Update()
    {
        if (_checkCard)
        {
            CheckGameRule();
            _checkCard = false;
        }
    }

    private void CheckGameRule()
    {
        _cards.Clear();

        foreach (var cardController in GetComponentsInChildren<CardController>())
        {
            _cards.Add(cardController);
        }

        ControlOfMoves();
    }

    //next step doesn't work properly
    private int _tempIndex = 1;

    private void ControlOfMoves()
    {
        if (_cards.Count > 0)
        {
            if (_tempIndex % 2 == 0 && !_player1.IsAttacker)
            {
                _defender.Add(_cards[_tempIndex - 1]);
                _player1.IsAttacker = true;
                _player2.IsAttacker = false;
            }
            else
            {
                _attacker.Add(_cards[_tempIndex - 1]);
                _player1.IsAttacker = false;
                _player2.IsAttacker = true;
            }

            ChangeText(_player1.IsAttacker);
        }

        _tempIndex++;
    }

    private void ChangeText(bool firstPlayer)
    {
        if (!_hud) Start();
        _hud.TimeForPlayer(firstPlayer);
        _player1.WaitForYourTurn(firstPlayer);
        _player2.WaitForYourTurn(!firstPlayer);
    }

    public void Defence()
    {
        var cardsGenerator = CardsGenerator.Instance;
        var tableCards = GetComponentsInChildren<CardController>();
        foreach (var card in tableCards)
        {
            card.GetComponent<Rigidbody2D>().velocity = _defencePosition.localPosition * _speedForSlide;
            Destroy(card.gameObject, 1f);
        }

        //make hand cards full (6 cards)
        var howMuch = 6 - _player1.PlayerCards.Count;
        if (howMuch > 0)
        {
            cardsGenerator.GiveCardsForPlayer(_player1, howMuch);
        }

        howMuch = 6 - _player2.PlayerCards.Count;
        if (howMuch > 0)
        {
            cardsGenerator.GiveCardsForPlayer(_player2, howMuch);
        }

        foreach (var t in _defender)
        {
            print("DEFENDER CARD: " + t.CurrentValue + ": " + t.CurrentSuit);
        }

        foreach (var t in _attacker)
        {
            print("ATTACKER CARD: " + t.CurrentValue + ": " + t.CurrentSuit);
        }

        _defender.Clear();
        _attacker.Clear();
        _tempIndex = 1;
        _player1.IsAttacker = !_player1.IsAttacker;
        _player2.IsAttacker = !_player2.IsAttacker;
    }

    public bool CheckSuitOfCards(SuitOfCards attacker, SuitOfCards defender)
    {
        if (attacker == defender) return true;
        else return false;
    }

    public bool CheckCardsValue(CardsValues attacker, CardsValues defender)
    {
        if ((int) attacker < (int) defender) return true;
        else return false;
    }

    public bool CheckTrump(CardController defender)
    {
        if (defender.CurrentSuit == CardsGenerator.Instance.Trump) return true;
        else return false;
    }

    public void TakeCards()
    {
        var playerAttack = Player1.IsAttacker ? _player1 : _player2;
        PlayerTookCards(playerAttack);
        Defence();
        _player1.IsAttacker = _player1.IsAttacker;
    }

    private void PlayerTookCards(PlayerController player)
    {
        var which = player == _player1 ? 0 : 1;
        foreach (var card in _cards)
        {
            player.PlayerCards.Add(card.gameObject);
            _attacker.Remove(card);
            _defender.Add(card);
            card.transform.parent = CardsGenerator.Instance.PlayerCardFolder[which].transform;
        }
    }
}