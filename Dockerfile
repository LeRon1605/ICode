FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY *.sln ./
COPY API/*.csproj ./API/
COPY Data/*.csproj ./Data/
COPY Models/*.csproj ./Models/
COPY Services/*.csproj ./Services/
COPY UnitTest/*.csproj ./UnitTest/

RUN dotnet restore

COPY . .

WORKDIR /src/Data
RUN dotnet build -c Release -o /app
WORKDIR /src/Models
RUN dotnet build -c Release -o /app
WORKDIR /src/Services
RUN dotnet build -c Release -o /app
WORKDIR /src/UnitTest
RUN dotnet publish -c Release -o /app
WORKDIR /src/API
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT [ "dotnet", "API.dll" ]