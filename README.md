NaijaRescue Backend 🚑

NaijaRescue is a mobile-first emergency response platform built to connect victims, responders, and agencies in real-time during emergencies in Nigeria.

This repository contains the backend API, designed with ASP.NET Core, following Onion Architecture and DDD (Domain-Driven Design) principles.

📌 Features

🔑 Authentication & Authorization — JWT with Multi-Factor Authentication (MFA)

👤 User Management — registration, verification, role-based access

🚨 Incident Reporting & Tracking — structured reports with geolocation support

🎥 Live Streaming & Media Uploads — evidence and real-time visibility during incidents

💬 Real-time Communication — powered by SignalR for chat & notifications

📍 Location Services — Google Maps API integration

📡 Agency & Responder Dashboards — operational tools for emergency services

📑 Event-Driven Architecture — leveraging domain events

🏗️ Tech Stack

Framework: ASP.NET Core Web API

Architecture: Onion Architecture + DDD

Database: MySQL (via EF Core)

Authentication: JWT + MFA

Real-time: SignalR

External Services:

Google Drive (media storage)

Email providers (Brevo, SendGrid)

📂 Project Structure
NaijaRescueSystem/
│── Application/       # Business logic, DTOs, validators, commands/queries
│── Domain/            # Entities, ValueObjects, Enums, Domain events
│── Infrastructure/    # EF Core, repositories, external services, configs
│── Host/              # API layer (controllers, startup, program)
│── NaijaRescueSystem.sln

📜 License

This project is licensed under the terms of the MIT License.
See the LICENSE
 file for details.
