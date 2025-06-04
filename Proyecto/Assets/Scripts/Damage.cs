using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    public GameObject player;
    public Camera mainCamera;
    public float zoomDuration = 2f;
    public float finalDelay = 1f;
    public string suicideTrigger = "Suicide"; // animation trigger name

    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private bool endingStarted = false;

    void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    public void StartEndingSequence()
    {
        if (endingStarted) return;
        endingStarted = true;
        StartCoroutine(EndingSequence());
    }

    IEnumerator EndingSequence()
    {
        // 1. Stop player input
        if (playerMovement != null)
            playerMovement.enabled = false;

        // 2. Stop all enemies (optional: find them and disable)
        foreach (ZombieMovement z in FindObjectsOfType<ZombieMovement>())
        {
            z.enabled = false;
        }

        // 3. Smooth camera zoom
        float startSize = mainCamera.orthographicSize;
        float targetSize = startSize * 0.5f;
        float t = 0f;

        while (t < zoomDuration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t / zoomDuration);
            t += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = targetSize;

        // 4. Wait then play suicide animation
        yield return new WaitForSeconds(finalDelay);

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(suicideTrigger);
        }

        // 5. Optional: Fade out or restart game after delay
    }
}
