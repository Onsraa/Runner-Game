using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnButtonClickScript : MonoBehaviour
{
    public string sceneName;
    public Button button;

    public bool loadSavedScene;

    // Start is called before the first frame update
    private void Start()
    {
        button.onClick.AddListener(LoadScene);
    }

    // Update is called once per frame
    private void LoadScene()
    {
        if (loadSavedScene && ProgressTracker.savedScene != String.Empty)
        {
            SceneManager.LoadScene(ProgressTracker.savedScene);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}