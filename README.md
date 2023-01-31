# ZooApplication

The first part of our Zoo Application. This features the use of Code-First Migrations to create our database, and WebAPI and LINQ to perform CRUD operations.

Videos which include this codebase:
- [Building With ASP.NET - Project Setup](https://youtu.be/NSpYP1YW9p0)
- [Building with ASP.NET - Object Relational Mapping](https://youtu.be/V1emgCxxRtI)
- [Building with ASP.NET - Base CRUD](https://youtu.be/uEgWxIZmX48)
- [Building with ASP.NET - HTTP Client & Views](https://youtu.be/dFIaeluKcAA)
- [Building with ASP.NET - ViewModels and Relationships](https://www.youtube.com/watch?v=mqXVCNdV_DQ)

# Scope
- Manage Animal, Species information
- Manage Trivia for Species
- Manage Keepers and associations between Keepers and animals
- Manage Tickets and Bookings

# Running this project 
- Project > ZooApplication Properties > Change target framework to 4.7.1 -> Change back to 4.7.2
- Make sure there is an App_Data folder in the project (Right click solution > View in File Explorer)
- Tools > Nuget Package Manager > Package Manage Console > Update-Database
- Check that the database is created using (View > SQL Server Object Explorer > MSSQLLocalDb > ..)
- Run API commands through CURL to create new animals


# Common Issues and Resolutions
- (update-database) Could not attach .mdf database SOLUTION: Make sure App_Data folder is created
- (update-database) Error. 'Type' cannot be null SOLUTION: (issue appears in Visual Studio 2022) Tools > Nuget Package Manager > Manage Nuget Packages for Solution > Install Latest Entity Framework version (eg. 6.4.4), restart visual studio and try again
- (update-database) System Exception: Exception has been thrown by the target of an invocation POSSIBLE SOLUTION: Project was cloned to a OneDrive or other restricted cloud-based storage. Clone the project repository to the actual drive on the machine.
- (running server) Could not find part to the path ../bin/roslyn/csc.exe SOLUTION: change target framework to 4.7.1 and back to 4.7.2
- (running server) Project Failed to build. System.Web.Http does not have reference to serialize... SOLUTION: Solution Explorer > References > Add Reference > System.Web.Extensions

Make sure to utilize jsondata/animal.json to formulate data you wish to send as part of the POST requests. {id} should be replaced with the animal's primary key ID. The port number may not always be the same

Get a List of Animals
curl https://localhost:44324/api/animaldata/listanimals

Get a Single Animal
curl https://localhost:44324/api/animaldata/findanimal/{id}

Add a new Animal (new animal info is in animal.json)
curl -H "Content-Type:application/json" -d @animal.json https://localhost:44324/api/animaldata/addanimal

Delete an Animal
curl -d "" https://localhost:44324/api/animaldata/deleteanimal/{id}

Update an Animal (existing animal info including id must be included in animal.json)
curl -H "Content-Type:application/json" -d @animal.json https://localhost:44324/api/animaldata/updateanimal/{id}

# Admin vs Guest
- Register an account
- View > SQL Server Object Explorer
- Create 'Guest', 'Admin' entries in AspNetRoles
- Copy UserID from AspNetUsers table
- Create entry between Guest Role x User Id, Admin Role x User Id in AspNetUserRoles bridging table
