using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stolen from https://gamedev.stackexchange.com/a/167332
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraSize : MonoBehaviour {

    // Set this to the in-world distance between the left & right edges of your scene.
    private float sceneWidth = 420;

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Start() {
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            // Don't do this for webGL deployments because it breaks hiDPI screens
            float unitsPerPixel = sceneWidth / Screen.width;
            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            GetComponent<Camera>().orthographicSize = desiredHalfHeight;
        }
    }
}