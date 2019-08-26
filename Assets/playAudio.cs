using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class playAudio : MonoBehaviour
{
    AudioSource audioData;
    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        audioData.Play(0);
        transform.localScale += new Vector3(-5, -5, -5);
        Debug.Log("started");
    }

}
