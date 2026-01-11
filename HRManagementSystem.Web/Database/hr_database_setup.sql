-- HR Management System Database Setup
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
(1, 'Human Resources', 'HR', 'Human Resources Department', 'active', NOW(), NOW()),
(2, 'Information Technology', 'IT', 'Information Technology Department', 'active', NOW(), NOW()),
(3, 'Finance', 'FIN', 'Finance Department', 'active', NOW(), NOW()),
(4, 'Marketing', 'MKT', 'Marketing Department', 'active', NOW(), NOW()),
(5, 'Sales', 'SALES', 'Sales Department', 'active', NOW(), NOW());

-- Insert default positions
INSERT INTO Positions (Id, Name, Description, Status, CreatedAt, UpdatedAt) VALUES
(1, 'CEO', 'Giám đốc điều hành', 'active', NOW(), NOW()),
(2, 'CTO', 'Giám đốc công nghệ', 'active', NOW(), NOW()),
(3, 'HR Manager', 'Quản lý nhân sự', 'active', NOW(), NOW()),
(4, 'Team Lead', 'Trưởng nhóm/Supervisor', 'active', NOW(), NOW()),
(5, 'Senior Developer', 'Lập trình viên cao cấp', 'active', NOW(), NOW()),
(6, 'Developer', 'Lập trình viên', 'active', NOW(), NOW()),
(7, 'Junior Developer', 'Lập trình viên thực tập', 'active', NOW(), NOW()),
(8, 'HR Specialist', 'Chuyên viên nhân sự', 'active', NOW(), NOW()),
(9, 'Accountant', 'Kế toán', 'active', NOW(), NOW()),
(10, 'Marketing Specialist', 'Chuyên viên marketing', 'active', NOW(), NOW());

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

-- Create admin user
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, EmployeeCode, HireDate, Status, CreatedAt, UpdatedAt, MustChangePassword) VALUES
('admin', 'admin@hrsystem.vn', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Admin', 'System', 'EMP001', NOW(), 'active', NOW(), NOW(), FALSE),
('employee', 'employee@hrsystem.vn', '$2a$11$y6Jya5k1Q0jUfsd7vYlW9OzB9z3QqF1r2p0oY5r4v6t8s7u9w0x1y', 'Employee', 'Default', 'EMP002', NOW(), 'active', NOW(), NOW(), FALSE);

-- Assign roles to users
INSERT INTO UserRoles (UserId, RoleId) VALUES
(1, 1), -- admin user gets admin role
(2, 3); -- employee user gets employee role

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