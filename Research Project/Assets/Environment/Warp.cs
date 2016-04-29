using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Warp : MonoBehaviour {

    public string scene = string.Empty;

	// Use this for initialization
	void Start () {

	}
	
	void OnTriggerEnter (Collider col) {
        if (col.CompareTag("Player")) {
            SceneManager.LoadScene(scene);
        }
    }

}
