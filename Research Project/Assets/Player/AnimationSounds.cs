using UnityEngine;
using System.Collections;

public class AnimationSounds : MonoBehaviour {

    public AudioClip footstep = null;

    AudioSource source = null;

    void Start () {
        source = GetComponent<AudioSource>();
    }

    public void Footstep() {
        if (source != null)
        {
            source.pitch = Random.Range(0.8f, 1.2f);
            source.PlayOneShot(footstep);
        }
    }
}
