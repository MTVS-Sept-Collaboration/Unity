using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WebViewRenderer : MonoBehaviour
{
    public int width = 1024;
    public int height = 768;
    private Texture2D webViewTexture;

    // 외부 DLL에서 구현된 함수들
    [DllImport("WebViewPlugin")]
    private static extern void InitializeWebView(string url);

    [DllImport("WebViewPlugin")]
    private static extern void UpdateWebView();

    [DllImport("WebViewPlugin")]
    private static extern IntPtr GetTextureDataPointer();

    void Start()
    {
        // 웹뷰 초기화
        InitializeWebView("https://www.naver.com");

        // Unity 텍스처 생성
        webViewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = webViewTexture;
    }

    void Update()
    {
        // 웹뷰 업데이트
        UpdateWebView();

        // 텍스처 데이터 가져오기
        IntPtr textureDataPtr = GetTextureDataPointer();
        byte[] textureData = new byte[width * height * 4];
        Marshal.Copy(textureDataPtr, textureData, 0, textureData.Length);

        // 텍스처 업데이트
        webViewTexture.LoadRawTextureData(textureData);
        webViewTexture.Apply();
    }

    // 마우스 입력 처리 등의 추가 기능을 여기에 구현...
}