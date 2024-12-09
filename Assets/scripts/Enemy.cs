using UnityEngine;

public class EnemyObject : CellObject
{
    public int Health = 3;

    private int E_CurrentHealth;

    private void Awake()
    {
        GameManager.Instance.GM_turnManager.OnTick += TurnHappened; //her turda düþmanýn hareket etmesini saðlar
    }

    private void OnDestroy()
    {
        GameManager.Instance.GM_turnManager.OnTick -= TurnHappened; //ölen düþmanýn çalýþmasýný engeller
    }

    public override void Init(Vector2Int coord)  //Düþmanýn baþlangýç konumunu ayarlar ve can deðerini baþlatýr.
    {
        base.Init(coord);
        E_CurrentHealth = Health;
    }

    public override bool PlayerWantsToEnter()
    {
        E_CurrentHealth -= 1;  

        if (E_CurrentHealth <= 0)  //0 veya altýna düþerse, düþmanýn GameObject'i yok edilir.
        {
            Destroy(gameObject);
        }

        return false;  //Oyuncunun bu hücreye girmesine izin verilmez.
    }


    // Hedef cellin(tilenin) geçerli olup olmadýðýný, geçilebilir (passable) olup olmadýðýný ve içinde baþka bir nesne olup olmadýðýný kontrol eder.
    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.Board;
        var targetCell = board.GetCellData(coord);  //data al

        //geçerli mi, geçilebilir mi ve boþ mu kontrol et
        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainedObject != null)
        {
            return false;  //harekete izin verme
        }

        //enemy'i bu cellden sil(sonraki celle gidecek)
        var currentCell = board.GetCellData(C_Cell);
        currentCell.ContainedObject = null;

        //enemy'i burada tekrar oluþtur
        targetCell.ContainedObject = this;
        C_Cell = coord;  // Yeni pozisyonunu ayarla
        transform.position = board.CellToWorld(coord);  //Oyundaki pozisyonu güncelle

        return true;  //harekete izin ver
    }

    void TurnHappened()
    {
        //oyuncunun hangi cellde olduðunu belirle
        var playerCell = GameManager.Instance.Player.P_CellPosition;

        //Oyuncu ile düþman arasýndaki X ve Y mesafesini hesapla
        int xDist = playerCell.x - C_Cell.x;
        int yDist = playerCell.y - C_Cell.y;

        //Mutlak deðerler alýnarak mesafe ölçümleri yap
        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            //player bitiþiik hücredeyse saldýr
            GameManager.Instance.ChangeFood(-3);
        }
        else
        {
            //Oyuncuya doðru hareket et: Önce X, sonra Y ekseni
            if (absXDist > absYDist)
            {
                //X ekseninde hareket etmeye çalýþ, baþarýsýzsa Y ekseninde hareket et
                if (!TryMoveInX(xDist))
                {
                   
                    TryMoveInY(yDist);
                }
            }
            else
            {
                //Y ekseninde hareket etmeye çalýþ, baþarýsýzsa X ekseninde hareket et
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        //x ekseninde yaklaþ
 
        //player saðda ise
        if (xDist > 0)
        {
            return MoveTo(C_Cell + Vector2Int.right);
        }

        //player solda ise
        return MoveTo(C_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        //y ekseninde yaklaþ

        //player üstte(ki tilede) ise
        if (yDist > 0)
        {
            return MoveTo(C_Cell + Vector2Int.up);
        }

        //player aþaðýda(ki tilede) ise
        return MoveTo(C_Cell + Vector2Int.down);
    }
}