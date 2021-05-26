/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TokenAuthorization;
using UnityEngine.Networking;

public class ReplicaAPICalling : MonoBehaviour
{
    TokenInformation token;
    public string account;
    public string password;
    public string text;

    // Start is called before the first frame update
    void Start()
    {
        token = null;
    }

    public void Authenticate()
    {
        token = ReplicaPythonAPI.Authenticate(account, password);
        Debug.Log(token.access_token);
        Debug.Log(token.refresh_token);
    }

    public void SampleVoice() {
        StartCoroutine(VoiceCoroutine(ReplicaPythonAPI.SampleVoice(token, text)));
    }

    public void AvailableVoices() {
        ReplicaPythonAPI.AvailableVoices(token);
    }

    IEnumerator VoiceCoroutine(string url)
    {
        Debug.Log(url);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            DownloadHandlerAudioClip dHA = new DownloadHandlerAudioClip(url, AudioType.WAV);
            dHA.streamAudio = true;
            www.downloadHandler = dHA;
            www.SendWebRequest();
            while (www.downloadProgress < 1.0)
            {
                Debug.Log("Download progress: " + www.downloadProgress);
                yield return new WaitForSeconds(.1f);
            }
            if (www.isNetworkError)
            {
                Debug.LogError("Connection failed.");
            }
            else
            {
                Debug.Log("Playing Audio");
                AudioSource audio = this.gameObject.AddComponent<AudioSource>();
                audio.clip = dHA.audioClip;
                audio.volume = 1;
                audio.loop = false;
                audio.Play();
                yield return new WaitForSeconds(audio.clip.length);
                Destroy(audio);
            }
        }
    }
}*/