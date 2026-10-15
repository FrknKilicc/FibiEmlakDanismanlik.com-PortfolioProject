-- =============================================================================
-- remove-popular-places-demo.sql
-- Amaç  : seed-popular-places-demo.sql tarafından eklenen tüm demo ilanları
--         6 tablodan temizler.
-- UYARI : Bu script migration DEĞİLDİR. Production ortamına uygulanmaz.
--         Yalnızca PropertyName LIKE '[DEMO] %' olan satırlar silinir.
-- =============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRAN;

    DECLARE @deleted INT;

    DELETE FROM dbo.ForSaleHousingPropertyListings
    WHERE PropertyName LIKE N'[[]DEMO] %';
    SET @deleted = @@ROWCOUNT;
    PRINT N'ForSaleHousingPropertyListings: ' + CAST(@deleted AS NVARCHAR(10)) + N' demo ilan silindi.';

    DELETE FROM dbo.ForSaleCommercialPropertyListings
    WHERE PropertyName LIKE N'[[]DEMO] %';
    SET @deleted = @@ROWCOUNT;
    PRINT N'ForSaleCommercialPropertyListings: ' + CAST(@deleted AS NVARCHAR(10)) + N' demo ilan silindi.';

    DELETE FROM dbo.ForSaleLandListings
    WHERE PropertyName LIKE N'[[]DEMO] %';
    SET @deleted = @@ROWCOUNT;
    PRINT N'ForSaleLandListings: ' + CAST(@deleted AS NVARCHAR(10)) + N' demo ilan silindi.';

    DELETE FROM dbo.RentalHousingListings
    WHERE PropertyName LIKE N'[[]DEMO] %';
    SET @deleted = @@ROWCOUNT;
    PRINT N'RentalHousingListings: ' + CAST(@deleted AS NVARCHAR(10)) + N' demo ilan silindi.';

    DELETE FROM dbo.RentalCommercialPropertyListings
    WHERE PropertyName LIKE N'[[]DEMO] %';
    SET @deleted = @@ROWCOUNT;
    PRINT N'RentalCommercialPropertyListings: ' + CAST(@deleted AS NVARCHAR(10)) + N' demo ilan silindi.';

    DELETE FROM dbo.RentalLandListings
    WHERE PropertyName LIKE N'[[]DEMO] %';
    SET @deleted = @@ROWCOUNT;
    PRINT N'RentalLandListings: ' + CAST(@deleted AS NVARCHAR(10)) + N' demo ilan silindi.';

    COMMIT;
    PRINT N'Demo veriler başarıyla temizlendi.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    PRINT N'HATA: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
