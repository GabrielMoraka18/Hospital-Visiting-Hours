# Hospital Visiting Hours  
** Public Hospital Visiting Hours Management System**

A **WPF C# desktop application** built to manage and monitor public hospital visitation efficiently — ensuring fair access to patients while adhering to regulations and safety protocols.

>  **This is a work in progress. Features are being refined and expanded continuously.**

---

## Introduction

This project started as a simple idea:  
**Can we improve how Public hospitals manage patient visits using software instead of paper sign-ins?**

Managing who’s in the public hospital wards, who’s waiting, and who already visited can get messy fast. Paper sign-ins and paper lists don't cut it anymore — especially when **patient safety**, **visitor fairness**, and **hospital efficiency** are on the line.

This system is an attempt to bring **structure, automation, and visibility** to that problem.

---

## Problem Statement

Public Hospitals often struggle with:

- ❌ **Overcrowding** during peak visiting hours  
- ❌ **Manual tracking** of visitors checking in/out  
- ❌ **Lack of real-time visibility** for hospital staff  
- ❌ **Unregistered or over-the-limit visits** that impact safety

**This is just a problem I thought could be solved with a simple, structured solution — and I’m building exactly that.**

The goal is to **digitally transform** the visitor management process, starting small and adapting to real-world Public hospital workflows over time.

---

## 🚀 Project Overview

This system was developed to:

- Simplify visitor registration  
- Enforce per-patient visitor limits  
- Separate **waiting** and **inside** queues  
- Monitor **who is visiting who** in real-time  
- Lay the foundation for digital entry/exit tracking (via QR codes)  
- Provide a clear **dashboard** view for admin oversight  

---

## 🧩 Current Features

- ✅ **Visitor Registration Form** with field validation  
- ✅ **QR Code Generator** (for digital check-in/check-out - future expansion)  
- ✅ **Search & Autocomplete** for patient names  
- ✅ **Per-patient visitor limits** (Max 5 total, 2 inside at a time)  
- ✅ **Live status tracking**:
  - Inside  
  - Waiting  
- ✅ **Dashboard/Admin Panel** showing:
  - Total Visitors  
  - Visitors Waiting  
  - Visitors Inside  
  - Active Patient Queues  
- ✅ **Switching Visitors** between Inside & Waiting  
- ✅ **Visitor removal** from queue  

---

## 🛠️ Built With

- **C# (.NET WPF)**
- **XAML** for UI  
- **QRCoder** (NuGet library) for generating QR codes  
- **In-memory data management** (no DB yet)  


---

## Roadmap / Upcoming Features

- [ ] QR Code-based digital entry system (Scan-to-check-in/out)  
- [ ] Time tracking & visit duration monitoring  
- [ ] Overstay alerts for inside visitors  
- [ ] Admin roles / role-based controls  
- [ ] Database support (SQLite / SQL Server)  
- [ ] Export reports for hospital records  

---

## 👤 Author

**Gabriel Moraka**  
