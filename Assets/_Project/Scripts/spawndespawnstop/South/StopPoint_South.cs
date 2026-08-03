using UnityEngine;

/// <summary>
/// StopPoint_South.cs (revisi)
/// Fix untuk 3.4: collider ini WAJIB jadi trigger, TIPIS (kayak garis stop line beneran),
/// dan center-nya pas di garis stop -> supaya mobil pertama antrean selalu berhenti
/// di posisi yang konsisten (jadi acuan tetap buat mobil-mobil di belakangnya).
///
/// Rule of thumb sizing Box Collider (isTrigger = true):
///  - Size.z (ketebalan searah jalan): ~0.3 - 0.5 unit. JANGAN lebih dari itu,
///    karena semakin tebal trigger, semakin besar variasi posisi berhenti mobil
///    (mobil bisa berhenti kapan aja begitu nyentuh sisi depan trigger, tapi kalau
///    tebal, titik "nyentuh" itu jadi nggak presisi/konsisten).
///  - Size.x (lebar): selebar 1 lajur jalan, jangan nyerempet lajur sebelah.
///  - Posisi: center Z HARUS pas di garis stop line visual di road mesh/texture.
/// </summary>
[RequireComponent(typeof(Collider))]
public class StopPoint_South : MonoBehaviour
{
    [SerializeField] private TrafficLight trafficLight;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out CarAI car)) return;

        bool mustStop = trafficLight != null && !trafficLight.BolehJalan();
        car.SetStoppedByLight(mustStop);
    }

    private void OnTriggerExit(Collider other)
    {
        // Begitu mobil lewat garis stop (misal pas lampu hijau), lepas status "stopped by light"-nya
        // supaya dia nggak nyangkut kalau lampu berubah lagi setelah dia lewat.
        if (other.TryGetComponent(out CarAI car))
        {
            car.SetStoppedByLight(false);
        }
    }

    // Bantu visualisasi ukuran/posisi trigger di Scene view -> mempermudah kerjain checklist 3.4
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        if (col is BoxCollider box)
        {
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}