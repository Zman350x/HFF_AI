using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace HFFinterface
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using HarmonyLib;
    using HumanAPI;
    using Multiplayer;
    using UnityEngine.Rendering.PostProcessing;

    [BepInPlugin("org.bepinex.plugins.humanfallflat.interface", "External Socket Interface", "0.0.1")]
    [BepInProcess("Human.exe")]
    public class SocketInterface : BaseUnityPlugin
    {
        static SocketInterface instance;

        //GameObject filters
        static Type[] include = { typeof(Collider) };
        static Type[] exclude = { typeof(Hint), typeof(Ambience), typeof(Reverb), typeof(MusicPlayer), typeof(PostProcessVolume) };
        static string[] special_exclude = { "achievement" };

        public void Start()
        {
            instance = this;
            Harmony.CreateAndPatchAll(typeof(SocketInterface));
            Shell.RegisterCommand("check", new System.Action(check));
        }

        public void Update()
        {
            
        }

        [HarmonyPatch(typeof(SteamRichPresence), "SetGameMode")]
        [HarmonyPostfix]
        public static void SetGameMode(string token)
        {
            if (token.Equals("#Local_Level"))
            {
                Shell.Print("LEVEL LOADED");
                check();
            }
        }

        public static void check()
        {
            List<GameObject> rootObjects = new List<GameObject>();
            Scene scene = SceneManager.GetActiveScene();
            scene.GetRootGameObjects(rootObjects);
            using (StreamWriter writer = new StreamWriter("hierarchy.txt", false))
            {
                for (int i = 0; i < rootObjects.Count; ++i)
                {
                    GameObject gameObject = rootObjects[i];
                    //function to iterate over children
                    DumpGameObject(gameObject, writer, "");
                }
            }
        }

        private static void DumpGameObject(GameObject gameObject, StreamWriter writer, string parentName)
        {
            if (filterGameObject(gameObject))
            {
                string path = parentName + gameObject.name;

                string components = "";
                List<Component> rawComponents = new List<Component>(gameObject.GetComponents<Component>());
                foreach (Component component in rawComponents)
                {
                    components += ", " + component.GetType().ToString();
                }

                writer.WriteLine(path + "\t" + components.ToString());
                foreach (Transform child in gameObject.transform)
                {
                    if (!path.EndsWith("/"))
                        path = path + "/";
                    DumpGameObject(child.gameObject, writer, path);
                }
            }
        }

        private static bool filterGameObject(GameObject gameObject)
        {
            bool filtered = true;
            /*foreach (Type type in include)
            {
                if (gameObject.GetComponentInChildren(type) != null) { filtered = true; break; }
            }
            if (filtered)
            {
                foreach (Type type in exclude)
                {
                    if (gameObject.GetComponent(type) != null) { filtered = false; return false; }
                }
                /*foreach (string in_type in special_exclude)
                {
                    List<Component> rawComponents = new List<Component>(gameObject.GetComponents<Component>());
                    foreach (Component component in rawComponents)
                    {
                        if (component.GetType().ToString().ToLower().Contains(in_type)) { filtered = false; return false; }
                    }
                }
            }*/
            return filtered;
        }
    }
}
