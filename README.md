## Database Setup
Hi Patrick 

im using a documented equivalent to `dotnet ef database update` so
SQL artifacts in `migrations/sql` are applied in order when running the shell script at the root of the application

I thought about just generating final schema artifact for you to apply,but i think its better if they are applied one after one.

For the ease of it,all that needs to be done is to run the apply-migrations.sh script.It will start the postgres db and apply all migrations in order from 1 to the last one.

You can run the api with dotnet run

db credentials are in appsettings.json config file
