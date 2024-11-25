using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // References
    public List<Renderer> drumRenderers;      
    public List<Renderer> speakerRenderers;
    [SerializeField] TextMeshProUGUI timerText;                    // Main timer UI text
    [SerializeField] TextMeshProUGUI scoreText;                    // Score UI text
    public Image progressBar;                 // Circular progress bar for hit timer
    public GameObject startMenu;              // Start menu panel
    public GameObject gameOverMenu;           // Game over panel
    public AudioSource drumSound;             // Correct drum hit sound
    public AudioSource wrongHitSound;         // Wrong hit sound
    public float primaryTimerDuration = 60f;  // Main timer duration (1 minute)
    public float hitTimerDuration = 5f;       // Time to hit the sequence (5 seconds)
    public float glowIntensity = 2f;          // Intensity of the glow effect

    private float primaryTimer;               // Main countdown timer
    private float hitTimer;                   // Timer for hitting the sequence
    private int score;                        // Player score
    private bool isGameActive = false;        // Flag for game activity
    private bool isShowingSequence = false;   // Flag for sequence display

    private List<Color> currentSequence = new List<Color>(); // The current sequence
    private int currentIndex = 0;             // Current position in the sequence

    private List<Color> possibleColors;       // Dynamically fetched list of colors

    // Start is called before the first frame update
    void Start()
    {
        FetchColorsFromMaterials();
        ShowStartMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameActive)
        {
            // Update the hit timer
            if (!isShowingSequence && hitTimer > 0)
            {
                hitTimer -= Time.deltaTime;
                UpdateProgressBar(hitTimer / hitTimerDuration);
            }
            else if (hitTimer <= 0 && !isShowingSequence)
            {
                HandleWrongHit();
            }

            // Update the primary timer
            if (primaryTimer > 0)
            {
                primaryTimer -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                EndGame();
            }
        }
    }

    // Dynamically fetch colors from drum and speaker materials
    void FetchColorsFromMaterials()
    {
        possibleColors = new List<Color>();

        // Add drum colors
        foreach (var drum in drumRenderers)
        {
            Color drumColor = drum.material.color;
            if (!possibleColors.Contains(drumColor))
            {
                possibleColors.Add(drumColor);
            }
        }

        // Add speaker colors
        foreach (var speaker in speakerRenderers)
        {
            Color speakerColor = speaker.material.color;
            if (!possibleColors.Contains(speakerColor))
            {
                possibleColors.Add(speakerColor);
            }
        }
    }

    // Show the start menu
    void ShowStartMenu()
    {
        startMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        isGameActive = false;
    }

    // Start the game
    public void StartGame()
    {
        startMenu.SetActive(false);
        score = 0;
        primaryTimer = primaryTimerDuration;
        isGameActive = true;
        StartCoroutine(GenerateColorSequence());
    }

    // End the game
    void EndGame()
    {
        isGameActive = false;
        gameOverMenu.SetActive(true);
    }

    // Update the score UI
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    // Update the timer UI
    void UpdateTimerUI()
    {
        timerText.text = "Time Left: " + Mathf.Floor(primaryTimer).ToString();
    }

    // Update the circular progress bar
    void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;
    }

    // Generate and show the color sequence
    IEnumerator GenerateColorSequence()
    {
        isShowingSequence = true;
        currentSequence.Clear();

        // Generate a random sequence of colors
        for (int i = 0; i < speakerRenderers.Count; i++)
        {
            Color randomColor = possibleColors[Random.Range(0, possibleColors.Count)];
            currentSequence.Add(randomColor);
        }

        // Show the sequence by making speakers glow
        for (int i = 0; i < currentSequence.Count; i++)
        {
            speakerRenderers[i].material.color = currentSequence[i] * glowIntensity;
            yield return new WaitForSeconds(0.5f);
            speakerRenderers[i].material.color = currentSequence[i];
            yield return new WaitForSeconds(0.2f);
        }

        currentIndex = 0;
        hitTimer = hitTimerDuration;
        isShowingSequence = false;
    }

    // Handle drum hit by player
    public void OnDrumHit(int drumIndex)
    {
        if (!isGameActive || isShowingSequence) return;

        // Check if the hit is correct
        if (drumRenderers[drumIndex].material.color == currentSequence[currentIndex])
        {
            // Correct hit
            drumSound.Play();
            score++;
            currentIndex++;
            UpdateScoreUI();

            // If sequence is complete, generate a new one
            if (currentIndex == currentSequence.Count)
            {
                StartCoroutine(GenerateColorSequence());
            }
        }
        else
        {
            // Incorrect hit
            HandleWrongHit();
        }

        // Reset the hit timer
        hitTimer = hitTimerDuration;
    }

    // Handle incorrect drum hit or timeout
    void HandleWrongHit()
    {
        wrongHitSound.Play();
        score = Mathf.Max(score - 1, 0);
        UpdateScoreUI();

        // Reset and show a new sequence
        currentIndex = 0;
        StartCoroutine(GenerateColorSequence());
    }
}

