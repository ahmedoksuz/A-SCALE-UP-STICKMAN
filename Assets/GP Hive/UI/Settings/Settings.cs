using System;
using System.Collections;
using System.Collections.Generic;
using ElephantSDK;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour
{
    private Animation animation;

    private bool settingsOption;
    private bool soundOption;
    private bool hapticsOption;

    private AudioListener audioListener;

    private void Start()
    {
        animation = GetComponent<Animation>();
        audioListener = Camera.main!.GetComponent<AudioListener>();

        soundOption = true;
        hapticsOption = true;
    }


    public void ToggleMenu()
    {
        if (animation.isPlaying) return;

        settingsOption = !settingsOption;

        animation.Play(settingsOption ? "Settings_anim" : "Settings_anim_close");
    }

    public void ToggleVibration()
    {
        if (animation.isPlaying) return;

        hapticsOption = !hapticsOption;
        MMVibrationManager.SetHapticsActive(hapticsOption);
    }

    public void ToggleSound()
    {
        if (animation.isPlaying) return;


        soundOption = !soundOption;
        audioListener.enabled = soundOption;
    }

    public void ShowPrivacyPolicy()
    {
        if (animation.isPlaying) return;

        Elephant.ShowSettingsView();
    }
}