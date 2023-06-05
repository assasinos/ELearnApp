# E-LearnApp

![GitHub License](https://img.shields.io/badge/license-MIT-blue.svg)

The E-LearnApp is a web application built with ASP.NET that provides an interactive platform for online learning.

## Features

- **Course Catalog**: Browse and enroll in a variety of courses across different subjects.
- **Interactive Lessons**: Access engaging lessons with multimedia content, including videos, slides, and interactive exercises.

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- ASP.NET Core
- Dapper ORM
- MariaDB (version 10.5.0 or higher) or another compatible database

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/assasinos/ELearnApp.git
   ```

2. Change into the project directory:

   ```bash
   cd ELearnApp
   ```

3. Restore the dependencies:

   ```bash
   dotnet restore
   ```

4. Configure the database connection string in the `appsettings.json` file. Edit the `ConnectionStrings.Default` value with your database credentials:

   ```json
   "ConnectionStrings": {
     "Default": "Server=ServerIP;User ID=USER;Password=PASSWORD;Database=DATABASE NAME"
   }
   ```

5. Apply the database migrations and set up the database tables. Execute the SQL script provided in the `/database/import.sql` file against your MariaDB database to create the required tables.

   ```bash
   mysql -u username -p database_name < /path/to/database/import.sql
   ```

   **Note:** Make sure you have MariaDB version 10.5.0 or higher, as the script may rely on specific features introduced in that version.

6. Build the application:

   ```bash
   dotnet build
   ```

## Usage

1. Launch the application:

   ```bash
   dotnet run
   ```

2. Access the application through your web browser:

   ```
   http://localhost:5000
   ```

3. SQL script creates default admin user with login "admin" and password "admin"

4. Explore the course catalog and enroll in the desired courses.


## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.

## Acknowledgements

- Dapper ORM: [https://github.com/DapperLib/Dapper](https://github.com/DapperLib/Dapper)
- MariaDB: [https://mariadb.org/](https://mariadb.org/)
