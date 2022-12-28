FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY *.sln .
COPY ICode.Web/*.csproj ./ICode.Web/
COPY ICode.Models/*.csproj ./ICode.Models/

RUN dotnet restore ./ICode.Models/*.csproj
RUN dotnet restore ./ICode.Web/*.csproj

COPY ICode.Web/. ./ICode.Web/
COPY ICode.Models/. ./ICode.Models/

WORKDIR /src/ICode.Models
RUN dotnet build -c Release -o /app
WORKDIR /src/ICode.Web
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT [ "dotnet", "ICode.Web.dll" ]