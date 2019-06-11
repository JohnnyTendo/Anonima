using UnityEngine;
[System.Serializable]
public class Note
{
    public string index;
    public string msgText;
    public string timestamp;
    public string userKey;

    public Note(string _index, string _msgText, string _timestamp, string _userKey) {
        index = _index;
        msgText = _msgText;
        timestamp = _timestamp;
        userKey = _userKey;
    }
}
