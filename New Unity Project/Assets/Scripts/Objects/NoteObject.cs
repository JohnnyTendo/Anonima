using UnityEngine;
using TMPro;

public class NoteObject : MonoBehaviour
{
    private PHPconnect php;
    private DataContainer dc;
    private RijndaelScript rijn;
    private string userKey;

    public GameObject optionsTab;
    public GameObject infoTab;

    public Note note;
    public TMP_Text msg;
    public TMP_Text info;

    private void Start()
    {
        php = PHPconnect.instance;
        dc = DataContainer.instance;
        rijn = RijndaelScript.instance;
    }

    public void Activate(Note _note)
    {
        userKey = _note.userKey;
        note = _note;
        msg.text = _note.msgText;
        info.text = _note.timestamp + " - " + _note.userKey;
    }

    public void ShowOptions()
    {
        msg.gameObject.SetActive(msg.gameObject.activeSelf);
        optionsTab.SetActive(!optionsTab.activeSelf);
    }

    public void RemoveSelf()
    {
        //trigger phpConnect script and initialize handler with new qualifier and send msgOrigin to identify the message
        dc.msgTextfield.text = msg.text;
        dc.msgOrigin = userKey;
        php.Handler("DELET");
        //add new qualifier DELETE to php-web script
    }

    public void ShowInfo()
    {
        optionsTab.SetActive(!optionsTab.activeSelf);
        infoTab.SetActive(!infoTab.activeSelf);
    }
}
