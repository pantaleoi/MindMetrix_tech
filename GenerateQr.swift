// Swift script for generating a JWT token and QR Code

import Foundation
import CryptoKit
import UIKit

struct JWT {
    static func generateJWT(secret: String, userId: String, sessionId: String) -> String? {
        let header = ["alg": "HS256", "typ": "JWT"]
        let payload = ["user_id": userId, "session_id": sessionId, "exp": Int(Date().timeIntervalSince1970) + 30]
        
        guard let headerData = try? JSONSerialization.data(withJSONObject: header),
              let payloadData = try? JSONSerialization.data(withJSONObject: payload) else {
            return nil
        }
        
        let headerBase64 = headerData.base64EncodedString().replacingOccurrences(of: "=", with: "").replacingOccurrences(of: "+", with: "-").replacingOccurrences(of: "/", with: "_")
        let payloadBase64 = payloadData.base64EncodedString().replacingOccurrences(of: "=", with: "").replacingOccurrences(of: "+", with: "-").replacingOccurrences(of: "/", with: "_")
        
        let signatureInput = "\(headerBase64).\(payloadBase64)"
        guard let secretData = secret.data(using: .utf8) else { return nil }
        
        let key = SymmetricKey(data: secretData)
        let signature = HMAC<SHA256>.authenticationCode(for: signatureInput.data(using: .utf8)!, using: key)
        let signatureBase64 = Data(signature).base64EncodedString().replacingOccurrences(of: "=", with: "").replacingOccurrences(of: "+", with: "-").replacingOccurrences(of: "/", with: "_")
        
        return "\(signatureInput).\(signatureBase64)"
    }
}

class QRCodeGenerator {
    static func generateQRCode(from string: String) -> UIImage? {
        let data = string.data(using: String.Encoding.ascii)

        if let filter = CIFilter(name: "CIQRCodeGenerator") {
            filter.setValue(data, forKey: "inputMessage")
            filter.setValue("H", forKey: "inputCorrectionLevel")

            if let outputImage = filter.outputImage {
                let transform = CGAffineTransform(scaleX: 10, y: 10)
                let scaledImage = outputImage.transformed(by: transform)
                return UIImage(ciImage: scaledImage)
            }
        }
        return nil
    }
}

// Example usage
let secretKey = "my_secret_key"
let userId = "12345"
let sessionId = "session_67890"

if let token = JWT.generateJWT(secret: secretKey, userId: userId, sessionId: sessionId) {
    print("Generated JWT Token:")
    print(token)

    if let qrCode = QRCodeGenerator.generateQRCode(from: token) {
        print("QR Code successfully generated!")
        // In a real app, you would display this QR code in a UIImageView.
    } else {
        print("Failed to generate QR Code.")
    }
} else {
    print("Failed to generate JWT Token.")
}
