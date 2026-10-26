-- Canlı veritabanı veri düzeltmeleri (migration'lardan SONRA, bir kez çalıştırılır; tekrar çalıştırmak zararsızdır)
SET NOCOUNT ON; SET XACT_ABORT ON;
BEGIN TRAN;

-- AboutUs görsel yolu: ters bölü ve küçük harfli yol yalnızca Windows'ta çalışıyordu; yeni görsel kuralı da reddediyor
UPDATE dbo.AboutUs
SET AboutUsImgUrl = N'/Realshed/assets/images/Fibi Emlak Danışmanlık başarı yolunda.png'
WHERE AboutUsImgUrl = N'\realshed\assets\images\fibi emlak danışmanlık başarı yolunda.png';
PRINT CONCAT('AboutUs düzeltilen satır: ', @@ROWCOUNT);

COMMIT;
