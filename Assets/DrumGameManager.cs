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
    
    public TextMeshProUGUI tempTimer;       
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

    private List<int> sequenceIndices = new List<int>();
    private int currentIndex = 0;           

    private bool isInitialCountdown = true;

    private bool isGameOver = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeGame();

        
    }

    void Update()
    {
        if (isGameOver) return;

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
            if (!isInitialCountdown && primaryTimer > 0)
            {
                primaryTimer -= Time.deltaTime;
                UpdateTimerUI();
            }
            else if(!isInitialCountdown && primaryTimer <= 0)
            {
                EndGame();
            }
        }
    }

    void ResetGameElements()
{
    foreach (var speaker in speakers)
    {
        speaker.material.color = Color.white;
    }
    foreach (var drum in drums)
    {
        drum.material.color = Color.white;
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
        isGameOver = true;
        isGameActive = false;
         StopAllCoroutines();
         ResetGameElements(); // Reset all game visuals
        gameOverMenu.SetActive(true);
        timerText.text = "Time's Up!";
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    
    // Update UI elements
    void UpdateTimerUI() => timerText.text = primaryTimer > 0 ? Mathf.Floor(primaryTimer).ToString() : "Time's Up!";
    void UpdateScoreUI() => scoreText.text = "" + score;
    void UpdateMissUI() => missText.text = "" + misses + "/5";

    // Update the progress bar
    void UpdateProgressBar(float progress)
    {
        hitTimerText.text = Mathf.Floor(hitTimer).ToString();
        progressBar.fillAmount = progress;
    }

   

    IEnumerator GenerateColorSequence()
    {
        if (isGameOver) yield break;

        isShowingSequence = true;
        sequenceIndices.Clear();

        // Countdown before the sequence starts
        for (int i = 3; i > 0; i--)
        {
            tempTimer.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }

        tempTimer.text = ""; // Clear the timer text

        // Allow the primary timer to start decrementing after the initial countdown
        if (isInitialCountdown)
        {
            isInitialCountdown = false;
        }

        // Hide all speaker colors (set to white)
        foreach (var speaker in speakers)
        {
            speaker.material.color = Color.white;
        }


        // Generate a random sequence
        for (int i = 0; i < drums.Count; i++)
        {
            sequenceIndices.Add(i);
        }

        sequenceIndices.Shuffle(); // Randomize the order

        // Show the sequence
        foreach (int index in sequenceIndices)
        {
            Renderer speaker = speakers[index];

            // Highlight the speaker
            Color originalColor = drums[index].material.color;
            speaker.material.color = originalColor * glowIntensity;

            // Play sound
            audioSource.PlayOneShot(drums_sounds[index]);

            // Wait
            yield return new WaitForSeconds(0.6f);

            // Reset to white
            //speaker.material.color = Color.white;
            Debug.Log("Nothing");

            yield return new WaitForSeconds(0.3f);
        }

        

        // Reset and allow user interaction
        currentIndex = 0;
        hitTimer = hitTimerDuration;
        isShowingSequence = false;
    }

    public void OnDrumSelected(Material drumMaterial)
    {

        Debug.Log($"Drum2 {drumMaterial.name}: Hit.");

        if (!isGameActive || isShowingSequence) return;

        int expectedIndex = sequenceIndices[currentIndex];

        // Check if selected drum matches the current sequence
        if (drumMaterial.color == drums[expectedIndex].material.color)
        {
            currentIndex++;

            if (currentIndex == sequenceIndices.Count)
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

// Extension method to shuffle a list
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
