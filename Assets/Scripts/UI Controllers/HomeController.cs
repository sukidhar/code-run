﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Michsky;
using UnityEngine.SceneManagement;
using Realms;
using System.Text.RegularExpressions;

public class HomeController : MonoBehaviour
{
    public enum TabPanel
    {
        Home,
        Campaign,
        Settings
    }
    [Header("Login Flow Settings")]
    public GameObject splashScreen;
    public GameObject mainPanels;
    public Michsky.UI.Shift.TimedEvent timedEvent;
    [Header("Sign Up Settings")]
    public TMP_InputField signUpEmailField;
    public TMP_InputField nickNameField;
    public TMP_InputField signUpPasswordField;
    public Michsky.UI.Shift.SwitchManager termsOfUseSwitch;
    public WebManager webManager = WebManager.Instance;
    public Michsky.UI.Shift.MainButton signUpButton;
    public TMP_Text passwordSupportText;

    [Header("Sign In Settings")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public Michsky.UI.Shift.MainButton signInButton;

    [Header("Home Panel Settings")]
    public TMP_Text welcomePlayerName;
    public TMP_Text playerName;
    public TMP_Text playerLevel;

    [Header("Home Panel Game Options")]
    public Michsky.UI.Shift.MainPanelManager mpm;
    //public Michsky.UI.Shift.MainButton continueButton;
    //public Michsky.UI.Shift.MainButton newGameButton;
    //public Michsky.UI.Shift.MainButton tutorialButton;
    //public Michsky.UI.Shift.MainButton aboutButton;



    private Realm _realm;
    private string emailRegex = @"^([0-9a-zA-Z]" + @"([\+\-_\.][0-9a-zA-Z]+)*" + @")+" + @"@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
    private bool[] signUpCheckers = { false, false, false };
    private bool isValidSignInEmail = false;
    private Button signIn, signUp;
    private GameUser gameUser;

    private void OnEnable()
    {
        //Realm.DeleteRealm(RealmConfiguration.DefaultConfiguration);
        _realm = Realm.GetInstance();
    }

    private void OnDisable()
    {
        _realm.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        termsOfUseSwitch.isOn = false;
        signUp = signUpButton.gameObject.GetComponent<Button>();
        signUp.interactable = false;
        passwordSupportText.gameObject.SetActive(false);
        signIn = signInButton.gameObject.GetComponent<Button>();
        signIn.interactable = false;
    }

    public void ClearSignUpText()
    {
        signUpEmailField.text = "";
        signUpPasswordField.text = "";
        nickNameField.text = "";
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.colorMultiplier = 1;
        signUpEmailField.colors = colors;
        nickNameField.colors = colors;
        signUpPasswordField.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {
        signUp.interactable = signUpCheckers.All(check => check) && termsOfUseSwitch.isOn;
        signIn.interactable = isValidSignInEmail && passwordField.text.Length > 0;
    }
    public void UserExistsCheck(System.Action<bool> callback)
    {
        Realm realm = Realm.GetInstance();
        //realm.Write(() =>
        //{
        //    realm.RemoveAll();
        //});
        IQueryable<GameUser> users = realm.All<GameUser>();
        callback(users.Count() > 0);
        if (users.Count() > 0)
        {
            this.gameUser = users.First();
            DidSetGameUser();
        }
        realm.Dispose();
    }
    private void DidSetGameUser()
    {
        welcomePlayerName.text = gameUser.username;
        playerName.text = gameUser.username;
        playerLevel.text = gameUser.level;
    }
    public void OnSignUpClicked()
    {
        StartCoroutine(webManager.RequestRegistration(signUpEmailField.text, signUpPasswordField.text, nickNameField.text,
           (gameUser, error) =>
           {
               if (error.Length > 0)
               {
                   //todo : Handle errors
                   return;
               }
               _realm.Write(() =>
               {
                   _realm.Add(gameUser);
                   timedEvent.StartIEnumerator();
               });
           }));
    }

    public void OnLoginClicked()
    {
        StartCoroutine(webManager.RequestLogin(emailField.text, passwordField.text, (gameUser, error) =>
        {
              if (error.Length > 0)
              {
                  //todo : Handle errors
                  return;
              }
              _realm.Write(() =>
              {
                  _realm.Add(gameUser);
                  timedEvent.StartIEnumerator();
              });
        }));
    }

    public void OnNewGameButtonClicked()
    {
        mpm.OpenPanel(TabPanel.Campaign.ToString());
        if (_realm.All<Chapter>().Count() > 0)
        {
            return;
        }
        StartCoroutine(webManager.RequestAllChapters((chapters, error) =>
        {
            chapters.ForEach(chapter =>
            {
                Debug.Log(chapter.id);
            });
        }));
    }

    public void OnEmailEditingEnded(string s)
    {
        if (Regex.IsMatch(s, emailRegex))
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.colorMultiplier = 1;
            signUpEmailField.colors = colors;
            signUpCheckers[0] = true;
        }
        else
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.red;
            colors.colorMultiplier = 5;
            signUpEmailField.colors = colors;
            signUpCheckers[0] = false;
        }
    }

    public void OnSignInEmailEditingEnded(string s)
    {
        if (Regex.IsMatch(emailField.text, emailRegex))
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.colorMultiplier = 1;
            emailField.colors = colors;
            isValidSignInEmail = true;
        }
        else
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.red;
            colors.colorMultiplier = 5;
            emailField.colors = colors;
            isValidSignInEmail = false;
        }
    }

    public void OnNickNameEditingEnded(string s)
    {
        if (s.Length >= 3)
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.colorMultiplier = 1;
            nickNameField.colors = colors;
            signUpCheckers[1] = true;
        }
        else
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.red;
            colors.colorMultiplier = 5;
            nickNameField.colors = colors;
            signUpCheckers[1] = false;
        }
    }
    public void OnPasswordTextChanged(string s)
    {
        passwordSupportText.gameObject.SetActive(true);
        signUpCheckers[2] = false;
    }
    public void OnPasswordEditingEnded(string s)
    {
        if (s.Length >= 8)
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.colorMultiplier = 1;
            signUpPasswordField.colors = colors;
            passwordSupportText.gameObject.SetActive(false);
            signUpCheckers[2] = true;
        }
        else
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.red;
            colors.colorMultiplier = 5;
            signUpPasswordField.colors = colors;
            signUpCheckers[2] = false;
        }
    }
}