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
    public ParticleSystem matchParticleEffect; // E�le�me s�ras�nda patlatmak i�in

    // �rnek: Skill 2 i�in ayr� bir particle ya da mekanik ekleyebilirsin
    // public ParticleSystem secondSkillParticle;

    /// <summary>
    /// Skoru s�f�rlay�p oyunu yeniden ba�latmak veya
    /// istedi�in ba�ka bir reset senaryosunu �al��t�rmak i�in.
    /// Bunu bir UI butonundan �a��rabilirsin.
    /// </summary>
    public void ResetGame()
    {
        // Skor s�f�rla
        Score = 0;
        ScoreText.text = "Score: " + Score;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Sahneyi yeniden y�kle veya prefablar� tekrar spawn et
        // (�imdilik sadece sahne reload �rne�i)
        // UnityEngine.SceneManagement.SceneManager.LoadScene(
        //     UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        // );

        // Ya da Fruits childlar�n� yok edip yeniden yerle�tirebilirsin
        // for (int i = Fruits.childCount - 1; i >= 0; i--)
        // {
        //     Destroy(Fruits.GetChild(i).gameObject);
        // }
        // Yeniden spawn...
    }
}
