using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;
using System.Security.Policy;
using System.Text.RegularExpressions;
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
