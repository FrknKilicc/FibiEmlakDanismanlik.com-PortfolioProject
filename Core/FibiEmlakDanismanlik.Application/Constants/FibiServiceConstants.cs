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
        public const int CalculationSummaryMaxLength = 8000;

        // Rate limiting — FibiServiceLead
        public const string LeadRateLimitPolicy = "fibi-leads";
        public const int LeadRateLimitPermitCount = 5;
        public const int LeadRateLimitWindowMinutes = 10;

        // String uzunlukları — BankLoanRate
        public const int BankNameMaxLength = 100;
        public const int LogoUrlMaxLength = 500;

        // Veri kaynağı ve senkronizasyon
        public const string EstimatedSeedDataSource = "FİBİ tahmini başlangıç verisi";
        public const int DataSourceMaxLength = 100;
        public const int ProviderKeyMaxLength = 50;
        public const int SourceUrlMaxLength = 500;
        public const int IndexPeriodMaxLength = 7;
        public const int SyncMessageMaxLength = 1000;
    }
}
