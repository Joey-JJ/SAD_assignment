namespace TheCardGame;
class Program
{
    static void setupPlayersAndCards() {
// #TODO: remember to change the following script as it is NOT for the assignment.
// IT is just an example.

        Player player1 = new Player("player1", 10);
        Player player2 = new Player("player2", 10);

        
        DemoGameFactory factory = new DemoGameFactory();

        List<Card> player1_cards = new List<Card>();
        player1_cards.Add(factory.createSpellCard("sorcery-1"));
        player1_cards.Add(factory.createSpellCard("sorcery-2"));       
        player1_cards.Add(factory.createSpellCard("sorcery-3"));                
        player1_cards.Add(factory.createLandCard("land-1"));
        player1_cards.Add(factory.createLandCard("land-2"));
        player1_cards.Add(factory.createCreatureCard("creature-1"));
      

        List<Card> player2_cards = new List<Card>();
        player2_cards.Add(factory.createSpellCard("sorcery-4"));
        player2_cards.Add(factory.createSpellCard("sorcery-5"));
        player2_cards.Add(factory.createSpellCard("sorcery-6"));
        player2_cards.Add(factory.createLandCard("land-3"));
        player2_cards.Add(factory.createLandCard("land-4"));
        player2_cards.Add(factory.createCreatureCard("creature-3"));
        
        player1.setCards(player1_cards);
        player2.setCards(player2_cards);

        GameBoard gb = new GameBoard();
        gb.setPlayers(player1, player2, player1);

    }

    static void setupACurrentSituation() {
        GameBoard gb = new GameBoard();
        gb.setupACurrentSituation();        
    }
    

    static void RunADemoGame() {
        GameBoard gb = new GameBoard();        
        

        //Player 1 - Turn 1                
        if (!gb.newTurn()) {return;}
        gb.drawCard("land-1");        
        gb.endTurn();
        gb.logCurrentSituation();       
        
       

        //Player 2  - Turn 2
        gb.prepareNewTurn();
        if (!gb.newTurn()) {return;}        
        gb.drawCard("land-3");
        gb.endTurn();
        gb.logCurrentSituation();
        
    }
    
    static void Main(string[] args)
    {
       setupPlayersAndCards();
       setupACurrentSituation();
       GameBoard gb = new GameBoard();
       gb.logCurrentSituation();

       RunADemoGame();
    
    }
}
