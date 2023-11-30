using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;
    private PhotonView photonView;
    private string currentSound;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        photonView = GetComponent<PhotonView>();
        foreach (Sound _sound in sounds)
        {
            _sound.source = gameObject.AddComponent<AudioSource>();
            _sound.source.clip = _sound.clip;

            _sound.source.volume = _sound.volume;
            _sound.source.pitch = _sound.pitch;
        }
    }

    public void  GetSoundToPlay(string _soundname)
    {
        currentSound = _soundname;
        photonView.RPC("Play", RpcTarget.All);
    }

    [PunRPC]
    public void Play()
    {
        Sound _sound = Array.Find(sounds, sound => sound.name == currentSound);
        if(_sound == null)
        {
            return;
        }
        _sound.source.Play();
    }
}
