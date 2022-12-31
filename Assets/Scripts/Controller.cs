using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI NumbersPromptText;
    public TextMeshProUGUI RestartPromptText;
    public TextMeshProUGUI StartPromptText;
    public TextMeshProUGUI ResultsText;
    public Color TempResultColor;
    public Color FinalResultColor;
    private bool running;
    public float NumChangePeriod;
    private float numberChangeTime;
    private int currNumber;
    private List<List<int>> Results;
    private bool completed;
    private float startedHolding;
    private RuleSetData currentRuleSet;
    private int currentNumSetIndex = 0;
    private RuleSetData.NumberSet currentNumberSet;
    private static Controller instance;
    public CanvasGroup GameSelectionPanel;
    private bool GameSelPanelMarkedForEnabling;
    private float GameSelEnTime = float.MaxValue;
    private float GameSelPanelEnableDelay = 0.5f;
    public float StartGameDelay = 0.5f;
    public AudioClipPlayer AudioPlayer;
    public bool LogSteps;
    public ParticleSystem NumberPickParticles;
    public KeyCode RestartKeyCode = KeyCode.R;

    public static Controller GetInstance()
    {
        if (instance == null)
        {
            var obj = new GameObject("Controller");
            obj.AddComponent<Controller>();
        }

        return instance;
    }

    public void SetRuleSet(RuleSetData data)
    {
        if (!running)
        {
            currentRuleSet = data;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        GameSelectionPanel.alpha = 1f;
        GameSelEnTime = float.MaxValue;
        ResultsText.text = "";
        NumbersPromptText.enabled = false;
        RestartPromptText.enabled = false;
        StartPromptText.enabled = true;
        ResultsText.enabled = false;
        completed = false;
        running = false;
        startedHolding = float.MaxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSelPanelMarkedForEnabling && !Input.GetMouseButton(0))
        {
            GameSelEnTime = Time.time + GameSelPanelEnableDelay;
            GameSelPanelMarkedForEnabling = false;
        }

        if (!GameSelectionPanel.interactable && Time.time > GameSelEnTime)
        {
            GameSelectionPanel.interactable = true;
        }

        if (running)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickCurrNum();
                return;
            }

            if (Time.time >= numberChangeTime)
            {
                ShowNextValidNumber();
            }
            return;
        }
        
        if (completed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startedHolding = Time.time;
                return;
            }
            
            if ((Input.GetMouseButton(0) && Time.time >= startedHolding + 5) || Input.GetKeyDown(RestartKeyCode))
                
            {
                Reset();
                GameSelPanelMarkedForEnabling = true;
            }
        }
    }

    private void StopRunning()
    {
        NumbersPromptText.enabled = false;
        RestartPromptText.enabled = true;
        StartPromptText.enabled = false;
        ResultsText.enabled = true;
        ResultsText.color = FinalResultColor;
        running = false;
        completed = true;
    }

    private void ShowNextValidNumber()
    {
        var validNumber = false;
        var maxIt = currentNumberSet.RangeEnd - currentNumberSet.RangeStart + 1;
        var it = 0;
        while (!validNumber && it <= maxIt)
        {
            currNumber++;
            if (currNumber > currentNumberSet.RangeEnd)
            {
                currNumber = currentNumberSet.RangeStart;
            }

            validNumber = !Results[currentNumSetIndex].Contains(currNumber);
            it++;
        }

        if (it >= maxIt)
        {
            Debug.LogError("Bug in ShowNextValidNumber algorithm");
        }

        SetDisplayNumber(currNumber);
    }

    private void PickCurrNum()
    {
        AudioPlayer.PlayRandom();
        NumberPickParticles.Play();
        if (LogSteps)
        {
            Debug.Log($"Picked: {currNumber}. currNumSetIndex: {currentNumSetIndex}.");
        }
        Results[currentNumSetIndex].Add(currNumber);
        if (LogSteps)
        {
            var i = 0;
            foreach (var resultSet in Results)
            {
                Debug.Log($"Results[{i}] size: {resultSet.Count}");
                i++;
            }
        }

        ResultsText.text = $"{ResultsText.text} {currNumber}".Trim();
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (currentRuleSet.CompletedRow(currentNumSetIndex, Results))
        {
            if (currentRuleSet.IsCompleted(Results))
            {
                StopRunning();
            }
            else
            {
                SetNumSetIndex(currentNumSetIndex + 1);
                ResultsText.text = $"{ResultsText.text}\n";
                DisplayRandomNumber();
            }
        }
    }

    public void StartRunning()
    {
        if (running || currentRuleSet == null || currentRuleSet.NumberSets.Length == 0)
        {
            return;
        }

        SetNumSetIndex(0);
        
        InitializeResults();

        completed = false;
        NumbersPromptText.enabled = true;
        RestartPromptText.enabled = false;
        StartPromptText.enabled = false;
        ResultsText.enabled = true;
        ResultsText.color = TempResultColor;
        DisplayRandomNumber();
        GameSelectionPanel.alpha = 0;
        GameSelectionPanel.interactable = false;
        StartCoroutine(SetRunning(StartGameDelay));
    }

    private IEnumerator SetRunning(float delay)
    {
        yield return new WaitForSeconds(delay);
        running = true;
    }

    private void InitializeResults()
    {
        if (Results == null)
        {
            Results = new List<List<int>>(2);
        }

        for (var i = Results.Count; i < currentRuleSet.NumberSets.Length; i++)
        {
            {
                Results.Add(new List<int>(8));
            }

        }
        
        foreach (var resultList in Results)
        {
            resultList.Clear();
        }
    }

    private void SetNumSetIndex(int value)
    {
        currentNumSetIndex = value;
        currentNumberSet = currentRuleSet.NumberSets[currentNumSetIndex];
    }

    private void DisplayRandomNumber()
    {
        currNumber = Random.Range(currentNumberSet.RangeStart, currentNumberSet.RangeEnd + 1);
        ShowNextValidNumber();
    }

    private void SetDisplayNumber(int value)
    {
        NumbersPromptText.text = $"{value:d2}";
        numberChangeTime = Time.time + NumChangePeriod;
    }
}