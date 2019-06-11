using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteObject : MonoBehaviour
{
    PHPconnect php;
    DataContainer dc;
    RijndaelScript rijn;
    string userKey;

    public Note note;
    public TMP_Text msg;
    public TMP_Text timestamp;

    public void Start()
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
        timestamp.text = _note.timestamp + " - " + _note.userKey;
    }

    public void RemoveSelf()
    {
        //trigger phpConnect script and initialize handler with new qualifier and send msgOrigin to identify the message
        dc.msgTextfield.text = msg.text;
        dc.msgOrigin = userKey;
        php.Handler("DELET");
        //add new qualifier DELETE to php-web script
    }
}
