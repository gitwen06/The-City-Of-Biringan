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

    public void LookAt(Transform target)
    {
        StartCoroutine(AnimateLookAt(target));
    }

    public void MoveTo(Transform target)
    {
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
        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            playerCameraTransform.rotation = Quaternion.Lerp(originalRotation, target.rotation, easedT);

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