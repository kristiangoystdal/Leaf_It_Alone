using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SweepClicker : MonoBehaviour
{
    public Animator broomAnimator; // Reference to the Animator component
    public Animator sliderAnimator; // Reference to the Animator component for the cooldown slider
    public GameObject cooldownSlider; // Reference to the cooldown slider GameObject
    public float sweepDuration = 1f; // Duration of the sweep animation
    public float cooldownDuration = 1f; // Cooldown duration before the next sweep
    private bool isSweeping = false; // Flag to check if the player is currently sweeping

    void Start()
    {
        // Ensure the cooldown slider is inactive at the start
        cooldownSlider.SetActive(false);

        sliderAnimator.speed = 1 / (sweepDuration + cooldownDuration); // Set the playback speed of the slider animation based on the total duration
    }

    void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // If not currently sweeping, start the sweep animation
            if (!isSweeping)
            {
                cooldownSlider.SetActive(true); // Activate the cooldown slider
                broomAnimator.SetTrigger("Sweep"); // Trigger the sweep animation
                sliderAnimator.SetTrigger("CooldownSlider"); // Trigger the cooldown animation for the slider
                isSweeping = true; // Set the sweeping flag to true
                StartCoroutine(SweepCooldown()); // Start the cooldown coroutine

                // Call the DeleteLeaf method from the IdentifyLeafes script
                IdentifyLeafes identifyLeafes = GetComponent<IdentifyLeafes>();
                if (identifyLeafes != null)
                {
                    identifyLeafes.DeleteLeavesInCenterArea(); // Call the method to delete leaves
                }
                // Optionally, you can add a sound effect or particle effect here
            }
        }
    }

    private IEnumerator SweepCooldown()
    {
        // Wait for the duration of the sweep animation
        yield return new WaitForSeconds(sweepDuration);

        // Wait for the cooldown duration before allowing another sweep
        yield return new WaitForSeconds(cooldownDuration);

        isSweeping = false; // Reset the sweeping flag to allow another sweep

        cooldownSlider.SetActive(false); // Deactivate the cooldown slider
    }
}
