using ChroMapper_UpdateChecker.Helpers;
using ChroMapper_UpdateChecker.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ChroMapper_UpdateChecker
{
    internal class UpdateChecker : MonoBehaviour
    {
        static UpdateChecker _instance;
        private List<Manifest> _manifestsToCheck = new();
        public List<(Manifest manifest, string newVersion)> OutdatedManifests = new();

        private GameObject _OutdatedPluginsUI;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }

            StartCoroutine(CheckForUpdatesCoroutine());
        }

        private IEnumerator CheckForUpdatesCoroutine()
        {
            ObtainManifests();

            foreach (var manifest in _manifestsToCheck)
            {
                yield return CheckForNewReleaseOnGithubCoroutine(manifest, (result) =>
                {
                    if (result.Item1)
                    {
                        OutdatedManifests.Add((manifest, result.Item2));
                    }
                });
            }

            if (OutdatedManifests.Count > 0)
            {
                CreateOutdatedPluginsUI();
            }
        }

        private void ObtainManifests()
        {
            foreach (var plugin in PluginLoader.plugins)
            {
                try
                {
                    var ass = Assembly.GetAssembly(plugin.pluginInstance.GetType());
                    string manifest = ass.GetManifestResourceNames().Where(name => name.Contains("manifest.json")).FirstOrDefault();
                    if (manifest == null)
                    {
                        Debug.Log("Manifest not found in " + plugin.Name);
                    }
                    else
                    {
                        StreamReader reader = new StreamReader(ass.GetManifestResourceStream(manifest));
                        string jsonString = reader.ReadToEnd();

                        Manifest manifestObject = JsonConvert.DeserializeObject<Manifest>(jsonString);

                        if (AreAllPropertiesNotNull(manifestObject))
                        {
                            _manifestsToCheck.Add(manifestObject);
                        } else
                        {
                            Debug.LogError($"Manifest found in {plugin.Name} is missing values");
                        }
                    }

                }
                catch
                {
                    Debug.LogError("Error obtaining manifest in " + plugin.Name);
                }
            }
        }

        public static IEnumerator CheckForNewReleaseOnGithubCoroutine(Manifest manifest, Action<(bool, string)> callback)
        {
            string repoOwner, repoName;
            try
            {
                (repoOwner, repoName) = ProcessGithubLink(manifest.Git.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                callback?.Invoke((false, ""));
                yield break;
            }

            string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";
            UnityWebRequest www = UnityWebRequest.Get(apiUrl);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                dynamic releaseInfo = JsonConvert.DeserializeObject(json);

                string latestReleaseTag = releaseInfo.tag_name;

                if (manifest.Version.ToString() != latestReleaseTag)
                {
                    Debug.Log($"A newer release is available for {manifest.Name}: {latestReleaseTag}");
                    callback?.Invoke((true, latestReleaseTag));
                }
                else
                {
                    Debug.Log($"Up to date. No newer releases available for {manifest.Name} on github.");
                    callback?.Invoke((false, latestReleaseTag));
                }
            }
            else
            {
                Debug.Log($"Failed to fetch release information for {manifest.Name} on github. Response Code: {www.responseCode}");
                callback?.Invoke((false, ""));
            }
        }

        public void RefreshOutdatedPluginsUI()
        {
            RemoveOutdatedPluginsUI();
            CreateOutdatedPluginsUI();
        }

        private void RemoveOutdatedPluginsUI()
        {
            Destroy(_OutdatedPluginsUI);
        }

        private void CreateOutdatedPluginsUI()
        {
            // add a toggle if statement here if i want
            AddOutdatedPluginsUI();
            _OutdatedPluginsUI.SetActive(true);
        }

        public void AddOutdatedPluginsUI()
        {
            Transform parent = GameObject.Find("SongSelectorCanvas").transform.Find("Update checker").transform;
            _OutdatedPluginsUI = new GameObject("Automodder Outdated Plugins UI");
            _OutdatedPluginsUI.transform.parent = parent;
            _OutdatedPluginsUI.SetActive(false);

            UIHelper.AttachTransform(_OutdatedPluginsUI, 200, 65, 1.063f, -1.4f, 0, 0, 1, 0);

            Image image = _OutdatedPluginsUI.AddComponent<Image>();
            image.sprite = PersistentUI.Instance.Sprites.Background;
            image.type = Image.Type.Sliced;
            image.color = new Color(0.35f, 0.35f, 0.35f);

            UIHelper.AddLabel(_OutdatedPluginsUI.transform, "OutdatedPluginsHeader", "Outdated plugins found!", new Vector2(0, -14));

            float y = -27;
            foreach (var item in OutdatedManifests)
            {
                UIHelper.AddLabel(_OutdatedPluginsUI.transform, "OutdatedPluginsItem", $"{item.manifest.Name} - {item.manifest.Version} > {item.newVersion}", new Vector2(0, y), null, null, Color.red, 10);
                y -= 10;
            }


        }

        private static (string repoOwner, string repoName) ProcessGithubLink(string url)
        {
            string repoOwner, repoName;
            Match match = Regex.Match(url, @"https://github.com/(.*)/([^/]+)");
            if (match.Success)
            {
                repoOwner = match.Groups[1].ToString();
                repoName = match.Groups[2].ToString();
                return (repoOwner, repoName);
            }
            else
            {
                throw new Exception($"Invalid URL: {url} is not a github link");
            }
        }

        public static bool AreAllPropertiesNotNull(object obj)
        {
            // Get all properties of the class
            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Check if the property value is null
                if (property.GetValue(obj) == null)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
