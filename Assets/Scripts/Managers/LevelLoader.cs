using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace ARtChat
{
    public class LevelLoader : MonoBehaviour
    {
        public Animator animator;
        public float transitionTime = 1f;

        private static int currentScene = 0;

        private static LevelLoader _instance;
        public static LevelLoader Instance => _instance;

        public void Awake()
        {
            _instance = this;
        }


        public void LoadNextLevel()
        {
            int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextScene > SceneManager.sceneCountInBuildSettings - 1)
            {
                nextScene = 0;
            }
            StartCoroutine(LoadLevel(nextScene));
        }

        public void LoadARScene()
        {
            LoaderUtility.Initialize();
            StartCoroutine(LoadLevel("AR Scene_OnlyLogin"));
        }

        public void LoadAppScene()
        {
            EventManager.LoadingAppScene?.Invoke();
            StartCoroutine(LoadLevel("App UI_OnlyLogin"));
            LoaderUtility.Deinitialize();
        }
        
        IEnumerator LoadLevel(int buildIndex)
        {
            animator.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            SceneManager.LoadSceneAsync(buildIndex);
        }
        
        IEnumerator LoadLevel(string sceneName)
        {
            animator.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
