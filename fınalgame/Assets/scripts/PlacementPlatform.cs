using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlacementPlatform : MonoBehaviour
{
    public MovableItem CurrentFruit;
    public int Score = 0;
    public TextMeshProUGUI ScoreText;
    public Transform Fruits;
    public GameObject ComplatePanel;

    [Header("Particles & Skills")]
    public ParticleSystem matchParticleEffect; // Eþleþme sýrasýnda patlatmak için

    // Örnek: Skill 2 için ayrý bir particle ya da mekanik ekleyebilirsin
    // public ParticleSystem secondSkillParticle;

    /// <summary>
    /// Skoru sýfýrlayýp oyunu yeniden baþlatmak veya
    /// istediðin baþka bir reset senaryosunu çalýþtýrmak için.
    /// Bunu bir UI butonundan çaðýrabilirsin.
    /// </summary>
    public void ResetGame()
    {
        // Skor sýfýrla
        Score = 0;
        ScoreText.text = "Score: " + Score;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Sahneyi yeniden yükle veya prefablarý tekrar spawn et
        // (Þimdilik sadece sahne reload örneði)
        // UnityEngine.SceneManagement.SceneManager.LoadScene(
        //     UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        // );

        // Ya da Fruits childlarýný yok edip yeniden yerleþtirebilirsin
        // for (int i = Fruits.childCount - 1; i >= 0; i--)
        // {
        //     Destroy(Fruits.GetChild(i).gameObject);
        // }
        // Yeniden spawn...
    }
}
