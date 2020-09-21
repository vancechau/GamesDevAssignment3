using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(AudioSource))]
 public class Audio: MonoBehaviour
 {
     public AudioSource Intro;
     public AudioSource Normal;
 
    void Start(){
        Intro.Play();
        Normal.PlayDelayed(Intro.clip.length);
    }
 }
