using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I, J, L, O, S, T, Z
}

// Structure pour stocker les données spécifiques à chaque tétrimino.
[System.Serializable] 

public struct TetrominoData
{
    public Tile tile; 
    public Tetromino tetromino; 

    // Propriétés pour accéder aux cellules et aux mouvements de rotation ("wall kicks").
    public Vector2Int[] cells { get; private set; } // Positions des cellules constituant le tétrimino.
    public Vector2Int[,] wallKicks { get; private set; } // Mouvements possibles lors de la rotation près d'un mur.


    public void Initialize()
    {
        // Attribution des cellules et des mouvements de rotation en fonction du type de tétrimino.
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }
}
