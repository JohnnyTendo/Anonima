using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    DataContainer dc;

    #region Singleton
    public static UIManager instance;

    void Awake()
    {
        UIManager.instance = this;
    }
    #endregion

    private void Start()
    {
        dc = DataContainer.instance;
    }

    public void SettingsOpen()
    {
        dc.userTextfield.text = dc.userKey;
        dc.accessTextfield.text = dc.accessKey;
        //TEST
        dc.keystoreDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (Keystore keystore in dc.keystores)
        {
            options.Add(keystore.accessKey);
        }
        dc.keystoreDropdown.AddOptions(options);
        //TEST-END
        dc.settingsDialog.SetActive(!dc.settingsDialog.activeSelf);
    }

    public void SettingsSave()
    {
        //TEST
        Keystore ks = new Keystore(dc.activeRijnIv, dc.activeRijnKey, dc.userTextfield.text, dc.accessTextfield.text);
        if (!dc.keystores.Contains(ks))
        {
            dc.keystores.Add(ks);
        }
        //TEST-END
        dc.userKey = dc.userTextfield.text;
        dc.accessKey = dc.accessTextfield.text;
        string keystores = "";
        foreach (Keystore keystore in dc.keystores)
        {
            keystores += keystore.ToString();
        }
        DataContainer.instance.settingsDialog.SetActive(false);
        PlayerPrefs.SetString("userKey", dc.userKey);
        PlayerPrefs.SetString("accessKey", dc.accessKey);
        PlayerPrefs.SetString("activeRijnKey", dc.activeRijnKey);
        PlayerPrefs.SetString("activeRijnIv", dc.activeRijnIv);
        PlayerPrefs.SetString("keystores", keystores);
        string _rijnKeys = "";
        foreach (string key in dc.rijnKeys)
        {
            _rijnKeys += key + "|";
        }
        PlayerPrefs.SetString("rijnKeys", _rijnKeys);
        string _rijnIvs = "";
        foreach (string iv in dc.rijnIvs)
        {
            _rijnIvs += iv + "|";
        }
        PlayerPrefs.SetString("rijnIvs", _rijnIvs);
    }

    public void ScanAccessKey()
    {
        dc.readerDialog.SetActive(true);
    }

    public void ViewAccessKey()
    {
        dc.viewerDialog.SetActive(true);

    }

    public void CloseScanAccessKey()
    {
        dc.readerDialog.SetActive(false);
    }

    public void CloseViewAccessKey()
    {
        dc.viewerDialog.SetActive(false);
    }

    public void SetDebug()
    {
        if (dc.debugSlider.value == 0)
        {
            dc.isDebugging = false;
        } else if (dc.debugSlider.value == 1)
        {
            dc.isDebugging = true;
        }
    }

    public void RefreshView()
    {
        Transform[] childs = dc.scrollview.GetComponentsInChildren<Transform>();
        foreach (Transform child in childs)
        {
            if (child != childs[0])
                Destroy(child.gameObject);
        }
        float yPos = 1650f;
        foreach (Note note in dc.notes)
        {
            Debugger.instance.WriteLog("UI: Note created");
            GameObject go = Instantiate(dc.notePrefab, new Vector3() { x = 540f, y = yPos, z = 0 }, Quaternion.identity, dc.scrollview.transform);
            go.GetComponent<NoteObject>().Activate(note);
            go.transform.position.Set(540f, yPos, 0);
            yPos -= 264f;            
        }
        Debugger.instance.WriteLog("UI: View refreshed");
    }

    public void SetKeystore()
    {
        Keystore ks = dc.keystores[dc.keystoreDropdown.value];
        ks.Initialize();
    }

    public void RemoveNote()
    {

    }
}
