using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicManager : MonoBehaviour
{
    public AudioSource _bgMusicSFX;
    // Start is called before the first frame update
    void Start()
    {
        _bgMusicSFX.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
