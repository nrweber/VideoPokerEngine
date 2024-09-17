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

    public enum HandTypes
    {
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




    public GameState State { get; private set; } = GameState.NewGame;
    public HandTypes HandValue { get; private set; } = HandTypes.None;

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




    private List<CardAndState> _table = new();
    private StandardPokerDeck _deck = default!;




    public JacksOrBetterVideoPoker() 
    {
        //So that the _table always has 5 cards on it. Load up a royal flush with them all on hold.
        _table.Add(new CardAndState(new Card(Suits.Spade, Values.Ace), true));
        _table.Add(new CardAndState(new Card(Suits.Spade, Values.King), true));
        _table.Add(new CardAndState(new Card(Suits.Spade, Values.Queen), true));
        _table.Add(new CardAndState(new Card(Suits.Spade, Values.Jack), true));
        _table.Add(new CardAndState(new Card(Suits.Spade, Values.Ten), true));
        HandValue = HandTypes.RoyalFlush;
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
            HandValue = ScoreHand(Cards);
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
            HandValue = ScoreHand(Cards);
        }
    }


    public void ToggleHold(int position)
    {
        if(GameState.FirstDeal != State)
            return;

        if(position >= 0 && _table.Count > position)
        {
            _table[position] = _table[position] with {
                Hold = !_table[position].Hold
            };
        }
    }


    public static HandTypes ScoreHand(Card[] cards)
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
                return HandTypes.RoyalFlush;
            return HandTypes.StraightFlush;
        }
        else if(isFlush)
        {
            return HandTypes.Flush;
        }
        else if(isStraight)
        {
            return HandTypes.Straight;
        }



        var counts = cards.GroupBy(c => c.Value, c=> c.Suit, (key, cards) => new { Value = key, Count = cards.ToList().Count}).ToList();

        if(counts.Count == 2)
        {
            var containsAFour = 1 == (counts.Where(c => c.Count == 4).ToList().Count);
            if(containsAFour)
                return HandTypes.FourOfAKind;
            return HandTypes.FullHouse;
        }

        if(counts.Count == 3)
        {
            var containsAThree = 1 == (counts.Where(c => c.Count == 3).ToList().Count);
            if(containsAThree)
                return HandTypes.ThreeOfAKind;
            return HandTypes.TwoPair;
        }

        if(counts.Count == 4)
        {
            foreach(var c in counts)
            {
                if(c.Count == 2 && (c.Value > Values.Nine || c.Value == Values.Ace))
                    return HandTypes.JacksOrBetter;
            }
        }

        return HandTypes.None;
    }
}
