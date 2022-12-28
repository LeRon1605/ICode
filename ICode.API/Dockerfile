FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY *.sln ./
COPY ICode.API/*.csproj ./ICode.API/
COPY ICode.Data/*.csproj ./ICode.Data/
COPY ICode.Models/*.csproj ./ICode.Models/

RUN dotnet restore ./ICode.API/*.csproj
RUN dotnet restore ./ICode.Data/*.csproj
RUN dotnet restore ./ICode.Models/*.csproj

COPY ICode.API/. ./ICode.API/
COPY ICode.Data/. ./ICode.Data/
COPY ICode.Models/. ./ICode.Models/

WORKDIR /src/ICode.Data
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.Models
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.API
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT [ "dotnet", "ICode.API.dll" ]