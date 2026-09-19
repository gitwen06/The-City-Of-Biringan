using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private Transform[] floatPoints;
    [Range(0f, 1f)]
    [SerializeField] private float buoyancyMultiplier = 1f;
    private const float MaxAccelerationFactor = 4f;

    private const float WaterDensity = 1000f;
    private const float DampingMultiplier = 2f;
    private const float WaterDrag = 3f;
    private const float WaterAngularDrag = 2f;
    private const float AirDrag = 0f;
    private const float AirAngularDrag = 0.05f;

    private Rigidbody rb;
    private bool inWaterVolume = false;

    private float buoyancyStrength;
    private float maxSubmersion;
    private float submersionThreshold;
    private float autoDamping;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CalculateBuoyancyValues();
    }

    private void CalculateBuoyancyValues()
    {
        float volume = CalculateVolume();
        float height = CalculateHeight();

        if (volume <= 0f || height <= 0f || floatPoints.Length == 0)
        {
            Debug.LogWarning($"{name}: Floater could not calculate buoyancy (volume={volume}, height={height}). This object will not float correctly.");
            buoyancyStrength = 0f;
            maxSubmersion = 1f;
            submersionThreshold = 0.05f;
            return;
        }

        buoyancyStrength = WaterDensity * Physics.gravity.magnitude * volume / (floatPoints.Length * height);
        maxSubmersion = height;
        submersionThreshold = height * 0.05f;
        autoDamping = DampingMultiplier * Mathf.Sqrt(buoyancyStrength * rb.mass) / floatPoints.Length;
    }

    private float CalculateVolume()
    {
        Collider col = GetComponent<Collider>();
        Vector3 scale = transform.lossyScale;

        if (col is SphereCollider sphere)
        {
            float avgScale = (scale.x + scale.y + scale.z) / 3f;
            float radius = sphere.radius * avgScale;
            return (4f / 3f) * Mathf.PI * radius * radius * radius;
        }
        else if (col is CapsuleCollider capsule)
        {
            float radiusScale = capsule.direction == 0 ? (scale.y + scale.z) / 2f :
                                 capsule.direction == 1 ? (scale.x + scale.z) / 2f :
                                                           (scale.x + scale.y) / 2f;
            float axisScale = capsule.direction == 0 ? scale.x :
                               capsule.direction == 1 ? scale.y : scale.z;

            float radius = capsule.radius * radiusScale;
            float totalHeight = capsule.height * axisScale;
            float cylinderHeight = Mathf.Max(totalHeight - 2f * radius, 0f);

            float cylinderVolume = Mathf.PI * radius * radius * cylinderHeight;
            float capVolume = (4f / 3f) * Mathf.PI * radius * radius * radius;

            return cylinderVolume + capVolume;
        }
        else if (col is BoxCollider box)
        {
            Vector3 size = Vector3.Scale(box.size, scale);
            return size.x * size.y * size.z;
        }
        else if (col is MeshCollider meshCol && meshCol.sharedMesh != null)
        {
            return CalculateMeshVolume(meshCol.sharedMesh, scale);
        }

        Bounds bounds = col.bounds;
        return bounds.size.x * bounds.size.y * bounds.size.z;
    }

    private float CalculateMeshVolume(Mesh mesh, Vector3 scale)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        float volume = 0f;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = Vector3.Scale(vertices[triangles[i]], scale);
            Vector3 p2 = Vector3.Scale(vertices[triangles[i + 1]], scale);
            Vector3 p3 = Vector3.Scale(vertices[triangles[i + 2]], scale);

            volume += Vector3.Dot(p1, Vector3.Cross(p2, p3)) / 6f;
        }

        return Mathf.Abs(volume);
    }

    private float CalculateHeight()
    {
        return GetComponent<Collider>().bounds.size.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OceanSurface>() != null)
            inWaterVolume = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<OceanSurface>() != null)
            inWaterVolume = false;
    }

    void FixedUpdate()
    {
        if (!inWaterVolume)
        {
            rb.linearDamping = AirDrag;
            rb.angularDamping = AirAngularDrag;
            return;
        }

        bool isSubmerged = false;

        foreach (Transform point in floatPoints)
        {
            float waveHeight = WaveManager.instance.GetHeightAtPosition(point.position, Time.time);
            float rawSubmersion = waveHeight - point.position.y;
            float effectiveSubmersion = rawSubmersion - submersionThreshold;

            if (effectiveSubmersion > 0f)
            {
                isSubmerged = true;

                float clampedSubmersion = Mathf.Min(effectiveSubmersion, maxSubmersion);
                Vector3 force = Vector3.up * buoyancyStrength * clampedSubmersion * buoyancyMultiplier;

                float pointVerticalVelocity = rb.GetPointVelocity(point.position).y;
                force -= Vector3.up * pointVerticalVelocity * autoDamping;

                float maxForce = rb.mass * Physics.gravity.magnitude * MaxAccelerationFactor / floatPoints.Length;
                force = Vector3.ClampMagnitude(force, maxForce);

                rb.AddForceAtPosition(force, point.position);
            }
        }

        rb.linearDamping = isSubmerged ? WaterDrag : AirDrag;
        rb.angularDamping = isSubmerged ? WaterAngularDrag : AirAngularDrag;
    }
}