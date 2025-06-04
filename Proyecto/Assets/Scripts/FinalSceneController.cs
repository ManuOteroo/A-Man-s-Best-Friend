using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalSceneController : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;
    public float targetZoom = 3.5f;
    public float zoomSpeed = 8f;
    public Vector3 cameraOffset = new Vector3(0, 0.5f, 0);

    public Image blackoutPanel;
    public GameObject uiCanvas;
    public GameObject ammoCanvas;

    public AudioClip cryingClip;
    public AudioClip gunshotClip;
    public AudioClip finalSongClip;

    public float cryingDelay = 0.5f;
    public float suicideDelay = 2.5f;
    public float gunshotDelay = 3.2f;

    public string mainMenuSceneName = "MainMenu"; // Set your menu scene name here

    private bool cutsceneTriggered = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (blackoutPanel != null)
        {
            Color c = blackoutPanel.color;
            c.a = 0f;
            blackoutPanel.color = c;
        }
    }

    public void TriggerFinalCutscene()
    {
        if (cutsceneTriggered) return;
        cutsceneTriggered = true;

        if (uiCanvas != null)
            uiCanvas.SetActive(false);
        if (ammoCanvas != null)
            ammoCanvas.SetActive(false);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (!(script is Animator) && script != this)
                script.enabled = false;
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isAiming", false);
            anim.SetBool("isWalking", false);
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsJumping", false);
            anim.Play("Idle", 0, 0f);
            anim.Update(0f);
        }

        Transform arms = player.Find("Arms") ?? player.Find("Gun");
        if (arms != null)
            arms.gameObject.SetActive(false);

        StartCoroutine(CameraZoomCutscene());
        StartCoroutine(SuicideSequence());
    }

    IEnumerator CameraZoomCutscene()
    {
        float duration = 0.5f;
        float time = 0f;

        float startZoom = mainCamera.orthographicSize;
        float endZoom = targetZoom;

        Vector3 startPos = mainCamera.transform.position;
        Vector3 endPos = player.position + cameraOffset;
        endPos.z = -10f;

        while (time < duration)
        {
            float t = time / duration;

            mainCamera.orthographicSize = Mathf.Lerp(startZoom, endZoom, t);
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = endZoom;
        mainCamera.transform.position = endPos;
    }

    IEnumerator SuicideSequence()
    {
        yield return new WaitForSeconds(cryingDelay);

        if (cryingClip != null && audioSource != null)
            audioSource.PlayOneShot(cryingClip);

        yield return new WaitForSeconds(suicideDelay - cryingDelay);

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("suicide");

        yield return new WaitForSeconds(gunshotDelay - suicideDelay);

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (gunshotClip != null && audioSource != null)
            audioSource.PlayOneShot(gunshotClip);

        if (blackoutPanel != null)
        {
            Color c = blackoutPanel.color;
            c.a = 1f;
            blackoutPanel.color = c;
        }

        if (finalSongClip != null && audioSource != null)
        {
            audioSource.clip = finalSongClip;
            audioSource.loop = false;
            audioSource.Play();

            yield return new WaitForSeconds(finalSongClip.length);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
