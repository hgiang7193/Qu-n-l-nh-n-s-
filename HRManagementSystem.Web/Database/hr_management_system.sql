-- Script tạo cơ sở dữ liệu cho Hệ thống Quản lý Nhân sự
-- File: hr_management_system.sql
-- Mật khẩu MySQL: 12345

-- Tạo cơ sở dữ liệu
CREATE DATABASE IF NOT EXISTS HRManagementSystem CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE HRManagementSystem;

-- Bảng Users
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    EmployeeCode VARCHAR(20) NOT NULL UNIQUE,
    Phone VARCHAR(20),
    Address TEXT,
    DateOfBirth DATE,
    HireDate DATETIME,
    Status ENUM('active', 'inactive') DEFAULT 'active',
    Salary DECIMAL(10, 2),
    Notes TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Bảng Roles
CREATE TABLE Roles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Bảng UserRoles (bảng liên kết giữa Users và Roles)
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

-- Bảng Permissions
CREATE TABLE Permissions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Bảng RolePermissions (bảng liên kết giữa Roles và Permissions)
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE
);

-- Bảng Positions
CREATE TABLE Positions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Status ENUM('active', 'inactive') DEFAULT 'active',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Bảng Departments
CREATE TABLE Departments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Description TEXT,
    ParentId INT NULL,
    ManagerId INT NULL,
    Status ENUM('active', 'inactive') DEFAULT 'active',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ParentId) REFERENCES Departments(Id),
    FOREIGN KEY (ManagerId) REFERENCES Users(Id)
);

-- Bảng Projects
CREATE TABLE Projects (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Description TEXT,
    StartDate DATE,
    EndDate DATE,
    Status ENUM('planning', 'active', 'completed', 'cancelled') DEFAULT 'active',
    Budget DECIMAL(12, 2),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Bảng ProjectAssignments
CREATE TABLE ProjectAssignments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjectId INT NOT NULL,
    EmployeeId INT NOT NULL,
    Role VARCHAR(100) NOT NULL,
    AssignmentDate DATE,
    Status ENUM('assigned', 'active', 'completed', 'cancelled') DEFAULT 'active',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Bảng Attendances
CREATE TABLE Attendances (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeId INT NOT NULL,
    Date DATE NOT NULL,
    CheckIn TIME NULL,
    CheckOut TIME NULL,
    Status ENUM('on_time', 'late', 'absent', 'leave', 'holiday') NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_employee_date (EmployeeId, Date)
);

-- Bảng LeaveRequests
CREATE TABLE LeaveRequests (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeId INT NOT NULL,
    LeaveType VARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Reason TEXT,
    Status ENUM('pending', 'approved', 'rejected') DEFAULT 'pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Bảng Worklogs
CREATE TABLE Worklogs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeId INT NOT NULL,
    ProjectId INT NOT NULL,
    Date DATE NOT NULL,
    Hours DECIMAL(4, 2) NOT NULL,
    Description TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);

-- Chèn dữ liệu mặc định

-- Chèn vai trò mặc định
INSERT INTO Roles (Name, Description) VALUES 
('admin', 'Administrator with full access'),
('hr', 'Human Resources staff'),
('manager', 'Department manager'),
('employee', 'Regular employee');

-- Chèn quyền mặc định
INSERT INTO Permissions (Name, Description) VALUES 
('user_management', 'Manage users and roles'),
('attendance_view', 'View attendance records'),
('attendance_edit', 'Edit attendance records'),
('project_management', 'Manage projects'),
('leave_approval', 'Approve/deny leave requests');

-- Chèn vị trí mặc định
INSERT INTO Positions (Name, Description) VALUES 
('Software Engineer', 'Develops software applications'),
('Project Manager', 'Manages projects and teams'),
('HR Specialist', 'Handles human resources tasks'),
('Designer', 'Creates visual designs'),
('QA Engineer', 'Tests and ensures quality assurance');

-- Chèn phòng ban mặc định
INSERT INTO Departments (Name, Code, Description) VALUES 
('Information Technology', 'IT', 'Information Technology Department'),
('Human Resources', 'HR', 'Human Resources Department'),
('Finance', 'FIN', 'Finance Department'),
('Marketing', 'MKT', 'Marketing Department'),
('Operations', 'OPS', 'Operations Department');

-- Chèn người dùng admin mặc định
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, EmployeeCode, Phone, Address, DateOfBirth, HireDate, Status, Salary, Notes) VALUES 
('admin', 'admin@company.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Admin', 'System', 'EMP001', '0123456789', 'Headquarters', '1980-01-01', NOW(), 'active', 10000000.00, 'System Administrator');

-- Gán vai trò admin cho người dùng admin
INSERT INTO UserRoles (UserId, RoleId) 
SELECT u.Id, r.Id 
FROM Users u, Roles r 
WHERE u.Username = 'admin' AND r.Name = 'admin';

-- Chèn dự án mẫu
INSERT INTO Projects (Name, Code, Description, StartDate, EndDate, Status, Budget) VALUES 
('Website Redesign', 'WEB001', 'Redesign company website', CURDATE(), DATE_ADD(CURDATE(), INTERVAL 3 MONTH), 'active', 50000000.00),
('Mobile App Development', 'MOB001', 'Develop mobile application', CURDATE(), DATE_ADD(CURDATE(), INTERVAL 6 MONTH), 'active', 100000000.00);

-- Chèn bản ghi chấm công mẫu
INSERT INTO Attendances (EmployeeId, Date, CheckIn, CheckOut, Status) VALUES 
(1, CURDATE(), '08:30:00', '17:30:00', 'on_time'),
(1, DATE_SUB(CURDATE(), INTERVAL 1 DAY), '08:45:00', '17:15:00', 'late');

-- Tạo indexes để tối ưu hiệu suất
CREATE INDEX idx_attendances_employee_date ON Attendances(EmployeeId, Date);
CREATE INDEX idx_attendances_date ON Attendances(Date);
CREATE INDEX idx_users_status ON Users(Status);
CREATE INDEX idx_projects_status ON Projects(Status);
CREATE INDEX idx_leaverequests_status ON LeaveRequests(Status);
CREATE INDEX idx_worklogs_date ON Worklogs(Date);

-- Hiển thị thông tin cơ sở dữ liệu
SELECT 'Database HRManagementSystem created successfully!' AS Message;
SELECT COUNT(*) AS Total_Tables FROM information_schema.tables WHERE table_schema = 'HRManagementSystem';