using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;

public class WebViewTest : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern int SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_SHOWNORMAL = 1;

    public void OpenLoginPage()
    {
        string url = "https://172.18.144.1:8080/oauth2/authorization/kakao";
        Application.OpenURL(url);

        // 유니티 창을 다시 포커스
        FocusUnityWindow();
    }

    private void FocusUnityWindow()
    {
        Process currentProcess = Process.GetCurrentProcess();
        IntPtr unityHWND = currentProcess.MainWindowHandle;

        if (unityHWND != IntPtr.Zero)
        {
            ShowWindow(unityHWND, SW_SHOWNORMAL);
            SetForegroundWindow(unityHWND);
        }
    }

    // 이 메서드를 호출하여 로그인 결과를 처리합니다
    public void HandleLoginResult(string result)
    {
        // 여기서 로그인 결과를 처리합니다
        UnityEngine.Debug.Log("Login result: " + result);

        // 예: 결과에 따라 다른 처리를 할 수 있습니다
        if (result.Contains("success"))
        {
            // 로그인 성공 처리
            UnityEngine.Debug.Log("Login successful!");
            // 여기에 로그인 성공 후의 로직을 추가하세요
        }
        else
        {
            // 로그인 실패 처리
            UnityEngine.Debug.Log("Login failed.");
            // 여기에 로그인 실패 시의 로직을 추가하세요
        }
    }

    // Unity UI 버튼에 연결할 수 있는 메서드
    public void OnLoginButtonClick()
    {
        OpenLoginPage();
    }
}