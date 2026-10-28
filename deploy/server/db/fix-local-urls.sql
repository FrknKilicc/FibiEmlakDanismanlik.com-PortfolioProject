-- Fibi Emlak: yerel geliştirme adreslerini (https://localhost:7015 vb.) canlı adrese çevirir.
-- Neden: yüklenen görsellerin adresi veritabanına tam URL olarak yazılıyor (FileStorage:PublicBaseUrl).
-- Yerelde yüklenen görseller "https://localhost:7015/uploads/..." olarak kayıtlı; canlıda kırık görünür.
--
-- Kullanım (fibi-db-init betiği çağırır):
--   1) Önizleme: -v Apply="0"  -> yalnızca hangi tablo/sütunda kaç satır değişeceğini listeler
--   2) Uygula:   -v Apply="1"  -> tek transaction içinde değiştirir; hata olursa hiçbir şey değişmez
-- Betik tekrar çalıştırılabilir; ikinci çalıştırmada değişecek satır kalmaz.

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE [fibiemlak.com];

DECLARE @Apply bit = $(Apply);
DECLARE @Target nvarchar(100) = N'https://fibiemlak.com';

DECLARE @Origins TABLE (Origin nvarchar(100) PRIMARY KEY);
INSERT INTO @Origins (Origin) VALUES
    (N'https://localhost:7015'), (N'http://localhost:5175'),
    (N'https://localhost:7012'), (N'http://localhost:5146'),
    (N'https://localhost:7255'), (N'http://localhost:5296'),
    (N'http://localhost:64814'), (N'http://localhost:59563'), (N'http://localhost:12143');

DECLARE @Columns TABLE (Id int IDENTITY PRIMARY KEY, SchemaName sysname, TableName sysname, ColumnName sysname);
INSERT INTO @Columns (SchemaName, TableName, ColumnName)
SELECT s.name, t.name, c.name
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
WHERE ty.name IN (N'nvarchar', N'varchar', N'nchar', N'char')
  AND t.is_ms_shipped = 0
  AND t.name <> N'__EFMigrationsHistory'
  AND t.name NOT LIKE N'%Audit%'   -- denetim kayıtları geçmişi değiştirilmez
  AND c.is_computed = 0;

DECLARE @Report TABLE (TableName nvarchar(300), ColumnName sysname, Origin nvarchar(100), RowsAffected int);

BEGIN TRANSACTION;

DECLARE @Id int = 1, @MaxId int = (SELECT MAX(Id) FROM @Columns);
DECLARE @Schema sysname, @Table sysname, @Column sysname, @Sql nvarchar(max), @Count int, @Origin nvarchar(100);

WHILE @Id <= @MaxId
BEGIN
    SELECT @Schema = SchemaName, @Table = TableName, @Column = ColumnName FROM @Columns WHERE Id = @Id;

    DECLARE origin_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT Origin FROM @Origins;
    OPEN origin_cursor;
    FETCH NEXT FROM origin_cursor INTO @Origin;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Sql = N'SELECT @cnt = COUNT(*) FROM ' + QUOTENAME(@Schema) + N'.' + QUOTENAME(@Table)
                 + N' WHERE ' + QUOTENAME(@Column) + N' LIKE N''%'' + @origin + N''%''';
        EXEC sp_executesql @Sql, N'@origin nvarchar(100), @cnt int OUTPUT', @origin = @Origin, @cnt = @Count OUTPUT;

        IF @Count > 0
        BEGIN
            INSERT INTO @Report VALUES (QUOTENAME(@Schema) + N'.' + QUOTENAME(@Table), @Column, @Origin, @Count);

            IF @Apply = 1
            BEGIN
                SET @Sql = N'UPDATE ' + QUOTENAME(@Schema) + N'.' + QUOTENAME(@Table)
                         + N' SET ' + QUOTENAME(@Column) + N' = REPLACE(' + QUOTENAME(@Column) + N', @origin, @target)'
                         + N' WHERE ' + QUOTENAME(@Column) + N' LIKE N''%'' + @origin + N''%''';
                EXEC sp_executesql @Sql, N'@origin nvarchar(100), @target nvarchar(100)', @origin = @Origin, @target = @Target;
            END
        END
        FETCH NEXT FROM origin_cursor INTO @Origin;
    END
    CLOSE origin_cursor;
    DEALLOCATE origin_cursor;

    SET @Id += 1;
END

COMMIT TRANSACTION;

SELECT TableName AS Tablo, ColumnName AS Sutun, Origin AS Eski_adres, RowsAffected AS Satir,
       CASE WHEN @Apply = 1 THEN N'DEĞİŞTİRİLDİ' ELSE N'önizleme' END AS Durum
FROM @Report
ORDER BY TableName, ColumnName;

-- Listede olmayan bir localhost adresi kaldıysa elle bakılmak üzere raporla
DELETE FROM @Report;
SET @Id = 1;
WHILE @Id <= @MaxId
BEGIN
    SELECT @Schema = SchemaName, @Table = TableName, @Column = ColumnName FROM @Columns WHERE Id = @Id;
    SET @Sql = N'SELECT @cnt = COUNT(*) FROM ' + QUOTENAME(@Schema) + N'.' + QUOTENAME(@Table)
             + N' WHERE ' + QUOTENAME(@Column) + N' LIKE N''%://localhost%'' OR ' + QUOTENAME(@Column) + N' LIKE N''%://127.0.0.1%''';
    EXEC sp_executesql @Sql, N'@cnt int OUTPUT', @cnt = @Count OUTPUT;
    IF @Count > 0
        INSERT INTO @Report VALUES (QUOTENAME(@Schema) + N'.' + QUOTENAME(@Table), @Column, N'(diğer localhost)', @Count);
    SET @Id += 1;
END

SELECT TableName AS Tablo, ColumnName AS Sutun, RowsAffected AS Kalan_localhost_satiri FROM @Report;
