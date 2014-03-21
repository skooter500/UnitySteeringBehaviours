using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// From https://developer.oculusvr.com/forums/viewtopic.php?f=37&t=3308

namespace BGE
{

    public class VRCamera
    {
        public string name;
        public GameObject camera;
        public Camera leftlense;
        public Camera rightlense;

        public VRCamera(string Name, GameObject Cam)
        {
            name = Name;
            camera = Cam;
            leftlense = camera.transform.FindChild("CameraLeft").camera;
            rightlense = camera.transform.FindChild("CameraRight").camera;
        }
    }

    public static class CameraManager
    {
        private static int currentScene;
        private static bool initialized;
        private static List<VRCamera> VRCameras;

        public static bool isInitialized()
        {
            if (!initialized)
                return false;

            if (currentScene != Application.loadedLevel)
                return false;

            return true;
        }

        public static void Initialize(bool force = false)
        {
            if (!isInitialized() || force)
            {
                VRCameras = new List<VRCamera>();

                foreach (Camera camera in GameObject.FindObjectsOfType(typeof(Camera)))
                {
                    if ((camera.name == "CameraLeft") || (camera.name == "CameraRight"))
                    {
                        if (VRCameraExistsInList(camera))
                            continue;

                        VRCameras.Add(new VRCamera(camera.transform.parent.name, camera.transform.parent.gameObject));
                    }
                }

                initialized = true;
                currentScene = Application.loadedLevel;
            }
        }

        public static bool VRCameraExistsInList(Camera camera)
        {
            foreach (VRCamera vrcam in VRCameras)
            {
                if (vrcam.name == camera.transform.parent.name)
                {
                    return true;
                }
            }

            return false;
        }

        public static GameObject FindVRCamera(string search)
        {
            foreach (VRCamera vrcam in VRCameras)
            {
                if (vrcam.name == "OVR" + search)
                    return vrcam.camera;
            }

            Debug.LogError("A Camera was referenced that does not have a VR pair: OVR" + search);
            return null;
        }

        public static void ToggleCamera(Camera camera, bool toggle)
        {
            if (!Params.riftEnabled)
            {
                camera.enabled = toggle;
                return;
            }

            Initialize();

            if ((FindVRCamera(camera.name)) == null)
                Initialize(true);

            FindVRCamera(camera.name).SetActive(toggle);
        }

        public static GameObject GetCurrentCamera(GameObject camera)
        {
            if (!Params.riftEnabled)
                return camera;

            Initialize();

            if ((FindVRCamera(camera.name)) == null)
                Initialize(true);

            return FindVRCamera(camera.name);
        }

    }
}
