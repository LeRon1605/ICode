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
- Ánh xạ port 8080 của API trên host với port 80 trên container của docker.
