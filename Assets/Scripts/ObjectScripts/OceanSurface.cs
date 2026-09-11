using UnityEngine;

public class OceanSurface : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(originalVertices[i]);

            Vector3 displacement = WaveManager.instance.GetWaveDisplacement(worldPos, Time.time);

            Vector3 newWorldPos = worldPos + displacement;

            displacedVertices[i] = transform.InverseTransformPoint(newWorldPos);
        }

        mesh.vertices = displacedVertices;
        mesh.RecalculateNormals();
    }
}