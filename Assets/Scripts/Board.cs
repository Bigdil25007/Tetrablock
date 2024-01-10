using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; } // Tilemap pour afficher les pi�ces.
    public Piece activePiece { get; private set; }
    public GameObject preview;
    public Preview previewPiece { get; private set; }
    public GameObject putAside;
    public PutAside AsidePiece { get; private set; }
    public SpriteRenderer grid; // recuperer le sprite renderer du grid pour automatiser la taille du board
    public TetrominoData[] tetrominoes;


    [HideInInspector]
    public Vector2Int boardSize;
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    // Calcul des limites du plateau de jeu.
    public RectInt Bounds   
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    // Initialisation du plateau de jeu.
    private void Awake()
    {
        Vector2 size = grid.size;
        boardSize = new Vector2Int((int)size.x, (int)size.y);

        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        previewPiece = preview.GetComponent<Preview>();
        AsidePiece = putAside.GetComponent<PutAside>();

        // Initialiser les donn�es de chaque t�trimino.
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    // Lancement du jeu.
    private void Start()
    {
        CreatePiece();
        SpawnPiece(previewPiece.data);
    }

    // Methode pour generer une nouvelle piece dans la preview.
    public void CreatePiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        previewPiece.Initialize(data);
    }

    // Methode pour envoyer la piece de la preview dans le jeu
    public void SpawnPiece(TetrominoData piece, bool reserve = false,int posY=8)
    {
        int randomX = Random.Range(boardSize.x - 14, boardSize.x - 7);
        spawnPosition.x = randomX;
        
        spawnPosition.y=posY;

        // On initialise une nouvelle piece en jeu et crée une nouvelle pièce en preview
        activePiece.Initialize(this, piece, spawnPosition);
        
        //Permetr de gérer le cas où on spawn une pièce depuis la reserve
        if (!reserve) 
            CreatePiece();
        

        // Verifier si la position de spawn est valide, sinon terminer le jeu.
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    // Gestion de la fin du jeu.
    public void GameOver()
    {
        tilemap.ClearAllTiles();
    }

    // M�thode pour placer une pi�ce sur le plateau.
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    // M�thode pour retirer une pi�ce du plateau.
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // V�rifier si une position est valide pour placer une pi�ce.
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // V�rifier si la position est hors limites ou occup�e.
            if (!bounds.Contains((Vector2Int)tilePosition) || tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    // M�thode pour v�rifier et effacer les lignes compl�tes.
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    // V�rifier si une ligne est compl�te.
    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    // Effacer une ligne et d�placer les lignes au-dessus vers le bas.
    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }
    void Update(){
        if (Input.GetKeyDown(KeyCode.C) )
        {
            Clear(activePiece);

            if (AsidePiece.data.cells == null) 
            {
                AsidePiece.Initialize(activePiece.data,activePiece.position.y);
                SpawnPiece(previewPiece.data,false,activePiece.position.y);// essaie de faire spawn a la position de la piece actuel pour pas avoir de probleme avec le verif de position
            } else {
                TetrominoData temp = AsidePiece.data;
                int tempPosY = AsidePiece.GetPosY();
                AsidePiece.ClearBackPiece();
                AsidePiece.Initialize(activePiece.data, activePiece.position.y);
                SpawnPiece(temp, true,tempPosY);
                
            }
        }
    }
}
