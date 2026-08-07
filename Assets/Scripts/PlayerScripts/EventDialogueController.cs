using UnityEngine;
using System.Collections;

public class EventDialogueController : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public static EventDialogueController instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void LookAt(string targetId)
    {
        if (CameraTargetRegistry.instance == null) { return; }

        Transform target = CameraTargetRegistry.instance.GetTarget(targetId); //get from cameratargetresgistry, returns Transform

        if (target == null) { return; } //null check, important i think

        StartCoroutine(AnimateLookAt(target)); //use transform as parameter for coroutine wow
    }

    public void ResetCameraStatic()
    {
        playerCameraTransform.position = originalPosition;
        playerCameraTransform.rotation = originalRotation;
    }

    IEnumerator AnimateLookAt(Transform target)
    {
        originalRotation = playerCameraTransform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - playerCameraTransform.position);

        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            playerCameraTransform.rotation = Quaternion.Lerp(originalRotation, targetRotation, easedT);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
//make that 100 lines