# Restaurant-Nyx
A restaurant management system using ASP.NET Core MVC with 3 layered architecture.
# Restaurant Management System

A comprehensive restaurant management system built with ASP.NET Core MVC, following a layered architecture for clean code organization, maintainability, and scalability.

## Features

- **Category Management:**  
  Add, edit, delete, and view food categories available in the restaurant.

- **Menu Item Management:**  
  Full control over menu items, with the ability to associate each item with its category.

- **Order Management:**  
  Create orders, add items to orders, update order status, and view detailed order information.

- **Clear Layered Separation:**
  - **DAL (Data Access Layer):**  
    Handles all database operations using Entity Framework Core.
  - **BL (Business Logic Layer):**  
    Contains business logic and services that coordinate operations between layers.
  - **PL (Presentation Layer):**  
    The user interface built with MVC, responsible for displaying data and interacting with users.

- **ViewModels Usage:**  
  Separates presentation logic from business logic, making data handling in views easier and safer.

- **Custom Middleware:**  
  For special functionalities such as automatically resetting menu item availability.

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (or any supported database)
- Bootstrap (for UI design)
- LINQ

## Getting Started

1. Make sure you have .NET 8.0 SDK installed on your machine.
2. Set up the database or run migrations if needed.
3. Run the project using Visual Studio or with the command:
   ```
  
   ```
4. Open your browser at the URL shown in the console.

## Contributions

Contributions are welcome!  
If you have suggestions or want to contribute, feel free to open an Issue or Pull Request.

---
