using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Realms;

public class PanelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject escapePanel;
    public GameObject codePanel;
    public GameObject deathPanel;

    public GameObject pauseIcon;
    public GameObject startHackingButton;
    public GameObject hackLoader;
    public GameObject hackButton;
    public GameObject cancelButton;
    public GameObject lead;
    public TMP_InputField codeField;
    public TMP_Text questionField;
    public TMP_Text keyField;
    public TMP_Text statusField;
    public Michsky.UI.Shift.BlurManager blurManager;

    [Header("UI Manager")]
    public Michsky.UI.Shift.UIManager UIManager;
    [Header("Error Settings")]
    public TMP_FontAsset errorFont;


    [Header("Respawn Settings")]
    public Transform spawnPoint;
    private GameObject gate;

    private Realm _realm;
    private GameUser user;
    private Chapter chapter;
    private Gate currentGate;
    private LeadController leadController;
    private Michsky.UI.Shift.HorizontalSelector languageSelectorScript;

    private void OnEnable()
    {
        _realm = Realm.GetInstance();
    }

    private void OnDisable()
    {
        _realm.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        user = _realm.All<GameUser>().First();
        chapter = _realm.Find<Chapter>(user.currentChapterID);
        leadController = lead.GetComponent<LeadController>();
        codePanel.SetActive(false);
        hackButton.SetActive(true);
        cancelButton.SetActive(false);
        escapePanel.SetActive(false);
    }


    public void onCodeTextChanged(string code)
    {
        startHackingButton.SetActive(code.Length != 0);
    }

    public void SetQuestion(GameObject gate)
    {
        codeField.text = "";
        statusField.text = "LOCKED";
        this.gate = gate;
        currentGate = chapter.gates.ToList().First(g => gate.CompareTag(g.tag));
        questionField.text = currentGate.question;
        keyField.text = currentGate.key;
    }

    bool ValidateAnswer(string op)
    {
        return currentGate.key.Equals(op);
    }

    private IEnumerator Wait(float time,System.Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    public void CompileCode()
    {
        StartCoroutine(WebManager.Instance.RequestCodeCompilation(user.currentLanguage,currentGate.id, codeField.text, (result, error) =>
          {
              if (error.Length > 0 || !result)
              {
                  var defaultFont = statusField.font;
                  statusField.text = "ERROR";
                  statusField.font = errorFont;
                  var defaultColor = UIManager.primaryColor;
                  UIManager.primaryColor = UIManager.negativeColor;
                  StartCoroutine(Wait(1.5f,()=>
                  {
                      statusField.text = "LOCKED";
                      statusField.font = defaultFont;
                      UIManager.primaryColor = defaultColor;
                  }));
                  return;
              }
              
              if (result)
              {
                  spawnPoint = gate.transform;
                  spawnPoint.position += Vector3.right * 15;
                  statusField.text = "UNLOCKED";
                  LazerFloor floor = gate.GetComponent<LazerFloor>();
                  if (floor != null)
                  {
                      floor.isUnlocked = true;
                  }
                  leadController.isHacking = false;
              }
          }));
    }


    // Update is called once per frame
    void Update()
    {
        if (leadController.isDead)
        {
            HideAllPanels();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }
        startHackingButton.SetActive(!WebManager.Instance.isLoading);
        hackLoader.SetActive(WebManager.Instance.isLoading);
        if (Time.timeScale != 0)
        {
            pauseIcon.SetActive(true);
            hackButton.SetActive(!leadController.isHacking);
            cancelButton.SetActive(leadController.isHacking);
            codePanel.SetActive(leadController.isHacking);
        }
        else
        {
            HideAllPanels();
        }
        escapePanel.SetActive(Time.timeScale == 0);

    }

    private void HideAllPanels()
    {
        pauseIcon.SetActive(false);
        hackButton.SetActive(false);
        codePanel.SetActive(false);
        cancelButton.SetActive(false);
    }

    public void HandleHackButtonPressed()
    {
        leadController.isHacking = !leadController.isHacking;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void QuitChapter()
    {
        //todo: transistion to main menu
    }

    public void OnRespawnEvent()
    {
        leadController.Respwan(spawnPoint.transform);
    }
}

