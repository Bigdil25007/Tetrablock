using UnityEngine;

public class Piece : MonoBehaviour
{
    // References et parametres de base de la piece.
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    // Delais pour les mouvements et le verrouillage de la piece.
    public float stepDelay = 0.1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.1f;

    // Timers pour gerer les delais.
    private float stepTime;
    private float moveTime;
    private float lockTime;

    // Initialisation de la piece avec ses donnees.
    public void Initialize(Board board, TetrominoData pieceData, Vector3Int position)
    {
        this.data = pieceData;
        this.board = board;
        this.position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null)
        {
            cells = new Vector3Int[pieceData.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)pieceData.cells[i];
        }
    }
    private void Update()
    {
        // Mise � jour de la position et de la rotation de la pi�ce � chaque frame.
        board.Clear(this);

        // Nous utilisons une minuterie pour permettre au joueur de faire des ajustements � la pi�ce
        lockTime += Time.deltaTime;

        // Handle rotation
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        // Handle hard drop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        // Allow the player to hold movement keys but only after a move delay
        // so it does not move too fast
        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > stepTime)
        {
            Step();
        }

        board.Set(this);
    }

    private void HandleMoveInputs()
    {
        // chute douce
        if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Move(Vector2Int.down))
            {
                // Mettre a jour le temps de pas pour eviter le double mouvement
                stepTime = Time.time + stepDelay;
            }
        }

        // mouvement lateraux
        if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
    }

    private void Step()
    {
        // Fait descendre la piece d'une ligne a intervalles reguliers.
        stepTime = Time.time + stepDelay;

        // Passez a la ligne suivante
        Move(Vector2Int.down);

        // Une fois que la piece est inactive depuis trop longtemps, elle devient verrouillee
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void HardDrop()
    {
        // Fait tomber la piece directement vers le bas jusqu'a ce qu'elle ne puisse plus bouger.
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private void Lock()
    {
        // Verrouille la pi�ce en place, efface les lignes compl�tes et g�n�re une nouvelle pi�ce.
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece(board.previewPiece.data);
    }

    private bool Move(Vector2Int translation)
    {
        // D�place la pi�ce si la nouvelle position est valide.
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // Enregistrer le mouvement uniquement si la nouvelle position est valide
        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        // Fait tourner la pi�ce si possible, sinon fait des ajustements ("wall kicks").
        int originalRotation = rotationIndex;

        // Faire pivoter toutes les cellules � l�aide d�une matrice de rotation
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Inverser la rotation si les tests �chouent
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        // Applique une matrice de rotation aux cellules de la pi�ce.
        float[] matrix = Data.RotationMatrix;

        // Faire pivoter toutes les cellules � l�aide de la matrice de rotation
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" et "O" sont tourn�s � partir d�un point central d�cal�
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        // Teste les ajustements de rotation pr�s des murs ("wall kicks").
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        // Obtient l'index pour les ajustements de rotation dans le tableau de "wall kicks".
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        // Enroule une valeur autour d'une plage donn�e (pour les rotations, par exemple).
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}