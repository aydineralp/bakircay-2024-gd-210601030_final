using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlacementPlatform : MonoBehaviour
{
    public MovableItem CurrentFruit;
    public int Score = 0;
    public TextMeshProUGUI ScoreText;
    public Transform Fruits;
    public GameObject ComplatePanel;
    public static int TotalScore = 0; // Statik değişkenle toplam skor
    private int matchCount = 0; // Eşleşme sayısı
    public int totalPairs = 7; // Toplam çift sayısı (senin durumunda 7)

    [Header("Particles & Skills")]
    public ParticleSystem matchParticleEffect;

    // 🔥 **2X Skor için değişkenler**
    private bool isDoubleScoreActive = false;
    private int scoreMultiplier = 1; // Başlangıçta normal skor

    void OnGUI()
    {
        // 2X Skor Butonu
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.fontSize = 30;

        if (GUI.Button(new Rect(50, 50, 150, 50), "2X Skor", guiStyle))
        {
            isDoubleScoreActive = true;
            scoreMultiplier = 2; // 2X Skor Aktif
        }
    }

    public void AddScore()
    {
        // Skor ekle
        Score += 1 * scoreMultiplier;
        ScoreText.text = $"Score: {Score}";

        // Eşleşme sayısını artır
        matchCount++;

        // Eğer tüm çiftler eşleşmişse
        if (matchCount == totalPairs)
        {
            TotalScore = Score; // Skoru sakla
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Sahneyi yeniden başlat
        }

        // Skor çarpanını sıfırla
        scoreMultiplier = 1;
        isDoubleScoreActive = false;
    }


    public void ResetGame()
    {
        Score = 0;
        ScoreText.text = "Score: " + Score;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Update()
    {
        // Eğer sahnede hiç meyve kalmadıysa
        if (Fruits.childCount == 0)
        {
            // Mevcut skoru statik değişkende sakla
            TotalScore = Score;

            // Sahneyi yeniden yükle
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void Start()
    {
        // Skoru geri yükle
        Score = TotalScore;
        ScoreText.text = $"Score: {Score}";
    }


}
