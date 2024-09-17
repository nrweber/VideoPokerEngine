using nic_weber.DeckOfCards;

namespace VideoPokerEngine.Test;

public class JacksOrBetterVideoPokerTest
{
    [Fact]
    public void NewGame_StateIsNewGame_FillerCardsOnTable()
    {
        JacksOrBetterVideoPoker game = new();
       
        //Game state set
        Assert.Equal(JacksOrBetterVideoPoker.GameState.NewGame, game.State);
       
        //Cards is empty
        Assert.Equal(5, game.Cards.Length);
    }

    [Fact]
    public void AfterFirstDeal_FiveCardsOnTable_AllNotOnHold_GameStateIsFirstDeal()
    {
        JacksOrBetterVideoPoker game = new();

        game.Deal();

        // 5 cards delt
        Assert.Equal(5, game.Cards.Length);

        //Game state set
        Assert.Equal(JacksOrBetterVideoPoker.GameState.FirstDeal, game.State);

        //All cards are not on hold
        foreach(var h in game.Holds)
            Assert.False(h);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(100)]
    [InlineData(5)]
    [InlineData(6)]
    public void CallingHoldCardWithIndexBelowZeroOrAboveFourDoesNothing(int index)
    {
        JacksOrBetterVideoPoker game = new();
        game.Deal();

        //All cards are not on hold
        foreach(var h in game.Holds)
            Assert.False(h);

        game.ToggleHold(index);

        //All cards are not on hold
        foreach(var h in game.Holds)
            Assert.False(h);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void AfterDeal_ValidIndexsMarkCardsOnHold(int index)
    {
       JacksOrBetterVideoPoker game = new();
       game.Deal();

       Assert.False(game.Holds[index]);

       game.ToggleHold(index);

       Assert.True(game.Holds[index]);

       game.ToggleHold(index);

       Assert.False(game.Holds[index]);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void OnNewGameOrGameOver_HoldDoesNothing(int index)
    {
       JacksOrBetterVideoPoker game = new();
       //Before first deal
       // Fake hand with all set to Hold
       Assert.True(game.Holds[index]);
       game.ToggleHold(index);
       Assert.True(game.Holds[index]);
      
       //First Deal and hold
       game.Deal();
       Assert.False(game.Holds[index]);
       game.ToggleHold(index);
       Assert.True(game.Holds[index]);

       //Game Over
       game.Deal();
       Assert.True(game.Holds[index]);
       game.ToggleHold(index);
       Assert.True(game.Holds[index]);
    }


    //Yep, this is ugly but I think for this type of application
    // you really need to check everything since it is not a long list 
    // and if you spell it out like this it is easy for a preogrammer 
    // to tell if you checked all the cases.
    [Theory]
    [InlineData(false, false, false, false, false)]
    [InlineData(false, false, false, false, true)]
    [InlineData(false, false, false, true,  false)]
    [InlineData(false, false, false, true,  true)]
    [InlineData(false, false, true,  false, false)]
    [InlineData(false, false, true,  false, true)]
    [InlineData(false, false, true,  true,  false)]
    [InlineData(false, false, true,  true,  true)]
    [InlineData(false, true,  false, false, false)]
    [InlineData(false, true,  false, false, true)]
    [InlineData(false, true,  false, true,  false)]
    [InlineData(false, true,  false, true,  true)]
    [InlineData(false, true,  true,  false, false)]
    [InlineData(false, true,  true,  false, true)]
    [InlineData(false, true,  true,  true,  false)]
    [InlineData(false, true,  true,  true,  true)]
    [InlineData(true,  false, false, false, false)]
    [InlineData(true,  false, false, false, true)]
    [InlineData(true,  false, false, true,  false)]
    [InlineData(true,  false, false, true,  true)]
    [InlineData(true,  false, true,  false, false)]
    [InlineData(true,  false, true,  false, true)]
    [InlineData(true,  false, true,  true,  false)]
    [InlineData(true,  false, true,  true,  true)]
    [InlineData(true,  true,  false, false, false)]
    [InlineData(true,  true,  false, false, true)]
    [InlineData(true,  true,  false, true,  false)]
    [InlineData(true,  true,  false, true,  true)]
    [InlineData(true,  true,  true,  false, false)]
    [InlineData(true,  true,  true,  false, true)]
    [InlineData(true,  true,  true,  true,  false)]
    [InlineData(true,  true,  true,  true,  true)]
    public void AfterSecondDeal_OnlyCardsNotOnHoldAreReplaced(bool holdZero, bool holdOne, bool holdTwo, bool holdThree, bool holdFour)
    {
        JacksOrBetterVideoPoker game = new();
        game.Deal();

        List<Card> firstCards = new();
        foreach (var c in game.Cards)
            firstCards.Add(c);

        if(holdZero) game.ToggleHold(0);
        if(holdOne) game.ToggleHold(1);
        if(holdTwo) game.ToggleHold(2);
        if(holdThree) game.ToggleHold(3);
        if(holdFour) game.ToggleHold(4);

        game.Deal();

        if(holdZero) 
            Assert.Equal(firstCards[0], game.Cards[0]);
        else
            Assert.NotEqual(firstCards[0], game.Cards[0]);

        if(holdOne) 
            Assert.Equal(firstCards[1], game.Cards[1]);
        else
            Assert.NotEqual(firstCards[1], game.Cards[1]);

        if(holdTwo) 
            Assert.Equal(firstCards[2], game.Cards[2]);
        else
            Assert.NotEqual(firstCards[2], game.Cards[2]);

        if(holdThree) 
            Assert.Equal(firstCards[3], game.Cards[3]);
        else
            Assert.NotEqual(firstCards[3], game.Cards[3]);

        if(holdFour) 
            Assert.Equal(firstCards[4], game.Cards[4]);
        else
            Assert.NotEqual(firstCards[4], game.Cards[4]);
    }

    [Fact]
    public void AfterSecondDeal_StateIsNowGameOver()
    {
        JacksOrBetterVideoPoker game = new();
        game.Deal();
        game.Deal();

        Assert.Equal(JacksOrBetterVideoPoker.GameState.GameOver, game.State);
    }



    [Fact]
    public void NewGame_HandValueIsSetToRoyalFlush()
    {
        JacksOrBetterVideoPoker game = new();

        // Dummy hand at the start so that there is always cards ready to be displayed
        // Set to always show a royal flush on startup
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.RoyalFlush, game.HandValue);
    }


    // Fragle test but I couldn't think of a better way to test it.
    // Hopefully within 100 games one of them has something in it
    // so we know the hand is being evaluated after the first deal
    [Fact]
    public void AfterFirstDeal_HandValueIsSetToGameNotOver()
    {
        bool found = false;
        for(int i = 0; i < 100; i++)
        {
            JacksOrBetterVideoPoker game = new();
            game.Deal();
                
            if(game.HandValue != JacksOrBetterVideoPoker.HandTypes.None)
            {
                found = true;
            }
        }

        Assert.True(found);
    }
    
    // Fragle test but I couldn't think of a better way to test it.
    // Hopefully within 100 games one of them has something in it
    // so we know the hand is being evaluated after the first deal
    [Fact]
    public void AfterSecondDeal_HandValueIsSetToGameNotOver()
    {
        bool found = false;
        for(int i = 0; i < 100; i++)
        {
            JacksOrBetterVideoPoker game = new();
            game.Deal();
            game.Deal();
                
            if(game.HandValue != JacksOrBetterVideoPoker.HandTypes.None)
            {
                found = true;
            }
        }

        Assert.True(found);
    }



    //Ok, This test is brital because there is a chance that each deal could be the same as the last deal
    // I play through three games so that the odds of the actually happening is very low.
    [Fact]
    public void DealAfterGameOverDoesAnotherDeal_AndTheCardsAreShuffledEachTime()
    {
        JacksOrBetterVideoPoker game = new();

        //First Game
        Assert.Equal(JacksOrBetterVideoPoker.GameState.NewGame, game.State);
        game.Deal();
        Assert.Equal(JacksOrBetterVideoPoker.GameState.FirstDeal, game.State);
        var deal1 = game.Cards;
        game.Deal();
        Assert.Equal(JacksOrBetterVideoPoker.GameState.GameOver, game.State);

        //Second Game
        game.Deal();
        Assert.Equal(JacksOrBetterVideoPoker.GameState.FirstDeal, game.State);
        var deal2 = game.Cards;
        game.Deal();
        Assert.Equal(JacksOrBetterVideoPoker.GameState.GameOver, game.State);
        
        //Third Game
        game.Deal();
        Assert.Equal(JacksOrBetterVideoPoker.GameState.FirstDeal, game.State);
        var deal3 = game.Cards;
        game.Deal();
        Assert.Equal(JacksOrBetterVideoPoker.GameState.GameOver, game.State);


        //If the first deal equals the second deal
        if( deal1[0] == deal2[0] && deal1[1] == deal2[1] && deal1[2] == deal2[2] && deal1[3] == deal2[3] && deal1[4] == deal2[4])
        {
            //And the second deal equals the third deal
            // we can almost safely say that we are not 
            // shuffling the cards
            if( deal2[0] == deal3[0] && deal2[1] == deal3[1] && deal2[2] == deal3[2] && deal2[3] == deal3[3] && deal2[4] == deal3[4])
            {
                Assert.False(true, "cards not shuffled");
            }
        }
    }






    [Fact]
    public void ScoreHand_ThrowsExceptionIfNotEnoughOrTwoManyCardsAreGiven()
    {
        Assert.Throws<ArgumentException>(() => JacksOrBetterVideoPoker.ScoreHand(new Card[]{}));
        Assert.Throws<ArgumentException>(() => JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace)
                    }));
        Assert.Throws<ArgumentException>(() => JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two)
                    }));
        Assert.Throws<ArgumentException>(() => JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Club, Values.Three)
                    }));
        Assert.Throws<ArgumentException>(() => JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Club, Values.Four)
                    }));
        Assert.Throws<ArgumentException>(() => JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Club, Values.Four),
                    }));
        Assert.Throws<ArgumentException>(() => JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Club, Values.Four),
                        new Card(Suits.Spade, Values.Ace),
                        new Card(Suits.Spade, Values.Two),
                    }));
    }


    [Fact]
    public void ScoreHand_Nothing()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Club, Values.Four),
                        new Card(Suits.Spade, Values.Two)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.None, result);
    }
    
    [Fact]
    public void ScoreHand_NothingPairOfLowCards()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Spade, Values.Ten),
                        new Card(Suits.Club, Values.Jack),
                        new Card(Suits.Diamond, Values.Ten),
                        new Card(Suits.Club, Values.Four)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.None, result);
    }


    [Fact]
    public void ScoreHand_JacksOrBetter()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Club, Values.Four),
                        new Card(Suits.Spade, Values.Ace)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.JacksOrBetter, result);
    }
    
    [Fact]
    public void ScoreHand_TwoPair()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Spade, Values.Ace)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.TwoPair, result);
    }


    [Fact]
    public void ScoreHand_ThreeOfAKind()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ace),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Spade, Values.Two),
                        new Card(Suits.Heart, Values.Two),
                        new Card(Suits.Spade, Values.Five)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.ThreeOfAKind, result);
    }
    
    [Fact]
    public void ScoreHand_FourOfAKind()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Diamond, Values.Two),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Spade, Values.Two),
                        new Card(Suits.Heart, Values.Two),
                        new Card(Suits.Spade, Values.Five)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.FourOfAKind, result);
    }
    
    [Fact]
    public void ScoreHand_FullHouse()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Diamond, Values.Two),
                        new Card(Suits.Club, Values.Two),
                        new Card(Suits.Spade, Values.Two),
                        new Card(Suits.Heart, Values.Five),
                        new Card(Suits.Spade, Values.Five)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.FullHouse, result);
    }

    [Fact]
    public void ScoreHand_Flush()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Diamond, Values.Two),
                        new Card(Suits.Diamond, Values.Three),
                        new Card(Suits.Diamond, Values.Five),
                        new Card(Suits.Diamond, Values.Seven),
                        new Card(Suits.Diamond, Values.Eight)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.Flush, result);
    }

    
    [Fact]
    public void ScoreHand_Straight()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Diamond, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Diamond, Values.Five),
                        new Card(Suits.Club, Values.Four),
                        new Card(Suits.Heart, Values.Six)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.Straight, result);
    }


    [Fact]
    public void ScoreHand_StraightWithAceLow()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Diamond, Values.Two),
                        new Card(Suits.Club, Values.Three),
                        new Card(Suits.Diamond, Values.Five),
                        new Card(Suits.Club, Values.Four),
                        new Card(Suits.Heart, Values.Ace)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.Straight, result);
    }
   

    [Fact]
    public void ScoreHand_StraightWithAceHigh()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Diamond, Values.Ten),
                        new Card(Suits.Club, Values.Queen),
                        new Card(Suits.Diamond, Values.King),
                        new Card(Suits.Club, Values.Jack),
                        new Card(Suits.Heart, Values.Ace)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.Straight, result);
    }

    [Fact]
    public void ScoreHand_StraightFlush()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Spade, Values.Seven),
                        new Card(Suits.Spade, Values.Eight),
                        new Card(Suits.Spade, Values.Five),
                        new Card(Suits.Spade, Values.Four),
                        new Card(Suits.Spade, Values.Six)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.StraightFlush, result);
    }

    [Fact]
    public void ScoreHand_StraightFlushAceLow()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Diamond, Values.Two),
                        new Card(Suits.Diamond, Values.Three),
                        new Card(Suits.Diamond, Values.Five),
                        new Card(Suits.Diamond, Values.Four),
                        new Card(Suits.Diamond, Values.Ace)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.StraightFlush, result);
    }

    [Fact]
    public void ScoreHand_RoyalFlush()
    {
        var result = JacksOrBetterVideoPoker.ScoreHand(new Card[]{
                        new Card(Suits.Club, Values.Ten),
                        new Card(Suits.Club, Values.Queen),
                        new Card(Suits.Club, Values.King),
                        new Card(Suits.Club, Values.Jack),
                        new Card(Suits.Club, Values.Ace)
                    });
        Assert.Equal(JacksOrBetterVideoPoker.HandTypes.RoyalFlush, result);
    }


    // Deck needs atleast 10 cards
}
