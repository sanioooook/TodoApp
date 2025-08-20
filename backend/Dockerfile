# === Build stage ===
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln ./
COPY Todo.Api/*.csproj ./Todo.Api/
COPY Todo.Application/*.csproj ./Todo.Application/
COPY Todo.Application.Tests/*.csproj ./Todo.Application.Tests/
COPY Todo.Domain/*.csproj ./Todo.Domain/
COPY Todo.Infrastructure/*.csproj ./Todo.Infrastructure/
COPY Todo.Infrastructure.Tests/*.csproj ./Todo.Infrastructure.Tests/
RUN dotnet restore

COPY . .

RUN dotnet publish Todo.Api/Todo.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Todo.Api.dll"]
