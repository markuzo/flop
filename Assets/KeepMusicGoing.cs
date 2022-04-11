using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepMusicGoing : MonoBehaviour
{
    public static bool Inited = false;

    public AudioSource AudioSource;

    private void Awake()
    {
        if (!Inited)
        {
            Inited = true;
            DontDestroyOnLoad(transform.gameObject);
            AudioSource.Play();
            Debug.Log("First");
        }
        else
        {
            Debug.Log("Not first");
            Destroy(transform.gameObject);
        }
    }

}
