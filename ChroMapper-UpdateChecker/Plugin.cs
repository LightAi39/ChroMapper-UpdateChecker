using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChroMapper_UpdateChecker
{
    [Plugin("UpdateChecker")]
    public class Plugin
    {
        [Init]
        private void Init()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 1)
            {
                GameObject newGO = new GameObject("UpdateChecker");
                newGO.AddComponent<UpdateChecker>();
            }
        }


        [Exit]
        private void Exit()
        {
        }

        
    }
}
