using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;

namespace HFFinterface
{
    class HFFObject : MonoBehaviour
    {
        private GameObject unityObject;

        private Bounds localBounds;
        private Vector3 location;
        private Vector3 rotation;
        private Vector3 scale;

        public HFFObject(GameObject gameObject)
        {
            unityObject = gameObject;
            location = unityObject.transform.position;
            rotation = unityObject.transform.eulerAngles;
            scale = unityObject.transform.lossyScale;
            localBounds = getLocalBounds(unityObject.GetComponent<Collider>());
        }

        public void renderBounds(GameObject cube)
        {
            GameObject visual;
            visual = UnityEngine.Object.Instantiate(cube);
            visual.transform.SetParent(null, false);
            visual.transform.localScale = Vector3.Scale(scale, localBounds.size);
            visual.transform.position = location + (Quaternion.Euler(rotation) * localBounds.center);
            visual.transform.eulerAngles = rotation;
            visual.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.66f, 0.0f, 0.33f));
            visual.SetActive(true);
        }

        private Bounds getLocalBounds(Collider collider)
        {
            if (collider is MeshCollider meshCollider)
            {
                return meshCollider.sharedMesh.bounds;
            }
            else if (collider is BoxCollider boxCollider)
            {
                return new Bounds(boxCollider.center, boxCollider.size);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                float diameter = sphereCollider.radius * 2.0f;
                Vector3 localBox = new Vector3(diameter, diameter, diameter);
                return new Bounds(sphereCollider.center, localBox);
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                float diameter = capsuleCollider.radius * 2.0f;
                Vector3 localBox = new Vector3(diameter, diameter, diameter);
                localBox[capsuleCollider.direction] = capsuleCollider.height;
                return new Bounds(capsuleCollider.center, localBox);
            }

            return new Bounds(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }
    
        public override string ToString()
        {
            string objString = "Location: " + location.ToString("F3") + " | Rotation: " + rotation.ToString("F3")
                + " | Scale: " + scale.ToString("F3");
            if (localBounds != null)
            {
                objString += " | Bounds: [" + localBounds.min.ToString("F3") + " " + localBounds.center.ToString("F3")
                    + " " + localBounds.max.ToString("F3") + "]";
            }
            return objString;
        }
    }
}
