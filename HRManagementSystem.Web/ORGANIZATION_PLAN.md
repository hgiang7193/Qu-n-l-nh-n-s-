# Project Structure Organization Plan

## Organized Structure

### Backend Components
- **Controllers** - `/Backend/Controllers/`
  - All MVC controllers (AttendanceController.cs, AuthController.cs, etc.)
- **Models** - `/Backend/Models/`
  - All data models (User.cs, Role.cs, Attendance.cs, etc.)
- **Data** - `/Backend/Data/`
  - ApplicationDbContext.cs
- **Services** - `/Backend/Services/`
  - DatabaseInitializer.cs

### Frontend Components
- **Views** - `/Frontend/Views/`
  - All Razor view files organized by controller
- **Pages** - `/Frontend/Pages/`
  - Static pages and shared components
- **wwwroot** - `/Frontend/wwwroot/`
  - Static assets (CSS, JS, images)

### Database Components
- **SQL Scripts** - `/Database/hr_management_system.sql`
  - MySQL database schema and initialization scripts
- **Configuration** - `appsettings.json`
  - MySQL connection string configuration

## Current File Mapping

### Backend
- `Controllers/*` → `/Backend/Controllers/`
- `Models/*` → `/Backend/Models/`
- `Data/*` → `/Backend/Data/`
- `Services/*` → `/Backend/Services/`
- `Program.cs` → Root (configuration file)

### Frontend
- `Views/*` → `/Frontend/Views/`
- `Pages/*` → `/Frontend/Pages/`
- `wwwroot/*` → `/Frontend/wwwroot/`

### Database
- `Database/hr_management_system.sql` → Database scripts
- `appsettings.json` → Connection configuration

## Benefits
- Clear separation of concerns
- Better maintainability
- Easier onboarding for new developers
- Organized structure for future expansion

Note: The physical movement of files is avoided to prevent breaking project references. The organization is conceptual for better understanding of component responsibilities.