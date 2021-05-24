using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenInfo
{
    public string access_token {get; set;}
    public string refresh_token {get; set;}
    
    public TokenInfo(string access, string refresh) {
        access_token = access;
        refresh_token = refresh;
    }
}
