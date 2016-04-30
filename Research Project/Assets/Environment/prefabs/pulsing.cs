using UnityEngine;
using System.Collections;

public class pulsing : MonoBehaviour {
    Material pulse;
    AudioSource sound;
    public float rng = 1f;
    public float spd = 1f;
    public Color color = new Color(0f, 1f, 1f);
    public float origin = 1f;

	// Use this for initialization
	void Start () {
        pulse = GetComponent<MeshRenderer>().material;
        sound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        var intensity = (rng * (Mathf.Sin((Time.time) * spd) + origin));
        pulse.SetColor("_EmissionColor", color * intensity);
        if (sound != null) sound.volume = Mathf.Clamp01(intensity);
	}
}
