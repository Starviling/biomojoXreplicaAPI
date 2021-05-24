using System.Collections;
using System.Collections.Generic;
using Python.Runtime;
using UnityEditor.Scripting.Python;

public class ReplicaPythonAPI
{
    // Calling Replica Authentication
    public static TokenInfo Authenticate(string client_id, string secret)
    {
        PythonRunner.EnsureInitialized();
        using (Py.GIL()) {
            try {
                // Import modules for Python
                dynamic sys = PythonEngine.ImportModule("sys");
                dynamic requests = PythonEngine.ImportModule("requests");
                dynamic json = PythonEngine.ImportModule("json");

                UnityEngine.Debug.Log($"python version: {sys.version}");

                // Headers has to be a dictionary
                PyDict headers = new PyDict();
                headers["Content-Type"] = new PyString("application/x-www-form-urlencoded");
                string payload = "client_id=" + client_id + "&" + "secret=" + secret;
                dynamic r = requests.post("https://api.replicastudios.com/auth", headers: headers, data: payload);

                return(new TokenInfo((string)r.json()["access_token"], (string)r.json()["refresh_token"]));
            } catch(PythonException e) {
                UnityEngine.Debug.LogException(e);
            }
            return(null);
        }
    }

    public static void SampleVoice(TokenInfo token, string voiceLine)
    {
        PythonRunner.EnsureInitialized();
        using (Py.GIL()) {
            try {
                // Import modules for Python
                dynamic requests = PythonEngine.ImportModule("requests");
                dynamic json = PythonEngine.ImportModule("json");

                // Headers has to be a dictionary
                PyDict headers = new PyDict();
                headers["Authorization"] = new PyString("Bearer " + token.access_token);
                PyDict voiceDetails = new PyDict();
                voiceDetails["txt"] = new PyString(voiceLine);
                voiceDetails["speaker_id"] = new PyString("c4fe46c4-79c0-403e-9318-ffe7bd4247dd");

                UnityEngine.Debug.Log(json.dumps(voiceDetails));
                // Py.kw() is supposed to be equivalent to {param:} param, but isn't
                // Not sure why exactly, but using Py.kw() on both here fixes it and allows it to recognize the speaker_id
                dynamic r = requests.get("https://api.replicastudios.com/speech", Py.kw("headers", headers), Py.kw("params", voiceDetails));

                UnityEngine.Debug.Log(json.dumps(r.json()));
            } catch(PythonException e) {
                UnityEngine.Debug.LogException(e);
            }
        }
    }

    public static string AvailableVoices(TokenInfo token)
    {
        PythonRunner.EnsureInitialized();
        using (Py.GIL()) {
            try {
                // Import modules for Python
                dynamic requests = PythonEngine.ImportModule("requests");
                dynamic json = PythonEngine.ImportModule("json");

                // Headers has to be a dictionary
                PyDict headers = new PyDict();
                headers["Authorization"] = new PyString("Bearer " + token.access_token);
                dynamic r = requests.get("https://api.replicastudios.com/voice", headers: headers);

                UnityEngine.Debug.Log(json.dumps(r.json()));
                return json.dumps(r.json());
            } catch(PythonException e) {
                UnityEngine.Debug.LogException(e);
            }
            return null;
        }
    }
}
