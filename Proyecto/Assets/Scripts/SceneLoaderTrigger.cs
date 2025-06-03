using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderTrigger : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FadeController fade = FindObjectOfType<FadeController>();
            if (fade != null)
            {
                fade.FadeAndLoadScene(nextSceneName);
            }
        }
    }
}
