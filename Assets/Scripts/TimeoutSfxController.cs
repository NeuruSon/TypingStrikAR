using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeoutSfxController : MonoBehaviour
{
    public AudioClip hSfx1, hSfx2, hSfx3, hSfx4;
    AudioSource currentSfx;

    private void Awake()
    {
        int ranNum = Random.Range(1, 5); //랜덤으로 효과음 1~4 선택. 
        currentSfx = GetComponent<AudioSource>();
        switch(ranNum)
        {
            case 1:
                currentSfx.clip = hSfx1;
                break;
            case 2:
                currentSfx.clip = hSfx2;
                break;
            case 3:
                currentSfx.clip = hSfx3;
                break;
            case 4:
                currentSfx.clip = hSfx4;
                break;
        }
    }

    void Start()
    {
        currentSfx.Play();
    }
}
