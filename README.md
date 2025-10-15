# Coder: Jhonatan Morales
## Clan: Van Rossum, C#
## Email: jhonatanmorales99@gmail.com
## c.c: 1000565363


# Hospital San Vicente

This project implements a ** Management System** in C#/.NET.  
It allows you to manage users through CRUD operations (Create, Read, Update, Delete) and provides several filtering, counting, and detailed query options.


---

## 🚀 Installation and Execution

### 1️⃣ Clone the repository

```bash
git clone https://github.com/JMoralesNunez/Prueba_M5.git
```

### 2️⃣ Prerequisites

- Git

- .NET SDK 6.0 or later

- A code editor (Visual Studio, VS Code, Rider)

Check your .NET version:

```bash
dotnet --version
```

### 3️⃣ Build and run the project

```bash
dotnet build
dotnet run
```

# ⚙️ Technologies Used

C# → Main programming language.

.NET 8.0 SDK → Framework for building and running the project.

OOP (Object-Oriented Programming) → Classes, properties, encapsulation, and collections.

Lists (List<T>) → Data structure used for local storage of users.

LINQ → Used for filtering, ordering, and querying users efficiently.

MVC with asp.Net

## 📋 Main Features

### Home Page

1. **Pacients**

    - List pacients
    - Add pacients
    - Edit pacients
    - Check pacients details
        
2. **Doctors**
    - List doctors
    - filter doctors
    - Add doctors
    - Edit doctors
    - Check doctor details

3. **Appointments**
    - List appointments
    - Filter appointments
    - Create appointments
    - Change appointment's state

4. **Additional feature**
    - A confirmation email will be sent to the pacient associated with a created appointment



## 🗂️ Project Structure

```markdown
sprint-2/
│
├── Program.cs
├── Controllers/
│   └── PacientsController.cs
│   └── DoctorsController.cs
│   └── AppointmentsController.cs
├── Data/
│   └── MySqlContext.cs
├── Models/
│   └── Appointment.cs
│   └── Doctor.cs
│   └── EmailHistory.cs
│   └── Pacient.cs
├── Services/
│   └── AppointmentService.cs
│   └── DoctorService.cs
│   └── EmailService.cs
│   └── PacientService.cs
└── Views/
    └── Appointments
    └── Doctors
    └── Pacients
```


👥 Creator

- [Jhonatan Morales](https://github.com/JMoralesNunez)



