### L.O.D.G.E. - Løsningsbasert Operasjonell Databasert Gjesteopplysing Enterprise

## Description

A backend C# system for hotel booking management built from customer input forms. This application takes booking requests from online forms and transforms them into specific booking models (Standard, Luxury, Group, Corporate) with dedicated service handling for each type.

## Features

- **Booking Form Processing**: Accepts customer booking requests with room type, check-in/check-out dates, special requests
- **Polymorphic Booking Models**: Transforms input into specialized booking types with unique logic
- **Service-Driven Architecture**: Dedicated services handle each booking type independently
- **Repository Pattern**: Central oversikt over all active bookings
- **Controller Delegation**: Routes between services and repositories
- **Clean Interface Contract**: All booking models implement common interface for consistent handling

## How to Use

1. **Build the project**:
   ```bash
   dotnet build
   ```

2. **Run the program**:
   ```bash
   dotnet run --project Console/Console.csproj
   ```

3. **Follow the prompts**:
   - The program will display a message asking you to enter your password
   - Type your password and press Enter
   - The program will validate and provide feedback

## Program Logic

- Accepts customer booking form data
- Parses input into appropriate booking model type
- Validates booking availability and requirements
- Executes booking-specific service logic
- Stores booking in central repository
- Provides feedback on booking confirmation

## Language

This program uses Norwegian language for user interface messages and booking management.