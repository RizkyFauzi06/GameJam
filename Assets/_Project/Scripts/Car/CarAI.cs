using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CarAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Deteksi Mobil (Tanpa Raycast!)")]
    [Tooltip("Jarak berhenti dari pusat mobil ke mobil depannya")]
    [SerializeField] private float safeDistance = 1f; 

    [Header("Stress System")]
    [Range(0, 100)] public float stress = 0f;
    public float stressGainWaiting = 5f;
    public float stressGainNormal = 0.5f;
    public float stressLossNearMiss = 25f;
    public float stressLossMinorCollision = 40f;
    public float stressLossRapidChange = 10f;
    
    public StressBarUI stressBarUI; 

    [Header("Debug")]
    [SerializeField] private bool drawGizmo = true;

    private bool _stoppedByLight;
    private bool _stoppedByCarAhead;

    // Registry global array/list
    public static readonly List<CarAI> ActiveCars = new List<CarAI>();

    private void Awake()
    {
        // Supaya tidak terpengaruh fisika/benturan aneh bawaan Unity
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
    }

    private void OnEnable() => ActiveCars.Add(this);
    private void OnDisable() => ActiveCars.Remove(this);

    private void Update()
    {
        // Gunakan fungsi pendeteksi berbasis Array/List
        _stoppedByCarAhead = IsCarAheadMath();
        
        bool isStopped = _stoppedByLight || _stoppedByCarAhead;

        if (!isStopped)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            stress += stressGainNormal * Time.deltaTime;
        }
        else
        {
            stress += stressGainWaiting * Time.deltaTime;
        }

        stress = Mathf.Clamp(stress, 0, 100);
        if (stressBarUI != null)
        {
            stressBarUI.SetValue(stress / 100f);
        }

        if (stress >= 100f && GameManager.Instance != null)
        {
            GameManager.Instance.TriggerTawuran(this);
        }
    }

    public void SetStoppedByLight(bool value)
    {
        _stoppedByLight = value;
    }

    // ==============================================================
    // LOGIKA BARU: Cek jarak pakai Array/List, Murni Matematika!
    // ==============================================================
    private bool IsCarAheadMath()
    {
        foreach (CarAI otherCar in ActiveCars)
        {
            // Jangan ngecek diri sendiri
            if (otherCar == this) continue;

            // Cari tahu arah mobil lain tersebut dari posisi kita
            Vector3 dirToOther = otherCar.transform.position - transform.position;
            
            // Cek apakah mobil tersebut ada di DEPAN kita (searah dengan kap mobil kita)
            // Dot Product > 0.8 berarti dia benar-benar berada di lajur depan kita
            if (Vector3.Dot(transform.forward, dirToOther.normalized) > 0.8f)
            {
                // Cek jarak pastinya (dari titik tengah ke titik tengah)
                float distance = dirToOther.magnitude;

                // Jika jaraknya lebih dekat dari jarak aman, BERHENTI!
                if (distance <= safeDistance)
                {
                    return true; 
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;
        
        Gizmos.color = _stoppedByCarAhead ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * safeDistance);
    }

    // (Fungsi stress relief ditaruh di bawah sini agar rapi)
    public void ApplyNearMiss() => stress = Mathf.Clamp(stress - stressLossNearMiss, 0, 100);
    public void ApplyMinorCollision() => stress = Mathf.Clamp(stress - stressLossMinorCollision, 0, 100);
    public void ApplyRapidChangeConfusion() => stress = Mathf.Clamp(stress - stressLossRapidChange, 0, 100);
}