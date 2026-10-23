namespace FibiEmlakDanismanlik.Application.Features.Results.AuthResults
{
    /// <summary>
    /// İki adımlı doğrulama kurulum başlangıcı sonucunda dönen gizli anahtar ve URI bilgileri.
    /// </summary>
    public class GetTwoFactorSetupResult
    {
        /// <summary>
        /// 4'lü gruplara ayrılmış okunabilir gizli anahtar.
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// Authenticator uygulamalarının taraması için standart otpauth:// URI'si.
        /// </summary>
        public string OtpAuthUri { get; set; } = string.Empty;
    }
}
