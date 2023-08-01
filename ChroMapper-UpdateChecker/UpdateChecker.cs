using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChroMapper_UpdateChecker
{
    internal class UpdateChecker : MonoBehaviour
    {
        static UpdateChecker _instance;

        void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }

            ObtainManifests();
        }

        void ObtainManifests()
        {
            foreach (var plugin in PluginLoader.plugins)
            {
                try
                {
                    var ass = Assembly.GetAssembly(plugin.pluginInstance.GetType());
                    string? manifest = ass.GetManifestResourceNames().Where(name => name.Contains("manifest.json")).FirstOrDefault();
                    if (manifest == null)
                    {
                        Debug.Log("Manifest not found in " + plugin.Name);
                    }
                    else
                    {
                        StreamReader reader = new StreamReader(ass.GetManifestResourceStream(manifest));
                        string jsonString = reader.ReadToEnd();

                        Debug.Log(jsonString);
                    }

                }
                catch
                {
                    Debug.Log("Error obtaining manifest in " + plugin.Name);
                }
            }
        }

    }
}
