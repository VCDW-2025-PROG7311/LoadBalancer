## Load Balancer Demo with .NET 9 API and NGINX

This repo sets up a load-balanced environment using NGINX and 10 instances of a default .NET 9 Web API. The goal is to simulate basic load balancing using round-robin and test request distribution across containers.

### ✅ What This Does
- Spins up 10 .NET 9 API containers (all identical, just return their container name)
- Adds an NGINX container as the load balancer
- Uses NGINX round-robin to distribute requests
- Includes a PowerShell script to test the setup with 100 requests
- Simulates uneven load on api3, api6, and api9 with a 500ms delay

### 📦 Requirements

- Docker + Docker Compose
- PowerShell (for Windows users) or Git Bash / WSL for Linux-style shell
- .NET 9 SDK

### 🚀 Getting Started

#### 1. Set Up the Folder and Files
Create a project folder and inside it, create the following:
- `api/` folder for the .NET 9 Web API
- `nginx/` folder with a `default.conf` file for NGINX config
- `docker-compose.yml` file at the root
- `test-load.ps1` file at the root for testing

You will paste the code provided into each of these files.

#### 2. Project Structure and Code

#### 🔹 Program.cs (`api/Program.cs`)
```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot"
});

builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", async () =>
{
    var hostname = Environment.MachineName;

    if (hostname == "api3" || hostname == "api6" || hostname == "api9")
    {
        await Task.Delay(500);
    }

    return $"Hello from {hostname}!";
});

app.Run();
```
#### 🔹 Dockerfile (`api/Dockerfile`)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
```

#### 🔹 NGINX Config (`nginx/default.conf`)
```nginx
events {}

http {
    upstream backend {
        server api1:80;
        server api2:80;
        server api3:80;
        server api4:80;
        server api5:80;
        server api6:80;
        server api7:80;
        server api8:80;
        server api9:80;
        server api10:80;
    }

    server {
        listen 80;

        location / {
            proxy_pass http://backend;
        }
    }
}
```
#### 🔹 Docker Compose (`docker-compose.yml`)
```yaml
services:
  nginx:
    image: nginx:latest
    container_name: nginx
    ports:
      - "8090:80"
    volumes:
      - ./nginx/default.conf:/etc/nginx/nginx.conf:ro
    depends_on:
      - api1
      - api2
      - api3
      - api4
      - api5
      - api6
      - api7
      - api8
      - api9
      - api10

  api1:
    build: ./api
    container_name: api1
    hostname: api1
  api2:
    build: ./api
    container_name: api2
    hostname: api2
  api3:
    build: ./api
    container_name: api3
    hostname: api3
  api4:
    build: ./api
    container_name: api4
    hostname: api4
  api5:
    build: ./api
    container_name: api5
    hostname: api5
  api6:
    build: ./api
    container_name: api6
    hostname: api6
  api7:
    build: ./api
    container_name: api7
    hostname: api7
  api8:
    build: ./api
    container_name: api8
    hostname: api8
  api9:
    build: ./api
    container_name: api9
    hostname: api9
  api10:
    build: ./api
    container_name: api10
    hostname: api10
```
#### 🔹 PowerShell Test Script (`test-load.ps1`)
```powershell
1..100 | ForEach-Object {
    $res = Invoke-WebRequest -Uri http://localhost:8090 -UseBasicParsing
    Write-Output "$($_) - $($res.Content)"
}
```

#### 3. Build & Run Everything

```
docker compose up --build
```

This starts:
- `api1` to `api10` (.NET 9 APIs)
- `nginx` load balancer on `localhost:8090`

### 🧪 Test the Load Balancer

#### PowerShell (Windows)
```powershell
1..100 | ForEach-Object {
    $res = Invoke-WebRequest -Uri http://localhost:8090 -UseBasicParsing
    Write-Output "$($_) - $($res.Content)"
}
```

You’ll see responses from each API container like:
```
1 - Hello from api1!
2 - Hello from api2!
...
```

> Note: The app adds a 500ms delay to `api3`, `api6`, and `api9` to simulate slow servers. You can modify this logic in `Program.cs`.

### 🧼 Tear Down
```bash
docker compose down
```

### 📍 Optional Extensions
- Switch to `least_conn` or `ip_hash` in the upstream block
- Add NGINX rate limiting using `limit_req`
- Add health checks (`max_fails` and `fail_timeout`)
- Use HTTPS with a self-signed certificate
- Use `hey` or `wrk` for concurrent load testing

