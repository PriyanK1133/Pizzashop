# 🍕 PizzaShop - Restaurant Management System

PizzaShop is a role-based restaurant management system built using **ASP.NET MVC** and **PostgreSQL**. It simplifies and digitizes restaurant operations such as menu management, table and section organization, order processing, role-based access control, tax configuration, and modifier handling.

This project supports multiple user roles: **Super Admin**, **Account Manager**, and **Chef** — each with tailored dashboards and permissions.

---

## 🚀 Key Features

- 🔐 **Role-Based Access (RBAC)**  
  Super Admin, Account Manager, and Chef dashboards with permission control for views, edits, and deletions.

- 🍽️ **Menu Management**  
  Manage food categories, items, modifier groups, and individual modifiers.

- 🪑 **Table & Section Management**  
  Add, update, and soft-delete sections and tables with real-time status (Available, Occupied).

- 🧾 **Order Management (KOT)**  
  End-to-end flow from table assignment to order completion, with status tracking.

- 👥 **Customer Management**  
  Create customers during table assignment and view history in the dashboard.

- 💰 **Tax & Fee Configuration**  
  Manage multiple tax types and apply defaults to items.

- 📊 **Dashboard Insights**  
  Tiles and charts for sales, new customers, top/least selling items, and waiting list.

- 📥 **Export to Excel**  
  Download detailed reports of orders and customers with filters and date ranges.

---

## 🛠️ Technology Stack

### Backend
- ![ASP.NET MVC](https://img.shields.io/badge/.NET%20MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
- ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-336791?style=for-the-badge&logo=postgresql&logoColor=white)
- Entity Framework (EF)

### Frontend
- Razor Views + jQuery
- Bootstrap for UI components
- HTML, CSS, JavaScript

---

## 📸 Screenshots

> Ensure screenshots are stored in `assets/screenshots/`

### 🏠 Super Admin Dashboard  
![Dashboard](./assets/screenshots/dashboard.png)

### 🍽️ Table & Section Management  
![Tables](./assets/screenshots/tables.png)

### 🧾 Order Flow (KOT)  
![Order](./assets/screenshots/order-flow.png)

### 🍕 Menu Management  
![Menu](./assets/screenshots/menu.png)

### 🧑‍🍳 Modifier Groups  
![Modifiers](./assets/screenshots/modifiers.png)

---

## ⚙️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/PizzaShop.git
cd PizzaShop
