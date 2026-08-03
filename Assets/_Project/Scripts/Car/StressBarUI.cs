using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// StressBarUI.cs (revisi)
/// Tambahan: emotion icon (senang/biasa aja/sedih/takut/marah) yang otomatis ganti
/// sesuai persentase stress -> dipanggil bareng dengan fill bar lewat SetValue() yang sama,
/// jadi CarAI nggak perlu tau apa-apa soal emotion, cukup panggil SetValue() kayak biasa.
/// </summary>
public class StressBarUI : MonoBehaviour
{
    [Header("Fill Bar")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient stressGradient; // Untuk ubah warna (Hijau -> Merah)

    [Header("Emotion Icon")]
    [SerializeField] private Image emotionIcon;
    [SerializeField] private Sprite senangSprite;   // 0%   - 20%
    [SerializeField] private Sprite biasaAjaSprite; // 20%  - 40%
    [SerializeField] private Sprite sedihSprite;    // 40%  - 65%
    [SerializeField] private Sprite takutSprite;    // 65%  - 85%
    [SerializeField] private Sprite marahSprite;    // 85%  - 100%

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
    /// Dipanggil oleh CarAI untuk update tampilan bar DAN emotion icon sekaligus.
    /// value harus antara 0f sampai 1f.
    /// </summary>
    public void SetValue(float value)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = value;
            fillImage.color = stressGradient.Evaluate(value);
        }

        if (emotionIcon != null)
        {
            emotionIcon.sprite = GetEmotionSprite(value);
        }
    }

    private Sprite GetEmotionSprite(float value)
    {
        if (value < 0.20f) return senangSprite;
        if (value < 0.40f) return biasaAjaSprite;
        if (value < 0.65f) return sedihSprite;
        if (value < 0.85f) return takutSprite;
        return marahSprite;
    }
}