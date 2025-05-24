$(document).ready(function () {
    let html5QrCode;
    const qrReaderId = "qr-reader";
    // Kiểm tra xem #resultModal có tồn tại không trước khi tạo instance
    const resultModalElement = document.getElementById('resultModal');
    let resultModalInstance;
    if (resultModalElement) {
        resultModalInstance = new bootstrap.Modal(resultModalElement);
    } else {
        console.error("Modal element with ID 'resultModal' not found.");
    }


    function onScanSuccess(decodedText, decodedResult) {
        $("#qr-scan-status").text(`Đã quét: ${decodedText.substring(0, 25)}...`);
        console.log(`QR Code Detected: ${decodedText}`);
        if (html5QrCode && html5QrCode.isScanning) {
            html5QrCode.stop().then(() => {
                $("#scanAgainButton").show();
                processScannedData(decodedText, true);
            }).catch(err => {
                console.error("Lỗi dừng trình quét:", err);
                $("#scanAgainButton").show();
                processScannedData(decodedText, true);
            });
        } else {
            processScannedData(decodedText, true);
        }
    }

    function onScanFailure(error) {
        // console.warn(`QR error = ${error}`);
    }

    function startScanner() {
        resetInfoDisplay();
        $("#scanAgainButton").hide();
        $("#qr-scan-status").empty();
        $(".processing-indicator").hide();

        if (!html5QrCode) {
            try {
                html5QrCode = new Html5Qrcode(qrReaderId);
            } catch (e) {
                console.error("Error creating Html5Qrcode object for qrReaderId '" + qrReaderId + "':", e);
                $("#qr-scan-status").text("Lỗi khởi tạo thư viện quét QR. Kiểm tra ID phần tử HTML.");
                if (resultModalInstance) showModal("Lỗi Thư Viện", "Không thể khởi tạo thư viện quét QR. Phần tử HTML 'qr-reader' có thể không tồn tại.", false);
                return;
            }
        }

        const config = {
            fps: 10,
            qrbox: (viewfinderWidth, viewfinderHeight) => {
                const minEdgePercentage = 0.65;
                const minEdgeSize = Math.min(viewfinderWidth, viewfinderHeight);
                const qrboxSize = Math.floor(minEdgeSize * minEdgePercentage);
                return { width: qrboxSize, height: qrboxSize };
            },
            rememberLastUsedCamera: true,
            // Bỏ dòng này nếu Html5QrcodeScanType không được định nghĩa hoặc gây lỗi
            // supportedScanTypes: [Html5QrcodeScanType.SCAN_TYPE_CAMERA] 
        };

        // Sử dụng facingMode thay vì cameraId để đơn giản hóa và để thư viện tự chọn
        html5QrCode.start({ facingMode: "environment" }, config, onScanSuccess, onScanFailure)
            .catch(err => {
                console.error("Không thể khởi động trình quét:", err);
                $("#qr-scan-status").text("Lỗi khởi động camera. Kiểm tra quyền truy cập hoặc chọn camera khác.");
                if (resultModalInstance) showModal("Lỗi Quét Mã", "Không thể khởi động trình quét QR. Vui lòng kiểm tra quyền truy cập camera.", false);
            });
    }

    $("#scanAgainButton").click(function () {
        if (html5QrCode && !html5QrCode.isScanning) {
            startScanner();
        }
    });

    $("#manualFindButton").click(function () {
        const mssv = $("#mssvManualInput").val().trim();
        if (!mssv) {
            if (resultModalInstance) showModal("Thiếu Thông Tin", "Vui lòng nhập MSSV của sinh viên.", false);
            return;
        }
        resetInfoDisplay();
        processScannedData(mssv, false);
    });

    function showProcessing(show) {
        if (show) {
            $(".processing-indicator").slideDown();
            $("#manualFindButton, #scanAgainButton").prop("disabled", true);
        } else {
            $(".processing-indicator").slideUp();
            $("#manualFindButton, #scanAgainButton").prop("disabled", false);
        }
    }

    function resetInfoDisplay() {
        $("#infoStatus").text("-").addClass("placeholder");
        $("#infoBienSo").text("-").addClass("placeholder");
        $("#infoMSSV").text("-").addClass("placeholder");
        $("#infoHoTen").text("-").addClass("placeholder");
        $("#infoLop").text("-").addClass("placeholder");
        $("#infoThoiGian").text("-").addClass("placeholder");
        $("#infoSlot").text("-").addClass("placeholder");
        $("#infoNgayHetHan").text("-").addClass("placeholder");
    }

    function updateInfoDisplay(data, serverMessage) {
        let statusText = "Không xác định";
        if (data.success) {
            statusText = data.action === "checkin" ? "CHECK-IN THÀNH CÔNG" : "CHECK-OUT THÀNH CÔNG";
        } else {
            statusText = data.action === "expired" ? "VÉ HẾT HẠN" : (data.action === "no_zone_slot" ? "HẾT CHỖ" : (data.message || "THẤT BẠI"));
        }
        $("#infoStatus").text(statusText).removeClass("placeholder");
        $("#infoBienSo").text(data.ticketDetails?.bienSoXe || "-").toggleClass("placeholder", !data.ticketDetails?.bienSoXe);
        $("#infoMSSV").text(data.student?.mssv || "-").toggleClass("placeholder", !data.student?.mssv);
        $("#infoHoTen").text(data.student?.hoTen || "-").toggleClass("placeholder", !data.student?.hoTen);
        $("#infoLop").text(data.student?.lop || "-").toggleClass("placeholder", !data.student?.lop);

        let timeDetail = "-";
        if (data.action === "checkin" && data.ticketDetails?.thoiGianVao) {
            timeDetail = data.ticketDetails.thoiGianVao;
        } else if (data.action === "checkout" && data.ticketDetails?.thoiGianRa) {
            timeDetail = data.ticketDetails.thoiGianRa;
        }
        $("#infoThoiGian").text(timeDetail).toggleClass("placeholder", timeDetail === "-");
        $("#infoSlot").text(data.slotName || (data.action === "checkout" ? data.slotName || "N/A" : "Chưa có")).toggleClass("placeholder", !data.slotName && !(data.action === "checkout"));
        $("#infoNgayHetHan").text(data.ticketDetails?.ngayHetHan || "-").toggleClass("placeholder", !data.ticketDetails?.ngayHetHan);
    }

    // Quan trọng: Định nghĩa biến ajaxUrlRoot ở đây để có thể dùng @Url.Action
    // Bạn cần truyền giá trị này từ View sang file JS, hoặc định nghĩa nó trực tiếp
    // Cách tốt hơn là dùng data-attributes trên một phần tử HTML trong View.
    // Ví dụ, trong Scan.cshtml, thêm: <div id="scanConfig" data-process-qr-url="@Url.Action("ProcessQRCode", "QRCode")" data-find-by-mssv-url="@Url.Action("FindAndProcessTicketByMSSV", "QRCode")"></div>

    function processScannedData(inputValue, isQrScan) {
        showProcessing(true);

        // Lấy URL từ data attributes (nếu bạn dùng cách này)
        // const processQrUrl = $("#scanConfig").data("process-qr-url");
        // const findByMssvUrl = $("#scanConfig").data("find-by-mssv-url");
        // const ajaxUrl = isQrScan ? processQrUrl : findByMssvUrl;

        // Hoặc hardcode (ít linh hoạt hơn, nhưng đơn giản cho ví dụ này nếu @Url.Action không dùng được trực tiếp trong file .js)
        // BẠN SẼ CẦN THAY ĐỔI CÁC URL NÀY CHO ĐÚNG VỚI ROUTING CỦA BẠN
        // Cách tốt nhất là truyền chúng từ view, ví dụ qua các biến global hoặc data-attributes
        const rootPath = ""; // Nếu app của bạn không ở root, thêm path vào đây, ví dụ "/MyAppName"
        const ajaxUrl = isQrScan ? `${rootPath}/QRCode/ProcessQRCode` : `${rootPath}/QRCode/FindAndProcessTicketByMSSV`;


        const requestData = isQrScan ? { ticketIdContent: inputValue } : { mssv: inputValue };

        if (!ajaxUrl) {
            console.error("AJAX URL is not defined.");
            if (resultModalInstance) showModal("Lỗi Cấu Hình", "Lỗi: Đường dẫn xử lý không được định nghĩa.", false);
            showProcessing(false);
            return;
        }


        $.ajax({
            url: ajaxUrl,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (response) {
                updateInfoDisplay(response, response.message);
                if (resultModalInstance) showModal(response.success ? "Thành Công" : "Thông Báo", response.message, response.success);
                if (!isQrScan && response.success) $("#mssvManualInput").val("");
            },
            error: function (xhr) {
                let errorMessage = "Lỗi không xác định. Vui lòng thử lại.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                } else if (xhr.responseText) {
                    try {
                        const errObj = JSON.parse(xhr.responseText);
                        if (errObj && errObj.message) errorMessage = errObj.message;
                    } catch (e) { /* Ignore parsing error, use default */ }
                }
                updateInfoDisplay({ success: false, message: errorMessage }, errorMessage);
                if (resultModalInstance) showModal("Lỗi Hệ Thống", errorMessage, false);
                console.error("Lỗi xử lý:", xhr.responseText);
            },
            complete: function () {
                showProcessing(false);
                if (html5QrCode && !html5QrCode.isScanning && $("#scanAgainButton").is(":hidden")) {
                    $("#scanAgainButton").show();
                }
            }
        });
    }

    function showModal(title, message, isSuccess) {
        if (!resultModalInstance) return; // Không làm gì nếu modal chưa được khởi tạo
        $("#resultModalLabel").text(title);
        $("#resultModalBody").html(message);

        const modalHeader = $("#resultModalHeader");
        modalHeader.removeClass("bg-success bg-danger").addClass(isSuccess ? "bg-success" : "bg-danger");

        resultModalInstance.show();
    }

    // Khởi động scanner
    // Phải đảm bảo DOM đã sẵn sàng và thư viện html5-qrcode đã được tải
    // Kiểm tra sơ bộ quyền truy cập camera.
    if (navigator.mediaDevices && typeof navigator.mediaDevices.getUserMedia === 'function') {
        console.log("getUserMedia is supported. Attempting to start scanner.");
        startScanner();
    } else {
        console.warn("getUserMedia is NOT supported or navigator.mediaDevices is undefined.");
        $("#qr-scan-status").text("Trình duyệt không hỗ trợ truy cập camera hoặc tính năng đã bị tắt.");
        if (resultModalInstance) showModal("Lỗi Camera", "Trình duyệt của bạn không hỗ trợ truy cập camera hoặc tính năng này đã bị tắt. Vui lòng sử dụng trình duyệt khác hoặc kiểm tra cài đặt.", false);
    }
});