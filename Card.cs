namespace TheCardGame;


public interface IColour {
    public abstract string getName();
}


public abstract class Card {

    private int energyCost = 0; // The amount of energy required to play this card.   
    private string description; 
    private string cardId; /* The unique id of this card in the game. */
    private CardState theState;
    
    public Card(string cardId) {        
        this.cardId = cardId;        
        this.description = string.Empty;
        this.theState = new InTheDeck(this);
    }

    public CardState State
    {
        get { return theState; }
        set { theState = value; }
    }

    public string getId() {
        return this.cardId;
    }

    public int getEnergyCost(){
        return this.energyCost;
    }

    public void setEnergyCost(int energyCost) {
        this.energyCost = energyCost;
    }

    public virtual int getDefenseValue() { throw new NotImplementedException(); }
    public virtual int subtractDefenseValue(int iAttackValue) { throw new NotImplementedException(); }
    public virtual int getInitialAttackValue() {throw new NotImplementedException(); }
    public virtual int getActualAttackValue() {throw new NotImplementedException(); }
    public virtual int getEnergyLevel() {throw new NotImplementedException();}
    
    public virtual bool dispose() {
        return this.State.dispose();
    }

    public virtual void onEndTurn() {
        this.State.onEndTurn();
    }

    public bool onDraw() {
        return this.State.onDraw();
    }

    public bool onIsTaken() {
        return this.State.onIsTaken();
    }

    public bool isNotYetInTheGame() {
        return this.State.isNotYetInTheGame();
    }

    public virtual void goDefending() {}

    public virtual void goAttacking() {}

    public virtual void peformAttack() {}

    public virtual void tapEnergy() {}

    public virtual int givesEnergyLevel() { 
        return this.State.givesEnergyLevel();
    }

}

public abstract class LandCard : Card
{
    /* Provides the energy to play the other cards */
    private int _energyLevel = 0;
   
    public LandCard(string cardId) : base(cardId)
    {
        
    }

    public override int getEnergyLevel() {
        return this._energyLevel;
    }

    public override void tapEnergy() {
        this.State.tapEnergy();
    }

    
}


public abstract class SpellCard : Card
{

    public SpellCard(string cardId) : base(cardId)
    {
       
    }
}

public abstract class CreatureCard : Card {
    /* Used to attack opponenent (decrease opponent lifePoint) or for defense.
    
    */
    private int initialAttackValue = 0; /* The attackValue defined on this card*/
    private int actualAttackValue = 0; /* The attackValue for this attack after defense cards came into action */
    private int defenseValue = 0;

    public CreatureCard(string cardId) : base(cardId)
    {        

    }

    public void decreaseActualAttackValue(int iNumber) {
        this.actualAttackValue -= iNumber;
    }
    
    public override void goDefending() {
        this.State.goDefending();
    }

    public override void peformAttack() {
        this.State.peformAttack();
    }

    public override void goAttacking()
    {
        this.State.goAttacking();
    }

    public override int subtractDefenseValue(int iAttackValue)
    {
        this.defenseValue = this.defenseValue - iAttackValue;
        return this.defenseValue;
    }

    public override int getInitialAttackValue() {return this.initialAttackValue;}
    public override int getActualAttackValue() {return this.actualAttackValue;}
    public override int getDefenseValue() {return this.defenseValue;}
}

