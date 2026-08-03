using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CarAI.cs (revisi)
/// Fix untuk Masalah A (antrean nggak rapi):
///  - 3.1: rayLength sekarang dihitung relatif ke ukuran mobil (bukan angka fix untuk semua prefab)
///  - 3.2: carLayer di-assign & divalidasi di Awake, warning kalau lupa di-set
///  - 3.3: origin raycast dihitung otomatis dari Collider.bounds mobil sendiri,
///         bukan offset hardcode transform.forward * 1.2f, jadi nggak nyangkut collider sendiri
///         walau ukuran/prefab mobil beda-beda.
///
/// Juga: daftar CarAI aktif (static list) supaya SpawnerSouth bisa tau JUMLAH mobil
/// aktif di scene secara akurat -> dipakai untuk fix Masalah B (cap jumlah mobil).
/// </summary>
[RequireComponent(typeof(Collider))]
public class CarAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Deteksi Mobil di Depan")]
    [Tooltip("Jarak aman TAMBAHAN di depan bumper mobil, biasanya sekitar 1x panjang mobil. " +
             "Ini BUKAN total ray length -> total = setengah panjang mobil (dari bounds) + nilai ini.")]
    [SerializeField] private float extraSafeDistance = 2.2f;

    [SerializeField] private LayerMask carLayer;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo = true;

    private Collider _myCollider;
    private float _halfLength;      // setengah panjang mobil di sumbu forward, dari bounds asli
    private bool _stoppedByLight;
    private bool _stoppedByCarAhead;

    // Registry global -> dipakai SpawnerSouth buat itung mobil aktif tanpa FindObjectsByType tiap frame
    public static readonly List<CarAI> ActiveCars = new List<CarAI>();

    private void Awake()
    {
        _myCollider = GetComponent<Collider>();

        // Hitung setengah panjang mobil di sumbu forward LOKAL, dari collider bounds asli (bukan world bounds
        // yang udah kena rotasi, biar konsisten walau mobil spawn dengan rotasi apapun).
        _halfLength = GetLocalHalfLengthOnForwardAxis(_myCollider);

        if (carLayer.value == 0)
        {
            Debug.LogWarning($"[CarAI] '{name}': Car Layer belum di-set (masih Nothing). " +
                              "Raycast deteksi mobil di depan TIDAK akan berfungsi. " +
                              "Set field Car Layer ke layer 'Car' di Inspector.", this);
        }
    }

    private void OnEnable() => ActiveCars.Add(this);
    private void OnDisable() => ActiveCars.Remove(this);

    private void Update()
    {
        _stoppedByCarAhead = IsCarAhead();
        bool isStopped = _stoppedByLight || _stoppedByCarAhead;

        if (!isStopped)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    /// <summary>Dipanggil oleh StopPoint_South saat mobil ada di trigger area lampu.</summary>
    public void SetStoppedByLight(bool value)
    {
        _stoppedByLight = value;
    }

    private bool IsCarAhead()
    {
        // Origin = posisi mobil + (setengah panjang mobil) ke arah depan.
        // Ini FIX untuk 3.3: origin sekarang selalu tepat di ujung depan collider mobil,
        // berapapun ukuran collider-nya, jadi ray nggak pernah start dari DALAM collider sendiri.
        Vector3 origin = transform.position + transform.forward * (_halfLength + 0.02f); // +0.02 biar bener2 di luar permukaan collider
        float totalRayLength = extraSafeDistance;

        bool hit = Physics.Raycast(origin, transform.forward, out RaycastHit hitInfo, totalRayLength, carLayer);

        if (drawGizmo)
        {
            Debug.DrawRay(origin, transform.forward * totalRayLength, hit ? Color.red : Color.green);
        }

        return hit;
    }

    /// <summary>
    /// Ambil setengah panjang collider di sumbu forward (Z lokal) dari local bounds,
    /// bukan world bounds -> supaya hasilnya konsisten independen dari rotasi transform saat ini.
    /// </summary>
    private float GetLocalHalfLengthOnForwardAxis(Collider col)
    {
        if (col is BoxCollider box)
        {
            // box.size dalam local space, sumbu Z = forward (asumsi model mobil menghadap +Z)
            Vector3 scaledSize = Vector3.Scale(box.size, transform.lossyScale);
            return scaledSize.z * 0.5f;
        }

        // Fallback untuk collider tipe lain (Capsule/Mesh): pakai world bounds extents.z
        // (kurang presisi kalau mobil lagi dirotasi, tapi cukup aman sebagai fallback)
        return col.bounds.extents.z;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmo) return;
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        float halfLen = Application.isPlaying ? _halfLength : GetLocalHalfLengthOnForwardAxis(col);
        Vector3 origin = transform.position + transform.forward * (halfLen + 0.02f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, 0.15f);
        Gizmos.DrawLine(origin, origin + transform.forward * extraSafeDistance);
    }
}