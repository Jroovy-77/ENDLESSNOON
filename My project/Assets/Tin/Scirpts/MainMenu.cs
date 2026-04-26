using UnityEngine;
using UnityEngine.SceneManagement; // สำคัญ: ต้องมีเพื่อใช้คำสั่งเปลี่ยนฉาก

public class MainMenu : MonoBehaviour
{
    // ฟังก์ชันสำหรับกดปุ่ม Play
    public void PlayGame()
    {
        // ใส่ชื่อ Scene ที่ต้องการจะเปลี่ยนไปในเครื่องหมาย " "
        SceneManager.LoadScene("StartCutSecne");
    }
    public void Mainmenu()
    {
        // ใส่ชื่อ Scene ที่ต้องการจะเปลี่ยนไปในเครื่องหมาย " "
        SceneManager.LoadScene("StartGame"); 
    }

    // ฟังก์ชันสำหรับกดปุ่ม Leave / Quit
    public void QuitGame()
    {
        Debug.Log("Game is exiting..."); // แสดงข้อความใน Console (เพราะใน Editor เกมจะไม่ปิดจริง)
        Application.Quit(); // คำสั่งปิดเกม (จะทำงานเมื่อ Build เป็นไฟล์ .exe แล้ว)
    }
}