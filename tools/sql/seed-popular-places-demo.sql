-- =============================================================================
-- seed-popular-places-demo.sql
-- Amaç  : "En Popüler Konumlar" bileşenini test etmek için geliştirme ortamına
--         demo ilan verileri ekler.
-- UYARI : Bu script migration DEĞİLDİR. Production ortamına uygulanmaz.
--         Tüm demo ilanların PropertyName alanı '[DEMO] ' ile başlar;
--         temizlik için remove-popular-places-demo.sql kullanılır.
-- =============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Tekrar çalıştırma koruması: herhangi bir tabloda [DEMO] ilan varsa çık
IF EXISTS (
    SELECT 1 FROM (
        SELECT PropertyName FROM dbo.ForSaleHousingPropertyListings WHERE PropertyName LIKE N'[[]DEMO] %'
        UNION ALL
        SELECT PropertyName FROM dbo.ForSaleCommercialPropertyListings WHERE PropertyName LIKE N'[[]DEMO] %'
        UNION ALL
        SELECT PropertyName FROM dbo.ForSaleLandListings WHERE PropertyName LIKE N'[[]DEMO] %'
        UNION ALL
        SELECT PropertyName FROM dbo.RentalHousingListings WHERE PropertyName LIKE N'[[]DEMO] %'
        UNION ALL
        SELECT PropertyName FROM dbo.RentalCommercialPropertyListings WHERE PropertyName LIKE N'[[]DEMO] %'
        UNION ALL
        SELECT PropertyName FROM dbo.RentalLandListings WHERE PropertyName LIKE N'[[]DEMO] %'
    ) AS _check
)
BEGIN
    PRINT N'[DEMO] ilan zaten mevcut. Script atlanıyor.';
    RETURN;
END

