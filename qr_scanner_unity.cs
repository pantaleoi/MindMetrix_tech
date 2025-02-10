using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System.Net.Http;
using System.Threading.Tasks;

public class QRScanner : MonoBehaviour
{
    public RawImage cameraView;
    private WebCamTexture webcamTexture; 
    private bool isScanning = true;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        cameraView.texture = webcamTexture;
        cameraView.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    void Update()
    {
        if (isScanning)
        {
            ScanQRCode();
        }
    }

    private void ScanQRCode()
    {
        IBarcodeReader barcodeReader = new BarcodeReader();
        var result = barcodeReader.Decode(webcamTexture.GetPixels32(), webcamTexture.width, webcamTexture.height);

        if (result != null)
        {
            Debug.Log("QR Code Scanned: " + result.Text);
            isScanning = false;
            ValidateToken(result.Text);
        }
    }

    private async void ValidateToken(string jwtToken)
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.PostAsync("https://azure.myflow.com/validate",
                new StringContent(jwtToken));

            if (response.IsSuccessStatusCode)
            {
                Debug.Log("User authenticated!");
            }
            else
            {
                Debug.LogError("Authentication failed. Please scan again.");
                isScanning = true;
            }
        }
    }
}
