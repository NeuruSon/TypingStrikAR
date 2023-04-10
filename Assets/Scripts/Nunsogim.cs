using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nunsogim : MonoBehaviour
{
    public AudioClip sfx1;
    bool isOk = false;
    GameObject tCon;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        tCon = GameObject.Find("tempPlayer");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tCon.GetComponent<TypingController>().isCleared && !isOk)
        {
            isOk = true;
            audioSource.loop = false;
            audioSource.clip = sfx1;
            audioSource.Play();
            Destroy(gameObject, 0.3f);
        }
    }
}
