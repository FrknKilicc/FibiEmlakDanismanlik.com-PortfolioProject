/**
 * Fibi Emlak Yönetim Paneli - 2FA Kurtarma Kodları İndirme Yardımcısı
 */
document.addEventListener("DOMContentLoaded", function () {
  const downloadButton = document.getElementById("btnDownloadCodes");
  const container = document.getElementById("recoveryCodesContainer");

  if (!downloadButton || !container) {
    return;
  }

  downloadButton.addEventListener("click", function () {
    let codesText = container.getAttribute("data-codes") || "";
    if (!codesText.trim()) {
      const codeElements = container.querySelectorAll(".recovery-code-item");
      const list = [];
      codeElements.forEach(function (el) {
        const c = el.getAttribute("data-code") || el.textContent.trim();
        if (c) {
          list.push(c);
        }
      });
      codesText = list.join("\r\n");
    } else {
      codesText = codesText.split("\n").join("\r\n");
    }

    const header = "FİBİ EMLAK YÖNETİM PANELİ - İKİ ADIMLI DOĞRULAMA KURTARMA KODLARI\r\n" +
      "Oluşturulma Tarihi: " + new Date().toLocaleString("tr-TR") + "\r\n" +
      "------------------------------------------------------------------\r\n" +
      "DİKKAT: Bu kodlar tek kullanımlıktır. Cihazınıza erişemediğinizde\r\n" +
      "giriş yapabilmek için bu dosyayı güvenli bir yerde saklayın.\r\n" +
      "------------------------------------------------------------------\r\n\r\n";

    const fullContent = header + codesText + "\r\n";

    const blob = new Blob([fullContent], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const downloadAnchor = document.createElement("a");

    downloadAnchor.href = url;
    downloadAnchor.download = "fibi-kurtarma-kodlari.txt";
    document.body.appendChild(downloadAnchor);
    downloadAnchor.click();

    setTimeout(function () {
      document.body.removeChild(downloadAnchor);
      URL.revokeObjectURL(url);
    }, 100);
  });
});
