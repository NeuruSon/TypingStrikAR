using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadController : MonoBehaviour
{
    float speed = 5.0f;
    public Transform tr;
    Vector3 changeScale = Vector3.zero;
    Transform cam;
    GameObject gCon;

    void Start()
    {
        gCon = GameObject.FindWithTag("GameController");
        cam = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(cam);
        changeScale = transform.localEulerAngles;
        changeScale.x = 0;
        changeScale.z = 0;
        transform.localEulerAngles = changeScale;
        if(gCon.GetComponent<GameController>().isNCol && Vector3.Distance(cam.transform.position, gameObject.transform.position) < 20) { Info_On(); }
        else { Info_Off(); }
        tr.localScale = Vector3.Lerp(tr.localScale, changeScale, Time.deltaTime * speed);
    }

    public void Info_On()
    {
        changeScale = Vector3.one;
    }

    public void Info_Off()
    {
        changeScale = Vector3.zero;
    }
}
