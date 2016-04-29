using UnityEngine;
using System.Collections;

public class fire : MonoBehaviour {
    Light lt = null;
    float delay = 0f;

	// Use this for initialization
	void Start () {
        lt = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        delay -= Time.deltaTime;
        if (delay < 0f){
            lt.intensity = Random.Range(2f, 4f);
            lt.color = new Color(1f, Random.Range(0.67f, 0.78f), 0.3f);
            delay += Random.Range(0.01f, 0.1f);
        }
        
	}
}
