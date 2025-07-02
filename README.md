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

## 📸 Screenshots (Click to Expand)

<details>
  <summary>🏠 Dashboard</summary>

  ![Dashboard](./assets/screenshots/1.png)  
  *Main dashboard showing key metrics and navigation tiles.*

</details>

<details>
  <summary>👤 Profile & 👥 User Management</summary>

  ![Profile](./assets/screenshots/3.png)  
  *User profile with basic details and role information.*

  ![Users](./assets/screenshots/2.png)  
  *Super admin view to manage user roles and permissions.*

</details>

<details>
  <summary>🍕 Menu & 🧑‍🍳 Modifier Groups</summary>

  ![Menu](./assets/screenshots/5.png)  
  *List and manage menu items with categories and prices.*

  ![Modifiers](./assets/screenshots/6.png)  
  *Create modifier groups like toppings, sizes, etc.*

  ![Modifiers](./assets/screenshots/16.png)  
  ![Modifiers](./assets/screenshots/17.png)  
  ![Modifiers](./assets/screenshots/18.png)  
  *Preview of multiple modifiers in different states.*

</details>

<details>
  <summary>🍽️ Table & Section Management</summary>

  ![Tables](./assets/screenshots/7.png)  
  *Organize tables by section and assign statuses (Available, Occupied).*

</details>

<details>
  <summary>💰 Tax & Fees Management</summary>

  ![Taxes](./assets/screenshots/8.png)  
  *Configure tax types and default fee structures for items.*

</details>

<details>
  <summary>🧾 Order Screen</summary>

  ![Order](./assets/screenshots/4.png)  
  *Order summary screen showing item details and customer selection.*

</details>

<details>
  <summary>📲 KOT Flow & Order App</summary>

  ![Order Table](./assets/screenshots/9.png)  
  ![Order Table](./assets/screenshots/10.png)  
  *View and assign tables to customers.*

  ![Waiting Token](./assets/screenshots/12.png)  
  *Display waiting queue with token numbers.*

  ![Order Menu](./assets/screenshots/14.png)  
  ![Order Menu](./assets/screenshots/15.png)  
  *Menu selection with modifiers inside order screen.*

  ![KOT](./assets/screenshots/13.png)  
  *Kitchen Order Ticket view by chefs.*

</details>



---

## ⚙️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/PizzaShop.git
cd PizzaShop
