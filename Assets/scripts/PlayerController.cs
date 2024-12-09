using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5.0f;

    private bool P_IsMoving;
    private Vector3 P_MoveTarget;
    private bool P_IsGameOver;

    private BoardManager P_Board;       //board managere eri�mek i�in 
    public Vector2Int P_CellPosition;  //cell(tile) pozisyonuna eri�mek i�in

    public void Spawn(BoardManager boardManager, Vector2Int cell)  //playeri board �zerinde belli bir pozisyonda olu�turma
    {
        P_Board = boardManager;
       MoveTo(cell, true);    
    }

    public void MoveTo(Vector2Int cell, bool immediate)  //hareket edilecek cellin pozisyonu
    {
        P_CellPosition = cell;

        if (immediate)
        {
            P_IsMoving = false;
            transform.position = P_Board.CellToWorld(P_CellPosition);
        }
        else
        {
            P_IsMoving = true;
            P_MoveTarget = P_Board.CellToWorld(P_CellPosition);
        }
    }

    public void Init()
    {
        P_IsMoving = false;
        P_IsGameOver = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void GameOver()
    {
        P_IsGameOver = true;
    }


    // Update is called once per frame
    private void Update()
    {
        if (P_IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.NewLevel();
            }

            return;
        }

        if (P_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, P_MoveTarget, MoveSpeed * Time.deltaTime);

            if (transform.position == P_MoveTarget)
            {
                P_IsMoving = false;
                var cellData = P_Board.GetCellData(P_CellPosition);
                if (cellData.ContainedObject != null)
                { cellData.ContainedObject.PlayerEntered(); }
            }
        }
            //hareket kontrol� burada yap�l�yor
            Vector2Int newCellTarget = P_CellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }

        if (hasMoved)
        {
            //yeni pozisyon hareket edilebilirse hareket et
            BoardManager.CellData cellData = P_Board.GetCellData(newCellTarget);

            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.GM_turnManager.Tick();  //tick(tur) olu�tur 

                //cell bo�sa hareket et
                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget, false);
                }

                //player bu h�creye girebilir mi? (engel var m�?)
                else if (cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget, false);
                }
            }
        }
    }
}
