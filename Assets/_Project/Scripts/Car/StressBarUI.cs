using UnityEngine;
using UnityEngine.UI;

public class StressBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient stressGradient; // Untuk ubah warna (Hijau -> Merah)

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Memastikan UI selalu menghadap ke arah kamera pemain (Billboard)
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                             _mainCamera.transform.rotation * Vector3.up);
        }
    }

    /// <summary>
    /// Dipanggil oleh CarAI untuk update tampilan bar.
    /// value harus antara 0f sampai 1f.
    /// </summary>
    public void SetValue(float value)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = value;
            fillImage.color = stressGradient.Evaluate(value);
        }
    }
}