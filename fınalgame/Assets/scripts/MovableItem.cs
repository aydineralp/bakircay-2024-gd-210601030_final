using System;
using TMPro;
using UnityEngine;

public class MovableItem : MonoBehaviour
{
    public string FruitName;
    public float backduration = 3f;
    public PlacementPlatform _sp;
    public float height = 1.5f; // Daha düşük yay çizmesi için azaltıldı
    public Animator myAnimator;

    [Header("Drag & Throw Settings")]
    public float maxDragHeight = 0.3f;    // Nesnenin sürüklenirken maksimum yükseldiği değer
    public float flingMaxSpeed = 0.5f;      // Bıraktıktan sonraki maksimum hız
    public float backForceMultiplier = 0.3f; // Geri fırlatma gücü çarpanı (mismatch durumunda)

    [Header("Game Area Bounds")]
    // Nesnelerin hareket edebileceği minimum ve maksimum koordinatları
    public Vector3 minBoundary = new Vector3(-8.26f, -3.80f, -7.20f);
    public Vector3 maxBoundary = new Vector3(6.56f, -3.00f, 3.40f);

    private Vector3 startposition;
    private Vector3 fallposition;
    private Camera mainCamera;
    private Rigidbody rb;

    private bool isDragging = false;
    private float elapsedTime = 0;
    private bool isBack = false;
    private Vector3 screenPoint;
    private Vector3 offset;
    private float initialY;

    // Kinematik aktifken velocity set etmeyeceğiz,
    // bu yüzden saklayıp sonra tekrar atayacağız.
    private Vector3 velocityBeforeKinematic = Vector3.zero;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        fallposition = transform.position;
        initialY = transform.position.y;
        startposition = transform.position; // Geri dönmek istediğinde, orijinal pozisyon
    }

    private void Update()
    {
        if (isBack)
        {
            // “isBack” durumunda nesneyi bir yay eğrisi ile geri taşıyoruz:
            if (elapsedTime < backduration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / backduration;

                // Sadece x-z ekseninde lineer interpolasyon
                Vector3 horizontalPosition = Vector3.Lerp(transform.position, startposition, t);

                // Y ekseninde küçük bir yay oluştur
                float arc = Mathf.Sin(t * Mathf.PI) * height;

                transform.position = new Vector3(
                    horizontalPosition.x,
                    horizontalPosition.y + arc,
                    horizontalPosition.z
                );
            }
            else
            {
                // Yay hareketi bittiğinde tekrar fizik kontrolü ver
                rb.isKinematic = false;
                // Saklanan velocity'yi geri ver (çarpan uygulanmış hâlini)
                rb.linearVelocity = velocityBeforeKinematic;

                isBack = false;
                elapsedTime = 0;
            }
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        rb.isKinematic = true;

        screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 rawOffset = transform.position
        - mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            screenPoint.z));

        offset = new Vector3(rawOffset.x, 0f, rawOffset.z);
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 currentPosition = mainCamera.ScreenToWorldPoint(currentScreenPoint) + offset;

            // Nesnenin fazla yukarı çıkmasını engelle
            currentPosition.y = Mathf.Clamp(currentPosition.y, initialY, initialY + maxDragHeight);

            // Oyun alanı sınırları içinde kalmasını sağla
            currentPosition.x = Mathf.Clamp(currentPosition.x, minBoundary.x, maxBoundary.x);
            currentPosition.y = Mathf.Clamp(currentPosition.y, minBoundary.y, maxBoundary.y);
            currentPosition.z = Mathf.Clamp(currentPosition.z, minBoundary.z, maxBoundary.z);

            // Kinematik bir rigidbody'yi MovePosition ile taşıyabiliriz
            rb.MovePosition(currentPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        // Kinematikten çıkmadan önce velocity uygulayacaksak burada yapabiliriz
        rb.isKinematic = false;

        // Fiziksel hızını kontrol et
        Vector3 newVelocity = rb.linearVelocity;
        if (newVelocity.magnitude > flingMaxSpeed)
        {
            newVelocity = newVelocity.normalized * flingMaxSpeed;
        }
        rb.linearVelocity = newVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Placement Area")
        {
            _sp = other.GetComponent<PlacementPlatform>();

            if (_sp.CurrentFruit == null)
            {
                _sp.CurrentFruit = this;
            }
            else if (_sp.CurrentFruit != this && _sp.CurrentFruit.FruitName == this.FruitName)
            {
                // Eşleşme durumu
                if (myAnimator != null)
                    myAnimator.SetTrigger("OnMatch");

                if (_sp.CurrentFruit.myAnimator != null)
                    _sp.CurrentFruit.myAnimator.SetTrigger("OnMatch");

                rb.isKinematic = false;
                gameObject.layer = 6;

                _sp.CurrentFruit.rb.isKinematic = false;
                _sp.CurrentFruit.gameObject.layer = 6;

                _sp.CurrentFruit = null;

                _sp.Score += 1;
                _sp.ScoreText.text = $"Score: {_sp.Score}";

                // İsteğe bağlı: Eşleşme particles
                if (_sp.matchParticleEffect)
                {
                    Instantiate(_sp.matchParticleEffect, transform.position, Quaternion.identity);
                }
            }
            else if (_sp.CurrentFruit != this && _sp.CurrentFruit.FruitName != this.FruitName)
            {
                // Eşleşme yoksa, geri gitmeli (isBack = true)
                isBack = true;
                // Kinematik yapmadan önce velocity’yi sakla ve azalt
                velocityBeforeKinematic = rb.linearVelocity * backForceMultiplier;

                rb.isKinematic = true;
                // Diğer bir skill/paticle burada da kullanılabilir.
            }
        }

        if (other.transform.name == "Destory Trigger Area")
        {
            Destroy(gameObject);
            if (_sp.Fruits.childCount <= 1)
            {
                _sp.ComplatePanel.SetActive(true);
            }
        }

        if (other.transform.name == "WrongFallArea")
        {
            transform.position = fallposition;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_sp != null && _sp.CurrentFruit == this)
        {
            _sp.CurrentFruit = null;
        }
    }
}
