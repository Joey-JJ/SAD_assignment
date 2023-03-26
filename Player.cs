namespace TheCardGame;



public class Player  {
    private List<Card> cards;
    private int healthValue;
    private string name = string.Empty;

    private List<PlayerObserver> observers = new List<PlayerObserver>();

    public Player(string name, int initialLife) {        
        this.cards = new List<Card>();        
        this.healthValue = initialLife;        
        this.name = name;
    }

    public void addObserver(PlayerObserver po) {
        this.observers.Add(po);
    }

    public void removeObserver(PlayerObserver po) {
        this.observers.Remove(po);

    }

    public void setCards(List<Card> cards)  {
        this.cards = cards;
    }

    public List<Card> getCards() {
        return this.cards;
    }

    public string getName() {
        return this.name;
    }

    public void decreaseHealthValue(int iValue){
        this.healthValue -= iValue;
        if (this.healthValue <= 0) {
            PlayerDiedEvent pde = new PlayerDiedEvent(this.getName(), this.getHealthValue(), "Health below or is zero");
            foreach(PlayerObserver po in this.observers) {
                po.playerDied(pde);
            }                        
        }
    }    
    public int getHealthValue() {
        return this.healthValue;
    }


    /* Take the first card from his deck and put it in his hand */
    public Card? takeCard(){                        
        foreach(Card card in this.cards) {                                
            if (card.isNotYetInTheGame()){
                if (card.onIsTaken() is true){
                    return card;
                }                
            }
        }

        PlayerDiedEvent pde = new PlayerDiedEvent(this.getName(), this.getHealthValue(), "No more cards in deck");
        foreach(PlayerObserver po in this.observers) {
            po.playerDied(pde);
        }                                                                                        
        return null;
    }


    /* Draw a card from his hand */
    public Card? drawCard(string cardId) {
                
        foreach(Card card in this.cards) {
            if (card.getId() == cardId) {
                if (card.onDraw() is true) {
                    return card;
                }                                                
            }
        }
        return null;               
    }
           
    public void trimCards(int maxCards) {
        
        int cnt = Support.countCards<InTheHand>(this.cards);
        if (cnt <= maxCards) {
            System.Console.WriteLine($"{this.getName()} trimmed 0 cards into discard pile.");
            return;
        }
    
        int cntDisposed = 0;            
        foreach(Card card in this.cards) {
        
            if (Support.cardIsIn<InTheHand>(card)) {
                bool isDisposed = card.dispose();
                if (isDisposed) {
                    System.Console.WriteLine($"Card {card.getId()} is disposed.");
                    cntDisposed++;
                }
            }

            cnt = Support.countCards<InTheHand>(this.cards);
            if (cnt <= maxCards) {                
                break;
            }                                
        } 
        System.Console.WriteLine($"Disposed {cntDisposed} cards");           
    }
}    

