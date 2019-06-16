using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.IO;
using System.Net.Http;
using UnityEngine;
using MihaZupan;

public class PHPconnect : MonoBehaviour
{
    #region Singleton
    public static PHPconnect instance;

    void Awake()
    {
        PHPconnect.instance = this;
    }
    #endregion

    private string url = "http://178.254.35.6/php/anonima.php";
    private string msgText;
    private string msgOrigin;
    private string qualifier; //can be RECEIVE (fetching notes with db) or SENT (add new note to userTable and fetch notes afterwards) or DELET (remove the note from db)
    private string encryptedMsg;
    private string token = "LaeQraXjGx2tgAdbNRd4";

    DataContainer dc;
    UIManager um;
    RijndaelScript rijn;

    private void Start()
    {
        dc = DataContainer.instance;
        um = UIManager.instance;
        rijn = RijndaelScript.instance;
    }

    public void Handler(string _qualifier)
    {
        msgOrigin = dc.msgOrigin;
        msgText = dc.msgTextfield.text;
        qualifier = _qualifier;
        encryptedMsg = rijn.Encrypt(msgText);
        StartCoroutine("SendRequest");
        dc.msgTextfield.text = "";
    }


    //is an IEnumerator not void when using UnityWebRequest
    private void SendRequest()
    {
        Debugger.instance.WriteLog("PHP: Notes cleared");
        dc.notes.Clear();
        Debugger.instance.WriteLog("PHP: Creating SendRequest");


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = WebRequestMethods.Http.Post;
        request.ContentType = "application/x-www-form-urlencoded";
        request.SendChunked = false;
        //set Proxy
        //if (dc.proxyHost != null)
        //    request.Proxy = new WebProxy(dc.proxyHost, dc.proxyPort);
        //format data to form data
        Debugger.instance.WriteLog("PHP: formatting SendRequest Data");
        NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(System.String.Empty);
        outgoingQueryString.Add("userKey", dc.userKey);
        outgoingQueryString.Add("accessKey", dc.accessKey);
        outgoingQueryString.Add("msgText", rijn.Encrypt(msgText));
        outgoingQueryString.Add("qualifier", qualifier);
        outgoingQueryString.Add("msgOrigin", msgOrigin);
        outgoingQueryString.Add("token", token);
        string postdata = outgoingQueryString.ToString();
        Debugger.instance.WriteLog(postdata);
        // Convert the post string to a byte array
        byte[] bytedata = System.Text.Encoding.UTF8.GetBytes(postdata);
        //append form data to url
        request.ContentLength = bytedata.Length;
        //send data
        Debugger.instance.WriteLog("PHP: SendRequest started");
        try
        {
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();
        }
        catch (System.Exception e)
        {
            Debugger.instance.WriteLog("PHP Error: " + e.Source + e.Message);
        }
        //get response
        Debugger.instance.WriteLog("PHP: SendRequest processed");
        HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
        Stream responseStream = httpWebResponse.GetResponseStream();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        using (StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                sb.Append(line);
            }
        }
        string rawData = sb.ToString();


        /*
         * Old but stable Code for using UnityWebRequest
        WWWForm form = new WWWForm();
        form.AddField("userKey", dc.userKey);
        form.AddField("accessKey", dc.accessKey);
        form.AddField("msgText", rijn.Encrypt(msgText));
        form.AddField("qualifier", qualifier);
        form.AddField("msgOrigin", msgOrigin);
        UnityWebRequest request = UnityWebRequest.Post(dc.url, form);
        request.chunkedTransfer = false;
        //set Proxy to HttpWebRequest
        yield return request.SendWebRequest();
        Debug.Log("...SendRequest processed...");
        string rawData = request.downloadHandler.text;
        */
        Debugger.instance.WriteLog("PHP: RawData received --> " + rawData);
        string[] setData = rawData.Split(';');
        foreach (string n in setData)
        {
            string[] rawNote = n.Split('|');
            if (rawNote.Length == 4)
            {
                Note note = new Note(rawNote[0], rijn.Decrypt(rawNote[1]), rawNote[2], rawNote[3]);
                dc.notes.Add(note);
            }
        }
        msgText = "";
        qualifier = "";
        um.RefreshView();
        dc.msgOrigin = "";
    }

    public async void TestSocksProxy()
    {
        HttpContent content = new ByteArrayContent(new byte[0]);
        var proxy = new HttpToSocks5Proxy("127.0.0.1", 1080);
        var handler = new HttpClientHandler { Proxy = proxy };
        HttpClient httpClient = new HttpClient(handler, true);
        HttpResponseMessage result = await httpClient.PostAsync(url, content);
        Debugger.instance.WriteLog("HTTPS POST: " + await result.Content.ReadAsStringAsync());
    }

}
