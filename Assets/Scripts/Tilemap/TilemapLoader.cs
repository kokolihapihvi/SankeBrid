using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class TilemapLoader : NetworkBehaviour {

    public GameObject snakePrefab;

    [SyncVar]
    public string level;

    public Tilemap tilemap;

    private int width, height;

    public override void OnStartServer() {
        tilemap = JsonConvert.DeserializeObject<Tilemap>( Resources.Load<TextAsset>( "Levels/" + level ).text );

        CreateTilemap();

        int spawnid = 0;

        //Spawn player snakes
        foreach(NetworkConnection conn in NetworkServer.connections) {
            GameObject snake = Instantiate( snakePrefab );
            snake.GetComponent<SnakeController>().tilemapLoader = this;

            NetworkServer.ReplacePlayerForConnection( conn, snake, 0 );

            snake.GetComponent<SnakeController>().LoadSpawn( tilemap.spawns[spawnid++] );
        }
    }

    public override void OnStartClient() {
        tilemap = JsonConvert.DeserializeObject<Tilemap>( Resources.Load<TextAsset>( "Levels/" + level ).text );

        CreateTilemap();
    }

    private void CreateTilemap() {
        width = tilemap.width;
        height = tilemap.height;

        //Empty any previous tiles
        foreach(Transform t in transform)
            Destroy( t.gameObject );

        //Create new tiles
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                Tile tile = tilemap.GetTile( x, y );

                if(tile != null) {
                    GameObject newtile = new GameObject( "Tile " + x + "," + y );
                    newtile.transform.SetParent( transform );
                    newtile.transform.position = TileToWorld( x, y );

                    newtile.AddComponent<TileObject>().tile = tile;

                    newtile.AddComponent<BoxCollider2D>();
                }
            }
        }
    }

    public Vector2 TileToWorld(int x, int y) {
        //Transform tilemap coordinate to world coordinate
        return Vector2.right * (x - Mathf.Round( width * 0.5f ) + transform.position.x) + Vector2.up * (y - Mathf.Round( height * 0.5f ) + transform.position.y) + Vector2.one * 0.5f;
    }

    internal Vector3 TileToWorld(Vector2 pos) {
        return TileToWorld( (int)pos.x, (int)pos.y );
    }

    public Vector2 WorldToTile(Vector2 worldPos) {
        //Transform world coordinate to tilemap coordinate
        //Round to ints and clamp
        worldPos.x = Mathf.Clamp( Mathf.FloorToInt( worldPos.x + width * 0.5f ), 0, width - 1 );
        worldPos.y = Mathf.Clamp( Mathf.FloorToInt( worldPos.y + height * 0.5f ), 0, height - 1 );

        return worldPos;
    }
}
