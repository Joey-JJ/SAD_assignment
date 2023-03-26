namespace TheCardGame;

//TODO: remove singleton
public class GameBoard : PlayerObserver {
      
    private Player player1;
    private Player player2;
    private Player currentTurnPlayer;
    private Player opponentPlayer;

    private int iTurnCnt;
    private bool gameEnded;

    public GameBoard() {              
        
        this.player1 = new Player("dummy1", 0);
        this.player2 = new Player("dummy2", 0);
        this.currentTurnPlayer = this.player1;
        this.opponentPlayer = this.player2;
        this.iTurnCnt = 0;
        this.gameEnded = false;
        
    }


    public void setPlayers(Player player1, Player player2, Player currentTurnPlayer){
        this.player1 = player1;
        this.player2 = player2;
        if (this.player1.getName() == this.player2.getName()) {
            throw new System.InvalidOperationException("The two players should have a unique name.");
        }
        this.currentTurnPlayer = currentTurnPlayer;
        if (currentTurnPlayer.getName() == player1.getName()) {            
            this.opponentPlayer = this.player2;                        
        } else {            
            this.opponentPlayer = this.player1;            
        }

        this.player1.addObserver(this);
        this.player2.addObserver(this);
    }

    public bool takeCard() {
        Player currentTurnPlayer = this.getCurrentTurnPlayer();        
        Card? card = currentTurnPlayer.takeCard();
        if (card == null) {
            System.Console.WriteLine($"{currentTurnPlayer.getName()} could not take card.");
            return false;
        } else {            
            System.Console.WriteLine($"{currentTurnPlayer.getName()} took card {card.getId()} from deck into hand.");           
            return true;
        }
    }

    public bool drawCard(string cardId) {
        Player currentTurnPlayer = this.getCurrentTurnPlayer();        
        Card? card = currentTurnPlayer.drawCard(cardId);
        if (card is null) {
            System.Console.WriteLine($"{currentTurnPlayer.getName()} Didn't draw card {cardId}: Not in his hand.");
            return false;
        } 
                       
        System.Console.WriteLine($"{currentTurnPlayer.getName()} draw card {card.getId()}.");
        return true;
    }

    private void SwapPlayer() {
        if (this.currentTurnPlayer.getName() == this.player1.getName()) {
            this.currentTurnPlayer = this.player2;
            this.opponentPlayer = this.player1;            
        } else {
            this.currentTurnPlayer = this.player1;
            this.opponentPlayer = this.player2;            
        }
    }

    public bool newTurn() {
        if (this.gameEnded) {
            return false;
        }
        this.iTurnCnt++;        
        this.takeCard();
        return true;
    }

    public void endTurn() {        
        
        foreach(Card card in this.currentTurnPlayer.getCards()) {
            card.onEndTurn();           
        }
        foreach(Card card in this.opponentPlayer.getCards()) {
            card.onEndTurn();            
        }        
    }

    public void prepareNewTurn() {
        this.currentTurnPlayer.trimCards(5);
        this.SwapPlayer();
    }

    /* returns the current cards on the board for the specififed player */
    public List<Card> getCardsOnBoard(Player player) {
        List<Card> cards = new List<Card>();
        
        if (player.getName() == this.opponentPlayer.getName()) {            
            return Support.getCardsCanBePlayed(this.opponentPlayer.getCards());
        } else {
            return Support.getCardsCanBePlayed(this.currentTurnPlayer.getCards());
        }
    }

    public Player getCurrentTurnPlayer() {
        return this.currentTurnPlayer;
    }
    public Player getOpponentPlayer() {
        return this.opponentPlayer;
    }
    public int getCurrentTurn() {
        return this.iTurnCnt;
    }

    public bool peformAttack(string cardId, List<string> opponentDefenseCardIds) {
        
        foreach(Card oCard in this.opponentPlayer.getCards()) {
            foreach(string defenseCardId in opponentDefenseCardIds) {
                if (oCard.getId() == defenseCardId) {
                    oCard.goDefending();
                }
            }
        }

        (Card card, int iPos) = Support.findCard(this.currentTurnPlayer.getCards(), cardId);
        CreatureCard? attackCard = card as CreatureCard;
        if(attackCard is not null) {
            attackCard.goAttacking();
            if (this.energyTapped() >= attackCard.getEnergyCost()) {
                attackCard.peformAttack();
                return true;
            }
        }
        return false;                        
    }

    
    
