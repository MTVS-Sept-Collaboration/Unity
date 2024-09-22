using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSWSoundManager : MonoBehaviour
{
    // enum, 열거형
    public enum ESoundType
    {
        // 버튼 사운드
        EFT_BTN,
        // 화면 이동 사운드
        EFT_SCENEMOVE,
        // 아바타 꾸미기 사운드
        EFT_AVATARDECO,
        // 아바타 꾸미기 완료 사운드
        EFT_DECOEND,
        // 프로필 씬 사운드
        EFT_PROFILESCENE,
        // 화면 이동 사운드
        EFT_SCENEMOVE2,
        // 게임 시작시 카운트 다운
        EFT_COUNDOWN,
        // 스코어 얻기
        EFT_GETSCORE,
        // 게임 시작할 대
        EFT_GAMESTART,
        // 게임 끝날 때 함수
        EFT_ENDINGCHEER
        //// 총1
        //EFT_GUN1,
        //// 총2
        //EFT_GUN2,
        //// 염동력 사운드
        //EFT_SKILL1,
        //// 염동력 던질 때 사운드
        //EFT_SKILLOUT1,
        //// 염동력2 사운드
        //EFT_SKILL2,
        //// 대쉬 사운드
        //EFT_DASH,
        //// 염동력3 사운드
        //EFT_SKILL3,
        //// 염동력3 던질 때 사운드
        //EFT_SKILLOUT3,
        //// 에너지총 기모음 사운드
        //EFT_ENERGYGUN,
        //// 물체 충돌시나는 소리
        //EFT_CRASH,
        //// walk 사운드
        //EFT_WALK,
        //// 날 때 소리
        //EFT_FLY,
        //// 엔딩 사운드
        //EFT_ENDING,
        //// 플레이어 히트사운드
        //EFT_HITPLAYER,
        //// 경보음 사운드
        //EFT_ALAR1,
        //// 경보음 사운드2
        //EFT_ALAR2
    }

    public enum EBgmType
    {
        // 아바타 브금
        BGM_AVAR,
        // 메인 브금
        BGM_MAIN,
        // Play 브금
        BGM_Play,
        // Playing 브금
        BGM_Playing,
        // End브금
        BGM_END
    }


    // 나를 담을 static 변수
    static JSWSoundManager instance;
    public static JSWSoundManager Get()
    {
        // 만약에 instance 가 null 이라면
        if (!instance)
        {
            // soundManager Prefab을 읽어오자

            GameObject soundManagerFactory = Resources.Load<GameObject>("SoundManager");
            // SoundManager 공장에서 SoundManager를 만들자.
            GameObject soundManager = Instantiate(soundManagerFactory);
        }

        return instance;
    }


    // audiosource
    public AudioSource eftAudio;
    public AudioSource bgmAudio;

    public AudioClip[] eftAudios;
    public AudioClip[] bgmAudios;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // 씬 전환이 되도 게임 오브젝트를 파괴하고 싶지않다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // effectSound Play 하는 함수
    public void PlayEftSound(ESoundType idx)
    {
        int audioIdx = (int)idx;
        eftAudio.PlayOneShot(eftAudios[audioIdx]);
    }

    // bgm Sound
    public void PlayBgmSound(EBgmType idx)
    {
        int bgmIdx = (int)idx;
        // 플레이할 AudioClip을 설정
        bgmAudio.clip = bgmAudios[bgmIdx];
        bgmAudio.Play();
    }

    public void StopBgmSound()
    {
        bgmAudio.Stop();
    }

    public bool MoveMainBgmSound()
    {
        return bgmAudio.clip == bgmAudios[1];
    }

    //public void AudioSourceEtc()
    //{

    //    // 일시 정지
    //    bgmAudio.Pause();
    //    // 완저 멈춤
    //    bgmAudio.Stop();
    //    // 현재 실행되고 있느 ㄴ시간
    //    float currTime = bgmAudio.time;
    //    // 시간 건너뛰기
    //    bgmAudio.time += 10;

    //}
}
