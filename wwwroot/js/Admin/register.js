    document.addEventListener('DOMContentLoaded', function() {
            const studentRadio = document.getElementById('student');
    const staffRadio = document.getElementById('staff');
    const studentIdField = document.getElementById('studentId-field');
    const classField = document.getElementById('class-field');
    const addressField = document.getElementById('address-field');

    // Xử lý sự kiện khi thay đổi lựa chọn
    function handleUserTypeChange() {
                if (staffRadio.checked) {
        // Nếu là Staff
        studentIdField.style.display = 'none';
    classField.style.display = 'none';
    addressField.style.display = 'block';
                } else {
        // Nếu là Student
        studentIdField.style.display = 'block';
    classField.style.display = 'block';
    addressField.style.display = 'none';
                }
            }

    // Gán sự kiện cho cả hai radio button
    studentRadio.addEventListener('change', handleUserTypeChange);
    staffRadio.addEventListener('change', handleUserTypeChange);

    // Gọi hàm một lần khi trang load để đảm bảo trạng thái ban đầu đúng
    handleUserTypeChange();
        });