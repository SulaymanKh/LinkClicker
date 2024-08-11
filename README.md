# LinkClicker

## Pull Down Source

### Setting up Backend

**Running the Web API Locally**

1. **Open the Solution in Visual Studio:**
   - Open `LinkClicker.sln` in Visual Studio.
   - Select **Build** > **Clean Solution**.

2. **Configure Database Connection:**
   - Open `appsettings.json` (or `appsettings.Development.json` if you are in development mode).
   - Update the `ConnectionStrings` section with your SQL Server connection details.

3. **Apply Entity Framework Migrations:**
   - Open the **Package Manager Console** in Visual Studio (`Tools` > `NuGet Package Manager` > `Package Manager Console`).
   - Run the following command to update the database schema:
     ```bash
     dotnet ef database update --project (project csproj path)
     ```

4. **Run the Web API:**
   - Start the Web API project by pressing **F5** or selecting **Debug** > **Start Debugging**.

### Setting up Front-end

**Running the Angular Application Locally**

1. **Open in Visual Studio Code:**
   - Open the Angular project folder in Visual Studio Code.

2. **Install Dependencies:**
   - Open a terminal in Visual Studio Code.
   - Run the following command to install the necessary packages:
     ```bash
     npm install
     ```

3. **Serve the Angular Application:**
   - In the terminal, run:
     ```bash
     ng serve
     ```
   - Open a web browser and navigate to `http://localhost:4200` to view the application.

**Note:** In `config.service.ts`, ensure that the URLs are correctly configured to avoid errors.
