/**
 * Fibi Emlak Yönetim Paneli - İki Adımlı Doğrulama (2FA) Giriş Ekranı Betiği
 * TOTP kodu ile kurtarma kodu giriş alanları arasındaki geçişi yönetir.
 */
document.addEventListener("DOMContentLoaded", function () {
  const useRecoveryCodeInput = document.getElementById("useRecoveryCodeInput");
  const totpSection = document.getElementById("totpSection");
  const totpCodeInput = document.getElementById("totpCodeInput");
  const btnSwitchToRecovery = document.getElementById("btnSwitchToRecovery");
  const recoverySection = document.getElementById("recoverySection");
  const recoveryCodeInput = document.getElementById("recoveryCodeInput");
  const btnSwitchToTotp = document.getElementById("btnSwitchToTotp");

  if (!totpSection || !recoverySection) {
    return;
  }

  // Kurtarma koduna geçiş
  if (btnSwitchToRecovery) {
    btnSwitchToRecovery.addEventListener("click", function () {
      if (useRecoveryCodeInput) {
        useRecoveryCodeInput.value = "true";
      }
      totpSection.classList.add("d-none");
      recoverySection.classList.remove("d-none");
      if (recoveryCodeInput) {
        recoveryCodeInput.focus();
      }
    });
  }

  // TOTP koduna geçiş
  if (btnSwitchToTotp) {
    btnSwitchToTotp.addEventListener("click", function () {
      if (useRecoveryCodeInput) {
        useRecoveryCodeInput.value = "false";
      }
      recoverySection.classList.add("d-none");
      totpSection.classList.remove("d-none");
      if (totpCodeInput) {
        totpCodeInput.focus();
      }
    });
  }
});
