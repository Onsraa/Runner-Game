using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaserScript : MonoBehaviour
{
    
    [Header("Laser System")]
    public Transform shootPoint;
    public LineRenderer lineRenderer;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.up, out hit))
        {
            // Affichage
            Vector3[] pos = new Vector3[2];
            pos[0] = shootPoint.position;
            pos[1] = hit.point;

            lineRenderer.SetPositions(pos);

            GameOver(hit.collider.gameObject);
        }
        else
        {
            Vector3[] pos = new Vector3[2];
            pos[0] = shootPoint.position;
            pos[1] = shootPoint.position + shootPoint.up * 50;
            lineRenderer.SetPositions(pos);
        }
    }
    
    private void GameOver(GameObject other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        ProgressTracker.savedScene = SceneManager.GetActiveScene().name;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Lose");
    }
}
