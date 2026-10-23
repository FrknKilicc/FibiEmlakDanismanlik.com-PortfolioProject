-- Yönetim paneli rolleri (AdminSecurityFoundation migration'ından sonra, her ortamda bir kez çalıştırılır).
-- Roller: 1 = Sahip (Owner), 2 = Yönetici (Manager), 3 = Danışman (Consultant).
-- E-posta ile eşleştirilir; kayıt yoksa satır etkilenmez. Şifre veya hash bu dosyaya yazılmaz (repo herkese açık olabilir).
-- Sahip hesabı açmak için ayrı, repoya girmeyen yerel bir betik kullanılır.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRAN;

UPDATE dbo.Agents SET Role = 1 WHERE Mail = N'ibrahimfibi@gmail.com';
UPDATE dbo.Agents SET Role = 2 WHERE Mail = N'atakanfibi@gmail.com';
UPDATE dbo.Agents SET Role = 3 WHERE Mail IN (N'enbiyafibi@gmail.com', N'emrahfibi@gmail.com');

-- Rol değişen hesapların açık oturumları düşsün
UPDATE dbo.Agents SET SecurityStamp = CONVERT(nvarchar(64), NEWID())
WHERE Mail IN (N'ibrahimfibi@gmail.com', N'atakanfibi@gmail.com', N'enbiyafibi@gmail.com', N'emrahfibi@gmail.com');

COMMIT;

SELECT AgentId, AgentName, Mail, Role, IsActive, ShowOnWebsite FROM dbo.Agents ORDER BY Role, AgentId;
