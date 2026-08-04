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
        instance = this;
    }

    public void LookAt(string targetId)
    {
        Transform target = CameraTargetRegistry.instance.GetTarget(targetId); //get from cameratargetresgistry, returns Transform
        if (target == null) { return; } //null check, important i think
        StartCoroutine(AnimateLookAt(target)); //use transform as parameter for coroutine wow
    }

    public void MoveTo(string targetId)
    {
        Transform target = CameraTargetRegistry.instance.GetTarget(targetId);
        if (target == null) { return; }
        StartCoroutine(AnimateMoveTo(target));
    }

    public void ResetCameraStatic()
    {
        playerCameraTransform.position = originalPosition;
        playerCameraTransform.rotation = originalRotation;
    }

    public void ResetCameraAnimated()
    {
        StartCoroutine(AnimateReset());
    }

    IEnumerator AnimateReset()
    {
        Quaternion startRot = playerCameraTransform.rotation;
        Vector3 startPos = playerCameraTransform.position;

        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            playerCameraTransform.rotation = Quaternion.Lerp(startRot, originalRotation, easedT);
            playerCameraTransform.position = Vector3.Lerp(startPos, originalPosition, easedT);

            elapsed += Time.deltaTime;
            yield return null;
        }
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

    IEnumerator AnimateMoveTo(Transform target)
    {
        originalPosition = playerCameraTransform.position;
        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            playerCameraTransform.position = Vector3.Lerp(originalPosition, target.position, easedT);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
//make that 100 lines