using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicaAPICalling : MonoBehaviour
{
    TokenInfo token;
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
        ReplicaPythonAPI.SampleVoice(token, text);
    }

    public void AvailableVoices() {
        ReplicaPythonAPI.AvailableVoices(token);
    }
}