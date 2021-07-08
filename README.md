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
- Make sure there is an App_Data folder in the project (Right click solution > View in File Explorer)
- Tools > Nuget Package Manager > Package Manage Console > Update-Database
- Check that the database is created using (View > SQL Server Object Explorer > MSSQLLocalDb > ..)
- Run API commands through CURL to create new animals

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


