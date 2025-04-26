USE PBL3;

CREATE TABLE ROLES (
	MA_CHUC_VU INT PRIMARY KEY,
	TEN_CHUC_VU NVARCHAR(20),
);

CREATE TABLE USERS (
	ID NVARCHAR(10) PRIMARY KEY,
	HO_TEN NVARCHAR(50),
	NGAY_SINH DATETIME,
	GIOI_TINH BIT,
	MA_CHUC_VU INT FOREIGN KEY REFERENCES ROLES(MA_CHUC_VU),
);

CREATE TABLE XE_DANG_GIU (
	STT INT PRIMARY KEY,
	ID NVARCHAR(10) FOREIGN KEY REFERENCES USERS(ID),
	HO_TEN_CHU_XE NVARCHAR(50),
	BIEN_SO_XE NVARCHAR(12),
	NGAY_GIU_XE DATETIME,
);

INSERT INTO ROLES (MA_CHUC_VU, TEN_CHUC_VU) VALUES
						(1, 'Admin'),
						(2, 'Quan Ly'),
						(3, 'Khach Hang');

INSERT INTO USERS (ID, HO_TEN, NGAY_SINH, GIOI_TINH, MA_CHUC_VU) VALUES
					('U001', 'Nguyen Van A', '1990-05-12', 1, 1), -- Admin
					('U002', 'Tran Thi B', '1992-08-23', 0, 2),
					('U003', 'Le Van C', '1988-11-05', 1, 2),
					('U004', 'Pham Thi D', '1995-04-14', 0, 2),
					('U005', 'Hoang Van E', '1993-09-30', 1, 2),
					('U006', 'Do Thi F', '1996-12-21', 0, 2),
					('U007', 'Ngo Van G', '1987-07-17', 1, 2),
					('U008', 'Dang Thi H', '1994-03-29', 0, 2),
					('U009', 'Bui Van I', '1991-06-08', 1, 2),
					('U010', 'Vu Thi J', '1997-01-15', 0, 2),
					('U011', 'Nguyen Van K', '1990-05-17', 1, 2),
					('U012', 'Tran Thi L', '1992-08-26', 0, 3),
					('U013', 'Le Van M', '1988-11-09', 1, 3),
					('U014', 'Pham Thi N', '1995-04-18', 0, 3),
					('U015', 'Hoang Van O', '1993-10-03', 1, 3),
					('U016', 'Do Thi P', '1996-12-24', 0, 3),
					('U017', 'Ngo Van Q', '1987-07-20', 1, 3),
					('U018', 'Dang Thi R', '1994-04-01', 0, 3),
					('U019', 'Bui Van S', '1991-06-11', 1, 3),
					('U020', 'Vu Thi T', '1997-01-18', 0, 3),
					('U021', 'Nguyen Van U', '1989-03-12', 1, 3),
					('U022', 'Tran Thi V', '1992-09-23', 0, 3),
					('U023', 'Le Van W', '1987-10-05', 1, 3),
					('U024', 'Pham Thi X', '1996-05-14', 0, 3),
					('U025', 'Hoang Van Y', '1993-12-30', 1, 3),
					('U026', 'Do Thi Z', '1996-01-21', 0, 3),
					('U027', 'Ngo Van AA', '1987-11-17', 1, 3),
					('U028', 'Dang Thi BB', '1994-08-29', 0, 3),
					('U029', 'Bui Van CC', '1991-02-08', 1, 3),
					('U030', 'Vu Thi DD', '1997-03-15', 0, 3);

-- Thêm 20 bản ghi vào bảng XE_DANG_GIU
INSERT INTO XE_DANG_GIU (STT, ID, HO_TEN_CHU_XE, BIEN_SO_XE, NGAY_GIU_XE) VALUES
(1, 'U003', 'Le Van C', '29A-12345', '2025-04-02'),
(2, 'U005', 'Hoang Van E', '30B-23456', '2025-04-03'),
(3, 'U007', 'Ngo Van G', '31C-34567', '2025-04-04'),
(4, 'U009', 'Bui Van I', '32D-45678', '2025-04-05'),
(5, 'U012', 'Tran Thi L', '33E-56789', '2025-04-06'),
(6, 'U014', 'Pham Thi N', '34F-67890', '2025-04-07'),
(7, 'U016', 'Do Thi P', '35G-78901', '2025-04-08'),
(8, 'U018', 'Dang Thi R', '36H-89012', '2025-04-09'),
(9, 'U020', 'Vu Thi T', '37I-90123', '2025-04-10'),
(10, 'U022', 'Tran Thi V', '38J-01234', '2025-04-11'),
(11, 'U024', 'Pham Thi X', '39K-12321', '2025-04-12'),
(12, 'U026', 'Do Thi Z', '40L-23432', '2025-04-13'),
(13, 'U028', 'Dang Thi BB', '41M-34543', '2025-04-14'),
(14, 'U030', 'Vu Thi DD', '42N-45654', '2025-04-15'),
(15, 'U002', 'Tran Thi B', '43O-56765', '2025-04-16'),
(16, 'U004', 'Pham Thi D', '44P-67876', '2025-04-17'),
(17, 'U006', 'Do Thi F', '45Q-78987', '2025-04-18'),
(18, 'U008', 'Dang Thi H', '46R-89098', '2025-04-19'),
(19, 'U010', 'Vu Thi J', '47S-90109', '2025-04-20'),
(20, 'U011', 'Nguyen Van K', '48T-01210', '2025-04-21');

