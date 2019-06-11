using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZXing;
using ZXing.QrCode;
using System;

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

    public void Start()
    {
        dc = DataContainer.instance;
    }

    public void SettingsOpen()
    {
        dc.userTextfield.text = dc.userKey;
        dc.accessTextfield.text = dc.accessKey;
        DataContainer.instance.settingsDialog.SetActive(true);
    }

    public void SettingsSave()
    {
        dc.userKey = dc.userTextfield.text;
        dc.accessKey = dc.accessTextfield.text;
        DataContainer.instance.settingsDialog.SetActive(false);
        PlayerPrefs.SetString("userKey", dc.userKey);
        PlayerPrefs.SetString("accessKey", dc.accessKey);
        PlayerPrefs.SetString("activeRijnKey", dc.activeRijnKey);
        PlayerPrefs.SetString("activeRijnIv", dc.activeRijnIv);
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
            Debug.Log("Created");
            GameObject go = Instantiate(dc.notePrefab, new Vector3() { x = 540f, y = yPos, z = 0 }, Quaternion.identity, dc.scrollview.transform);
            go.GetComponent<NoteObject>().Activate(note);
            go.transform.position.Set(540f, yPos, 0);
            yPos -= 264f;
            //go.GetComponent<Button>().onClick.AddListener(delegate { RemoveNote(); });
            
        }
        Debug.Log("View refreshed");
    }

    public void RemoveNote()
    {

    }
}
