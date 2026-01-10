using CureLogix.Business.Abstract;
using QRCoder;

namespace CureLogix.Business.Concrete
{
    public class QrCodeManager : IQrCodeService
    {
        public byte[] GenerateQrCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            // PngByteQRCode kullanıyoruz (System.Drawing bağımlılığını azaltmak için modern yöntem)
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            return qrCodeImage;
        }
    }
}