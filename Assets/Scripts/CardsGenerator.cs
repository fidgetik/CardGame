using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab, _cardsFolder;
    [SerializeField] private RectTransform _table;

    [SerializeField] private GameObject[] _playerCardFolder;
    [SerializeField] private Sprite[] _suitSprites;

    private List<GameObject> _deckOfCards;
    private TableController _tableController;
    private GameController _gameController;
    private string _trumpString;

    private Vector2 _maxBorder, _minBorder;
    private Sprite _currentSuitSprite;
    private SuitOfCards _trump;

    private static CardsGenerator _instance;

    private readonly int _maximumCard = 36;
    private readonly int _cardInSuite = 9;
    private float _cardPackOffset = 100f;

    #region Properties

    public static CardsGenerator Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    public SuitOfCards Trump
    {
        get { return _trump; }
        set { _trump = value; }
    }

    public GameObject[] PlayerCardFolder
    {
        get { return _playerCardFolder; }
        set { _playerCardFolder = value; }
    }

    public RectTransform Table
    {
        get { return _table; }
        set { _table = value; }
    }

    public string TrumpString
    {
        get { return _trumpString; }
        set { _trumpString = value; }
    }

    #endregion

    private void Awake()
    {
        _instance = this;
        _gameController = GameController.Instance;

        _deckOfCards = new List<GameObject>();
        GenerateAllPack();
        ShufflePackCards();
        SetTrump();
    }

    private void Start()
    {
        _tableController = TableController.Instance;
        GiveCardsForPlayer(_tableController.Player1, 6);
        GiveCardsForPlayer(_tableController.Player2, 6);
        _tableController.SmallerCardOnHands();
    }

    private GameObject CreateCard(CardsValues value, SuitOfCards suit)
    {
        var card = Instantiate(_cardPrefab);
        card.GetComponentsInChildren<Image>()[1].sprite = _suitSprites[(int) suit];

        var newValue = (int) value > 4 ? value.ToString().Substring(0, 1) : ((int) value + 6).ToString();
        card.GetComponentInChildren<Text>().text = newValue;

        var cardControll = card.GetComponent<CardController>();
        cardControll.CurrentSuit = suit;
        cardControll.CurrentValue = value;

        card.transform.parent = _cardsFolder.transform;
        card.transform.localScale = Vector3.one;
        card.transform.localPosition = new Vector3(-_cardPackOffset, 0f, 0f);
        card.name = value + ": " + suit;

        return card;
    }

    private void GenerateAllPack()
    {
        var counter = 0;
        var typeOfSuite = 0;
        var suiteCounter = 0;

        for (var i = 0; i < _maximumCard - 1; i++)
        {
            if (suiteCounter >= _cardInSuite)
            {
                suiteCounter = 0;
                typeOfSuite++;
                counter = 0;
            }

            _deckOfCards.Add(CreateCard((CardsValues) counter, (SuitOfCards) typeOfSuite));
            counter++;
            suiteCounter++;
        }
    }

    private void ShufflePackCards()
    {
        for (var i = 0; i < _deckOfCards.Count; i++)
        {
            var tempCard = _deckOfCards[i];
            var rand = Random.Range(i, _deckOfCards.Count);

            _deckOfCards[i] = _deckOfCards[rand];
            _deckOfCards[rand] = tempCard;

            ChangeHierarchyIndex(_deckOfCards[i], _deckOfCards[rand]);
        }
    }

    private void ChangeHierarchyIndex(GameObject currentObj, GameObject randomObj)
    {
        var indexRand = randomObj.transform.GetSiblingIndex();
        var indexCur = currentObj.transform.GetSiblingIndex();

        currentObj.transform.SetSiblingIndex(indexRand);
        randomObj.transform.SetSiblingIndex(indexCur);
    }

    public void GiveCardsForPlayer(PlayerController player, int howMuch)
    {
        var lastIndex = _deckOfCards.Count - 1;
        var whichPlayer = player == _tableController.Player1 ? 0 : 1;

        if (lastIndex > 0)
        {
            for (var i = 0; i < howMuch; i++)
            {
                var newIndex = lastIndex - i;
                AddCard(player, newIndex);

                _deckOfCards[newIndex].GetComponent<CardController>().ShowCard(true);
                _deckOfCards[newIndex].transform.parent = _playerCardFolder[whichPlayer].transform;
                _deckOfCards.Remove(_deckOfCards[newIndex]);
            }
        }
    }

    private void AddCard(PlayerController player, int index)
    {
        player.PlayerCards.Add(_deckOfCards[index]);
    }

    private void SetTrump()
    {
        var lastCard = _deckOfCards[0];
        _trumpString += lastCard.name.Remove(0, lastCard.name.IndexOf(' '));

        var lastCardTransform = lastCard.transform;
        lastCardTransform.position = new Vector3(lastCardTransform.position.x + _cardPackOffset * 2,
            lastCardTransform.position.y, 0f);

        var cardController = lastCard.GetComponent<CardController>();
        cardController.EnableToDrag = false;
        cardController.ShowCard(true);
        _trump = lastCard.GetComponent<CardController>().CurrentSuit;
    }
}