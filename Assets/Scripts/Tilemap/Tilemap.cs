using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tilemap {

    public int width, height;
    public Tile[] tiles;
    public List<TileEntity> entities = new List<TileEntity>();
    public List<SnakeSpawn> spawns = new List<SnakeSpawn>();

    public Tilemap(int width, int height) {
        this.width = width;
        this.height = height;

        this.tiles = new Tile[width * height];
    }

    public void SetTile(Vector2 pos, Tile t) {
        SetTile( Mathf.RoundToInt( pos.x ), Mathf.RoundToInt( pos.y ), t );
    }

    public void SetTile(int x, int y, Tile tile) {
        tiles[width * y + x] = tile;
    }

    public Tile GetTile(int x, int y) {
        return tiles[width * y + x];
    }

    public Tile GetTile(Vector2 pos) {
        return GetTile( Mathf.RoundToInt( pos.x ), Mathf.RoundToInt( pos.y ) );
    }

    public bool IsTileEmpty(Vector2 pos) {
        //Check tile
        if(GetTile( pos ) != null)
            return false;

        //Check entities
        foreach(TileEntity e in entities)
            if(e.pos == pos)
                return false;

        //Check spawns
        foreach(SnakeSpawn spawn in spawns) {
            if(spawn.head == pos)
                return false;

            foreach(Vector2 t in spawn.tail)
                if(t == pos)
                    return false;
        }

        return true;
    }

    public void AddSnakeSpawn(SnakeSpawn spawn) {
        spawns.Add( spawn );
    }

    public SnakeSpawn GetSnakeSpawn(int n) {
        return spawns[n];
    }
}
