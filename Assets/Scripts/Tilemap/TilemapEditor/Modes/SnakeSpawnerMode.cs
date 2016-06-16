using System.Collections.Generic;
using UnityEngine;

namespace TilemapEditor.Modes {
    class SnakeSpawnerMode : ITilemapEditorMode {

        private SnakeSpawn spawn;
        private Vector2 lastPos;

        private bool making = false;

        private List<GameObject> vis = new List<GameObject>();

        public void LeftClick(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
            if(!making)
                return;

            if(tilepos != lastPos) {
                if(!spawn.Occupies( tilepos ) && tilemap.IsTileEmpty(tilepos)) {
                    spawn.AddToTail( tilepos );
                    lastPos = tilepos;

                    GameObject cube = GameObject.CreatePrimitive( PrimitiveType.Cube );
                    cube.transform.position = editor.TileToWorld( lastPos );

                    vis.Add( cube );
                }

            }
        }

        public void LeftClickDown(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
            spawn = new SnakeSpawn( tilepos );
            making = true;
            lastPos = tilepos;

            GameObject cube = GameObject.CreatePrimitive( PrimitiveType.Cube );
            cube.transform.position = editor.TileToWorld( tilepos );

            vis.Add( cube );
        }

        public void LeftClickUp(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
            making = false;
            tilemap.AddSnakeSpawn( spawn );

            editor.Refresh();

            foreach(GameObject g in vis) {
                Object.Destroy( g );
            }
        }

        public void RightClick(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {

        }

        public void RightClickDown(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
            foreach(SnakeSpawn s in tilemap.spawns) {
                if(s.head == tilepos) {
                    tilemap.spawns.Remove( s );
                    break;
                }
            }
            editor.Refresh();
        }

        public void RightClickUp(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {

        }

        public void Initialize(TilemapEditor editor) {}
    }
}
