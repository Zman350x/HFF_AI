using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using InControl;

namespace HFFinterface
{
    class HFFInputDevice : InputDevice
    {
        public HFFInputDevice() : base("AIController", false) { }

        public void update(InputControlType controlType)
        {
            this.Update(this.GetControl(controlType).UpdateTick, Time.deltaTime);
        }

        public void jump()
        {
            this.UpdateWithState(InputControlType.Action1, true, this.GetControl(InputControlType.Action1).UpdateTick, Time.deltaTime);
        }
    }
}
