using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BottelGet : MonoBehaviour
{
    public GameObject bigBottel;

    private PlayableDirector playableDirector;
    private BoxCollider col;

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        col = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bigBottel.SetActive(false);
            playableDirector.Play();
            col.enabled = false;
            ++PlayerPosition.player.DrunkState;
        }
    }
}
