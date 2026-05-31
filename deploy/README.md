# 서버 배포 가이드

## 사전 요구사항
- .NET 10 SDK
- Docker, Docker Compose
- AWS EC2 (Ubuntu)

## 초기 세팅

### 1. systemd 서비스 등록
```bash
sudo cp deploy/speakat.service /etc/systemd/system/speakat.service
sudo systemctl daemon-reload
sudo systemctl enable speakat
```

### 2. appsettings.Production.json 작성
`publish/` 경로에 `appsettings.Production.json` 수동 작성
(`deploy/appsettings.Production.example.json` 참고)

### 3. Docker 컨테이너 실행
```bash
docker compose up -d
```

### 4. DB 마이그레이션 적용
Production 환경에서는 마이그레이션이 자동 실행되지 않으므로 수동으로 적용합니다.
```bash
dotnet ef database update --project Speakat.Infrastructure --startup-project Speakat.Api
```

> `dotnet ef` 가 없으면 먼저 설치합니다.
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### 5. 빌드 및 배포
```bash
dotnet publish Speakat.Api/Speakat.Api.csproj -c Release -o publish
sudo systemctl start speakat
```

## 배포 업데이트

### 마이그레이션 변경이 없는 경우
```bash
git pull
dotnet publish Speakat.Api/Speakat.Api.csproj -c Release -o publish
sudo systemctl restart speakat
```

### 마이그레이션 변경이 있는 경우
서비스 중단 후 마이그레이션을 먼저 적용하고 재시작합니다.
```bash
git pull
sudo systemctl stop speakat
dotnet ef database update --project Speakat.Infrastructure --startup-project Speakat.Api
dotnet publish Speakat.Api/Speakat.Api.csproj -c Release -o publish
sudo systemctl start speakat
```