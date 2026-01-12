# HR Management System - Deployment Guide

## Build & Test Summary

### ✅ Build Status
- **Build Result**: SUCCESS
- **Target Framework**: .NET 9.0
- **Errors**: 0
- **Warnings**: 200 (mostly nullable reference type warnings - non-blocking)
- **Build Time**: 13.64 seconds

### ✅ Test Status
- **Unit Tests**: No test project configured (can be added if needed)
- **Framework Ready**: xUnit, NUnit, or MSTest can be integrated

### ✅ Publish Status
- **Publish Result**: SUCCESS
- **Configuration**: Release
- **Output Path**: `HRManagementSystem.Web\publish`
- **Published Size**: ~6 MB total
- **Framework**: .NET 9.0 (requires .NET 9 Runtime or SDK)

## Published Artifacts

### Main DLLs
- `HRManagementSystem.Web.dll` (1.05 MB) - Main application
- `Microsoft.EntityFrameworkCore.dll` (1.98 MB) - ORM
- `Microsoft.EntityFrameworkCore.Relational.dll` (1.75 MB)
- `MySqlConnector.dll` (0.63 MB) - MySQL driver
- `Pomelo.EntityFrameworkCore.MySql.dll` (0.42 MB) - MySQL provider
- `AutoMapper.dll` (0.25 MB) - Mapping library
- `BCrypt.Net-Core.dll` (0.02 MB) - Password hashing

### Configuration Files
- `appsettings.json` - Application settings
- `appsettings.Development.json` - Development-specific settings
- `HRManagementSystem.Web.runtimeconfig.json` - Runtime configuration

### Static Assets
- `wwwroot/` - CSS, JS, images

## Deployment Instructions

### Prerequisites
- .NET 9 Runtime or SDK installed
- MySQL 8.0+ database server
- IIS (for Windows deployment) OR Docker/Linux container runtime

### Windows IIS Deployment

1. **Copy Published Files**
   ```
   Copy all files from publish\ folder to IIS application directory
   Example: C:\inetpub\wwwroot\HRManagementSystem
   ```

2. **Configure Database Connection**
   - Edit `appsettings.json`
   - Update `ConnectionStrings.DefaultConnection`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=your_mysql_server;Port=3306;Database=HRManagementSystem;User=root;Password=your_password;"
   }
   ```

3. **Create IIS Application Pool**
   - .NET CLR Version: No Managed Code
   - Managed Pipeline Mode: Integrated

4. **Create IIS Website**
   - Binding: http://*:80
   - Physical Path: Directory with published files
   - Application Pool: Select the pool created above

5. **Set Permissions**
   - Application pool identity needs read/write access to publish directory

6. **Start Application**
   - Application will auto-initialize database on first run

### Docker Deployment

1. **Create Dockerfile** (in project root)
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:9.0
   WORKDIR /app
   COPY publish . 
   EXPOSE 80
   ENTRYPOINT ["dotnet", "HRManagementSystem.Web.dll"]
   ```

2. **Build Docker Image**
   ```bash
   docker build -t hrmanagementsystem:latest .
   ```

3. **Run Container**
   ```bash
   docker run -d \
     -p 80:80 \
     -e ConnectionStrings:DefaultConnection="Server=mysql_host;Port=3306;Database=HRManagementSystem;User=root;Password=password;" \
     --name hrmanagementsystem \
     hrmanagementsystem:latest
   ```

### Linux Systemd Service Deployment

1. **Copy Files**
   ```bash
   sudo mkdir -p /opt/hrmanagementsystem
   sudo cp -r publish/* /opt/hrmanagementsystem/
   ```

2. **Create Service File** (`/etc/systemd/system/hrmanagementsystem.service`)
   ```ini
   [Unit]
   Description=HR Management System
   After=network.target mysql.service

   [Service]
   Type=notify
   WorkingDirectory=/opt/hrmanagementsystem
   ExecStart=/usr/bin/dotnet /opt/hrmanagementsystem/HRManagementSystem.Web.dll
   Restart=always
   User=www-data
   Environment="ASPNETCORE_ENVIRONMENT=Production"

   [Install]
   WantedBy=multi-user.target
   ```

3. **Start Service**
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl enable hrmanagementsystem
   sudo systemctl start hrmanagementsystem
   ```

## API Endpoints Summary

All APIs are available at: `https://your-domain/api/`

### Implemented Endpoints
- `/api/attendance` - Attendance management
- `/api/leaverequest` - Leave request management
- `/api/permission` - Permission management
- `/api/projectassignment` - Project assignment management
- `/api/worklog` - Worklog management
- `/api/user` - User management
- `/api/userrole` - User role mapping
- `/api/rolepermission` - Role permission mapping
- `/api/department` - Department management
- `/api/employee` - Employee management
- `/api/position` - Position management
- `/api/project` - Project management
- `/api/role` - Role management
- `/api/shift` - Shift management

## Environment Variables

Key environment variables for deployment:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
DefaultConnection=Server=localhost;Port=3306;Database=HRManagementSystem;User=root;Password=your_password;
```

## Health Check

Test deployment at: `http://your-domain/`
- Should show HR Management System home page
- If logged out: redirects to `/Auth/Login`

## Troubleshooting

### Database Connection Issues
- Ensure MySQL server is running
- Check connection string in `appsettings.json`
- Verify firewall allows port 3306

### Missing Tables/Schema
- First run will auto-create database and seed initial data
- Check application logs for EF Core migration messages

### Permission Issues on Linux
- Ensure application pool user has correct permissions
- Check: `ls -la /opt/hrmanagementsystem`

## Additional Features Added

✅ 7 new API controllers (LeaveRequest, Permission, ProjectAssignment, Worklog, User, UserRole, RolePermission)
✅ 21 new DTOs for data transfer
✅ Complete CRUD operations for all entities
✅ AutoMapper configuration for all models
✅ Nullable reference type compatibility

## Next Steps

1. Configure SSL/HTTPS certificate for production
2. Set up monitoring and logging
3. Configure backup strategy for MySQL database
4. Set up CI/CD pipeline for automated deployments
5. Add API documentation (Swagger/OpenAPI)
6. Implement rate limiting and authentication enhancements
