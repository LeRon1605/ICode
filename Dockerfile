FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY *.sln ./
COPY ICode.API/*.csproj ./ICode.API/
COPY ICode.Data/*.csproj ./ICode.Data/
COPY ICode.Models/*.csproj ./ICode.Models/
COPY ICode.Services/*.csproj ./ICode.Services/
COPY ICode.UnitTest/*.csproj ./ICode.UnitTest/
COPY ICode.Mapper/*.csproj ./ICode.Mapper/
COPY ICode.Common/*.csproj ./ICode.Common/
COPY ICode.Web/*.csproj ./ICode.Web/

RUN dotnet restore

COPY . .

WORKDIR /src/ICode.Common
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.Web
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.Mapper
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.Data
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.Models
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.Services
RUN dotnet build -c Release -o /app
# WORKDIR /src/ICode.UnitTest
# RUN dotnet publish -c Release -o /app
WORKDIR /src/ICode.API
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT [ "dotnet", "ICode.API.dll" ]