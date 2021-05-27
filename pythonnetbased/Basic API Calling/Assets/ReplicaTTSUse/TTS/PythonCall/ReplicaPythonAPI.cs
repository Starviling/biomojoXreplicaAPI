using Python.Runtime;
using System.Collections;
using TokenAuthorization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ReplicaPythonAPI : MonoBehaviour
{
    TokenInformation token;

    public InputField account;
    public InputField password;
    public InputField text;

    public string speaker_id = "c4fe46c4-79c0-403e-9318-ffe7bd4247dd";

    void Start()
    {
        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/python-3.9.5-embed-amd64/python39.dll";
        PythonEngine.Initialize(mode: ShutdownMode.Reload);
    }

    public void OnApplicationQuit()
    {
        if (PythonEngine.IsInitialized)
        {
            PythonEngine.Shutdown(ShutdownMode.Reload);
        }
    }

    // Calls POST request to Replica to get Authentication token
    public void Authenticate()
    {
        using (Py.GIL()) {
            try {
                // Import modules for Python
                dynamic sys = PyModule.Import("sys");
                dynamic requests = PyModule.Import("requests");
                dynamic json = PyModule.Import("json");

                Debug.Log($"python version: {sys.version}");

                // Headers has to be a dictionary
                PyDict headers = new PyDict();
                headers["Content-Type"] = new PyString("application/x-www-form-urlencoded");
                string payload = "client_id=" + account.text + "&" + "secret=" + password.text;
                dynamic r = requests.post("https://api.replicastudios.com/auth", headers: headers, data: payload);

                token = new TokenInformation((string)r.json()["access_token"], (string)r.json()["refresh_token"]);
                Debug.Log((string)r.json()["access_token"]);
            } catch(PythonException e) {
                UnityEngine.Debug.LogException(e);
            }
        }
        Debug.Log("Authentication ran.");
    }

    // Calls the GET request to Replica to get the link to voice clip
    public void SampleVoice()
    {
        using (Py.GIL()) {
            try {
                // Import modules for Python
                dynamic requests = PyModule.Import("requests");
                dynamic json = PyModule.Import("json");

                // Headers has to be a dictionary
                PyDict headers = new PyDict();
                headers["Authorization"] = new PyString("Bearer " + token.access_token);
                PyDict voiceDetails = new PyDict();
                voiceDetails["txt"] = new PyString(text.text);
                voiceDetails["speaker_id"] = new PyString(speaker_id);

                // Py.kw() is supposed to be equivalent to {param:} param, but isn't
                // Not sure why exactly, but using Py.kw() on both here fixes it and allows it to recognize the speaker_id
                dynamic r = requests.get("https://api.replicastudios.com/speech", Py.kw("headers", headers), Py.kw("params", voiceDetails));

                Debug.Log(json.dumps(r.json()["url"]));
                StartCoroutine(VoiceCoroutine((string) json.dumps(r.json()["url"]).strip('"')));
            } catch(PythonException e) {
                Debug.LogException(e);
            }
        }
    }

    // Displays the available voices
    public void AvailableVoices()
    {
        using (Py.GIL()) {
            try {
                // Import modules for Python
                dynamic requests = PyModule.Import("requests");
                dynamic json = PyModule.Import("json");

                // Headers has to be a dictionary
                PyDict headers = new PyDict();
                headers["Authorization"] = new PyString("Bearer " + token.access_token);
                dynamic r = requests.get("https://api.replicastudios.com/voice", headers: headers);

                Debug.Log(json.dumps(r.json()));
            } catch(PythonException e) {
                Debug.LogException(e);
            }
        }
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
