using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataContainer : MonoBehaviour
{
    #region Singleton
    public static DataContainer instance;

    private void Awake()
    {
        DataContainer.instance = this;
        userKey = PlayerPrefs.GetString("userKey");
        accessKey = PlayerPrefs.GetString("accessKey");
        activeRijnKey = PlayerPrefs.GetString("activeRijnKey");
        activeRijnIv = PlayerPrefs.GetString("activeRijnIv");
        if (PlayerPrefs.GetString("rijnKeys") != "")
        {
            string[] _rijnKeys = PlayerPrefs.GetString("rijnKeys").Split('|');
            //rijnKeys.AddRange(_rijnKeys);
        }
        if (PlayerPrefs.GetString("rijnIvs") != "")
        {
            string[] _rijnIvs = PlayerPrefs.GetString("rijnIvs").Split('|');
            //rijnIvs.AddRange(_rijnIvs);
        }
        if (PlayerPrefs.GetString("keystores") != "")
        {
            string[] _keystores = PlayerPrefs.GetString("keystores").Split('|');
            foreach (string s in _keystores)
            {
                Keystore ks = new Keystore();
                ks.FromString(s);
                keystores.Add(ks);
            }
        }
    }
    #endregion

    public string userKey = "testuser";
    public string accessKey = "test";
    
    public List<Note> notes = new List<Note>();
    public TMP_InputField msgTextfield;

    [Header("View")]
    public GameObject scrollview;
    public GameObject notePrefab;

    [Header("Settings Dialog")]
    public List<Keystore> keystores;
    public GameObject settingsDialog;
    public TMP_Dropdown keystoreDropdown;
    public TMP_InputField userTextfield;
    public TMP_InputField accessTextfield;
    public Slider debugSlider;
    public bool isDebugging = false;

    [Header("QR-Reader Dialog")]
    public GameObject readerDialog;
    public GameObject viewerDialog;
    public GameObject camView;
    public GameObject qrView;
    public TMP_Text tester;

    [Header("Temp")]
    public string msgOrigin;

    [Header("Rijndael Settings")]
    public string activeRijnIv;
    public string activeRijnKey;
    public List<string> rijnKeys;
    public List<string> rijnIvs;

    [Header("TOR Settings")]
    public string proxyHost;
    public int proxyPort;
}