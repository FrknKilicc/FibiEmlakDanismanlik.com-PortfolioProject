/**
 * Fibi Emlak Danışmanlık - Kredi Taksit Hesaplayıcı
 * Satılık ilan detay sayfasında bağımsız çalışan anüite hesaplama modülü.
 */
(function ($) {
    'use strict';

    var trCurrencyFormatter = new Intl.NumberFormat('tr-TR', {
        maximumFractionDigits: 0,
        minimumFractionDigits: 0
    });

    function formatTL(amount) {
        var num = Math.round(Number(amount) || 0);
        return trCurrencyFormatter.format(num) + ' TL';
    }

    function initLoanCalculator($container) {
        var rawPrice = parseFloat($container.attr('data-price'));
        if (isNaN(rawPrice) || rawPrice <= 0) {
            return;
        }

        var $bankSelect = $container.find('.loan-calc-bank');
        var $downPaymentRange = $container.find('.loan-calc-downpayment-range');
        var $downPaymentPercent = $container.find('.loan-calc-downpayment-percent');
        var $downPaymentAmount = $container.find('.loan-calc-downpayment-amount');
        var $termSelect = $container.find('.loan-calc-term');

        var $outAmount = $container.find('.loan-calc-amount');
        var $outMonthly = $container.find('.loan-calc-monthly-installment');
        var $outTotalPayment = $container.find('.loan-calc-total-payment');
        var $outTotalInterest = $container.find('.loan-calc-total-interest');
        var $outAllocationFee = $container.find('.loan-calc-allocation-fee');

        function updateTermLimits() {
            var $selectedBank = $bankSelect.find('option:selected');
            var maxTerm = parseInt($selectedBank.attr('data-max-term'), 10);
            if (isNaN(maxTerm) || maxTerm <= 0) {
                maxTerm = 120;
            }

            var currentTerm = parseInt($termSelect.val(), 10);
            var isCurrentDisabled = false;

            $termSelect.find('option').each(function () {
                var termVal = parseInt($(this).val(), 10);
                if (termVal > maxTerm) {
                    $(this).prop('disabled', true);
                    if (termVal === currentTerm) {
                        isCurrentDisabled = true;
                    }
                } else {
                    $(this).prop('disabled', false);
                }
            });

            // Seçili vade artık izin verilenden büyükse veya geçersizse:
            // 120 izinliyse 120, değilse izin verilen en büyük vadeyi seç
            if (isCurrentDisabled || isNaN(currentTerm)) {
                var bestTerm = null;
                $termSelect.find('option:not(:disabled)').each(function () {
                    var v = parseInt($(this).val(), 10);
                    if (bestTerm === null || v > bestTerm) {
                        bestTerm = v;
                    }
                });

                if (bestTerm !== null) {
                    $termSelect.val(bestTerm.toString());
                }
            }
        }

        function calculate() {
            var $selectedBank = $bankSelect.find('option:selected');
            if (!$selectedBank.length) {
                return;
            }

            var monthlyRate = parseFloat($selectedBank.attr('data-rate')) || 0;
            var feeRate = parseFloat($selectedBank.attr('data-fee')) || 0;
            var downPaymentPercent = parseInt($downPaymentRange.val(), 10) || 0;
            var termMonths = parseInt($termSelect.val(), 10) || 0;

            // Peşinat ve kredi ana para tutarı
            var downPayment = rawPrice * (downPaymentPercent / 100);
            var loanAmount = rawPrice - downPayment;
            if (loanAmount < 0) {
                loanAmount = 0;
            }

            // Peşinat metinlerini güncelle
            $downPaymentPercent.text(downPaymentPercent.toString());
            $downPaymentAmount.text(formatTL(downPayment));

            // Anüite taksit formülü:
            // r = aylık oran / 100, n = vade
            // r > 0 ise taksit = K × r / (1 − (1 + r)^−n)
            // r = 0 ise K / n
            var r = monthlyRate / 100;
            var monthlyPayment = 0;

            if (loanAmount > 0 && termMonths > 0) {
                if (r > 0) {
                    monthlyPayment = loanAmount * (r / (1 - Math.pow(1 + r, -termMonths)));
                } else {
                    monthlyPayment = loanAmount / termMonths;
                }
            }

            var totalPayment = monthlyPayment * termMonths;
            var totalInterest = totalPayment > loanAmount ? totalPayment - loanAmount : 0;
            var allocationFee = loanAmount * (feeRate / 100);

            // Sonuçları DOM'a yalnızca text() ile yaz
            $outAmount.text(formatTL(loanAmount));
            $outMonthly.text(formatTL(monthlyPayment));
            $outTotalPayment.text(formatTL(totalPayment));
            $outTotalInterest.text(formatTL(totalInterest));
            $outAllocationFee.text(formatTL(allocationFee));
        }

        $bankSelect.on('change', function () {
            updateTermLimits();
            calculate();
        });

        $downPaymentRange.on('input change', function () {
            calculate();
        });

        $termSelect.on('change', function () {
            calculate();
        });

        // İlk çalıştırma
        updateTermLimits();
        calculate();
    }

    $(function () {
        $('.loan-calculator').each(function () {
            initLoanCalculator($(this));
        });
    });
})(jQuery);
