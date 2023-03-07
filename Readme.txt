
This solution contains 2 web api services, 3 Background jobs(Worker services) and a docker compose project.

To run migration on the Gamawabs247 api, use the dotnet EF command below
dotnet ef migrations "migration-action-name" --project "../../Libraries/Infrastructure.Data/

To run migration for the BillProcessor Api 
dotnet ef migrations "migration-action-name"