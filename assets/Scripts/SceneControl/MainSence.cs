using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Dock
{
    public class MainSence : MonoBehaviour
    {
        public List<Button> Buttons = new List<Button>();

        private static SceneRecorder sceneRecorder;

        private void Awake()
        {
            if (sceneRecorder == null)
            {
                var go = new GameObject("SceneRecorder");
                sceneRecorder = go.AddComponent<SceneRecorder>();
                DontDestroyOnLoad(go);
                foreach (var item in Buttons)
                {
                    if (item.gameObject.name == "SparseSpatialMap")
                    {
                        sceneRecorder.Record(item, this);
                    }
                }
            }
            else
            {
                sceneRecorder.Recover(this);
            }

            foreach (var item in Buttons)
            {
                item.onClick.AddListener(() =>
                {
                    sceneRecorder.Record(item, this);
                });
            }
        }

        public void OpenScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
           
        }
       
    }
}