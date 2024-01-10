using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I, J, L, O, S, T, Z
}

// Structure pour stocker les donn�es sp�cifiques � chaque t�trimino.
[System.Serializable] 

public struct TetrominoData
{
    public Tile tile; 
    public Tetromino tetromino; 

    // Propri�t�s pour acc�der aux cellules et aux mouvements de rotation ("wall kicks").
    public Vector2Int[] cells { get; private set; } // Positions des cellules constituant le t�trimino.
    public Vector2Int[,] wallKicks { get; private set; } // Mouvements possibles lors de la rotation pr�s d'un mur.


    public void Initialize()
    {
        // Attribution des cellules et des mouvements de rotation en fonction du type de t�trimino.
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }
}
