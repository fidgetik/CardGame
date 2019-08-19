using UnityEngine;
using UnityEngine.UI;

public enum SuitOfCards
{
    Heart = 0,
    Diamond = 1,
    Spade = 2,
    Club = 3
}

public enum CardsValues
{
    Six = 0,
    Seven = 1,
    Eight = 2,
    Nine = 3,
    Ten = 4,
    Jack = 5,
    Queen = 6,
    King = 7,
    Ace = 8,
}


public class CardController : MonoBehaviour
{
    private static CardController _instance;

    private RectTransform _cardRectTransform;
    private PlayerController _playerController;
    private Vector2 _previousPositon;
    private Rigidbody2D _rigidbody2D;
    private TableController _table;

    private CardsValues _currentValue;
    private SuitOfCards _currentSuit;

    public bool EnableToDrag = true;

    #region Properties

    public CardsValues CurrentValue
    {
        get { return _currentValue; }
        set { _currentValue = value; }
    }

    public SuitOfCards CurrentSuit
    {
        get { return _currentSuit; }
        set { _currentSuit = value; }
    }

    public static CardController Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    #endregion

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _table = TableController.Instance;
        _cardRectTransform = GetComponent<RectTransform>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void BeginDrag()
    {
        _previousPositon = transform.position;
    }

    private void Update()
    {
        //if (GameController.Instance.State == GameState.Pause)
        //{
        //  EnableToDrag = false;
        //}
    }

    public void Drag()
    {
        if (EnableToDrag)
        {
            var tempVector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = (tempVector);
        }
    }

    public void ShowCard(bool showCard)
    {
        GetComponentsInChildren<Image>()[1].enabled = showCard;
        GetComponentInChildren<Text>().enabled = showCard;
    }

    private bool IsCardOnTable()
    {
        var borders = _table.Borders;
        var anchorMax = borders[0].position;
        var anchorMin = borders[1].position;

        var cardX = _cardRectTransform.position.x;
        var cardY = _cardRectTransform.position.y;

        if ((cardX < anchorMax.x && cardY < anchorMax.y) && (cardX > anchorMin.x && cardY > anchorMin.y))
            return true;

        return false;
    }


    private bool IsPossibleDefence()
    {
        print("DEFENCE START");
        if (!_playerController.IsAttacker)
        {
            if (_table && _table.Attacker.Count > 0)
            {
                return true;
            }

            var indexAttacker = _table.Attacker.Count - 1;
            var attacker = indexAttacker > 0 ? _table.Attacker[indexAttacker] : null;
            if (!attacker) return false;

            if (_table.CheckTrump(this))
            {
                print("DEFENDER BY TRUMP!");
                return true;
            }

            if (_table.CheckSuitOfCards(attacker.CurrentSuit, _currentSuit))
            {
                print("SUIT OK!");
                if (_table.CheckCardsValue(attacker.CurrentValue, _currentValue))
                {
                    print("DEFENCE SUCSSESED!");
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsPossibleAttack()
    {
        if (_table && _table.Attacker.Count == 0)
        {
            _playerController.IsAttacker = true;
            return true;
        }

        if (_playerController.IsAttacker)
        {
            _playerController.IsAttacker = true;
            foreach (var attackCard in _table.Attacker)
            {
                if (attackCard.CurrentSuit == _currentSuit || attackCard._currentValue == _currentValue) return true;
            }
        }

        _playerController.IsAttacker = true;
        return true;
    }


    public void SendCardOnTable()
    {
        _playerController = GetComponentInParent<PlayerController>();
        //here should be checker for Defender or Attacker
        print(IsCardOnTable() + "; " + EnableToDrag + "; " + IsPossibleAttack() +"; "+ IsPossibleDefence() +
              "; ATTACKER:  " + _playerController.IsAttacker);

        if (IsCardOnTable() && EnableToDrag && ((IsPossibleAttack() && _playerController.IsAttacker) ||
                                                (IsPossibleDefence() && !_playerController.IsAttacker)))
        {
            _playerController.SendCardFromHand(this);
            _table.CheckCard = true;
            transform.parent = _table.transform;
            transform.localPosition = Vector3.zero;
            EnableToDrag = false;
        }
        else
        {
            transform.position = _previousPositon;
        }
    }
}