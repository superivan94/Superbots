:First we need the compiled project
echo "Compiling the project"
dotnet clean
dotnet build

echo "updating the dotnet tools"
:Update the dotnet tools to last version
dotnet tool update -g dotnet-ef




echo "Creating the databases context"
:Create Entity Framework DatabaseContexts
:This is the context for the application settings
dotnet ef migrations add InitAppDbContext --context AppDbContext --output-dir ./Common/Data

:This is the context for the Chat feature
dotnet ef migrations add InitChatDbContext --context ChatDbContext --output-dir ./Features/Chat/Data




echo "Creating the databases"
:Create the databases
:This is the database for the application settings
dotnet ef database update --context AppDbContext

:This is the database for the Chat feature
dotnet ef database update --context ChatDbContext

echo "Commands reached the end!"