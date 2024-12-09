using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile EndTile;

    public override void Init(Vector2Int coord)  //celler �zerinde oynama yapabilmek i�in
    {
        base.Init(coord);
        GameManager.Instance.Board.SetCellTile(coord, EndTile);  //celli exit olarak ayarlar
    }

    public override void PlayerEntered()  //player celle girerse
    {
        GameManager.Instance.NewLevel();  //yeni level olu�tur
        Debug.Log("��k��a ula��ld�");
    }
}
