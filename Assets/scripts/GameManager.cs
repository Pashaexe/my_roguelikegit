using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour //oyun buradan yönetilecek
{
    public static GameManager Instance { get; private set; } //Bu özellik, GameManager sýnýfýnýn sadece bir örneðinin var olmasýný saðlar.(gpt'den kopya çektim)

    public TurnManager GM_turnManager;

    public BoardManager Board;
    public PlayerController Player;
    public UIDocument UIDoc;

    //UI için referanslar
    private int GM_FoodAmount = 100;
    private Label GM_FoodLabel;
    private VisualElement GM_GameOverPanel;
    private Label GM_GameOverMessage;

    private int GM_CurrentLevel = 1; //þuanki level sayýsý

    //gamemanager öðesinin instance'ýný kontrol eder
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GM_turnManager = new TurnManager();
        GM_turnManager.OnTick += OnTurnHappen;

        GM_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel"); //UI da yazý yazabilmek için

        GM_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");  //Game Over panelini bulur ve bir deðiþkene atar.
        GM_GameOverMessage = GM_GameOverPanel.Q<Label>("GameOverMessage");             //Panel içindeki mesaj elemanýna eriþir.
         
        GM_GameOverPanel.style.visibility = Visibility.Hidden;                         //Paneli baþlangýçta görünmez yapar.
 
        GM_FoodLabel.text = "Food : " + GM_FoodAmount;  //UI daki yazý GM_FoodLabel.text = "Food : " + GM_FoodAmount;  //UI daki yazý

        NewLevel();
    }

    
    public void NewLevel()
    {
        GM_GameOverPanel.style.visibility = Visibility.Hidden;  //Paneli baþlangýçta görünmez yapar.

        GM_CurrentLevel = 1;
        GM_FoodAmount = 30;
        GM_GameOverPanel.style.visibility = Visibility.Hidden;  //Paneli baþlangýçta görünmez yapar.

        Board.Clean();  //sil
        Board.Init();   //yeniden kur
        Player.Init();
        Player.Spawn(Board, new Vector2Int(1, 1));  //playeri konumlandýr
    }

    void OnTurnHappen()  //her tur baþýnda
    {
        ChangeFood(-1);  //foodu 1 azalt
    }


    public void ChangeFood(int amount)  //food deðerini güncelle
    {
        GM_FoodAmount += amount;
        //UI ve Logu güncelle
        GM_FoodLabel.text = "Food : " + GM_FoodAmount;
        Debug.Log("Current amount of food : " + GM_FoodAmount);

        //eðer food 0 altýna düþerse gamover yazýsýný görünür yap
        if (GM_FoodAmount <= 0)
        {
            Player.GameOver();
            GM_GameOverPanel.style.visibility = Visibility.Visible;
            GM_GameOverMessage.text = "Game Over!\n\nYou traveled through\n\npress enter " + GM_CurrentLevel + " levels";  //n/n satýr atlamak için 

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
