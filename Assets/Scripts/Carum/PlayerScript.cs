using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class PlayerScript : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void CallReact(string userName, int score);

    [DllImport("__Internal")]
    private static extern void ReactRouting(string to);

    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("보이녕");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UnityCall()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        CallReact("싸피",score);
#endif
    }

    public void ButtonClick()
    {
        score++;
        UnityCall();
    }

    public void MoveSceneTo(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SendSignalToReact(string to)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        ReactRouting(to);
#endif
    }
}