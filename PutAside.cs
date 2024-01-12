using UnityEngine;
using UnityEngine.Tilemaps;

public class PutAside : MonoBehaviour
{
    
    public Tilemap tilemap { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    private int posY;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();   
    }

   public void Initialize(TetrominoData data, int posY)
    {
        this.data = data;
        this.posY = posY;

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
    public void ResetData()
    {
        data = new TetrominoData();//remet les data a null
        this.posY = 0;

    if (cells != null)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            tilemap.SetTile(cells[i], null);
        }
        cells = null;
    }

    public int ClearBackPiece()
    {
        tilemap.ClearAllTiles();
        return this.posY;
    }

}
