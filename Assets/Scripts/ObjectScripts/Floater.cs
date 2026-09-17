using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private Transform[] floatPoints;
    [SerializeField] private float buoyancyStrength = 10f;
    [SerializeField] private float waterDrag = 1f;
    [SerializeField] private float waterAngularDrag = 1f;
    [SerializeField] private float airDrag = 0f;
    [SerializeField] private float airAngularDrag = 0.05f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        bool isSubmerged = false;

        foreach (Transform point in floatPoints)
        {
            float waveHeight = WaveManager.instance.GetHeightAtPosition(point.position, Time.time);
            float submersion = waveHeight - point.position.y;

            if (submersion > 0)
            {
                isSubmerged = true;

                float clampedSubmersion = Mathf.Min(submersion, 2f);
                Vector3 force = Vector3.up * buoyancyStrength * clampedSubmersion;
                rb.AddForceAtPosition(force, point.position);
            }
        }

        rb.linearDamping = isSubmerged ? waterDrag : airDrag;
        rb.angularDamping = isSubmerged ? waterAngularDrag : airAngularDrag;
    }
}