using UnityEngine;

namespace TilemapEditor.Modes {
    public interface ITilemapEditorMode {
        void Initialize(TilemapEditor editor);

        void LeftClick(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos);
        void LeftClickDown(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos);
        void LeftClickUp(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos);

        void RightClick(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos);
        void RightClickDown(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos);
        void RightClickUp(TilemapEditor editor, Tilemap tilemap, Vector2 tilepos);
    }
}
