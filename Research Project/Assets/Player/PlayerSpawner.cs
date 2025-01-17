﻿using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

    public GameObject prefab = null;
    public float spawnTime = 1.0f;

    GameObject player = null;
    float timer = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        bool dead = (player == null);
        if (player != null) {
            if (!player.GetComponentInChildren<CharacterHealth>().Alive()) {
                dead = true;
            }
        }
	    if (dead) {
            timer -= Time.deltaTime;
            if (timer < 0.0f) {
                if (player != null) Destroy(player);
                player = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
                player.name = prefab.name;
                timer = spawnTime;
            }
        }
	}
}
