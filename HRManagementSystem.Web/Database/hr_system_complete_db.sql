-- HR Management System Complete Database Schema
-- Database: hrmanagementsystem
-- Password: 12345

DROP DATABASE IF EXISTS hrmanagementsystem;
CREATE DATABASE hrmanagementsystem CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE hrmanagementsystem;

-- Create Users table
CREATE TABLE Users (
    Id INT NOT NULL AUTO_INCREMENT,
    Username VARCHAR(80) NOT NULL UNIQUE,
    Email VARCHAR(120) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    EmployeeCode VARCHAR(20) NOT NULL UNIQUE,
    Phone VARCHAR(4000),
    HireDate DATETIME,
    Notes VARCHAR(4000),
    Status VARCHAR(20) DEFAULT 'active',
    DepartmentId INT,
    PositionId INT,
    ManagerId INT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PasswordChangedAt DATETIME,
    MustChangePassword BOOLEAN DEFAULT FALSE,
    PRIMARY KEY (Id),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id) ON DELETE SET NULL,
    FOREIGN KEY (PositionId) REFERENCES Positions(Id) ON DELETE SET NULL,
    FOREIGN KEY (ManagerId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Create Departments table
CREATE TABLE Departments (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Description VARCHAR(4000),
    ParentId INT,
    ManagerId INT,
    Status VARCHAR(20) DEFAULT 'active',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    FOREIGN KEY (ParentId) REFERENCES Departments(Id) ON DELETE SET NULL,
    FOREIGN KEY (ManagerId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Create Positions table
CREATE TABLE Positions (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(4000),
    Status VARCHAR(20) DEFAULT 'active',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- Create Projects table
CREATE TABLE Projects (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Code VARCHAR(20) NOT NULL,
    Description VARCHAR(4000),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME,
    Status VARCHAR(20) DEFAULT 'active',
    ProjectManagerId INT,
    Budget DECIMAL(10,2),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    FOREIGN KEY (ProjectManagerId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Create ProjectAssignments table
CREATE TABLE ProjectAssignments (
    Id INT NOT NULL AUTO_INCREMENT,
    ProjectId INT NOT NULL,
    EmployeeId INT NOT NULL,
    Role VARCHAR(100) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME,
    Status VARCHAR(20) DEFAULT 'active',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create Roles table
CREATE TABLE Roles (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(4000),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- Create Permissions table
CREATE TABLE Permissions (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(4000),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- Create RolePermission table (many-to-many relationship)
CREATE TABLE RolePermission (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE
);

-- Create UserRoles table (many-to-many relationship)
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

-- Create Attendances table
CREATE TABLE Attendances (
    Id INT NOT NULL AUTO_INCREMENT,
    EmployeeId INT NOT NULL,
    Date DATE NOT NULL,
    CheckIn TIME,
    CheckOut TIME,
    Status VARCHAR(20) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create Shifts table
CREATE TABLE Shifts (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(4000),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- Create LeaveRequests table
CREATE TABLE LeaveRequests (
    Id INT NOT NULL AUTO_INCREMENT,
    EmployeeId INT NOT NULL,
    LeaveType VARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Reason VARCHAR(4000),
    Status VARCHAR(20) DEFAULT 'pending',
    ApprovedBy INT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ApprovedBy) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Create Worklogs table
CREATE TABLE Worklogs (
    Id INT NOT NULL AUTO_INCREMENT,
    EmployeeId INT NOT NULL,
    ProjectId INT NOT NULL,
    Date DATE NOT NULL,
    Hours DOUBLE NOT NULL,
    Description VARCHAR(4000) NOT NULL,
    Status VARCHAR(20) DEFAULT 'submitted',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);

-- Insert default departments
INSERT INTO Departments (Id, Name, Code, Description, Status, CreatedAt, UpdatedAt) VALUES
(1, 'Phòng Nhân Sự', 'HR', 'Phòng Nhân Sự', 'active', NOW(), NOW()),
(2, 'Phòng Công Nghệ Thông Tin', 'IT', 'Phòng Công Nghệ Thông Tin', 'active', NOW(), NOW()),
(3, 'Phòng Tài Chính', 'FIN', 'Phòng Tài Chính', 'active', NOW(), NOW()),
(4, 'Phòng Marketing', 'MKT', 'Phòng Marketing', 'active', NOW(), NOW()),
(5, 'Phòng Kinh Doanh', 'SALES', 'Phòng Kinh Doanh', 'active', NOW(), NOW());

-- Insert default positions
INSERT INTO Positions (Id, Name, Description, Status, CreatedAt, UpdatedAt) VALUES
(1, 'Giám đốc điều hành', 'Giám đốc điều hành', 'active', NOW(), NOW()),
(2, 'Giám đốc công nghệ', 'Giám đốc công nghệ', 'active', NOW(), NOW()),
(3, 'Quản lý nhân sự', 'Quản lý nhân sự', 'active', NOW(), NOW()),
(4, 'Trưởng nhóm/Supervisor', 'Trưởng nhóm/Supervisor', 'active', NOW(), NOW()),
(5, 'Lập trình viên cao cấp', 'Lập trình viên cao cấp', 'active', NOW(), NOW()),
(6, 'Lập trình viên', 'Lập trình viên', 'active', NOW(), NOW()),
(7, 'Lập trình viên thực tập', 'Lập trình viên thực tập', 'active', NOW(), NOW()),
(8, 'Chuyên viên nhân sự', 'Chuyên viên nhân sự', 'active', NOW(), NOW()),
(9, 'Kế toán', 'Kế toán', 'active', NOW(), NOW()),
(10, 'Chuyên viên marketing', 'Chuyên viên marketing', 'active', NOW(), NOW());

-- Insert default shifts
INSERT INTO Shifts (Id, Name, Description, StartTime, EndTime, CreatedAt, UpdatedAt) VALUES
(1, 'Ca Sáng', 'Ca làm việc buổi sáng', '08:00:00', '12:00:00', NOW(), NOW()),
(2, 'Ca Chiều', 'Ca làm việc buổi chiều', '13:00:00', '17:00:00', NOW(), NOW()),
(3, 'Ca Tối', 'Ca làm việc buổi tối', '18:00:00', '22:00:00', NOW(), NOW()),
(4, 'Ca Đêm', 'Ca làm việc ban đêm', '22:00:00', '06:00:00', NOW(), NOW());

-- Insert default projects
INSERT INTO Projects (Id, Name, Code, Description, StartDate, EndDate, Status, Budget, CreatedAt, UpdatedAt) VALUES
(1, 'Dự Án Website', 'WEB001', 'Dự án phát triển website cho khách hàng', '2026-01-01', '2026-06-30', 'active', 50000.00, NOW(), NOW()),
(2, 'Dự Án Mobile App', 'MOB001', 'Dự án phát triển ứng dụng di động', '2026-02-01', '2026-08-31', 'active', 75000.00, NOW(), NOW()),
(3, 'Dự Án ERP', 'ERP001', 'Dự án hệ thống quản lý doanh nghiệp', '2026-03-01', '2026-12-31', 'active', 120000.00, NOW(), NOW());

-- Insert default roles
INSERT INTO Roles (Id, Name, Description, CreatedAt) VALUES
(1, 'admin', 'Quản trị viên với quyền truy cập đầy đủ hệ thống', NOW()),
(2, 'hr', 'Nhân sự với quyền hạn nhân sự cụ thể', NOW()),
(3, 'employee', 'Nhân viên thông thường với quyền truy cập hạn chế', NOW()),
(4, 'pm', 'Quản lý dự án với quyền hạn dự án cụ thể', NOW());

-- Insert default permissions
INSERT INTO Permissions (Id, Name, Description, CreatedAt) VALUES
(1, 'can_view_employees', 'Có thể xem thông tin nhân viên', NOW()),
(2, 'can_edit_employees', 'Có thể chỉnh sửa thông tin nhân viên', NOW()),
(3, 'can_delete_employees', 'Có thể xóa nhân viên', NOW()),
(4, 'can_view_projects', 'Có thể xem thông tin dự án', NOW()),
(5, 'can_edit_projects', 'Có thể chỉnh sửa thông tin dự án', NOW()),
(6, 'can_view_reports', 'Có thể xem báo cáo', NOW()),
(7, 'can_manage_roles', 'Có thể quản lý vai trò và quyền hạn', NOW());

-- Insert sample employees/users
INSERT INTO Users (Id, Username, Email, PasswordHash, FirstName, LastName, EmployeeCode, Phone, HireDate, Status, DepartmentId, PositionId, CreatedAt, UpdatedAt, MustChangePassword) VALUES
(1, 'admin', 'admin@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Admin', 'System', 'EMP001', '0123456789', '2025-01-01', 'active', 1, 1, NOW(), NOW(), FALSE),
(2, 'john.smith', 'john@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'John', 'Smith', 'EMP002', '0987654321', '2025-02-01', 'active', 2, 5, NOW(), NOW(), FALSE),
(3, 'jane.doe', 'jane@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Jane', 'Doe', 'EMP003', '0912345678', '2025-03-01', 'active', 3, 9, NOW(), NOW(), FALSE),
(4, 'mike.johnson', 'mike@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Mike', 'Johnson', 'EMP004', '0923456789', '2025-04-01', 'active', 2, 6, NOW(), NOW(), FALSE),
(5, 'sarah.williams', 'sarah@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Sarah', 'Williams', 'EMP005', '0934567890', '2025-05-01', 'active', 4, 10, NOW(), NOW(), FALSE),
(6, 'david.brown', 'david@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'David', 'Brown', 'EMP006', '0945678901', '2025-01-15', 'active', 2, 5, NOW(), NOW(), FALSE),
(7, 'lisa.jones', 'lisa@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Lisa', 'Jones', 'EMP007', '0956789012', '2025-02-15', 'active', 1, 8, NOW(), NOW(), FALSE),
(8, 'tom.wilson', 'tom@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Tom', 'Wilson', 'EMP008', '0967890123', '2025-03-15', 'active', 5, 4, NOW(), NOW(), FALSE),
(9, 'amy.taylor', 'amy@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Amy', 'Taylor', 'EMP009', '0978901234', '2025-04-15', 'active', 2, 6, NOW(), NOW(), FALSE),
(10, 'chris.martinez', 'chris@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Chris', 'Martinez', 'EMP010', '0989012345', '2025-05-15', 'active', 3, 9, NOW(), NOW(), FALSE),
(11, 'jessica.davis', 'jessica@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Jessica', 'Davis', 'EMP011', '0990123456', '2025-06-01', 'active', 4, 10, NOW(), NOW(), FALSE),
(12, 'kevin.garcia', 'kevin@company.com', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Kevin', 'Garcia', 'EMP012', '0901234567', '2025-06-15', 'active', 2, 7, NOW(), NOW(), FALSE);

-- Assign roles to users
INSERT INTO UserRoles (UserId, RoleId) VALUES
(1, 1), -- admin user gets admin role
(2, 3), -- john gets employee role
(3, 3), -- jane gets employee role
(4, 3), -- mike gets employee role
(5, 3); -- sarah gets employee role

-- Assign project assignments
INSERT INTO ProjectAssignments (ProjectId, EmployeeId, Role, StartDate, EndDate, Status, CreatedAt, UpdatedAt) VALUES
(1, 2, 'Developer', '2026-01-01', '2026-06-30', 'active', NOW(), NOW()),
(1, 4, 'Tester', '2026-01-01', '2026-06-30', 'active', NOW(), NOW()),
(2, 2, 'Lead Developer', '2026-02-01', '2026-08-31', 'active', NOW(), NOW()),
(2, 5, 'Designer', '2026-02-01', '2026-08-31', 'active', NOW(), NOW());

-- Insert sample attendances
INSERT INTO Attendances (EmployeeId, Date, CheckIn, CheckOut, Status, CreatedAt, UpdatedAt) VALUES
(2, '2026-01-10', '08:15:00', '17:30:00', 'late', NOW(), NOW()),
(2, '2026-01-11', '08:00:00', '17:00:00', 'on_time', NOW(), NOW()),
(3, '2026-01-10', '08:05:00', '16:55:00', 'on_time', NOW(), NOW()),
(3, '2026-01-11', '08:00:00', '17:00:00', 'on_time', NOW(), NOW()),
(4, '2026-01-10', '07:55:00', '17:15:00', 'on_time', NOW(), NOW()),
(4, '2026-01-11', '08:20:00', '17:00:00', 'late', NOW(), NOW());

-- Insert sample worklogs
INSERT INTO Worklogs (EmployeeId, ProjectId, Date, Hours, Description, Status, CreatedAt, UpdatedAt) VALUES
(2, 1, '2026-01-10', 8.0, 'Hoàn thành module đăng nhập', 'approved', NOW(), NOW()),
(2, 1, '2026-01-11', 7.5, 'Sửa lỗi bảo mật', 'submitted', NOW(), NOW()),
(4, 1, '2026-01-10', 8.0, 'Test module đăng nhập', 'approved', NOW(), NOW()),
(4, 2, '2026-01-11', 6.0, 'Thiết kế giao diện chính', 'submitted', NOW(), NOW());

-- Insert sample leave requests
INSERT INTO LeaveRequests (EmployeeId, LeaveType, StartDate, EndDate, Reason, Status, CreatedAt, UpdatedAt) VALUES
(2, 'annual_leave', '2026-02-15', '2026-02-20', 'Nghỉ Tết dương lịch', 'approved', NOW(), NOW()),
(3, 'sick_leave', '2026-01-12', '2026-01-12', 'Ốm đau', 'pending', NOW(), NOW());

-- Create indexes for better performance
CREATE INDEX idx_users_department ON Users(DepartmentId);
CREATE INDEX idx_users_position ON Users(PositionId);
CREATE INDEX idx_users_manager ON Users(ManagerId);
CREATE INDEX idx_attendances_employee ON Attendances(EmployeeId);
CREATE INDEX idx_attendances_date ON Attendances(Date);
CREATE INDEX idx_projects_manager ON Projects(ProjectManagerId);
CREATE INDEX idx_project_assignments_project ON ProjectAssignments(ProjectId);
CREATE INDEX idx_project_assignments_employee ON ProjectAssignments(EmployeeId);
CREATE INDEX idx_leaverequests_employee ON LeaveRequests(EmployeeId);
CREATE INDEX idx_worklogs_employee ON Worklogs(EmployeeId);
CREATE INDEX idx_worklogs_project ON Worklogs(ProjectId);
CREATE INDEX idx_departments_manager ON Departments(ManagerId);
CREATE INDEX idx_departments_parent ON Departments(ParentId);

-- Grant permissions to application user
-- Note: In production, create a dedicated user with limited permissions
-- GRANT SELECT, INSERT, UPDATE, DELETE ON hrmanagementsystem.* TO 'hrapp'@'%' IDENTIFIED BY '12345';
-- FLUSH PRIVILEGES;

COMMIT;