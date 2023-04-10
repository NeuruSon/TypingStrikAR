using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTranslate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnTranslate()
    {
        gameObject.transform.Translate(new Vector3(5, 0, -5));
    }
}
