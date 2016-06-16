using System.Collections.Generic;

[System.Serializable]
public class SnakeSpawn {

    public Pos2 head;
    public List<Pos2> tail = new List<Pos2>();

    public SnakeSpawn(Pos2 head) {
        this.head = head;
    }

    public void AddToTail(Pos2 pos) {
        if(!Occupies( pos ))
            tail.Add( pos );
    }

    public bool Occupies(Pos2 pos) {
        if(pos == head)
            return true;

        foreach(Pos2 tailpos in tail)
            if(tailpos == pos)
                return true;

        return false;
    }
}