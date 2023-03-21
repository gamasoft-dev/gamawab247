
This solution contains 2 web api services, 3 Background jobs(Worker services) and a docker compose project.

To run migration on the Gamawabs247 api, use the dotnet EF command below
cd github/gamasoft-dev/gamawab247/src/services/Gamawabs247API/
dotnet ef migrations add "migration-action-name" --project "../../Libraries/Infrastructure.Data/"

To run migration for the BillProcessor Api
cd gamasoft-dev/gamawab247/src/services/BillProcessorAPI/
dotnet ef migrations add "migration-action-name" --project "../../Libraries/Infrastructure.Data/"
dotnet ef database update

After the migration addition. Update the database with the command below.
dotnet ef database update