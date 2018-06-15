using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Texture2D texture_muted;
    [SerializeField] private Texture2D testire_playing;
    [SerializeField] private Renderer r;
    void Start()
    {
        musicSource.Play();
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "MusicButton") {
                    if (!musicSource.isPlaying) {
                        musicSource.Play();
                        r.material.mainTexture = testire_playing;
                    }
                    else if (musicSource.isPlaying) {
                        musicSource.Stop();
                        r.material.mainTexture = texture_muted;
                    }
                }
            }
        }
    }
}
