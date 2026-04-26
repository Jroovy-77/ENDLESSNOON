using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    [System.Serializable]
    public struct FrameData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float timestamp;
    }

    public List<FrameData> recordedFrames = new List<FrameData>();
    public bool isRecording = true;

    void Update()
    {
        if (!isRecording) return;

        recordedFrames.Add(new FrameData
        {
            position  = transform.position,
            rotation  = transform.rotation,
            timestamp = Time.time
        });
    }

    public void StopRecording() => isRecording = false;
    public void ClearRecording() => recordedFrames.Clear();
}