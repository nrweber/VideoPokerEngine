using System.Collections.ObjectModel;
using nic_weber.DeckOfCards;

namespace VideoPokerEngine;


public class JacksOrBetterVideoPoker
{

    public record CardAndState(Card Card, bool Hold=false);

    public enum GameState
    {
        NewGame,
        FirstDeal,
        GameOver
    };

    public enum GameResult
    {
        GameNotOver,
        None,
        JacksOrBetter,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    };




    public Card[] Cards 
    { 
        get 
        {
            return _table.Select(x => x.Card).ToArray();
        }
    }

    public bool[] Holds
    {
        get
        {
            return _table.Select(x => x.Hold).ToArray();
        }
    }

    public GameState State { get; private set; } = GameState.NewGame;
    public GameResult Result { get; private set; } = GameResult.GameNotOver;

    private List<CardAndState> _table = new();
    private StandardPokerDeck _deck = default!;


    public JacksOrBetterVideoPoker() 
    { 
    }


    public void Deal()
    {
        if(GameState.NewGame == State || GameState.GameOver == State)
        {
            _deck = new(StartingStates.Standard52);
            _deck.Shuffle();
            _table = new();

            if(_deck.CardsInDeck < 10)
                throw new InvalidDataException($"Deck does not have enough cards in it to start a new game. Count: '{_deck.CardsInDeck}'. Need atleast 10 carads");

            for(int i = 0; i < 5; i++)
                _table.Add(new CardAndState(_deck.Pop()!));

            State = GameState.FirstDeal;
        }
        else if(GameState.FirstDeal == State)
        {
            for(int i = 0; i < _table.Count; i++)
            {
                if(false == _table[i].Hold)
                {
                    _table[i] = new CardAndState(_deck.Pop()!);
                }
            }

            State = GameState.GameOver;
            Result = ScoreHand(Cards);
        }
    }

    public void ToggleHold(int position)
    {
        if(position >= 0 && _table.Count > position)
        {
            _table[position] = _table[position] with {
                Hold = !_table[position].Hold
            };
        }
    }

    public static GameResult ScoreHand(Card[] cards)
    {
        if(cards.Length != 5)
            throw new ArgumentException("need 5 cards");
        
        var values = cards.Select(c => c.Value).ToList();
        values.Sort();

        var isFlush = 1 == (cards.Select(x => x.Suit).ToHashSet().Count);
        var isStraight = (values[0] == values[1]-1 && values[1] == values[2]-1 && values[2] == values[3]-1 && values[3] == values[4]-1)
                            ||
                         (values[0] == Values.Ace && values[1] == Values.Ten && values[2] == Values.Jack && values[3] == Values.Queen && values[4] == Values.King);

        if(isFlush && isStraight)
        {
            if(values[0] == Values.Ace && values[4] == Values.King)
                return GameResult.RoyalFlush;
            return GameResult.StraightFlush;
        }
        else if(isFlush)
        {
            return GameResult.Flush;
        }
        else if(isStraight)
        {
            return GameResult.Straight;
        }



        var counts = cards.GroupBy(c => c.Value, c=> c.Suit, (key, cards) => new { Value = key, Count = cards.ToList().Count}).ToList();

        if(counts.Count == 2)
        {
            var containsAFour = 1 == (counts.Where(c => c.Count == 4).ToList().Count);
            if(containsAFour)
                return GameResult.FourOfAKind;
            return GameResult.FullHouse;
        }

        if(counts.Count == 3)
        {
            var containsAThree = 1 == (counts.Where(c => c.Count == 3).ToList().Count);
            if(containsAThree)
                return GameResult.ThreeOfAKind;
            return GameResult.TwoPair;
        }

        if(counts.Count == 4)
        {
            foreach(var c in counts)
            {
                if(c.Count == 2 && (c.Value > Values.Nine || c.Value == Values.Ace))
                    return GameResult.JacksOrBetter;
            }
        }

        return GameResult.None;
    }
}
