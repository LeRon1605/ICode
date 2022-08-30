# ICode API: API quản lí hệ thống luyện code
> Ron Le tập tành viết API

## Build
1. Clone cái repo này về
```
git clone https://github.com/LeRon1605/ICode.git
```
2. Migrate database
```
dotnet ef database update
```
3. Sử dụng redis làm cache (chạy container trên docker hoặc download tại [đây](https://redis.io/))
```
docker run -d -p 6379:6379 redis
```
4. Build project
```
dotnet build
```
5. Chạy thôi
```
dotnet run
```

## Chạy project bằng docker-compose
1. Clone cái repo này về
```
git clone https://github.com/LeRon1605/ICode.git
```
2. Run
```
docker-compose up
```
- Database sẽ được ánh xạ vào folder `db`.
- API chạy trên host với port 8080 được ánh xạ với port 80 trên container của docker.

## Tech Stack
1. Platforms/Framework: ASP.NET Core, Entity Framework, Docker.
2. Database: SQL Server.
3. Cache Server: Redis.
4. Tools: Postman, LINQPad.

## Cơ sở dữ liệu (Bị thiếu mà lười sửa quá, mở database diagram trong MSSMS xem nha)
<p align="center">
  <img src="https://user-images.githubusercontent.com/78067510/187449316-e058ea00-c08e-43f5-838d-2c12abb6ed33.png">
</p>

## Hệ thống (Vẽ bậy thui hehe)
<p align="center">
  <img src="https://user-images.githubusercontent.com/78067510/187453115-f6323385-09cc-46b0-ab60-130d79175688.png">
</p>

