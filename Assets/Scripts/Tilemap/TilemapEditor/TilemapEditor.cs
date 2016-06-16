using Newtonsoft.Json;
using System;
using TilemapEditor.Modes;
using UnityEngine;
using UnityEngine.UI;

namespace TilemapEditor {
    public class TilemapEditor : MonoBehaviour {

        public string loadLevel;
        public int width, height;

        public GameObject selectionMenu;
        public GameObject selectionContent;

        public GameObject imageButtonPrefab;

        private ITilemapEditorMode mode;

        private Vector2 mousePos;
        private Tilemap tilemap;

        void Start() {
            tilemap = new Tilemap( width, height );

            if(loadLevel != "") {
                LoadLevel( loadLevel );
            }
        }

        public void LoadLevel(string loadLevel) {
            tilemap = JsonConvert.DeserializeObject<Tilemap>( Resources.Load<TextAsset>( "Levels/" + loadLevel ).text );
            width = tilemap.width;
            height = tilemap.height;

            Refresh();
        }

        void Update() {
            //Ignore 20% from the bottom of the screen (toolbar)
            if(Input.mousePosition.y < Screen.height * 0.2f)
                return;

            mousePos = (Camera.main.ScreenToWorldPoint( Input.mousePosition ) - Vector3.one * 0.5f + Vector3.up * (height * 0.5f) + Vector3.right * (width * 0.5f));

            mousePos.x = Mathf.Clamp( Mathf.RoundToInt( mousePos.x ), 0, width - 1 );
            mousePos.y = Mathf.Clamp( Mathf.RoundToInt( mousePos.y ), 0, height - 1 );

            if(mode != null) {
                if(Input.GetMouseButtonDown( 0 )) {
                    mode.LeftClickDown( this, tilemap, mousePos );
                }
                if(Input.GetMouseButton( 0 )) {
                    mode.LeftClick( this, tilemap, mousePos );
                }
                if(Input.GetMouseButtonUp( 0 )) {
                    mode.LeftClickUp( this, tilemap, mousePos );
                }

                if(Input.GetMouseButtonDown( 1 )) {
                    mode.RightClickDown( this, tilemap, mousePos );
                }
                if(Input.GetMouseButton( 1 )) {
                    mode.RightClick( this, tilemap, mousePos );
                }
                if(Input.GetMouseButtonUp( 1 )) {
                    mode.RightClickUp( this, tilemap, mousePos );
                }
            }
        }

        public void SetMode(string mode) {
            HideSelectionMenu();

            switch(mode) {
                case "paint":
                    this.mode = new PaintTilesMode();
                    break;
                case "snake":
                    this.mode = new SnakeSpawnerMode();
                    break;
            }

            this.mode.Initialize( this );
        }

        public void Refresh() {
            //Clear the tilemap
            foreach(Transform t in transform)
                Destroy( t.gameObject );

            GameObject tiles = new GameObject( "Tile Layer" );
            tiles.transform.parent = transform;

            //Create tiles
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    Tile tile = tilemap.GetTile( x, y );

                    if(tile != null) {
                        GameObject newtile = new GameObject( "Tile " + x + "," + y );
                        newtile.transform.SetParent( tiles.transform );
                        newtile.transform.position = TileToWorld( x, y );

                        newtile.AddComponent<TileObject>().tile = tile;
                    }
                }
            }

            //Create entities
            GameObject entities = new GameObject( "Entity Layer" );
            entities.transform.parent = transform;

            //Create snake spawns
            GameObject spawns = new GameObject( "Snake Spawner Layer" );
            spawns.transform.parent = transform;

