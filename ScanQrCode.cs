using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System.Net.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

public class QRScanner : MonoBehaviour
{
    public RawImage cameraView;
    private WebCamTexture webcamTexture; 
    private bool isScanning = true;
    public string userId = "";
    public string sessionId = "";

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
            var response = await client.PostAsync("https://azure.myflow.ch/validate",
                new StringContent(jwtToken));

            if (response.IsSuccessStatusCode)
            {
                Debug.Log("User authenticated!");
                
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(jwtToken);
            
                userId = jwtToken.Claims.First(claim => claim.Type == "user_id").Value;
                sessionId = jwtToken.Claims.First(claim => claim.Type == "session_id").Value;
            }
            else
            {
                Debug.LogError("Authentication failed. Please scan again.");
                isScanning = true;
            }
        }
    }
}
