using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class FirebaseManager : MonoBehaviour
{
    private const string WEB_API_KEY = "AIzaSyB-FDd8lVZC1RmSUKVOr6JaTS2BSWixt7g";
    
    private const string DATABASE_LINK = "https://pawsarena-b05de-default-rtdb.firebaseio.com/";
    private const string USERS_LINK = DATABASE_LINK+"users/";
    
    public static FirebaseManager Instance;

    private string userLocalId;
    private string token;

    public string UserLocalId => userLocalId;
    public string Token => token;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Authenticate(string _principal,Action _callBack)
    {
        string _email = GetEmail(_principal);
        string _password = GetPassword(_principal);
        
        string _loginParms = "{\"email\":\"" + _email + "\",\"password\":\"" + _password +
                             "\",\"returnSecureToken\":true}";

        StartCoroutine(Post("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + WEB_API_KEY,
            _loginParms, (_result) =>
            {
                SignInResponse _signInResponse = JsonConvert.DeserializeObject<SignInResponse>(_result);
                userLocalId = _signInResponse.LocalId;
                token = _signInResponse.IdToken;
                _callBack?.Invoke();
            }, (_result) =>
            {
                Register(_callBack, _loginParms);
            }, false));
    }

    private string GetEmail(string _inputString)
    {
        if (string.IsNullOrEmpty(_inputString))
            return string.Empty;
            
        string _hash = GenerateHash(_inputString);
        string _username = _hash.Substring(0, 12).ToLower();
        
        if (_inputString.Length >= 6)
        {
            _username += "_" + _inputString.Substring(0, 6).ToLower();
        }
        
        int _numericSuffix = HexToInt(_hash.Substring(20, 4)) % 10000;
        _username += _numericSuffix.ToString();
        
        return _username + "@pawsarena.com";
    }

    private string GetPassword(string _inputString)
    {
        if (string.IsNullOrEmpty(_inputString))
            return string.Empty;
            
        string _hash = GenerateHash(_inputString);
        
        string _uppercasePart = _hash.Substring(0, 6).ToUpper();
        string _lowercasePart = _hash.Substring(6, 6).ToLower();
        string _numberPart = _hash.Substring(12, 6);
        
        char[] _specialChars = {'!', '@', '#', '$', '%', '&', '*', '?', '+', '=', '-', '_', '<', '>', '~', '^'};
        StringBuilder _specialCharPart = new StringBuilder();
        for (int _i = 18; _i < 24; _i++)
        {
            int _charIndex = HexToInt(_hash[_i].ToString()) % _specialChars.Length;
            _specialCharPart.Append(_specialChars[_charIndex]);
        }
        
        string _secondHash = GenerateHash(_hash);
        
        string _moreLowercase = _secondHash.Substring(0, 3).ToLower();
        string _moreUppercase = _secondHash.Substring(3, 3).ToUpper();
        
        return _uppercasePart + _specialCharPart + _lowercasePart + _numberPart + _moreUppercase + _moreLowercase;
    }
    
    string GenerateHash(string _input)
    {
        StringBuilder _result = new StringBuilder(64);
        
        int _seed = 0;
        foreach (char _c in _input)
        {
            _seed = (_seed * 31 + _c) & 0x7FFFFFFF;
        }
        
        UnityEngine.Random.State _originalState = UnityEngine.Random.state;
        UnityEngine.Random.InitState(_seed);
        
        for (int _i = 0; _i < 32; _i++)
        {
            int _val = UnityEngine.Random.Range(0, 16);
            _result.Append(_val.ToString("x"));
        }
        
        UnityEngine.Random.state = _originalState;
        
        return _result.ToString();
    }
    
    private int HexToInt(string _hex)
    {
        int _result = 0;
        foreach (char _c in _hex)
        {
            _result *= 16;
            if (_c >= '0' && _c <= '9')
                _result += _c - '0';
            else if (_c >= 'a' && _c <= 'f')
                _result += 10 + _c - 'a';
            else if (_c >= 'A' && _c <= 'F')
                _result += 10 + _c - 'A';
        }
        return _result;
    }
    
    private void Register(Action _callBack, string _parms)
    {
        StartCoroutine(Post("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + WEB_API_KEY, _parms,
            (_result) =>
            {
                RegisterResponse _registerResult = JsonConvert.DeserializeObject<RegisterResponse>(_result);
                userLocalId = _registerResult.LocalId;
                token = _registerResult.IdToken;
                _callBack?.Invoke();
            }, (_result) =>
            {
                Debug.Log(_result);
            }));
    }

    public void GetPlayerData(Action<string> _callBack)
    {
        string _url = $"{USERS_LINK}{userLocalId}.json?auth={token}";
        StartCoroutine(GetRequest(_url, _callBack));
    }
    
    public void SavePlayerData(string _json)
    {
        string _url = $"{USERS_LINK}/{userLocalId}.json?auth={token}";
        StartCoroutine(PutRequest(_url, _json));
    }
    
    private IEnumerator GetRequest(string _url, Action<string> _callback)
    {
        using UnityWebRequest _webRequest = UnityWebRequest.Get(_url);
        yield return _webRequest.SendWebRequest();

        if (_webRequest.result == UnityWebRequest.Result.Success)
        {
            _callback?.Invoke(_webRequest.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error getting data: {_webRequest.error}");
            _callback?.Invoke(null);
        }
    }
    
    private IEnumerator PutRequest(string _url, string _jsonData)
    {
        byte[] _bodyRaw = Encoding.UTF8.GetBytes(_jsonData);
        using UnityWebRequest _webRequest = new UnityWebRequest(_url, "PUT");
        _webRequest.uploadHandler = new UploadHandlerRaw(_bodyRaw);
        _webRequest.downloadHandler = new DownloadHandlerBuffer();
        _webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return _webRequest.SendWebRequest();

        if (_webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error saving data: {_webRequest.error}");
        }
    }

    
    private IEnumerator Post(string _uri, string _jsonData, Action<string> _onSuccess, Action<string> _onError,
        bool _includeHeader = true)
    {
        using UnityWebRequest _webRequest = UnityWebRequest.Post(_uri, _jsonData);
        byte[] _jsonToSend = new System.Text.UTF8Encoding().GetBytes(_jsonData);
        _webRequest.uploadHandler = new UploadHandlerRaw(_jsonToSend);
        _webRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return _webRequest.SendWebRequest();

        if (_webRequest.result == UnityWebRequest.Result.Success)
        {
            _onSuccess?.Invoke(_webRequest.downloadHandler.text);
        }
        else
        {
            _onError?.Invoke(_webRequest.error);
        }

        _webRequest.uploadHandler.Dispose();
        _webRequest.downloadHandler.Dispose();
    }

    
}