    /* Tap Energry from a land-card currently on the board 
    Returns the energy-level tapped.*/
    public void tapFromCard(string cardId) {        
        foreach(Card card in this.currentTurnPlayer.getCards()) { 
            if (card.getId() == cardId) {
                card.tapEnergy();
            }
        }                   
    }

    public int energyTapped(){
        int iSumEnergy = 0;
        foreach(Card card in this.currentTurnPlayer.getCards()) {            
            LandCard? landCard = card as LandCard;
            if (landCard is not null) {
                iSumEnergy += landCard.givesEnergyLevel();
            }            
        }
        System.Console.WriteLine($"Energy-tapped: {iSumEnergy}");
        return iSumEnergy;
    }

    public override void playerDied(PlayerDiedEvent pde) {
                           
        System.Console.WriteLine($"Player {pde.getPlayerName()} died. Health: {pde.getHealth()}, {pde.getReason()}");
        if (pde.getPlayerName() == this.player1.getName()) {
            System.Console.WriteLine($"Player {this.player2.getName()} is the winner!");
        } else {
            System.Console.WriteLine($"Player {this.player1.getName()} is the winner!");
        }
        this.gameEnded = true;        
    }

    /* These are methods just for Demo stuff */
    public void setupACurrentSituation() {
        
        for(int cnt = 0; cnt < 6; cnt++) {
            this.player1.takeCard();            
        }
        for(int cnt = 0; cnt < 6; cnt++) {
            this.player2.takeCard();            
        }
    }

    public void logCurrentSituation() {
        
        System.Console.WriteLine("==== Current situation");
        System.Console.WriteLine($"Current turn-player: {this.currentTurnPlayer.getName()}, Turn: {this.iTurnCnt}");
        System.Console.WriteLine($"Player {this.player1.getName()}: Health: {this.player1.getHealthValue()}");
        System.Console.WriteLine($"Player {this.player2.getName()}: Health: {this.player2.getHealthValue()}");

        List<Card> cards_player1 = this.player1.getCards();
        System.Console.WriteLine($"Player {this.player1.getName()}: (ontheboard/indeck/inhand/indiscard-pile) {Support.countCards<OnTheBoard>(cards_player1)}/{Support.countCards<InTheDeck>(cards_player1)}/{Support.countCards<InTheHand>(cards_player1)}/{Support.countCards<OnTheDisposedPile>(cards_player1)}");
        System.Console.WriteLine($"Player {this.player1.getName()} on the board: " + Support.CardIdsHumanFormatted<OnTheBoard>(cards_player1));
        System.Console.WriteLine($"Player {this.player1.getName()} in deck: " + Support.CardIdsHumanFormatted<InTheDeck>(cards_player1));        
        System.Console.WriteLine($"Player {this.player1.getName()} in hand: " + Support.CardIdsHumanFormatted<InTheHand>(cards_player1));        
        System.Console.WriteLine($"Player {this.player1.getName()} on the discard-pile: " + Support.CardIdsHumanFormatted<OnTheDisposedPile>(cards_player1));

        List<Card> cards_player2 = this.player2.getCards();
        System.Console.WriteLine($"Player {this.player2.getName()}: (ontheboard/indeck/inhand/indiscard-pile) {Support.countCards<OnTheBoard>(cards_player2)}/{Support.countCards<InTheDeck>(cards_player2)}/{Support.countCards<InTheHand>(cards_player2)}/{Support.countCards<OnTheDisposedPile>(cards_player2)}");
        System.Console.WriteLine($"Player {this.player2.getName()} on the board: " + Support.CardIdsHumanFormatted<OnTheBoard>(cards_player2));
        System.Console.WriteLine($"Player {this.player2.getName()} in deck: " + Support.CardIdsHumanFormatted<InTheDeck>(cards_player2));        
        System.Console.WriteLine($"Player {this.player2.getName()} in hand: " + Support.CardIdsHumanFormatted<InTheHand>(cards_player2));        
        System.Console.WriteLine($"Player {this.player2.getName()} on the discard-pile: " + Support.CardIdsHumanFormatted<OnTheDisposedPile>(cards_player2));
                
        System.Console.WriteLine("==== END Current situation");
    }
}
