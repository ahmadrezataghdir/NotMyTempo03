using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DrumGameManager : MonoBehaviour
{
    // References
    public List<Renderer> drums;        
    public List<Renderer> speakers;         
    
    public TextMeshProUGUI timerText;       
    public TextMeshProUGUI hitTimerText;
    public TextMeshProUGUI scoreText;       
    public TextMeshProUGUI missText;
    public Image progressBar;               
    public GameObject startMenu;            
    public GameObject gameOverMenu;         

    private AudioSource audioSource;
    public AudioClip correctSound;          
    public AudioClip wrongSound;
    public List<AudioClip> drums_sounds;
    public float primaryTimerDuration = 60f; 
    private float hitTimerDuration = 10f;     
    public float glowIntensity = 3f;        

    private float primaryTimer;             
    private float hitTimer;                 
    private int score;                      

    private int misses = 0;
    private bool isGameActive = false;      
    private bool isShowingSequence = false; 

    private List<Color> currentSequence = new List<Color>(); 
    private int currentIndex = 0;           

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeGame();

        
    }

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
                HandleMiss();
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

    // Initialize game settings
    void InitializeGame()
    {
        score = 0;
        misses = 0;
        primaryTimer = primaryTimerDuration;

        startMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        isGameActive = false;

        UpdateScoreUI();
        UpdateMissUI();
        timerText.text = "" + Mathf.Floor(primaryTimer);


        

    }

    private void MakeWhite(){
        foreach (var speaker in speakers)
        {
            speaker.material.color = Color.white;
        }
    }

    // Start the game
    public void StartGame()
    {
        startMenu.SetActive(false);
        isGameActive = true;
        StartCoroutine(GenerateColorSequence());
    }

    // End the game
    void EndGame()
    {
        isGameActive = false;
        gameOverMenu.SetActive(true);
        timerText.text = "Time's Up!";
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Update the timer UI
    void UpdateTimerUI()
    {
        timerText.text = primaryTimer > 0 ? "" + Mathf.Floor(primaryTimer).ToString() : "Time's Up!";
    }

    // Update score UI
    void UpdateScoreUI()
    {
        scoreText.text = "" + score;
    }

    // Update miss UI
    void UpdateMissUI()
    {
        missText.text = "" + misses + "/5";
    }

    // Update the progress bar
    void UpdateProgressBar(float progress)
    {
        hitTimerText.text = Mathf.Floor(hitTimer).ToString();
        progressBar.fillAmount = progress;
    }

    // Generate and show the color sequence
    IEnumerator GenerateColorSequence()
    {
        isShowingSequence = true;
        currentSequence.Clear();

        
        // Randomize speaker colors based on drum colors
        foreach (var speaker in speakers)
        {
            int randomIndex = Random.Range(0, drums.Count);
            Color randomColor = drums[randomIndex].material.color;
            currentSequence.Add(randomColor);
            speaker.material.color = randomColor;
            //audioSource.PlayOneShot(drums_sounds[randomIndex]);
        }

        
        foreach (var speaker in speakers)
        {
            speaker.material.color *= glowIntensity;
            yield return new WaitForSeconds(0.6f);
            speaker.material.color /= glowIntensity;
            yield return new WaitForSeconds(0.3f);
        }
        

        currentIndex = 0;
        hitTimer = hitTimerDuration;
        isShowingSequence = false;
    }


    public void OnDrumSelected(Material drum)
    {

        Debug.Log($"Drum2 {drum.name}: Hit.");

        if (!isGameActive || isShowingSequence) return;

        // Check if selected drum matches the current sequence
        if (drum.color == currentSequence[currentIndex])
        {
            currentIndex++;

            // If sequence is complete, generate a new one
            if (currentIndex == currentSequence.Count)
            {
                audioSource.PlayOneShot(correctSound);
                score += 5;
                UpdateScoreUI();
                StartCoroutine(GenerateColorSequence());
            }
        }
        else
        {
            HandleMiss();
        }
    }

/*
    public void OnDrumHit(DrumBehavior drum)
    {
        if (!isGameActive || isShowingSequence) return;


        // Find the corresponding cylinder index for the drum
        int drumIndex = drums.FindIndex(c => c.transform == drum.transform);

        Debug.Log($"Drum {drumIndex}: Hit detected.");
        if (drumIndex >= 0)
        {
            // Check if the hit matches the current sequence
            if (drums[drumIndex].material.color == currentSequence[currentIndex])
            {
                currentIndex++;
                if (currentIndex == currentSequence.Count)
                {
                    audioSource.PlayOneShot(correctSound);
                    score += 5;
                    UpdateScoreUI();
                    StartCoroutine(GenerateColorSequence());
                }
            }
            else
            {
                HandleMiss();
            }
        }
    }
    */

    // Handle a miss
    void HandleMiss()
    {
        audioSource.PlayOneShot(wrongSound);
        misses++;
        UpdateMissUI();

        // Deduct score after 5 misses
        if (misses == 5)
        {
            misses = 0;
            score = Mathf.Max(score - 1, 0);
            UpdateScoreUI();
            UpdateMissUI();
        }

        // Reset sequence
        currentIndex = 0;
        StartCoroutine(GenerateColorSequence());
    }
}
