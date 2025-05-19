using System.Collections;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource footstep, sprint;

    private Coroutine stopSound;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                footstep.enabled = false;
                sprint.enabled = true;
            }

            else
            {
                footstep.enabled = true;
                sprint.enabled = false;
            }

            if (stopSound != null)
            {
                StopCoroutine(stopSound);
                stopSound = null;
            }
        }
        else
        {
            if (stopSound == null)
            {
                stopSound = StartCoroutine(StopSoundsAfterDelay());
            }
        }
    }


    private IEnumerator StopSoundsAfterDelay()
    {
        yield return new WaitForSeconds(0.5f); // Delay after stopping movement

        footstep.enabled = false;
        sprint.enabled = false;
        stopSound = null;
    }
}

