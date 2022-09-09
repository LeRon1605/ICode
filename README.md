# ICode API
> Ron Le tập tành viết API

API documentation: [here](https://documenter.getpostman.com/view/15687929/VUxLy9Jm).
## Usage
Clone this repository by running following command
```
git clone https://github.com/LeRon1605/ICode.git
```
The project is configured to run by Docker Compose. The services are listed in the docker-compose.yml file. You can launch this project by the following command.
```
docker-compose up
```
Then visit http://localhost:5001/swagger/index.html for Swagger document for the web API project.

**NOTE**: You can configure your database connection string by enviroment variables at docker-compose.yml file.

## Tech Stack
1. Platforms/Framework: ASP.NET Core, Entity Framework, Docker.
2. Database: SQL Server.
3. Cache Server: Redis.
4. Tools: Postman, LINQPad, Swagger.
