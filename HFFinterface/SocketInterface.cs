using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace HFFinterface
{
    using UnityEngine;

    [BepInPlugin("org.bepinex.plugins.humanfallflat.interface", "External Socket Interface", "0.0.1")]
    [BepInProcess("Human.exe")]
    public class SocketInterface : BaseUnityPlugin
    {
        public void Start()
        {

        }

        public void Update()
        {
            HumanAPI.Checkpoint[] lvlCPs = FindObjectsOfType<HumanAPI.Checkpoint>();
            foreach (Component c in lvlCPs)
            {
                Debug.Log("name " + c.name + " type " + c.GetType() + " basetype " + c.GetType().BaseType);
                foreach (FieldInfo fi in c.GetType().GetFields())
                {
                    System.Object obj = (System.Object)c;
                    Debug.Log("fi name " + fi.Name + " val " + fi.GetValue(obj));
                }
            }
        }
    }
}
