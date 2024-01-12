using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board mainBoard;
    public Piece trackingPiece;

    // Tilemap pour afficher le fant�me et les informations de position des cellules.
    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        // Initialisation de la tilemap et allocation de l'espace pour les cellules.
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        // Mise � jour du fant�me 
        Clear();   // Efface le fant�me pr�c�dent.
        Copy();    // Copie la position des cellules de la pi�ce suivie.
        Drop();    // Calcule position du fant�me.
        Set();     // Le place � la nouvelle position.
    }

    private void Clear()
    {
        // Efface les tile du fant�me de la tilemap.
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        // Copie les cellules de la pi�ce suivie dans le fant�me.
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        // Calcule la position la plus basse o� le fant�me peut �tre plac� sans collision.
        Vector3Int position = trackingPiece.position;

        int current = position.y;
        int bottom = -mainBoard.boardSize.y / 2 - 1;

        mainBoard.Clear(trackingPiece);

        // Boucle pour trouver la position la plus basse valide.
        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if (mainBoard.IsValidPosition(trackingPiece.cells, position))
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }

        mainBoard.Set(trackingPiece);
    }

    private void Set()
    {
        // Place le fant�me � la position calcul�e.
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }

}
