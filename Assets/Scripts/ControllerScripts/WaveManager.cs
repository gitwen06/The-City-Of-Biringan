using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    [SerializeField] private List<GerstnerWave> waves;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public Vector3 GetWaveDisplacement(Vector3 worldPos, float time)
    {
        Vector3 result = Vector3.zero;

        foreach (GerstnerWave wave in waves)
        {
            Vector2 dir = wave.direction.normalized;
            float k = 2f * Mathf.PI / wave.wavelength;
            float phase = k * (Vector2.Dot(dir, new Vector2(worldPos.x, worldPos.z)) - wave.speed * time);
            float a = wave.steepness / k;

            result.x += dir.x * a * Mathf.Cos(phase);
            result.z += dir.y * a * Mathf.Cos(phase);
            result.y += a * Mathf.Sin(phase);
        }

        return result;
    }

    public float GetHeightAtPosition(Vector3 worldPos, float time)
    {
        float height = 0f;

        foreach (GerstnerWave wave in waves)
        {
            Vector2 dir = wave.direction.normalized;
            float k = 2f * Mathf.PI / wave.wavelength;
            float phase = k * (Vector2.Dot(dir, new Vector2(worldPos.x, worldPos.z)) - wave.speed * time);
            float a = wave.steepness / k;

            height += a * Mathf.Sin(phase);
        }

        return height;
    }
}