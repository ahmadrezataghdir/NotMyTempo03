using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehavior : MonoBehaviour
{
    public MeshRenderer drumHeadRenderer; 
    public AudioSource drumSound;         
    public Color hitColor = Color.black;    
    public float resetDelay = 0.5f; 
    public float colorIntensity = 2.0f;      

    private Color originalColor;          
    private Material drumMaterial;
    void Start()
    {
        
        if (drumHeadRenderer != null && drumHeadRenderer.material != null)
        {
            drumMaterial = drumHeadRenderer.material;
            //originalColor = drumHeadRenderer.material.color;
            originalColor = drumMaterial.color;
        }
    }

    
    public void OnDrumHit()
    {
        // Change the color of the drumhead
        if (drumHeadRenderer != null && drumHeadRenderer.material != null)
        {
            //drumHeadRenderer.material.color = hitColor;
            // Create a "hit" effect by blending the original color with the hit color
            drumMaterial.color = originalColor * colorIntensity + hitColor;

            Invoke(nameof(ResetColor), resetDelay);
        }

        // Play the drum sound
        if (drumSound != null)
        {
            drumSound.pitch = Random.Range(0.9f, 1.1f);
            drumSound.Play();
        }
    }

    
    private void ResetColor()
    {
        if (drumHeadRenderer != null && drumHeadRenderer.material != null)
        {
            //drumHeadRenderer.material.color = originalColor;
            drumMaterial.color = originalColor;
        }
    }
}
