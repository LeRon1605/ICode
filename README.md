# ICode
> Write an API using ASP.NET Core and consume it by MVC Client. Deploy at Linode.
## What am I doing so far?
<p align="center">
  <img  src="https://user-images.githubusercontent.com/78067510/210708944-c12bd652-91d1-4c08-b99d-7220a1718bb0.png">
</p>

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
## Live IDE
![screencapture-icode1605-tech-Home-CodeExecutor-2023-01-05-12_18_31](https://user-images.githubusercontent.com/78067510/210709311-fce835b9-0e9a-4093-8f15-a84fe994a2d2.png)
## Tech Stack
1. Platforms/Framework: ASP.NET Core, Entity Framework, Docker.
2. Database: SQL Server.
3. Cache Server: Redis.
4. Background Service: Hangfire.
5. Tools: Postman, LINQPad, Swagger.
6. Web server: Nginx
