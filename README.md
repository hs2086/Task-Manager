# ✅ Task Manager Platform

A full-featured **Task Management System** built using **ASP.NET MVC**, designed to help users manage their tasks effectively with automated email notifications and user management features.

---

## 🔧 Key Features

- ✅ **Full CRUD operations** for managing tasks  
- 🔐 **User authentication & authorization** using **ASP.NET Identity**  
- 📧 **Email verification** and **password reset** functionality with **MailKit**  
- 🕒 **Background Services**:
  - Send task reminder emails **1 hour before the deadline**
  - **Automatically unblock users** after 1 month of being blocked
- 🛠️ **Admin Dashboard**:
  - Manage users and their tasks
  - Block/unblock users
  - Auto unblock after block duration expires

---

## 🛠️ Technologies Used

- **ASP.NET MVC**  
- **C#**  
- **Entity Framework Core**  
- **SQL Server**  
- **ASP.NET Identity**  
- **MailKit**  
- **BackgroundService (HostedService)**

---

## 📸 Screenshots

*(Add screenshots of the UI, Admin Dashboard, Email system, etc. here if available)*

---

## 🚀 How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/hs2086/Task-Manager.git
2. Set up your database connection in appsettings.json.
3. Apply migrations:
         Update-Database
4. Run the project using Visual Studio or: (dotnet run) from Visual Studio Code
