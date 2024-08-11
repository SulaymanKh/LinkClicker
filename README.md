---LinkClicker Project---

**Running the Project Locally**
Web API

Open the Solution:

Open LinkClicker.sln in Visual Studio.
Clean and Build:

In Visual Studio, select Build > Clean Solution.
Then, select Build > Rebuild Solution.
Configure Database Connection:

Open appsettings.json (or appsettings.Development.json if you are in development mode).
Update the ConnectionStrings section with your SQL Server connection details.
Apply Entity Framework Migrations:

Open the Package Manager Console in Visual Studio (Tools > NuGet Package Manager > Package Manager Console).
Run the following command to update the database schema:
powershell
Update-Database



**Run the Web API:**

Start the Web API project by pressing F5 or selecting Debug > Start Debugging.
Web App (Angular)
Open in Visual Studio Code:

Open the Angular project folder in Visual Studio Code.
Install Dependencies:

Open a terminal in Visual Studio Code.
Run the following command to install the necessary packages:
npm install

**Serve the Angular Application:**

In the terminal, run:
ng serve

Open a web browser and navigate to http://localhost:4200 to view the application.
