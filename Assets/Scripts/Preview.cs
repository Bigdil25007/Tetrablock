using UnityEngine;
using UnityEngine.Tilemaps;

public class Preview : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }

    private void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
    }

    // Initialisation de la piece avec ses donnees.
    public void Initialize(TetrominoData data)
    {
        this.data = data;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        // On vide la preview avant d'affecter les nouvelles cellules dans la preview
        tilemap.ClearAllTiles();

        int offsetY = (data.tetromino == Tetromino.I ? 1 : 0);

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];

            tilemap.SetTile(cells[i], data.tile);
        }
    }
}
