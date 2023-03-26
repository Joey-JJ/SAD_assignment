namespace TheCardGame;


public class PlayerDiedEvent  {
    private string _playername;
    private int _health;
    private string _reason;
    public PlayerDiedEvent(string playername, int health, string reason){
        this._playername = playername;
        this._health = health;
        this._reason = reason;
    }

    public string getPlayerName() {
        return this._playername;
    }

    public int getHealth() {
        return this._health;
    }
    public string getReason() {
        return this._reason;
    }
}


public abstract class PlayerObserver {
    abstract public void playerDied(PlayerDiedEvent eventInfo);
}