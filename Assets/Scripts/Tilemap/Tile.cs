[System.Serializable]
public class Tile {
    public bool solid = true;
    public string graphic;

    public Tile() {
        graphic = "placeholderwall";
    }

    public Tile(string graphic) {
        this.graphic = graphic;
    }

    public Tile(bool solid, string graphic) {
        this.solid = solid;
        this.graphic = graphic;
    }
}
