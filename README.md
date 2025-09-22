# NaijaRescue Backend 🚑

NaijaRescue is a **mobile-first emergency response platform** designed to connect victims, responders, and agencies in real-time during emergencies in Nigeria.  
This repository contains the **backend API**, built with **ASP.NET Core (Onion Architecture + DDD principles)**.

---

## 📌 Features
- 🔑 **Authentication & Authorization** (JWT, MFA support)  
- 👤 **User Management** (registration, verification, roles)  
- 🚨 **Incident Reporting & Tracking**  
- 🎥 **Live Streaming & Media Uploads**  
- 💬 **Real-time Communication** (SignalR)  
- 📍 **Geolocation Support** (Google Maps API integration)  
- 📡 **Agency & Responder Dashboards**  
- 📑 **Event-driven Design** with Domain Events  

---

## 🏗️ Tech Stack
- **Backend Framework**: ASP.NET Core Web API  
- **Architecture**: Onion Architecture + DDD  
- **Database**: MySQL (with EF Core)  
- **Authentication**: JWT + MFA  
- **Real-time**: SignalR  
- **External Services**: Google Drive (storage), Email (Brevo/SendGrid)  

---

## 📂 Project Structure
```bash
NaijaRescueSystem/
│── Application/       # Business logic, DTOs, validators, commands/queries
│── Domain/            # Entities, ValueObjects, Enums, Domain events
│── Infrastructure/    # EF Core, Repositories, External services, Configs
│── Host/              # API layer (controllers, startup, program)
│── NaijaRescueSystem.sln
