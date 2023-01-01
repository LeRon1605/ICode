FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY *.sln ./
COPY ICode.CodeExecutor/*.csproj ./ICode.CodeExecutor/

RUN dotnet restore ./ICode.CodeExecutor/*.csproj

COPY ./ICode.CodeExecutor ./ICode.CodeExecutor/

WORKDIR /src/ICode.CodeExecutor
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN apt-get update && \
    apt-get -y install g++
RUN apt install time
# RUN apt install -y default-jdk && \
#     apt install -y default-jre

WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT [ "dotnet", "ICode.CodeExecutor.dll" ]