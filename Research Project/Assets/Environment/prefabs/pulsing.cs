using UnityEngine;
using System.Collections;

public class pulsing : MonoBehaviour {
    Material pulse;
    public float rng = 1f;
    public float spd = 1f;
	// Use this for initialization
	void Start () {
        pulse = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        pulse.SetColor("_EmissionColor", new Color(0f, 1f, 1f) *(rng*(Mathf.Sin((Time.time)*spd)+1)));
	}
}
