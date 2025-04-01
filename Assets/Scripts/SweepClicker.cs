using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SweepClicker : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public float sweepDuration = 1f; // Duration of the sweep animation
    public float cooldownDuration = 1f; // Cooldown duration before the next sweep
    private bool isSweeping = false; // Flag to check if the player is currently sweeping

    void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // If not currently sweeping, start the sweep animation
            if (!isSweeping)
            {
                animator.SetTrigger("Sweep"); // Trigger the sweep animation
                isSweeping = true; // Set the sweeping flag to true
                StartCoroutine(SweepCooldown()); // Start the cooldown coroutine
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
    }
}
