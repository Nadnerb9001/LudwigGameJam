using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private string volumneName;
    [SerializeField] private AudioMixer mixer;

    private void Start()
    {
        Slider slider = this.gameObject.GetComponent<Slider>();
        float storedValue = PlayerPrefs.GetFloat(volumneName);
        slider.value = storedValue;
    }

    public void UpdateValueOnChange(Single value)
    {
        mixer.SetFloat(volumneName, value);
        PlayerPrefs.SetFloat(volumneName, value);
    }
}
