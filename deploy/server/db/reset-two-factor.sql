-- Fibi Emlak: taşıma sonrası iki adımlı doğrulama (2FA) kayıtlarını sıfırlar.
-- Neden: 2FA gizli anahtarları yerel Windows bilgisayarın Data Protection (DPAPI) anahtarlarıyla şifreli.
-- Bu anahtarlar başka bir bilgisayarda çözülemez; sıfırlanmazsa 2FA açık kullanıcılar panele giremez.
-- Sonuç: Sahip ve Yönetici ilk girişte QR kodu yeniden okutarak 2FA'yı kurar (uygulama bunu zorunlu tutar).
-- Şifreler etkilenmez (BCrypt, makineden bağımsız).

SET NOCOUNT ON;
SET XACT_ABORT ON;
USE [fibiemlak.com];

BEGIN TRANSACTION;
DELETE FROM [dbo].[AgentRecoveryCodes];
UPDATE [dbo].[Agents] SET [TwoFactorEnabled] = 0, [TwoFactorSecretProtected] = NULL
WHERE [TwoFactorEnabled] = 1 OR [TwoFactorSecretProtected] IS NOT NULL;
SELECT @@ROWCOUNT AS [2FA_sifirlanan_kullanici];
COMMIT TRANSACTION;
