using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoader : MonoBehaviour
{
    [SerializeField]
    string NextScene;

    public void LoadNext()
    {
        SceneManager.LoadScene(NextScene);
    }
}
