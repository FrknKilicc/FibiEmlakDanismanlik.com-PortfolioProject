using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.AuthResults
{
    /// <summary>
    /// İki adımlı doğrulama tamamlandığında dönen kurtarma kodları ve giriş sonucu.
    /// </summary>
    public class GetEnableTwoFactorResult
    {
        /// <summary>
        /// Yalnızca kurulum tamamlandığında bir kez gösterilen tek kullanımlık kurtarma kodları.
        /// </summary>
        public List<string> RecoveryCodes { get; set; } = new();

        /// <summary>
        /// Başarılı 2FA etkinleştirmesi sonrası JWT ve danışman bilgileri.
        /// </summary>
        public GetLoginResult Login { get; set; } = null!;
    }
}