BEGIN TRY
    BEGIN TRAN;

    -- =========================================================================
    -- Satılık ilanlar dağılımı (usageType=1):
    --   İstanbul/Kadıköy     6  -> ForSaleHousing 4, ForSaleCommercial 1, ForSaleLand 1
    --   İstanbul/Zeytinburnu 5  -> ForSaleHousing 3, ForSaleCommercial 1, ForSaleLand 1
    --   Ankara/Çankaya        7  -> ForSaleHousing 5, ForSaleCommercial 1, ForSaleLand 1
    --   İzmir/Karşıyaka       5  -> ForSaleHousing 3, ForSaleCommercial 1, ForSaleLand 1
    --   Antalya/Muratpaşa     4  -> ForSaleHousing 2, ForSaleCommercial 1, ForSaleLand 1
    --   Bursa/Nilüfer         3  -> ForSaleHousing 2, ForSaleCommercial 1, ForSaleLand 0
    --   Muğla/Bodrum          2  -> ForSaleHousing 1, ForSaleCommercial 1, ForSaleLand 0
    --
    -- Kiralık ilanlar dağılımı (usageType=2):
    --   İstanbul/Beşiktaş    8  -> RentalHousing 6, RentalCommercial 1, RentalLand 1
    --   İstanbul/Zeytinburnu 4  -> RentalHousing 3, RentalCommercial 1, RentalLand 0
    --   İzmir/Bornova        6  -> RentalHousing 4, RentalCommercial 1, RentalLand 1
    --   Ankara/Keçiören      3  -> RentalHousing 2, RentalCommercial 1, RentalLand 0
    --   Eskişehir/Tepebaşı   5  -> RentalHousing 4, RentalCommercial 1, RentalLand 0
    --   Antalya/Konyaaltı    2  -> RentalHousing 1, RentalCommercial 1, RentalLand 0
    -- =========================================================================

    DECLARE @ColList   NVARCHAR(MAX);
    DECLARE @Sql       NVARCHAR(MAX);
    DECLARE @TableName NVARCHAR(128);

    -- =========================================================================
    -- ForSaleHousingPropertyListings
    -- =========================================================================
    SET @TableName = N'ForSaleHousingPropertyListings';

    IF NOT EXISTS (SELECT 1 FROM dbo.ForSaleHousingPropertyListings WHERE PropertyStatus = N'Aktif')
        PRINT N'ForSaleHousingPropertyListings: Şablon yok, atlanıyor.';
    ELSE
    BEGIN
        SELECT @ColList = STRING_AGG(QUOTENAME(c.name), N', ') WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c
        WHERE c.object_id = OBJECT_ID(N'dbo.' + @TableName)
          AND c.is_identity = 0
          AND c.is_computed = 0;

        DECLARE @FshData TABLE (City NVARCHAR(100), District NVARCHAR(100), Cnt INT);
        INSERT INTO @FshData VALUES
            (N'İstanbul',  N'Kadıköy',    4),
            (N'İstanbul',  N'Zeytinburnu',3),
            (N'Ankara',    N'Çankaya',    5),
            (N'İzmir',     N'Karşıyaka',  3),
            (N'Antalya',   N'Muratpaşa',  2),
            (N'Bursa',     N'Nilüfer',    2),
            (N'Muğla',     N'Bodrum',     1);

        DECLARE fsh_cur CURSOR LOCAL FAST_FORWARD FOR SELECT City, District, Cnt FROM @FshData;
        DECLARE @fsh_city NVARCHAR(100), @fsh_dist NVARCHAR(100), @fsh_cnt INT, @fsh_i INT;
        OPEN fsh_cur;
        FETCH NEXT FROM fsh_cur INTO @fsh_city, @fsh_dist, @fsh_cnt;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @fsh_i = 1;
            WHILE @fsh_i <= @fsh_cnt
            BEGIN
                SET @Sql = N'
                    INSERT INTO dbo.ForSaleHousingPropertyListings (' + @ColList + N')
                    SELECT ' + @ColList + N'
                    FROM (SELECT TOP 1 * FROM dbo.ForSaleHousingPropertyListings
                          WHERE PropertyStatus = N''Aktif'' ORDER BY (SELECT NULL)) AS _src;
                ';
                EXEC sp_executesql @Sql;
                -- Override: yeni eklenen satırı güncelle (identity max)
                UPDATE dbo.ForSaleHousingPropertyListings
                SET City           = @fsh_city,
                    District       = @fsh_dist,
                    PropertyName   = N'[DEMO] ' + @fsh_dist + N' ' + CAST(@fsh_i AS NVARCHAR(10)),
                    PropertyStatus = N'Aktif',
                    BestDeals      = 0,
                    CreatedDate    = GETDATE()
                WHERE ForSaleHousingListId = (SELECT MAX(ForSaleHousingListId) FROM dbo.ForSaleHousingPropertyListings);

                SET @fsh_i = @fsh_i + 1;
            END;
            FETCH NEXT FROM fsh_cur INTO @fsh_city, @fsh_dist, @fsh_cnt;
        END;
        CLOSE fsh_cur; DEALLOCATE fsh_cur;
    END;

    -- =========================================================================
    -- ForSaleCommercialPropertyListings
    -- =========================================================================
    SET @TableName = N'ForSaleCommercialPropertyListings';

    IF NOT EXISTS (SELECT 1 FROM dbo.ForSaleCommercialPropertyListings WHERE PropertyStatus = N'Aktif')
        PRINT N'ForSaleCommercialPropertyListings: Şablon yok, atlanıyor.';
    ELSE
    BEGIN
        SELECT @ColList = STRING_AGG(QUOTENAME(c.name), N', ') WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c
        WHERE c.object_id = OBJECT_ID(N'dbo.' + @TableName)
          AND c.is_identity = 0 AND c.is_computed = 0;

        DECLARE @FscData TABLE (City NVARCHAR(100), District NVARCHAR(100), Cnt INT);
        INSERT INTO @FscData VALUES
            (N'İstanbul',  N'Kadıköy',    1),
            (N'İstanbul',  N'Zeytinburnu',1),
            (N'Ankara',    N'Çankaya',    1),
            (N'İzmir',     N'Karşıyaka',  1),
            (N'Antalya',   N'Muratpaşa',  1),
            (N'Bursa',     N'Nilüfer',    1),
            (N'Muğla',     N'Bodrum',     1);

        DECLARE fsc_cur CURSOR LOCAL FAST_FORWARD FOR SELECT City, District, Cnt FROM @FscData;
        DECLARE @fsc_city NVARCHAR(100), @fsc_dist NVARCHAR(100), @fsc_cnt INT, @fsc_i INT;
        OPEN fsc_cur;
        FETCH NEXT FROM fsc_cur INTO @fsc_city, @fsc_dist, @fsc_cnt;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @fsc_i = 1;
            WHILE @fsc_i <= @fsc_cnt
            BEGIN
                SET @Sql = N'INSERT INTO dbo.ForSaleCommercialPropertyListings (' + @ColList + N')
                    SELECT ' + @ColList + N' FROM (SELECT TOP 1 * FROM dbo.ForSaleCommercialPropertyListings
                    WHERE PropertyStatus = N''Aktif'' ORDER BY (SELECT NULL)) AS _src;';
                EXEC sp_executesql @Sql;
                UPDATE dbo.ForSaleCommercialPropertyListings
                SET City           = @fsc_city,
                    District       = @fsc_dist,
                    PropertyName   = N'[DEMO] ' + @fsc_dist + N' ' + CAST(@fsc_i AS NVARCHAR(10)),
                    PropertyStatus = N'Aktif',
                    BestDeals      = 0,
                    CreatedDate    = GETDATE()
                WHERE ForSaleCommercialListingId = (SELECT MAX(ForSaleCommercialListingId) FROM dbo.ForSaleCommercialPropertyListings);
                SET @fsc_i = @fsc_i + 1;
            END;
            FETCH NEXT FROM fsc_cur INTO @fsc_city, @fsc_dist, @fsc_cnt;
        END;
        CLOSE fsc_cur; DEALLOCATE fsc_cur;
    END;

    -- =========================================================================
    -- ForSaleLandListings
    -- =========================================================================
    SET @TableName = N'ForSaleLandListings';

    IF NOT EXISTS (SELECT 1 FROM dbo.ForSaleLandListings WHERE PropertyStatus = N'Aktif')
        PRINT N'ForSaleLandListings: Şablon yok, atlanıyor.';
    ELSE
    BEGIN
        SELECT @ColList = STRING_AGG(QUOTENAME(c.name), N', ') WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c
        WHERE c.object_id = OBJECT_ID(N'dbo.' + @TableName)
          AND c.is_identity = 0 AND c.is_computed = 0;

        DECLARE @FslData TABLE (City NVARCHAR(100), District NVARCHAR(100), Cnt INT);
        INSERT INTO @FslData VALUES
            (N'İstanbul', N'Kadıköy',    1),
            (N'İstanbul', N'Zeytinburnu',1),
            (N'Ankara',   N'Çankaya',    1),
            (N'İzmir',    N'Karşıyaka',  1),
            (N'Antalya',  N'Muratpaşa',  1);

        DECLARE fsl_cur CURSOR LOCAL FAST_FORWARD FOR SELECT City, District, Cnt FROM @FslData;
        DECLARE @fsl_city NVARCHAR(100), @fsl_dist NVARCHAR(100), @fsl_cnt INT, @fsl_i INT;
        OPEN fsl_cur;
        FETCH NEXT FROM fsl_cur INTO @fsl_city, @fsl_dist, @fsl_cnt;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @fsl_i = 1;
            WHILE @fsl_i <= @fsl_cnt
            BEGIN
                SET @Sql = N'INSERT INTO dbo.ForSaleLandListings (' + @ColList + N')
                    SELECT ' + @ColList + N' FROM (SELECT TOP 1 * FROM dbo.ForSaleLandListings
                    WHERE PropertyStatus = N''Aktif'' ORDER BY (SELECT NULL)) AS _src;';
                EXEC sp_executesql @Sql;
                UPDATE dbo.ForSaleLandListings
                SET City           = @fsl_city,
                    District       = @fsl_dist,
                    PropertyName   = N'[DEMO] ' + @fsl_dist + N' ' + CAST(@fsl_i AS NVARCHAR(10)),
                    PropertyStatus = N'Aktif',
                    BestDeals      = 0,
                    CreatedDate    = GETDATE()
                WHERE ForSaleLandListingId = (SELECT MAX(ForSaleLandListingId) FROM dbo.ForSaleLandListings);
                SET @fsl_i = @fsl_i + 1;
            END;
            FETCH NEXT FROM fsl_cur INTO @fsl_city, @fsl_dist, @fsl_cnt;
        END;
        CLOSE fsl_cur; DEALLOCATE fsl_cur;
    END;

    -- =========================================================================
    -- RentalHousingListings
    -- =========================================================================
    SET @TableName = N'RentalHousingListings';

    IF NOT EXISTS (SELECT 1 FROM dbo.RentalHousingListings WHERE PropertyStatus = N'Aktif')
        PRINT N'RentalHousingListings: Şablon yok, atlanıyor.';
    ELSE
    BEGIN
        SELECT @ColList = STRING_AGG(QUOTENAME(c.name), N', ') WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c
        WHERE c.object_id = OBJECT_ID(N'dbo.' + @TableName)
          AND c.is_identity = 0 AND c.is_computed = 0;

        DECLARE @RhData TABLE (City NVARCHAR(100), District NVARCHAR(100), Cnt INT);
        INSERT INTO @RhData VALUES
            (N'İstanbul',   N'Beşiktaş',   6),
            (N'İstanbul',   N'Zeytinburnu',3),
            (N'İzmir',      N'Bornova',    4),
            (N'Ankara',     N'Keçiören',   2),
            (N'Eskişehir',  N'Tepebaşı',   4),
            (N'Antalya',    N'Konyaaltı',  1);

        DECLARE rh_cur CURSOR LOCAL FAST_FORWARD FOR SELECT City, District, Cnt FROM @RhData;
        DECLARE @rh_city NVARCHAR(100), @rh_dist NVARCHAR(100), @rh_cnt INT, @rh_i INT;
        OPEN rh_cur;
        FETCH NEXT FROM rh_cur INTO @rh_city, @rh_dist, @rh_cnt;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @rh_i = 1;
            WHILE @rh_i <= @rh_cnt
            BEGIN
                SET @Sql = N'INSERT INTO dbo.RentalHousingListings (' + @ColList + N')
                    SELECT ' + @ColList + N' FROM (SELECT TOP 1 * FROM dbo.RentalHousingListings
                    WHERE PropertyStatus = N''Aktif'' ORDER BY (SELECT NULL)) AS _src;';
                EXEC sp_executesql @Sql;
                UPDATE dbo.RentalHousingListings
                SET City           = @rh_city,
                    District       = @rh_dist,
                    PropertyName   = N'[DEMO] ' + @rh_dist + N' ' + CAST(@rh_i AS NVARCHAR(10)),
                    PropertyStatus = N'Aktif',
                    BestDeals      = 0,
                    CreatedDate    = GETDATE()
                WHERE RentalHousingListId = (SELECT MAX(RentalHousingListId) FROM dbo.RentalHousingListings);
                SET @rh_i = @rh_i + 1;
            END;
            FETCH NEXT FROM rh_cur INTO @rh_city, @rh_dist, @rh_cnt;
        END;
        CLOSE rh_cur; DEALLOCATE rh_cur;
    END;

    -- =========================================================================
    -- RentalCommercialPropertyListings
    -- =========================================================================
    SET @TableName = N'RentalCommercialPropertyListings';

    IF NOT EXISTS (SELECT 1 FROM dbo.RentalCommercialPropertyListings WHERE PropertyStatus = N'Aktif')
        PRINT N'RentalCommercialPropertyListings: Şablon yok, atlanıyor.';
    ELSE
    BEGIN
        SELECT @ColList = STRING_AGG(QUOTENAME(c.name), N', ') WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c
        WHERE c.object_id = OBJECT_ID(N'dbo.' + @TableName)
          AND c.is_identity = 0 AND c.is_computed = 0;

        DECLARE @RcData TABLE (City NVARCHAR(100), District NVARCHAR(100), Cnt INT);
        INSERT INTO @RcData VALUES
            (N'İstanbul',  N'Beşiktaş',   1),
            (N'İstanbul',  N'Zeytinburnu',1),
            (N'İzmir',     N'Bornova',    1),
            (N'Ankara',    N'Keçiören',   1),
            (N'Eskişehir', N'Tepebaşı',   1),
            (N'Antalya',   N'Konyaaltı',  1);

        DECLARE rc_cur CURSOR LOCAL FAST_FORWARD FOR SELECT City, District, Cnt FROM @RcData;
        DECLARE @rc_city NVARCHAR(100), @rc_dist NVARCHAR(100), @rc_cnt INT, @rc_i INT;
        OPEN rc_cur;
        FETCH NEXT FROM rc_cur INTO @rc_city, @rc_dist, @rc_cnt;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @rc_i = 1;
            WHILE @rc_i <= @rc_cnt
            BEGIN
                SET @Sql = N'INSERT INTO dbo.RentalCommercialPropertyListings (' + @ColList + N')
                    SELECT ' + @ColList + N' FROM (SELECT TOP 1 * FROM dbo.RentalCommercialPropertyListings
                    WHERE PropertyStatus = N''Aktif'' ORDER BY (SELECT NULL)) AS _src;';
                EXEC sp_executesql @Sql;
                UPDATE dbo.RentalCommercialPropertyListings
                SET City           = @rc_city,
                    District       = @rc_dist,
                    PropertyName   = N'[DEMO] ' + @rc_dist + N' ' + CAST(@rc_i AS NVARCHAR(10)),
                    PropertyStatus = N'Aktif',
                    BestDeals      = 0,
                    CreatedDate    = GETDATE()
                WHERE RentalCommercialListId = (SELECT MAX(RentalCommercialListId) FROM dbo.RentalCommercialPropertyListings);
                SET @rc_i = @rc_i + 1;
            END;
            FETCH NEXT FROM rc_cur INTO @rc_city, @rc_dist, @rc_cnt;
        END;
        CLOSE rc_cur; DEALLOCATE rc_cur;
    END;

    -- =========================================================================
    -- RentalLandListings
    -- =========================================================================
    SET @TableName = N'RentalLandListings';

    IF NOT EXISTS (SELECT 1 FROM dbo.RentalLandListings WHERE PropertyStatus = N'Aktif')
        PRINT N'RentalLandListings: Şablon yok, atlanıyor.';
    ELSE
    BEGIN
        SELECT @ColList = STRING_AGG(QUOTENAME(c.name), N', ') WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c
        WHERE c.object_id = OBJECT_ID(N'dbo.' + @TableName)
          AND c.is_identity = 0 AND c.is_computed = 0;

        DECLARE @RlData TABLE (City NVARCHAR(100), District NVARCHAR(100), Cnt INT);
        INSERT INTO @RlData VALUES
            (N'İstanbul', N'Beşiktaş', 1),
            (N'İzmir',    N'Bornova',  1);

        DECLARE rl_cur CURSOR LOCAL FAST_FORWARD FOR SELECT City, District, Cnt FROM @RlData;
        DECLARE @rl_city NVARCHAR(100), @rl_dist NVARCHAR(100), @rl_cnt INT, @rl_i INT;
        OPEN rl_cur;
        FETCH NEXT FROM rl_cur INTO @rl_city, @rl_dist, @rl_cnt;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @rl_i = 1;
            WHILE @rl_i <= @rl_cnt
            BEGIN
                SET @Sql = N'INSERT INTO dbo.RentalLandListings (' + @ColList + N')
                    SELECT ' + @ColList + N' FROM (SELECT TOP 1 * FROM dbo.RentalLandListings
                    WHERE PropertyStatus = N''Aktif'' ORDER BY (SELECT NULL)) AS _src;';
                EXEC sp_executesql @Sql;
                UPDATE dbo.RentalLandListings
                SET City           = @rl_city,
                    District       = @rl_dist,
                    PropertyName   = N'[DEMO] ' + @rl_dist + N' ' + CAST(@rl_i AS NVARCHAR(10)),
                    PropertyStatus = N'Aktif',
                    BestDeals      = 0,
                    CreatedDate    = GETDATE()
                WHERE RentalLandListingId = (SELECT MAX(RentalLandListingId) FROM dbo.RentalLandListings);
                SET @rl_i = @rl_i + 1;
            END;
            FETCH NEXT FROM rl_cur INTO @rl_city, @rl_dist, @rl_cnt;
        END;
        CLOSE rl_cur; DEALLOCATE rl_cur;
    END;

    -- =========================================================================
    -- Konum ID'leri (uygulama SaveChanges'te dolduruyor; doğrudan SQL eklemede elle)
    -- =========================================================================
    DECLARE @LocationTables TABLE (TableName NVARCHAR(128));
    INSERT INTO @LocationTables (TableName) VALUES
        (N'dbo.ForSaleHousingPropertyListings'),
        (N'dbo.ForSaleLandListings'),
        (N'dbo.ForSaleCommercialPropertyListings'),
        (N'dbo.RentalHousingListings'),
        (N'dbo.RentalLandListings'),
        (N'dbo.RentalCommercialPropertyListings');

    DECLARE @NormFmt NVARCHAR(MAX) = N'REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(LTRIM(RTRIM({col})) COLLATE Turkish_CS_AS),N''İ'',N''I''),N''Ş'',N''S''),N''Ğ'',N''G''),N''Ü'',N''U''),N''Ö'',N''O''),N''Ç'',N''C''),N''Â'',N''A''),N''Î'',N''I''),N''Û'',N''U'')';
    DECLARE @Norm_c_City NVARCHAR(MAX) = REPLACE(@NormFmt, N'{col}', N'c.Name');
    DECLARE @Norm_l_City NVARCHAR(MAX) = REPLACE(@NormFmt, N'{col}', N'l.City');
    DECLARE @Norm_d_Dist NVARCHAR(MAX) = REPLACE(@NormFmt, N'{col}', N'd.Name');
    DECLARE @Norm_l_Dist NVARCHAR(MAX) = REPLACE(@NormFmt, N'{col}', N'l.District');
    DECLARE @Norm_n_Neig NVARCHAR(MAX) = REPLACE(@NormFmt, N'{col}', N'n.Name');
    DECLARE @Norm_l_Neig NVARCHAR(MAX) = REPLACE(@NormFmt, N'{col}', N'l.Neighborhood');

    DECLARE @LocTable NVARCHAR(128);
    DECLARE loc_cur CURSOR LOCAL FAST_FORWARD FOR SELECT TableName FROM @LocationTables;
    OPEN loc_cur;
    FETCH NEXT FROM loc_cur INTO @LocTable;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- 1. CityId
        SET @Sql = CAST(N'
            UPDATE l
            SET l.CityId = c.CityId
            FROM ' AS NVARCHAR(MAX)) + @LocTable + N' l
            INNER JOIN [Cities] c
                ON ' + @Norm_c_City + N' = ' + @Norm_l_City + N'
            WHERE l.CityId IS NULL AND l.City IS NOT NULL;';
        EXEC sp_executesql @Sql;

        -- 2. DistrictId
        SET @Sql = CAST(N'
            UPDATE l
            SET l.DistrictId = d.DistrictId
            FROM ' AS NVARCHAR(MAX)) + @LocTable + N' l
            INNER JOIN [Districts] d
                ON d.CityId = l.CityId
               AND ' + @Norm_d_Dist + N' = ' + @Norm_l_Dist + N'
            WHERE l.DistrictId IS NULL AND l.CityId IS NOT NULL AND l.District IS NOT NULL;';
        EXEC sp_executesql @Sql;

        -- 3. NeighborhoodId
        SET @Sql = CAST(N'
            UPDATE l
            SET l.NeighborhoodId = (
                SELECT MIN(n.NeighborhoodId)
                FROM [Neighborhoods] n
                WHERE n.DistrictId = l.DistrictId
                  AND (
                      ' AS NVARCHAR(MAX)) + @Norm_n_Neig + N' = ' + @Norm_l_Neig + N' + N'' MAHALLESI''
                   OR ' + @Norm_n_Neig + N' = ' + @Norm_l_Neig + N'
                  )
            )
            FROM ' + @LocTable + N' l
            WHERE l.NeighborhoodId IS NULL
              AND l.DistrictId IS NOT NULL
              AND l.Neighborhood IS NOT NULL
              AND EXISTS (
                  SELECT 1
                  FROM [Neighborhoods] n
                  WHERE n.DistrictId = l.DistrictId
                    AND (
                        ' + @Norm_n_Neig + N' = ' + @Norm_l_Neig + N' + N'' MAHALLESI''
                     OR ' + @Norm_n_Neig + N' = ' + @Norm_l_Neig + N'
                    )
              );';
        EXEC sp_executesql @Sql;

        FETCH NEXT FROM loc_cur INTO @LocTable;
    END;
    CLOSE loc_cur; DEALLOCATE loc_cur;

    COMMIT;
    PRINT N'Demo veriler başarıyla eklendi.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    PRINT N'HATA: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
