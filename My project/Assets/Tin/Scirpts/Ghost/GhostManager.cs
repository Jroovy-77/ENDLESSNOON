using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public GhostRecorder playerRecorder;
    public GameObject ghostPrefab; // ใส่ mesh เหมือน player แต่ใส แบบ ghost

    public void OnPlayerDeath()
    {
        playerRecorder.StopRecording();

        // Spawn ghost แล้วเริ่ม playback
        GameObject ghost = Instantiate(ghostPrefab);
        var playback = ghost.GetComponent<GhostPlayback>();
        playback.StartPlayback(playerRecorder.recordedFrames);

        // Respawn player แล้วเริ่ม record ใหม่
        RespawnPlayer();
    }

    void RespawnPlayer()
    {
        playerRecorder.ClearRecording();
        playerRecorder.isRecording = true;
        // ... teleport player กลับ start point
    }
}