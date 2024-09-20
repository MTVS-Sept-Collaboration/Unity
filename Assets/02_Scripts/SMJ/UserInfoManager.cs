using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Text;
using System.Net;
using System.Globalization;
using TMPro;
using UnityEditor.PackageManager.Requests;

public class UserInfoManager : MonoBehaviour
{
    public Button loginButton;
    public Button registerButton;
    public Button updateInfoButton;
    private TMP_InputField usernameRegisterInput;
    private TMP_InputField passwordRegisterInput;
    private TMP_InputField usernameInput;
    private TMP_InputField passwordInput;
    private TMP_InputField nicknameInput;
    private TMP_InputField birthdayInput;
    private TMP_InputField heightInput;
    private TMP_InputField weightInput;
    private readonly string LoginUrl = "https://125.132.216.190:12502/api/login";
    private readonly string RegisterUrl = "https://125.132.216.190:12502/api/register";
    private readonly string UserInfoUrl = "https://125.132.216.190:12502/api/user/";

    public delegate void StatusChanged(bool status);
    public event StatusChanged OnLoginStatusChanged;
    public event StatusChanged OnRegisterStatusChanged;
    public event StatusChanged OnUpdateInfoStatusChanged;

    void Start()
    {
        usernameInput = LoginUIManager.instance.usernameInput;
        passwordInput = LoginUIManager.instance.passwordInput;
        usernameRegisterInput = LoginUIManager.instance.usernameRegitsterInput;
        passwordRegisterInput = LoginUIManager.instance.passwordRegitsterInput;
        nicknameInput = LoginUIManager.instance.playerNameInput.GetComponent<TMP_InputField>();
        birthdayInput = LoginUIManager.instance.playerBirthInput.GetComponent<TMP_InputField>();
        heightInput = LoginUIManager.instance.playerHeightInput.GetComponent<TMP_InputField>();
        weightInput = LoginUIManager.instance.playerWeightInput.GetComponent<TMP_InputField>();

        loginButton.onClick.AddListener(OnLoginButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        updateInfoButton.onClick.AddListener(OnUpdateInfoButtonClick);

        // SSL 인증서 검증 우회 설정
        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    }

    void OnLoginButtonClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("모든 필드를 입력해주세요.");
            // UI에 에러 메시지 표시
            return;
        }
        else
        {
            StartCoroutine(LoginCoroutine(username, password));
        }
    }

    void OnRegisterButtonClick()
    {
        string username = usernameRegisterInput.text;
        string password = passwordRegisterInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("모든 필드를 입력해주세요.");
            // UI에 에러 메시지 표시
            return;
        }
        else
        {
            StartCoroutine(RegisterCoroutine(username, password));
        }
    }

    void OnUpdateInfoButtonClick()
    {
        string nickname = nicknameInput.text;
        string birthday = birthdayInput.text;
        string height = heightInput.text;
        string weight = weightInput.text;
        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(birthday) || 
            string.IsNullOrEmpty(height) || string.IsNullOrEmpty(weight))
        {
            Debug.LogError("모든 필드를 입력해주세요.");
            // UI에 에러 메시지 표시
            return;
        }
        else
        {
            StartCoroutine(UpdateUserInfoCoroutine(nickname, birthday, height, weight));
        }
    }

    IEnumerator LoginCoroutine(string username, string password)
    {
        string jsonBody = JsonUtility.ToJson(new LoginData { userName = username, password = password });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(LoginUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new BypassCertificate();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("로그인 실패: " + www.error);
                OnLoginStatusChanged?.Invoke(false);
            }
            else
            {
                string responseBody = www.downloadHandler.text;
                Debug.Log($"로그인 응답: {responseBody}");

                try
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(responseBody);

                    if (response != null && !string.IsNullOrEmpty(response.jwtToken))
                    {
                        PlayerPrefs.SetString("userId", response.userId.ToString());
                        PlayerPrefs.SetString("jwtToken", response.jwtToken);
                        PlayerPrefs.Save();

                        Debug.Log($"로그인 성공. 사용자 ID: {response.userId}, 토큰: {response.jwtToken}");
                        OnLoginStatusChanged?.Invoke(true);
                        StartCoroutine(GetUserInfoCoroutine(response.jwtToken, response.userId));
                    }
                    else
                    {
                        Debug.LogError("유효하지 않은 로그인 응답");
                        OnLoginStatusChanged?.Invoke(false);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON 파싱 오류: {e.Message}");
                    OnLoginStatusChanged?.Invoke(false);
                }
            }
        }
    }

    IEnumerator RegisterCoroutine(string username, string password)
    {
        RegisterData registerData = new RegisterData { userName = username, password = password, role = "USER" };
        string jsonBody = JsonUtility.ToJson(registerData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        Debug.Log("회원가입 요청 데이터: " + jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(RegisterUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new BypassCertificate();

            yield return www.SendWebRequest();

            Debug.Log("회원가입 응답 코드: " + www.responseCode);

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("회원가입 실패: " + www.error);
                Debug.LogError("에러 응답: " + www.downloadHandler.text);
                OnRegisterStatusChanged?.Invoke(false);
            }
            else
            {
                string responseBody = www.downloadHandler.text;
                Debug.Log("회원가입 성공. 서버 응답: " + responseBody);
                OnRegisterStatusChanged?.Invoke(true);
                StartCoroutine(LoginCoroutine(username, password));
            }
        }
    }

    IEnumerator GetUserInfoCoroutine(string jwtToken, string _userId)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(UserInfoUrl + _userId))
        {
            www.SetRequestHeader("Authorization", "Bearer " + jwtToken);
            www.certificateHandler = new BypassCertificate();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"사용자 정보 가져오기 실패: {www.error}");
            }
            else
            {
                string responseBody = www.downloadHandler.text;
                Debug.Log($"사용자 정보 응답: {responseBody}");

                try
                {
                    UpdateUserData userInfo = JsonUtility.FromJson<UpdateUserData>(responseBody);
                    AvatarInfo.instance.NickName = userInfo.nickName;
                    AvatarInfo.instance.Birthday = userInfo.birthday;
                    AvatarInfo.instance.Height = userInfo.height;
                    AvatarInfo.instance.Weight = userInfo.weight;
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON 파싱 오류: {e.Message}");
                }
            }
        }
    }

    IEnumerator UpdateUserInfoCoroutine(string _nickname, string _birthday, string _height, string _weight)
    {
        yield return new WaitForSeconds(0.1f);

        string jwtToken = PlayerPrefs.GetString("jwtToken");
        string userId = PlayerPrefs.GetString("userId");
        string url = UserInfoUrl + userId;
        string formattedBirthday = FormatDate(_birthday);

        string jsonBody = JsonUtility.ToJson(new UpdateUserData
        {
            nickName = _nickname,
            birthday = formattedBirthday,
            height = float.Parse(_height),
            weight = float.Parse(_weight)
        });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(url, "PUT"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + jwtToken);
            www.certificateHandler = new BypassCertificate();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("사용자 정보 수정 실패: " + www.error);
                OnUpdateInfoStatusChanged?.Invoke(false);
            }
            else
            {
                string responseBody = www.downloadHandler.text;
                Debug.Log("사용자 정보 수정 성공. 서버 응답: " + responseBody);
                OnUpdateInfoStatusChanged?.Invoke(true);
            }
        }
    }
    private string FormatDate(string inputDate)
    {
        if (string.IsNullOrEmpty(inputDate) || inputDate.Length != 8)
        {
            Debug.LogError("잘못된 날짜 형식입니다.");
            return inputDate; // 오류 시 원본 반환
        }

        try
        {
            DateTime date = DateTime.ParseExact(inputDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            return date.ToString("yyyy-MM-dd");
        }
        catch (Exception e)
        {
            Debug.LogError($"날짜 변환 중 오류 발생: {e.Message}");
            return inputDate; // 오류 시 원본 반환
        }
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("id");
        OnLoginStatusChanged?.Invoke(false);
    }
}

[Serializable]
public class LoginData
{
    public string userName;
    public string password;
}

[Serializable]
public class RegisterData
{
    public string userName;
    public string password;
    public string role;
}

[Serializable]
public class UpdateUserData
{
    public string nickName;
    public string birthday;
    public float height;
    public float weight;
}

[Serializable]
public class LoginResponse
{
    public string userId;
    public string jwtToken;
}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}