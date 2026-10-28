-- Fibi Emlak: canlı sunucuda SQL hesaplarını oluşturur.
-- Veritabanı geri yüklendikten SONRA, sa ile bir kez çalıştırılır (fibi-db-init betiği çağırır).
-- Şifreler sqlcmd değişkeni olarak verilir: -v AppPassword="..." MigratorPassword="..."
--
-- fibiemlak_app      : uygulamanın kullandığı hesap, yalnızca okuma/yazma (şema değiştiremez)
-- fibiemlak_migrator : migration ve yedek için; şema değiştirebilir, yedek alabilir
-- Geri yüklenen yedekte eski sunucudan kalan kullanıcılar varsa yeni login'lere yeniden bağlanır.

USE [master];
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'fibiemlak_app')
    CREATE LOGIN [fibiemlak_app] WITH PASSWORD = N'$(AppPassword)', CHECK_POLICY = ON, DEFAULT_DATABASE = [fibiemlak.com];
ELSE
    ALTER LOGIN [fibiemlak_app] WITH PASSWORD = N'$(AppPassword)';
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'fibiemlak_migrator')
    CREATE LOGIN [fibiemlak_migrator] WITH PASSWORD = N'$(MigratorPassword)', CHECK_POLICY = ON, DEFAULT_DATABASE = [fibiemlak.com];
ELSE
    ALTER LOGIN [fibiemlak_migrator] WITH PASSWORD = N'$(MigratorPassword)';
GO

-- Yedeğin doğrulanması (RESTORE VERIFYONLY) bu sunucu yetkisini ister
GRANT CREATE ANY DATABASE TO [fibiemlak_migrator];
GO

USE [fibiemlak.com];
GO

-- Yedekten gelen eski kullanıcı kayıtlarını (orphan) bu sunucudaki login'e bağla veya oluştur
IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'fibiemlak_app')
    ALTER USER [fibiemlak_app] WITH LOGIN = [fibiemlak_app];
ELSE
    CREATE USER [fibiemlak_app] FOR LOGIN [fibiemlak_app];
GO

IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'fibiemlak_migrator')
    ALTER USER [fibiemlak_migrator] WITH LOGIN = [fibiemlak_migrator];
ELSE
    CREATE USER [fibiemlak_migrator] FOR LOGIN [fibiemlak_migrator];
GO

ALTER ROLE [db_datareader] ADD MEMBER [fibiemlak_app];
ALTER ROLE [db_datawriter] ADD MEMBER [fibiemlak_app];

ALTER ROLE [db_datareader]     ADD MEMBER [fibiemlak_migrator];
ALTER ROLE [db_datawriter]     ADD MEMBER [fibiemlak_migrator];
ALTER ROLE [db_ddladmin]       ADD MEMBER [fibiemlak_migrator];
ALTER ROLE [db_backupoperator] ADD MEMBER [fibiemlak_migrator];
GO

-- Yerel sunucudan gelebilecek, canlıda kullanılmayan kullanıcıları listele (silmeden önce gözden geçir)
SELECT name AS [Gozden_gecirilecek_kullanici]
FROM sys.database_principals
WHERE type IN ('S', 'U', 'G')
  AND name NOT IN (N'dbo', N'guest', N'INFORMATION_SCHEMA', N'sys', N'fibiemlak_app', N'fibiemlak_migrator');
GO
