using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using HumanAPI;
using Multiplayer;
using UnityEngine.Rendering.PostProcessing;
using InControl;

namespace HFFinterface
{
    class HFFInputManager : MonoBehaviour
    {
        HFFInputDevice aiController = new HFFInputDevice();

        public HFFInputManager()
        {
            InputManager.AttachDevice(aiController);
        }

        public void jump()
        {
            aiController.update(InputControlType.Action1);
            StartCoroutine(jumpInternal());
        }
        private IEnumerator jumpInternal()
        {
            yield return null;
            aiController.jump();
        }
    }
}
