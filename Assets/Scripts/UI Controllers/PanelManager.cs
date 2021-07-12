using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Michsky;

public class PanelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject escapePanel;
    public GameObject codePanel;
    public GameObject deathPanel;



    public Level level;
    public Question question;
    public GameObject pauseIcon;
    public GameObject languageSelector;
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

    private GameObject gate;
    

    private LeadController leadController;
    private Michsky.UI.Shift.HorizontalSelector languageSelectorScript;
    // Start is called before the first frame update
    void Start()
    {
        languageSelectorScript = languageSelector.GetComponent<Michsky.UI.Shift.HorizontalSelector>();
        leadController = lead.GetComponent<LeadController>();
        codePanel.SetActive(false);
        hackButton.SetActive(true);
        cancelButton.SetActive(false);
        escapePanel.SetActive(false);
        StartCoroutine(WebManager.Instance.RequestLevelCoroutine("level1", (level, error) => {
            if (error.Length == 0 && level != null)
            {
                this.level = level;
            }
            else
            {
                Debug.Log(error);
            }
        }));
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
        question = level.questions.Find((question => question.tag == gate.tag));
        questionField.text = question.query;
        keyField.text = question.key;
    }

    public void OnDeathEvent()
    {

        //Michsky.UI.Shift.ModalWindowManager manager = deathPanel.GetComponent<Michsky.UI.Shift.ModalWindowManager>();
        //manager.Start();
        //manager.ModalWindowIn();
        //blurManager.BlurInAnim();
    }


    public void CompileCode()
    {
        string language = languageSelectorScript.itemList[languageSelectorScript.index].itemTitle;
        switch (language)
        {
            case "Python":
                language = "python";
                break;
            case "C":
                language = "clang";
                break;
            case "C++":
                language = "cpp";
                break;
            default:
                break;
        }
        StartCoroutine(WebManager.Instance.RequestCodeCompilation(language,codeField.text,(result,error)=>
        {
            string op = string.Join("\n",result.output);
            if (question.ValidateAnswer(op))
            {
                statusField.text = "UNLOCKED";
                LazerFloor floor = gate.GetComponent<LazerFloor>();
                if (floor != null)
                {
                    floor.isUnlocked = true;
                }
                leadController.isHacking = false;
            }
            else
            {
                statusField.text = "LOCKED";
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
}

