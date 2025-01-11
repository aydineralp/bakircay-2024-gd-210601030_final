using System;
using TMPro;
using UnityEngine;

public class MovableItem : MonoBehaviour
{
    public string FruitName;
    public float backduration = 3f;
    public PlacementPlatform _sp;
    public float height = 3f; // Daha d���k yay �izmesi i�in azalt�ld�
    public Animator myAnimator;

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
    private float maxDragHeight = 1.5f; // Maksimum yukar� ��k�� s�n�r� eklendi
    private float backForceMultiplier = 0.5f; // Geri f�rlatma g�c� azalt�c� �arpan

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        fallposition = transform.position;
        initialY = transform.position.y;
    }

    private void Update()
    {
        if (isBack)
        {
            if (elapsedTime < backduration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / backduration;

                Vector3 horizontalPosition = Vector3.Lerp(transform.position, startposition, t);

                float arc = Mathf.Sin(t * Mathf.PI) * height;
                transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + arc, horizontalPosition.z);
            }
            else
            {
                rb.isKinematic = false;
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
        offset = transform.position
                 - mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 currentPosition = mainCamera.ScreenToWorldPoint(currentScreenPoint) + offset;

            // Nesnenin fazla yukar� ��kmas�n� engelle
            currentPosition.y = Mathf.Clamp(currentPosition.y, initialY, initialY + maxDragHeight);

            rb.MovePosition(currentPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        rb.isKinematic = false;

        // B�rakt�ktan sonra h�z �ok y�ksekse azalt
        if (rb.linearVelocity.magnitude > 2f) // E�er h�z �ok b�y�kse, s�n�rla
        {
            rb.linearVelocity = rb.linearVelocity.normalized * 2f; // H�z� 2 birimle s�n�rla
        }
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
            }
            else if (_sp.CurrentFruit != this && _sp.CurrentFruit.FruitName != this.FruitName)
            {
                // E�er farkl� bir nesneye denk geldiyse geri gitmeli ve f�rlatma g�c� azalt�lmal�
                isBack = true;
                rb.isKinematic = true;

                // F�rlatma g�c�n� daha da d���r
                rb.linearVelocity *= backForceMultiplier;
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
