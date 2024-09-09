using Gpm.WebView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Gpm.WebView.GpmWebViewCallback;
using static Gpm.WebView.GpmWebViewRequest;

public class WebviewManager : MonoBehaviour
{
    public string URL = "https://naver.com/";
    public RawImage webViewDisplay;
    public int width = 1008;
    public int height = 567;

    private bool isWebViewVisible = false;
    private Texture2D webViewTexture;

    public void ShowUrl()
    {
        if (!isWebViewVisible)
        {
            StartCoroutine(LoadWebPage());
            isWebViewVisible = true;
            webViewDisplay.gameObject.SetActive(true);
        }
    }

    public void HideUrl()
    {
        if (isWebViewVisible)
        {
            StopAllCoroutines();
            webViewDisplay.gameObject.SetActive(false);
            isWebViewVisible = false;
        }
    }

    private IEnumerator LoadWebPage()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // 웹 페이지의 내용을 텍스처로 변환
                webViewTexture = new Texture2D(width, height);
                webViewTexture.LoadImage(www.downloadHandler.data);

                // RawImage에 텍스처 적용
                webViewDisplay.texture = webViewTexture;
                webViewDisplay.SetNativeSize();
            }
        }
    }

    private void OnDestroy()
    {
        if (webViewTexture != null)
        {
            Destroy(webViewTexture);
        }
    }
}