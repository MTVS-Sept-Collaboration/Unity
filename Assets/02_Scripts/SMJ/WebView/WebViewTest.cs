﻿using UnityEngine;

public class WebViewTest : MonoBehaviour
{
    public void OpenLoginPage()
    {
        //가지고 있는 토큰 값을 서버에 post로 보내서 유효하고 같은 토큰인지 확인
        //같고 유효하다면 nickname, weight, height, birthday 받아서 동기화
        string url = "https://192.168.0.183:8080/oauth2/authorization/kakao";
        Application.OpenURL(url);
    }

    public void OnLoginButtonClick()
    {
        OpenLoginPage();
    }
}