using UnityEngine;

[System.Serializable]
public class Pos2 {
    public int x, y;

    public Pos2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2(Pos2 p) {
        return new Vector2( p.x, p.y );
    }

    public static implicit operator Pos2(Vector2 v) {
        return new Pos2( (int)v.x, (int)v.y );
    }

    public static implicit operator Vector3(Pos2 p) {
        return new Vector3( p.x, p.y, 0 );
    }
}
