namespace FibiEmlakDanismanlik.Application.Interfaces
{
    /// <summary>
    /// İki adımlı doğrulama (TOTP) ve gizli anahtar şifreleme/çözme işlemlerini tanımlayan servis arayüzü.
    /// </summary>
    public interface ITwoFactorService
    {
        /// <summary>
        /// Base32 kodlamalı yeni bir TOTP gizli anahtarı üretir.
        /// </summary>
        string GenerateSecret();

        /// <summary>
        /// Authenticator uygulamalarının tarayabilmesi için otpauth:// formatında URI oluşturur.
        /// </summary>
        string BuildOtpAuthUri(string accountMail, string secret);

        /// <summary>
        /// Kullanıcının girdiği 6 haneli TOTP kodunu gizli anahtara göre doğrular.
        /// </summary>
        bool VerifyCode(string secret, string code);

        /// <summary>
        /// Kodu doğrular ve aynı hesap için daha önce kabul edilmiş (aynı veya daha eski) zaman adımındaki bir kodu reddeder (tekrar kullanım koruması).
        /// </summary>
        bool VerifyCodeOnce(int agentId, string secret, string code);

        /// <summary>
        /// TOTP gizli anahtarını Data Protection ile şifreler.
        /// </summary>
        string Protect(string secret);

        /// <summary>
        /// Şifrelenmiş TOTP gizli anahtarını çözer; çözülemezse null döndürür.
        /// </summary>
        string? Unprotect(string? protectedSecret);
    }
}