            foreach(SnakeSpawn s in tilemap.spawns) {
                GameObject cube = GameObject.CreatePrimitive( PrimitiveType.Cube );

                cube.transform.SetParent( spawns.transform );
                cube.transform.localScale = Vector3.one * 0.6f;
                cube.transform.position = TileToWorld( s.head );

                cube.GetComponent<Renderer>().material.color = Color.red;

                foreach(Vector2 t in s.tail) {
                    cube = GameObject.CreatePrimitive( PrimitiveType.Cube );

                    cube.transform.SetParent( spawns.transform );
                    cube.transform.localScale = Vector3.one * 0.6f;
                    cube.transform.position = TileToWorld( t );

                    cube.GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }

        public Vector2 TileToWorld(int x, int y) {
            return Vector2.right * (x - (width / 2)) + Vector2.up * (y - (height / 2)) + Vector2.one / 2;
        }

        public Vector2 TileToWorld(Vector2 pos) {
            return TileToWorld( (int)pos.x, (int)pos.y );
        }

        public Vector2 WorldToTile(Vector2 worldPos) {
            worldPos.x = Mathf.Clamp( Mathf.FloorToInt( worldPos.x + width * 0.5f ), 0, width - 1 );
            worldPos.y = Mathf.Clamp( Mathf.FloorToInt( worldPos.y + height * 0.5f ), 0, height - 1 );

            return worldPos;
        }

        public void OnRenderObject() {
            GL.PushMatrix();

            GL.MultMatrix( transform.localToWorldMatrix );

            GL.Begin( GL.LINES );

            float halfheight = height * 0.5f;
            float halfwidth = width * 0.5f;

            //Transform by offsets
            float heightoffset = halfheight % 1;
            float widthoffset = halfwidth % 1;
            GL.MultMatrix( Matrix4x4.TRS( Vector3.up * heightoffset + Vector3.right * widthoffset, Quaternion.identity, Vector3.one ) );

            //Border
            GL.Color( Color.black );
            GL.Vertex3( -halfwidth, halfheight, 0 );
            GL.Vertex3( halfwidth, halfheight, 0 );

            GL.Color( Color.black );
            GL.Vertex3( halfwidth, halfheight, 0 );
            GL.Vertex3( halfwidth, -halfheight, 0 );

            GL.Color( Color.black );
            GL.Vertex3( halfwidth, -halfheight, 0 );
            GL.Vertex3( -halfwidth, -halfheight, 0 );

            GL.Color( Color.black );
            GL.Vertex3( -halfwidth, -halfheight, 0 );
            GL.Vertex3( -halfwidth, halfheight, 0 );

            //Grid
            GL.Color( Color.white );
            for(int i = 1; i < width; i++) { //Vertical lines
                GL.Color( Color.black );
                GL.Vertex3( -halfwidth + i, -halfheight, 0 );
                GL.Vertex3( -halfwidth + i, halfheight, 0 );
            }

            for(int i = 1; i < height; i++) { //Horizonal lines
                GL.Color( Color.black );
                GL.Vertex3( -halfwidth, -halfheight + i, 0 );
                GL.Vertex3( halfwidth, -halfheight + i, 0 );
            }

            GL.End();


            GL.PopMatrix();
        }

        public void Save() {
            string jsonmap = JsonConvert.SerializeObject( tilemap );
            Debug.Log( jsonmap );
            GUIUtility.systemCopyBuffer = jsonmap;
        }

        public void HideSelectionMenu() {
            selectionMenu.SetActive( false );

            foreach(Transform t in selectionContent.transform) {
                Destroy( t.gameObject );
            }
        }

        public void ShowSelectionMenu(Texture2D[] textures, Action<string> callback) {
            selectionMenu.SetActive( true );

            foreach(Texture2D tex in textures) {
                GameObject imagebutt = Instantiate( imageButtonPrefab );
                imagebutt.name = tex.name;

                imagebutt.transform.SetParent( selectionContent.transform );

                imagebutt.GetComponentInChildren<RawImage>().texture = tex;

                //Add hide menu callback to each button
                imagebutt.GetComponent<Button>().onClick.AddListener( HideSelectionMenu );

                //Call the modes callback
                imagebutt.GetComponent<Button>().onClick.AddListener( () => {
                    callback( imagebutt.name );
                } );
            }
        }
    }
}