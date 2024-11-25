using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehavior : MonoBehaviour
{
    public MeshRenderer drumHeadRenderer; 
    public AudioSource drumSound;         
    public Color hitGlowColor = Color.white; 
    public float glowIntensity = 2.0f;
    public float resetDelay = 0.5f; 


    private Color originalEmissionColor;          
    private Material drumMaterial;
    void Start()
    {
        if (drumHeadRenderer != null)
        {
            // Create a unique material instance for the drum
            drumMaterial = new Material(drumHeadRenderer.material);
            drumHeadRenderer.material = drumMaterial;

            // Store the original emission color
            if (drumMaterial.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = drumMaterial.GetColor("_EmissionColor");
            }
            else
            {
                originalEmissionColor = Color.black; // Default to black if not set
            }
        }
    }

    
    public void OnDrumHit()
    {
       Debug.Log($"Drum {name}: Hit detected.");
        // Trigger the glow effect
        if (drumMaterial != null)
        {
            if (drumMaterial.HasProperty("_EmissionColor"))
            {
                Color glowColor = hitGlowColor * glowIntensity;
                drumMaterial.SetColor("_EmissionColor", glowColor);
                drumMaterial.EnableKeyword("_EMISSION");
                
            }

            // Reset the color after the delay
            Invoke(nameof(ResetGlow), resetDelay);
        }
        if (drumMaterial != null && drumMaterial.IsKeywordEnabled("_EMISSION"))
        {
            Color glowColor = hitGlowColor * glowIntensity;
            drumMaterial.EnableKeyword("_EMISSION");
            drumMaterial.SetColor("_EmissionColor", glowColor);
            

            // Reset glow after delay
            Invoke(nameof(ResetGlow), resetDelay);
        }

        // Play the drum sound
        if (drumSound != null)
        {
            drumSound.pitch = Random.Range(0.9f, 1.1f);
            drumSound.Play();
            Debug.Log($"Drum {name}: Sound played.");
        }
    }

    
    private void ResetGlow()
    {
        // Reset the emission color to its original state
        if (drumMaterial != null && drumMaterial.IsKeywordEnabled("_EMISSION"))
        {
            drumMaterial.SetColor("_EmissionColor", originalEmissionColor);
            
        }
    }
}
