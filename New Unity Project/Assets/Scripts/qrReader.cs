using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using System;

public class qrReader : MonoBehaviour
{
    DataContainer dc;
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public RawImage viewer;
    public AspectRatioFitter fit;

    private void Start()
    {
        dc = DataContainer.instance;
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            dc.tester.text = "No cameras found";
            camAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if(!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (backCam == null)
        {
            dc.tester.text += "No backCam found";
            return;
        }
        backCam.Play();
        background.texture = backCam;

        camAvailable = true;
    }

    private void Update()
    {
        if (dc.viewerDialog.activeSelf)
        {
            GenerateQR();
        }
        if (!camAvailable)
            return;
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;
        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0,0,orient);
        if (dc.readerDialog.activeSelf)
        {
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                // decode the current frame
                var result = barcodeReader.Decode(backCam.GetPixels32(), backCam.width, backCam.height);
                if (result != null)
                {
                    dc.tester.text = "DECODED TEXT FROM QR: " + result.Text;
                    if (result.Text != "")
                    {
                        //seperate the values saved in qrCode and assign them
                        string[] dataSet = result.Text.Split('|');
                        foreach (string s in dataSet)
                        {
                            Debugger.instance.WriteLog("Reader: DataSet --> " + s);
                        }
                        dc.accessKey = dataSet[0];
                        dc.accessTextfield.text = dataSet[0];
                        dc.activeRijnKey = dataSet[1];
                        dc.activeRijnIv = dataSet[2];
                        dc.readerDialog.SetActive(false);
                    }
                }
            }
            catch (Exception ex) { dc.tester.text = ex.Message; }
        }
    }

    private void GenerateQR()
    {
        var encoded = new Texture2D(256, 256);
        //combine values and generate qrCode
        string textForEncoding = dc.accessKey + "|" + dc.activeRijnKey + "|" + dc.activeRijnIv;
        var color32 = Encode(textForEncoding, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        viewer.texture = encoded;
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}
