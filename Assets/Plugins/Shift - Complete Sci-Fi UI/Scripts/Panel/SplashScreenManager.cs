﻿using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Shift
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("Resources")]
        public GameObject splashScreen;
        public GameObject mainPanels;

        private Animator splashScreenAnimator;
        private Animator mainPanelsAnimator;
        private TimedEvent ssTimedEvent;

        [Header("Settings")]
        public bool disableSplashScreen;
        public bool enablePressAnyKeyScreen;
        public bool enableLoginScreen;

        public UserChecker userExistsChecker;

        MainPanelManager mpm;

        void Start()
        {
            splashScreenAnimator = splashScreen.GetComponent<Animator>();
            ssTimedEvent = splashScreen.GetComponent<TimedEvent>();
            mainPanelsAnimator = mainPanels.GetComponent<Animator>();
            mpm = gameObject.GetComponent<MainPanelManager>();

            if (disableSplashScreen == true)
            {
                splashScreen.SetActive(false);
                mainPanels.SetActive(true);

                mainPanelsAnimator.Play("Start");
                mpm.OpenFirstTab();
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                splashScreenAnimator.Play("Login");
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }
        }

        public void LoginScreenCheck()
        {
            if (enableLoginScreen == true && enablePressAnyKeyScreen == true)
            {
                userExistsChecker?.Invoke((isPresent=>
                {
                    if (isPresent)
                    {
                        splashScreenAnimator.Play("Loading");
                        ssTimedEvent.StartIEnumerator();
                    }
                    else
                    {
                        splashScreenAnimator.Play("Press Any Key to Login");
                    }
                }));
            }
                

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == true)
            {
                splashScreenAnimator.Play("Press Any Key to Loading");
                ssTimedEvent.StartIEnumerator();
            }

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == false)
            {
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }
        }
    }
}

[System.Serializable]
public class UserChecker : UnityEvent<System.Action<bool>>
{
}