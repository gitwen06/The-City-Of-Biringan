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

    public void MoveAndLook(string moveTargetId, string lookTargetId)
    {
        if (CameraTargetRegistry.instance == null) { return; }

        Transform moveTarget = CameraTargetRegistry.instance.GetTarget(moveTargetId);
        Transform lookTarget = CameraTargetRegistry.instance.GetTarget(lookTargetId);

        if (moveTarget == null || lookTarget == null) { return; }

        StartCoroutine(AnimateMoveAndLook(moveTarget, lookTarget));
    }

    public void LookAt(string targetId)
    {
        if (CameraTargetRegistry.instance == null) { return; }

        Transform target = CameraTargetRegistry.instance.GetTarget(targetId); //get from cameratargetresgistry, returns Transform

        if (target == null) { return; } //null check, important i think

        StartCoroutine(AnimateLookAt(target)); //use transform as parameter for coroutine wow
    }

    public void MoveTo(string targetId)
    {
        if (CameraTargetRegistry.instance == null) { return; }

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

    IEnumerator AnimateMoveAndLook(Transform moveTarget, Transform lookTarget)
    {
        originalPosition = playerCameraTransform.position;
        originalRotation = playerCameraTransform.rotation;
        float elapsed = 0f;
        float duration = 1.5f;
        float rotationSpeed = 3f; 

        while (elapsed < duration)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget.position - playerCameraTransform.position);
            float t = elapsed / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            playerCameraTransform.position = Vector3.Lerp(originalPosition, moveTarget.position, easedT);
            playerCameraTransform.rotation = Quaternion.Slerp(playerCameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap to exact final position/rotation to avoid floating point drift
        playerCameraTransform.position = moveTarget.position;
        playerCameraTransform.rotation = Quaternion.LookRotation(lookTarget.position - moveTarget.position);
    }
}
//make that 100 lines