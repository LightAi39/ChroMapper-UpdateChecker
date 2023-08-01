using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace ChroMapper_UpdateChecker
{
    [Plugin("UpdateChecker")]
    public class UpdateChecker
    {
        [Init]
        private void Init()
        {

        }

        [Exit]
        private void Exit()
        {

        }


        public static async void CheckForNewReleaseOnGithub(string repoOwner, string repoName, string currentVersionTag)
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic releaseInfo = JsonConvert.DeserializeObject(json);

                    string latestReleaseTag = releaseInfo.tag_name;

                    if (currentVersionTag != latestReleaseTag)
                    {
                        Debug.Log($"A newer release is available for {repoOwner}/{repoName}: {latestReleaseTag}");
                        // do shit with ui
                    }
                    else
                    {
                        Debug.Log($"Up to date. No newer releases available for {repoOwner}/{repoName}.");
                    }
                }
                else
                {
                    Debug.Log($"Failed to fetch release information for {repoOwner}/{repoName}. Status Code: {response.StatusCode}");
                }
            }

        }
    }
}
