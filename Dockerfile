FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY PhoneAssignments/ PhoneAssignments/
RUN dotnet publish PhoneAssignments/PhoneAssignments.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
# Render supplies $PORT; fall back to 8080 locally
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet PhoneAssignments.dll"]
