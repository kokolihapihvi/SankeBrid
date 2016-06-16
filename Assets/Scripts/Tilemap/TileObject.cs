using UnityEngine;
using System.Collections;

public class TileObject : MonoBehaviour {

    public Tile tile;

    // Use this for initialization
    void Start() {
        Sprite spri = Resources.Load<Sprite>( "Sprites/Tiles/" + tile.graphic );
        gameObject.AddComponent<SpriteRenderer>().sprite = spri;
    }

    // Update is called once per frame
    void Update() {

    }
}
