# Canlı veritabanı güncellemesi

1. **Yedek al** (canlıda): `BACKUP DATABASE [fibiemlak.com] TO DISK = N'...\fibiemlak-oncesi.bak' WITH COPY_ONLY;`
2. **Migration'lar:** `migrations-idempotent.sql` dosyasını schema değiştirme yetkisi olan bir hesapla çalıştır (uygulama hesabı `fibiemlak_app` şema değiştiremez). Betik idempotent: yalnızca `__EFMigrationsHistory`'de olmayan migration'ları uygular, tekrar çalıştırmak zararsız.
   Yeniden üretmek için (repo kökünde):
   `dotnet ef migrations script --idempotent --project Infrastructure/FibiEmlakDanismanlik.Persistance --startup-project Presentation/FibiEmlakDanismanlik.WebApi -o tools/sql/deploy/migrations-idempotent.sql`
3. **Veri düzeltmeleri:** `data-fixes.sql`.
4. Yeni tablolar (`SiteTexts` vb.) için `fibiemlak_app` ek yetki istemez (db_datareader/db_datawriter yeterli).

`migrations-idempotent.sql` üretilen bir dosyadır, repoya eklenmez (`.gitignore`).
