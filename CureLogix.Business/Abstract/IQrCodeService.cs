namespace CureLogix.Business.Abstract
{
    public interface IQrCodeService
    {
        byte[] GenerateQrCode(string text);
    }
}