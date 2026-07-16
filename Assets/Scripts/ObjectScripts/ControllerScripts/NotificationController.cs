using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NotificationController : MonoBehaviour
{
    public static NotificationController instance;
    [SerializeField] private RectTransform notificationArea;
    [SerializeField] private GameObject notificationPrefab;

    private List<ActiveNotification> notificationList = new List<ActiveNotification>();

    public void Awake()
    { 
        instance = this;
    }

    public void ShowNotification(string message)
    {
        if(notificationList.Count >= 5)
        {
            ActiveNotification oldestEntry = notificationList[0];
            StopCoroutine(oldestEntry.notificationCoroutine);
            Destroy(oldestEntry.notificationObject);
            notificationList.RemoveAt(0);
        }

        GameObject newNotification = Instantiate(notificationPrefab, notificationArea);
        TMP_Text newNotificationText = newNotification.GetComponentInChildren<TextMeshProUGUI>();
        newNotificationText.text = message;

        Coroutine anim = StartCoroutine(AnimateNotification(newNotification));

        ActiveNotification notification = new ActiveNotification();
        notification.notificationObject = newNotification;
        notification.notificationCoroutine = anim;
        notificationList.Add(notification);
    }

    IEnumerator AnimateNotification(GameObject notif)
    {
        RectTransform rect = notif.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = notif.GetComponent<CanvasGroup>();

        Vector2 startPos = new Vector2(0.02f, -50f);
        Vector2 endPos = new Vector2(0.02f, 50f);

        Vector3 startScale = new Vector3(0.6f, 0.6f, 0.6f);
        Vector3 endScale = Vector3.one; // (1, 1, 1) — full size

        rect.anchoredPosition = startPos;
        rect.localScale = startScale;
        canvasGroup.alpha = 1f; 

        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            rect.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t); 

            elapsed += Time.deltaTime;
            yield return null;
        }

        ActiveNotification entry = notificationList.Find(n => n.notificationObject == notif);
        if (entry != null)
        {
            notificationList.Remove(entry);
        }

        Destroy(notif);
    }
}
