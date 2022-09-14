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

    struct Control
    {
        public float look_x;
        public float look_y;
        public bool jump;
        public bool arm_left;
        public bool arm_right;
        public bool forward;
        public bool backward;
        public bool left;
        public bool right;
    }

    class HFFInput
    {
        public static Control hffControl;

        public static void patch()
        {
            Harmony.CreateAndPatchAll(typeof(HFFInput));

            Shell.RegisterCommand("jump", (string j) => { hffControl.jump = (j[0] - '0') == 1; });
            Shell.RegisterCommand("left", (string j) => { hffControl.arm_left = (j[0] - '0') == 1; });
            Shell.RegisterCommand("right", (string j) => { hffControl.arm_right = (j[0] - '0') == 1; });
            Shell.RegisterCommand("w", (string j) => { hffControl.forward = (j[0] - '0') == 1; });
            Shell.RegisterCommand("a", (string j) => { hffControl.left = (j[0] - '0') == 1; });
            Shell.RegisterCommand("s", (string j) => { hffControl.backward = (j[0] - '0') == 1; });
            Shell.RegisterCommand("d", (string j) => { hffControl.right = (j[0] - '0') == 1; });
            Shell.RegisterCommand("x", (string x) => { hffControl.look_x = (float)Convert.ToDouble(x); });
            Shell.RegisterCommand("y", (string y) => { hffControl.look_y = (float)Convert.ToDouble(y); });
            Shell.RegisterCommand("dead", new System.Action(dead));
            Shell.RegisterCommand("reload", new System.Action(reload));
        }

        [HarmonyPatch(typeof(Input), "GetAxisRaw")]
        [HarmonyPostfix]
        public static void axis(string axisName, ref float __result)
        {
            if (axisName.Equals("mouse x"))
            {
                __result = hffControl.look_x;
            }
            else if (axisName.Equals("mouse y"))
            {
                __result = hffControl.look_y;
            }
            else if (axisName.Equals("mouse z"))
            {
                __result = 0f;
            }
        }

        [HarmonyPatch(typeof(Game), "GetKey")]
        [HarmonyPostfix]
        public static void key(KeyCode key, ref bool __result)
        {
            switch (key)
            {
                case (KeyCode.Space):
                    __result = hffControl.jump;
                    break;
                case (KeyCode.W):
                    __result = hffControl.forward;
                    break;
                case (KeyCode.A):
                    __result = hffControl.left;
                    break;
                case (KeyCode.S):
                    __result = hffControl.backward;
                    break;
                case (KeyCode.D):
                    __result = hffControl.right;
                    break;
                default:
                    break;
            }
        }

        [HarmonyPatch(typeof(Input), "GetMouseButton")]
        [HarmonyPostfix]
        public static void click(int button, ref bool __result)
        {
            switch (button)
            {
                case (0):
                    __result = hffControl.arm_left;
                    break;
                case (1):
                    __result = hffControl.arm_right;
                    break;
                default:
                    break;
            }
        }

        public static void reload()
        {
            Game.instance.RestartCheckpoint();
        }

        public static void dead()
        {
            Human.instance.MakeUnconscious();
        }
    }
}
