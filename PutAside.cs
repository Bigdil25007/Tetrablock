using UnityEngine;
using UnityEngine.Tilemaps;

public class PutAside : MonoBehaviour
{
    
    public Tilemap tilemap { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();   
    }

   public void Initialize(TetrominoData data)
    {
        this.data = data;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        int offsetY = (data.tetromino == Tetromino.I ? 1 : 0);

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];

            tilemap.SetTile(cells[i], data.tile);
        }
    }

    public TetrominoData GetBackPiece()
    {
        tilemap.ClearAllTiles();
        
        return data;
    }
}
