namespace TheCardGame;

public abstract class CardState {

    protected Card card;
    
    public CardState(Card card) {
        this.card = card;
    }

    public Card Card
    {
        get { return card; }
        set { card = value; }
    }

    
    public virtual bool onIsTaken() {return false;}
    public virtual bool onDraw() { return false;}
    public virtual void onEndTurn() {}

    public virtual bool isNotYetInTheGame() {return false;}
    public virtual bool isInTheHand() {return false;}
    public virtual bool canBePlayed() {return false;}

    
    public virtual int givesEnergyLevel() {return 0;}    
    public virtual void tapEnergy() {}
    public virtual void goDefending() {}
    public virtual void goAttacking() {}
    public virtual bool peformAttack() { return false;}
    public virtual (bool, int) absorbAttack(int iAttackValue) { return (false, 0); }
    public virtual bool dispose() {return false;}
    
}

public class InTheDeck : CardState {
    
    public InTheDeck(CardState state) : base(state.Card) {        
    }

    public InTheDeck(Card card) : base(card) {        
    }

    public override int givesEnergyLevel() {
        return 0;
    }

    public override bool isNotYetInTheGame()
    {
        return true;
    }

    public override bool onIsTaken()
    {
        this.card.State = new InTheHand(this);
        return true;
    }
}

public class InTheHand : CardState {
    
    public InTheHand(CardState state) : base(state.Card) {        
    }

    public override int givesEnergyLevel() {
        return 0;
    }

    public override bool isInTheHand() {
        return true;
    }

    public override bool dispose() {
        this.card.State = new OnTheDisposedPile(this);
        return true;
    }

    public override bool onDraw() {
        this.card.State = new OnTheBoard(this);
        return true;
    }   
}

public class OnTheBoard : CardState {
    
    public OnTheBoard(CardState state) : base(state.Card) {        
    }

    public override void tapEnergy() {
        this.card.State = new IsTapped(this);
    }

    public override void goDefending() {
        this.card.State = new IsDefending(this);
    }

    public override void goAttacking() {
        this.card.State = new IsAttacking(this);
    }

    public override int givesEnergyLevel() {
        return 0;
    }

    public override bool canBePlayed() {return true;}
}

public class OnTheDisposedPile : CardState {

    public OnTheDisposedPile(CardState state) : base(state.Card) {        
    }

    public override int givesEnergyLevel() {
        return 0;
    }
    
}

public class IsAttacking : CardState {
    public IsAttacking(CardState state) : base(state.Card) {        
    }

    public override int givesEnergyLevel() {
        return 0;
    }

    public override bool peformAttack() {
        System.Console.WriteLine($"{this.card.getId()} Peforms attack.");
                
        CreatureCard? ccard = this.card as CreatureCard;
        if (ccard is not null) {
            bool defended = false;
            int attackValue = this.card.getInitialAttackValue();
            GameBoard gb = new GameBoard();
            foreach(Card card in gb.getOpponentPlayer().getCards()) {
                (bool cardDefended, int attackValueLeft) = card.State.absorbAttack(attackValue);
                if (cardDefended) {
                    defended = true;
                    attackValue = attackValueLeft;
                }
            }
            
            if (!defended) {
                gb.getOpponentPlayer().decreaseHealthValue(attackValue);
            }
                                    
            return true;
        }
        return false;

    }

    public override void onEndTurn() {
        this.card.State = new OnTheBoard(this); 
    }
}

public class IsDefending : CardState {
    public IsDefending(CardState state) : base(state.Card) {        
    }

    public override int givesEnergyLevel() {
        return 0;
    }

    public override void onEndTurn() {
        this.card.State = new OnTheBoard(this);
    }

    public override (bool, int) absorbAttack(int iAttackValue) { 
        int defenseValue = this.card.getDefenseValue();
        int attackValueLeft = iAttackValue - defenseValue;
        int defenseValueLeft = this.card.subtractDefenseValue(iAttackValue);
        System.Console.WriteLine($"Card '{this.card.getId()}' with defense-value {defenseValue} absorbed attack-value {iAttackValue}. Attack value left: {attackValueLeft}");
        if (defenseValueLeft <= 0) {
            this.card.State = new OnTheDisposedPile(this);                                    
        }
        return (true, attackValueLeft);
    }   
}

public class IsTapped : CardState {
    public IsTapped(CardState state) : base(state.Card) {        
    }

    
    public override int givesEnergyLevel() {
        return this.card.getEnergyLevel();
    }

    public override void onEndTurn() {
        this.card.State = new OnTheBoard(this);
    }
}
