using UnityEngine;
using UnityEngine.UI;

public class QuitGameOnClick : MonoBehaviour
{
    public Button button;

    // Start is called before the first frame update
    private void Start()
    {
        button.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}