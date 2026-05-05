# Student Management System (StudentMS)

A comprehensive Student Management System built with **C#**, **.NET 8**, and **WPF**. This application provides a modern desktop interface for managing students, teachers, departments, and financial records.

## 🚀 Features

- **Authentication & Security**: Secure login with BCrypt password hashing, session management, and account lockout protection.
- **Student Management**: Full CRUD operations for student records.
- **Teacher Management**: Manage faculty information and assignments.
- **Department Management**: Organize the institution by departments.
- **Fees Management**: Track and manage student fee records.
- **User Management**: Admin tools for managing users and password updates.
- **Logging**: Integrated activity logging and error tracking using Serilog.
- **Modern UI**: Clean and responsive interface powered by Material Design in XAML.

## 🛠️ Tech Stack

- **Framework**: .NET 8.0 (WPF)
- **Architecture**: Multi-layered (Business, Infrastructure, Models, Common, UI)
- **Database**: SQL Server (via Microsoft.Data.SqlClient)
- **UI Library**: MaterialDesignThemes
- **MVVM Toolkit**: CommunityToolkit.Mvvm
- **Logging**: Serilog
- **Testing**: xUnit, FsCheck (Property-based testing), NSubstitute

## 📂 Project Structure

- `StudentMS.UI`: WPF Application (Views and ViewModels)
- `StudentMS.Business`: Business logic and service layer
- `StudentMS.Infrastructure`: Data access layer (Direct SQL implementation)
- `StudentMS.Models`: Domain, Request, and Response models
- `StudentMS.Common`: Shared utilities, logging, and session management
- `StudentMS.Tests`: Comprehensive test suite

## ⚙️ Setup & Installation

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB supported)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Steps
1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd StudentMS
   ```

2. **Configure the Database**:
   - Update the connection string in `StudentMS.UI/appsettings.json`:
     ```json
     "ConnectionStrings": {
       "MainDB": "Server=(localdb)\\MSSQLLocalDB;Database=StudentMS;Trusted_Connection=True;..."
     }
     ```
   - Ensure the `StudentMS` database exists and run the required SQL scripts (if available in `Database/` folder).

3. **Restore & Build**:
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run the Application**:
   ```bash
   dotnet run --project StudentMS.UI/StudentMS.UI.csproj
   ```

## 🧪 Running Tests
```bash
dotnet test
```

