using UnityEngine;

public class EnemyObject : CellObject
{
    public int Health = 3;

    private int E_CurrentHealth;

    private void Awake()
    {
        GameManager.Instance.GM_turnManager.OnTick += TurnHappened; //her turda d��man�n hareket etmesini sa�lar
    }

    private void OnDestroy()
    {
        GameManager.Instance.GM_turnManager.OnTick -= TurnHappened; //�len d��man�n �al��mas�n� engeller
    }

    public override void Init(Vector2Int coord)  //D��man�n ba�lang�� konumunu ayarlar ve can de�erini ba�lat�r.
    {
        base.Init(coord);
        E_CurrentHealth = Health;
    }

    public override bool PlayerWantsToEnter()
    {
        E_CurrentHealth -= 1;  

        if (E_CurrentHealth <= 0)  //0 veya alt�na d��erse, d��man�n GameObject'i yok edilir.
        {
            Destroy(gameObject);
        }

        return false;  //Oyuncunun bu h�creye girmesine izin verilmez.
    }


    // Hedef cellin(tilenin) ge�erli olup olmad���n�, ge�ilebilir (passable) olup olmad���n� ve i�inde ba�ka bir nesne olup olmad���n� kontrol eder.
    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.Board;
        var targetCell = board.GetCellData(coord);  //data al

        //ge�erli mi, ge�ilebilir mi ve bo� mu kontrol et
        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainedObject != null)
        {
            return false;  //harekete izin verme
        }

        //enemy'i bu cellden sil(sonraki celle gidecek)
        var currentCell = board.GetCellData(C_Cell);
        currentCell.ContainedObject = null;

        //enemy'i burada tekrar olu�tur
        targetCell.ContainedObject = this;
        C_Cell = coord;  // Yeni pozisyonunu ayarla
        transform.position = board.CellToWorld(coord);  //Oyundaki pozisyonu g�ncelle

        return true;  //harekete izin ver
    }

    void TurnHappened()
    {
        //oyuncunun hangi cellde oldu�unu belirle
        var playerCell = GameManager.Instance.Player.P_CellPosition;

        //Oyuncu ile d��man aras�ndaki X ve Y mesafesini hesapla
        int xDist = playerCell.x - C_Cell.x;
        int yDist = playerCell.y - C_Cell.y;

        //Mutlak de�erler al�narak mesafe �l��mleri yap
        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            //player biti�iik h�credeyse sald�r
            GameManager.Instance.ChangeFood(-3);
        }
        else
        {
            //Oyuncuya do�ru hareket et: �nce X, sonra Y ekseni
            if (absXDist > absYDist)
            {
                //X ekseninde hareket etmeye �al��, ba�ar�s�zsa Y ekseninde hareket et
                if (!TryMoveInX(xDist))
                {
                   
                    TryMoveInY(yDist);
                }
            }
            else
            {
                //Y ekseninde hareket etmeye �al��, ba�ar�s�zsa X ekseninde hareket et
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        //x ekseninde yakla�
 
        //player sa�da ise
        if (xDist > 0)
        {
            return MoveTo(C_Cell + Vector2Int.right);
        }

        //player solda ise
        return MoveTo(C_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        //y ekseninde yakla�

        //player �stte(ki tilede) ise
        if (yDist > 0)
        {
            return MoveTo(C_Cell + Vector2Int.up);
        }

        //player a�a��da(ki tilede) ise
        return MoveTo(C_Cell + Vector2Int.down);
    }
}