using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TokenAuthorization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ReplicaAPICalling : MonoBehaviour
{
    TokenInformation token;

    public InputField account;
    public InputField password;
    public InputField text;
    public string speaker_id = "c4fe46c4-79c0-403e-9318-ffe7bd4247dd";

    private static readonly HttpClient client = new HttpClient();

    // Calling Replica Authentication
    public async void Authenticate()
    {
        await processAuthenticatAsync();
        Debug.Log("Authentication ran.");
    }

    // Calls POST request to Replica to get Authentication token
    private async Task processAuthenticatAsync()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

        var values = new Dictionary<string, string>
        {
            { "client_id", account.text },
            { "secret", password.text }
        };
        var payload = new FormUrlEncodedContent(values);

        HttpResponseMessage response = await client.PostAsync("https://api.replicastudios.com/auth", payload);
        var responseString = await response.Content.ReadAsStringAsync();

        token = JsonConvert.DeserializeObject<TokenInformation>(responseString);
        Debug.Log(token.access_token);
    }

    // Calling Replica Speech
    public async void SampleVoice()
    {
        await processVoiceAsync();
    }

    // Calls the GET request to Replica to get the link to voice clip
    private async Task processVoiceAsync()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);

        string requestParams = string.Empty;
        List<KeyValuePair<string, string>> voiceDetails = new List<KeyValuePair<string, string>>();
        // Converting Request Params to Key Value Pair.  
        voiceDetails.Add(new KeyValuePair<string, string>("txt", text.text));
        voiceDetails.Add(new KeyValuePair<string, string>("speaker_id", speaker_id));

        // URL Request Query parameters.  
        requestParams = new FormUrlEncodedContent(voiceDetails).ReadAsStringAsync().Result;
        Debug.Log(requestParams);

        UriBuilder uriBuilder = new UriBuilder("https://api.replicastudios.com/speech");
        uriBuilder.Query = requestParams;
        Uri uri = uriBuilder.Uri;

        Debug.Log(uri.ToString());
        HttpResponseMessage response = await client.GetAsync(uri);
        var responseString = await response.Content.ReadAsStringAsync();

        Debug.Log(responseString);
        VoiceClip voice = JsonConvert.DeserializeObject<VoiceClip>(responseString);
        Debug.Log(voice.Url);
        StartCoroutine(VoiceCoroutine(voice.Url));
    }

    // Coroutine to play the audio of voice
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
}
