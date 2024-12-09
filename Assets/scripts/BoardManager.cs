using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using NUnit.Framework;

public class BoardManager : MonoBehaviour
{

    public class CellData
    {
        public bool Passable; //ge�ilebilir mi ?
        public CellObject ContainedObject;  //�st�nde bir birim var m� ? (kaps�lleme) (*��erebilir*)
    }

    private CellData[,] BM_BoardData; //boarddaki t�m tilelar�n durumunu saklar (gpt sa�olsun)
    private Tilemap BM_Tilemap; //tilemap i�in de�i�ken
    private Grid BM_Grid;

    public int Width;          //x eksenindeki tile say�s� (map geni�li�i)
    public int Height;         //y eksenindeki tile say�s� (map y�ksekli�i)
    public Tile[] GroundTiles; //ground tile arrayi
    public Tile[] WallTiles;   //duvar tile arrayi
    public List<Vector2Int> BM_EmptyCellsList; //bo� celler(tile) i�in liste
    public PlayerController Player; //player i�in referans
    public FoodObject FoodPrefab;   //food i�in referans
    public WallObject WallPrefab;   //wall i�in referans
    public ExitCellObject ExitCellPrefab;  //exit i�in referans
    public EnemyObject EnemyPrefab;   //wall i�in referans


    public void Init()
    {
        //random map olu�turmak i�in y�ntem
        BM_Tilemap = GetComponentInChildren<Tilemap>(); //tilemap ��esine eri�im

        BM_Grid = GetComponentInChildren<Grid>();       //grid ��esine eri�im

        BM_EmptyCellsList = new List<Vector2Int>();     //listeyi olu�tur/ba�lat

        BM_BoardData = new CellData[Width, Height];     //celldata toplamak i�in y�ntem

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                
                Tile tile;
                BM_BoardData[x, y] = new CellData();  //boarddatay� g�ncelleme

                //belirtilen konum kenarlarsa duvar, de�ilse ground olu�tur
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    BM_BoardData[x, y].Passable = false;  //olu�turulan cell'in(tile) ge�ilebilir olma durumunu kontrol etmek i�in
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    BM_BoardData[x, y].Passable = true;  //olu�turulan cell'in(tile) ge�ilebilir olma durumunu kontrol etmek i�in
                    BM_EmptyCellsList.Add(new Vector2Int(x, y));  //bu konumu listeye ekle
                }

                BM_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        BM_EmptyCellsList.Remove(new Vector2Int(1, 1));  //bu konumu(player konumu) listeden sil

        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);  //exitin pozisyonu
        AddObject(Instantiate(ExitCellPrefab), endCoord);  //objenin klonlamas� ve ayarlanmas�
        BM_EmptyCellsList.Remove(endCoord);  //bu konumu listeden sil

        GenerateWall();
        GenerateFood();
        GenerateEnemy();
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)  //dikat
    {
        BM_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);  //initte tilelar� de�i�tirebilmek i�in
    }


    //cellerdeki bilgileri kullanarak haritadaki konumunu saptamak i�in 
    public Vector3 CellToWorld(Vector2Int cellIndex)               //vector2Int cinsinden cell bilgilerini world positiona �evirir (ilk defa kulland�m)
    {
        return BM_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    //cellerin ge�ilebilir olup olmad���n� kontor etmek i�in(sahnenin kenar�(duvar) olma durumu)
    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return BM_BoardData[cellIndex.x, cellIndex.y];  //data al
    }

    //ayn� foodtaki gibi random konumda wall yaratmak i�in
    void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; ++i)
        {
            // random konum belirleme (listede olan konumlar i�inden se�er)
            int randomIndex = Random.Range(0, BM_EmptyCellsList.Count);
            Vector2Int coord = BM_EmptyCellsList[randomIndex];

            BM_EmptyCellsList.RemoveAt(randomIndex);  //bu konumu listeden sil
            CellData data = BM_BoardData[coord.x, coord.y];  //data al

            //Wall yaratmak i�in onu prefabden klonlama, pozisyonunu belirleme ve datay� g�ncelleme
            WallObject newWall = Instantiate(WallPrefab);
            AddObject(newWall, coord);
        }
    }

    //random konumda food yaratmak i�in
    void GenerateFood()
    {
        //Food birimini ayarlama
        int foodCount = 5;
        for (int i = 0; i < foodCount; ++i)
        {
            // random konum belirleme (listede olan konumlar i�inden se�er)
            int randomIndex = Random.Range(0, BM_EmptyCellsList.Count);
            Vector2Int coord = BM_EmptyCellsList[randomIndex];

            BM_EmptyCellsList.RemoveAt(randomIndex);  //bu konumu listeden sil
            CellData data = BM_BoardData[coord.x, coord.y];  //data al
            if (data.Passable && data.ContainedObject == null)  //belirtilen konumdaki cell(tile) ge�ilebilir ve i�erebilir ise �al���r
            {
                //Food yaratmak i�in onu prefabden klonlama, pozisyonunu belirleme ve datay� g�ncelleme
                FoodObject newFood = Instantiate(FoodPrefab);
                AddObject(newFood, coord);
            }
        }
    }

    void GenerateEnemy()
    {
        int enemyCount = Random.Range(1, 4);
        for (int i = 0; i < enemyCount; ++i)
        {
            // random konum belirleme (listede olan konumlar i�inden se�er)
            int randomIndex = Random.Range(0, BM_EmptyCellsList.Count);
            Vector2Int coord = BM_EmptyCellsList[randomIndex];

            BM_EmptyCellsList.RemoveAt(randomIndex);  //bu konumu listeden sil
            CellData data = BM_BoardData[coord.x, coord.y];  //data al

            //enemy yaratmak i�in onu prefabden klonlama, pozisyonunu belirleme ve datay� g�ncelleme
            EnemyObject newEnemy = Instantiate(EnemyPrefab);
            AddObject(newEnemy, coord);
        }
    }


    //hatalar� engellemek ve kontrol� i�in d�zenleme
    void AddObject(CellObject obj, Vector2Int coord)  
    {
        CellData data = BM_BoardData[coord.x, coord.y];  //datay� al
        obj.transform.position = CellToWorld(coord);     //pozisyonu belirleme
        data.ContainedObject = obj;  //cell �zerindeki obj datas�
        obj.Init(coord);  //init ayar�
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return BM_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    //Sil ba�tan ba�lamak gerek bazen, GameBoard� s�f�rlamak...
    public void Clean()
    {
        
        if (BM_BoardData == null)  //gb'�n olu�up olu�mad���n� kontrol eder
            return;

        //b�t�n h�creleri tarar ve hepsini siler sonra da bo� de�er verir
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = BM_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }

}