[System.Serializable]
public class Keystore
{
    string rijnIv;
    string rijnKey;
    string user;
    public string accessKey { get; set; }

    public Keystore(string _rijnIv = "", string _rijnKey = "", string _user = "", string _accessKey = "")
    {
        rijnIv = _rijnIv;
        rijnKey = _rijnKey;
        user = _user;
        accessKey = _accessKey;
    }

    public void FromString(string input)
    {
        string[] data = input.Split(';');
        rijnIv = data[0];
        rijnKey = data[1];
        user = data[2];
        accessKey = data[3];
    }

    public override string ToString()
    {
        string output = rijnIv + ";" + rijnKey + ";" + user + ";" + accessKey + "|";
        return output;
    }

    public void Initialize()
    {
        if (DataContainer.instance != null)
        {
            DataContainer dc = DataContainer.instance;
            dc.accessKey = accessKey;
            dc.accessTextfield.text = accessKey;
            dc.activeRijnIv = rijnIv;
            dc.activeRijnKey = rijnKey;
            dc.userTextfield.text = user;
            dc.userKey = user;
        }
    }
}