using UnityEngine;

namespace TilemapEditor.Modes {
    class PaintTilesMode : ITilemapEditorMode {
        private string currentTile;

        public void Initialize(TilemapEditor editor) {
            editor.ShowSelectionMenu( Resources.LoadAll<Texture2D>( "Sprites/Tiles" ), SetTile );
        }

        private void SetTile(string a) {
            currentTile = a;
        }

        public void LeftClick(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
            if(currentTile != null && tilemap.IsTileEmpty( tilepos )) {
                tilemap.SetTile( tilepos, new Tile( currentTile ) );

                editor.Refresh();
            }
        }

        public void LeftClickDown(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
        }
        public void LeftClickUp(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
        }

        public void RightClick(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
            if(tilemap.GetTile( tilepos ) != null) {
                tilemap.SetTile( tilepos, null );

                editor.Refresh();
            }
        }

        public void RightClickDown(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
        }
        public void RightClickUp(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos) {
        }
    }
}
