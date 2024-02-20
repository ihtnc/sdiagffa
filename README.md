<img alt="App Preview" src="/preview.gif?raw=true" width="500px" />

Repository for the APP (ReactJS) and API (.NET6) projects of the Six degrees in a galaxy far, far away (sdiagffa) application.

# sdiagffa Application
Finds the shortest connection between two characters in the Star Wars universe. At the moment, connection is limited to the films the characters were in, or their listed homeworld (planet).

Based on the concept, [six degrees of Kevin Bacon](https://en.wikipedia.org/wiki/Six_Degrees_of_Kevin_Bacon). Data provided by the endpoints in [Starwars API](https://swapi.dev/)


## Initialise both projects:
### API
    \sdiagffa> dotnet restore
    \sdiagffa> dotnet build

### APP
    \sdiagffa\sdiagffa.ui> npm install
    \sdiagffa\sdiagffa.ui> npm run build

These commands will retrieve the package dependencies of both projects then build the applications.

## Run tests on both projects:
### API
    \sdiagffa> dotnet test

### APP
    \sdiagffa\sdiagffa.ui> npm run test

With these commands, both the project's test suites will be executed.

## Run the application:
### API
    \sdiagffa\sdiagffa.host> dotnet run --urls="https://localhost:44300"

### APP
    \sdiagffa\sdiagffa.ui> npm run dev

In these commands, each project will run on its own process. To view the application, navigate to the port opened by the `npm run dev` command (default is https://localhost:3000).

NOTE: The APP is hard-coded to call the API at https://localhost:44300.
