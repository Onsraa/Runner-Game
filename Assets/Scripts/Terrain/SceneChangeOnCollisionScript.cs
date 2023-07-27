using UnityEngine;
using UnityEngine.SceneManagement;

namespace Terrain
{
    public class SceneChangeOnCollisionScript : MonoBehaviour
    {
        public string sceneName;

        public bool saveOtherScene;
        public string otherScene;
    
        private void OnTriggerEnter(Collider other)
        {
            GameOver(other.gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            GameOver(other.gameObject);
        }

        private void GameOver(GameObject other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (saveOtherScene)
            {
                ProgressTracker.savedScene = otherScene;
            }
            else
            {
                ProgressTracker.savedScene = SceneManager.GetActiveScene().name;
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(sceneName);
        }
    }
}