using UnityEngine;
using System.Collections;

public class AnimationSounds : MonoBehaviour {

    public AudioClip footstep = null;

    AudioSource source = null;

    void Start () {
        source = GetComponent<AudioSource>();
    }

	public void Footstep () {
        source.PlayOneShot(footstep);
    }
}
