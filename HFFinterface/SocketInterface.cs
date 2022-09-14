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

        bool mod_enabled;

        GameObject cubePrimitive;
        
        //GameObject filters
        static Type[] include = { typeof(Collider) };
        static Type[] exclude = { typeof(Hint), typeof(Ambience), typeof(Reverb), typeof(AudioSource), typeof(MusicPlayer),
                                  typeof(PostProcessVolume), typeof(Light), typeof(LightProbeGroup), typeof(NarrativeBlock),
                                  typeof(NarrativeForceTrigger), typeof(CloudBox), typeof(Sound2), typeof(SoundManagerPrefab) };
        static string[] special_exclude = { "achievement", "tutorial" };

        Dictionary<int, HFFObject> levelObjects = new Dictionary<int, HFFObject>();

        public void Start()
        {
            instance = this;
            mod_enabled = false;
            Harmony.CreateAndPatchAll(typeof(SocketInterface));
            //Shell.RegisterCommand("check", new System.Action(check));
            Shell.RegisterCommand("enable", () => { if (!mod_enabled) { HFFInput.patch(); mod_enabled = true; } } );

            /*cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer objRenderer = cubePrimitive.GetComponent<Renderer>();
            StandardShaderUtils.ChangeRenderMode(objRenderer.material, StandardShaderUtils.BlendMode.Transparent);
            objRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            objRenderer.receiveShadows = false;
            cubePrimitive.GetComponent<Collider>().enabled = false;
            DontDestroyOnLoad(cubePrimitive);
            cubePrimitive.SetActive(false);*/
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
                clearLevelData();
                //instance.Invoke("check", 5);
            }
            else
            {
                clearLevelData();
            }
        }

        public static void clearLevelData()
        {
            instance.levelObjects.Clear();
        }

        public void check()
        {
            clearLevelData();
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
            foreach (HFFObject obj in instance.levelObjects.Values)
            {
                obj.renderBounds(instance.cubePrimitive);
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

                writer.WriteLine(path + "\t" + components.ToString() + "\t" + gameObject.GetInstanceID());

                if (gameObject.GetComponent<Collider>() != null)
                {
                    instance.levelObjects.Add(gameObject.GetInstanceID(), new HFFObject(gameObject));
                    writer.WriteLine(instance.levelObjects[gameObject.GetInstanceID()]);
                }

                writer.WriteLine("");

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
            foreach (Type type in include)
            {
                if (gameObject.GetComponentInChildren(type) == null) { return false; }
            }
            foreach (Type type in exclude)
            {
                if (gameObject.GetComponent(type) != null) { return false; }
            }
            foreach (string in_type in special_exclude)
            {
                List<Component> rawComponents = new List<Component>(gameObject.GetComponents<Component>());
                foreach (Component component in rawComponents)
                {
                    if (component.GetType().ToString().ToLower().Contains(in_type)) { return false; }
                }
            }

            return true;
        }
    }

    public static class StandardShaderUtils
    {
        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    standardShaderMaterial.SetFloat("_Mode", 0.0f);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    standardShaderMaterial.SetFloat("_Mode", 1.0f);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    standardShaderMaterial.SetFloat("_Mode", 2.0f);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    standardShaderMaterial.SetFloat("_Mode", 3.0f);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
            }
        }
    }
}
