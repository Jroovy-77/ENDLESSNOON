using System.Collections.Generic;
using UnityEngine;

public class GhostPlayback : MonoBehaviour
{
    private List<GhostRecorder.FrameData> frames;
    private float startTime;
    private int currentIndex = 0;
    private bool isPlaying = false;

    public void StartPlayback(List<GhostRecorder.FrameData> recordedFrames)
    {
        frames    = recordedFrames;
        startTime = Time.time;
        currentIndex = 0;
        isPlaying = true;

        if (frames.Count > 0)
            transform.position = frames[0].position;
    }

    void Update()
    {
        if (!isPlaying || frames == null || currentIndex >= frames.Count) return;

        float elapsed      = Time.time - startTime;
        float recordStart  = frames[0].timestamp;

        // เดิน index ไปจนถึง frame ที่ตรงกับเวลาปัจจุบัน
        while (currentIndex < frames.Count - 1 &&
               frames[currentIndex + 1].timestamp - recordStart <= elapsed)
        {
            currentIndex++;
        }

        // Lerp ระหว่าง frame ปัจจุบันกับ frame ถัดไป
        if (currentIndex < frames.Count - 1)
        {
            var a = frames[currentIndex];
            var b = frames[currentIndex + 1];

            float segmentDuration = b.timestamp - a.timestamp;
            float segmentElapsed  = (elapsed - (a.timestamp - recordStart));
            float t = segmentDuration > 0 ? segmentElapsed / segmentDuration : 1f;

            transform.position = Vector3.Lerp(a.position, b.position, t);
            transform.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
        }
        else
        {
            // จบ playback
            isPlaying = false;
        }
    }
}