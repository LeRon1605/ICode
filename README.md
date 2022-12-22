# ICode
> Write an API using ASP.NET Core and consume it by MVC Client.

API documentation: [here](https://documenter.getpostman.com/view/15687929/VUxLy9Jm).
## What I'm doing so far?
![diagrams-13 - containers drawio (1)](https://user-images.githubusercontent.com/78067510/209193592-e89d88a9-edee-4d65-8e50-5edddc1ce645.png)
## Usage
Clone this repository by running following command
```
git clone https://github.com/LeRon1605/ICode.git
```
The project is configured to run by Docker Compose. The services are listed in the docker-compose.yml file. You can launch this project by the following command.
```
docker-compose up
```
Visit http://localhost:3000/api/swagger/index.html for Swagger document for the web API project.

Visit http://localhost:3000 to access web application.

## Tech Stack
1. Platforms/Framework: ASP.NET Core, Entity Framework, Docker.
2. Database: SQL Server.
3. Cache Server: Redis.
4. Background Service: Hangfire.
5. Tools: Postman, LINQPad, Swagger.
6. Web server: Nginx
