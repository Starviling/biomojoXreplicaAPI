using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TokenAuthorization;
using UnityEngine;
using UnityEngine.UI;

public class ReplicaAPICalling : MonoBehaviour
{
    TokenInformation token;

    public InputField account;
    public InputField password;
    public InputField text;

    private static readonly HttpClient client = new HttpClient();

    // Calling Replica Authentication
    public void Authenticate()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        processAuthenticatAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Debug.Log("Authentication ran.");
    }

    private async Task processAuthenticatAsync()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        /*        client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");*/

        var values = new Dictionary<string, string>
        {
            { "client_id", account.text },
            { "secret", password.text }
        };
        var payload = new FormUrlEncodedContent(values);

        HttpResponseMessage response = await client.PostAsync("https://api.replicastudios.com/auth", payload);
        var responseString = await response.Content.ReadAsStringAsync();

        Debug.Log(responseString);
    }
}
