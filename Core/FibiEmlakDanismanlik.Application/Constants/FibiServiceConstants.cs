namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>FİBİ Hizmetler modülü sabitleri.</summary>
    public static class FibiServiceConstants
    {
        // Varsayılan değerler
        public const decimal DefaultAgencyMarginMultiplier = 1.08m;
        public const decimal DefaultBandSpreadPercentage = 15m;

        // String uzunlukları — FibiServiceLead
        public const int FullNameMaxLength = 150;
        public const int PhoneNumberMaxLength = 20;
        public const int EmailMaxLength = 256;
        public const int CustomerNoteMaxLength = 1000;

        // String uzunlukları — BankLoanRate
        public const int BankNameMaxLength = 100;
        public const int LogoUrlMaxLength = 500;
    }
}
