namespace TheCardGame;

class CardNotFoundException : System.Exception {
    public CardNotFoundException(){}
    public CardNotFoundException(string message) : base(message){}
    public CardNotFoundException(string message, Exception inner) : base(message, inner){}
};

static class Support {
    /*Count the number of cards in the specififed location*/
    
    static public int countCards<T>(List<Card> cards) {
        int cnt = 0;
        foreach(Card card in cards) {
            if (card.State is T) {
                cnt++;
            }
        }
        return cnt;
    }

    static public bool cardIsIn<T>(Card card) {
        return card.State is T;
    }

    static public string CardIdsHumanFormatted<T>(List<Card> cards) {    
        List<string> cardIds = new List<string>();
        foreach(Card card in cards) {
            if (card.State is T) {
                cardIds.Add(card.getId());
            }
        }                
        return String.Join<string>(", ", cardIds);
    }  
        
    static public List<Card> getCardsCanBePlayed(List<Card> cards) {
        List<Card> cardsPlayable = new List<Card>();
        foreach(Card card in cards){
            if (card.State.canBePlayed()){
                cardsPlayable.Add(card);
            }
        }
        return cardsPlayable;
    }

    
    
    /* returns the specified card. Raise CardNotFoundException if card is not there. */
    static public (Card, int) findCard(List<Card> sourceList, string cardId) {
        int iPos = 0;
        Card? cardFound = null;
        foreach(Card card in sourceList) {
            if (card.getId() == cardId) {
                cardFound = card;
                break;
            }
            iPos++;
        }

        if (cardFound == null) {
            throw new CardNotFoundException($"Card with id: '{cardId}' not found");
        }

        return (cardFound, iPos);
    }
    
}