============================================

🧱 Build Angular App

============================================

FROM node:20 AS ng-builder WORKDIR /app COPY ../DeviceArchiving.UI/ ./ RUN npm install RUN npm run build --prod

============================================

🧱 Build ASP.NET Core App

============================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build WORKDIR /src

Copy and restore dependencies

COPY DeviceArchiving.Api/DeviceArchiving.Api.csproj DeviceArchiving.Api/ COPY DeviceArchiving.Service/DeviceArchiving.Service.csproj DeviceArchiving.Service/ COPY DeviceArchiving.Data/DeviceArchiving.Data.csproj DeviceArchiving.Data/ RUN dotnet restore "DeviceArchiving.Api/DeviceArchiving.Api.csproj"

Copy everything and build

COPY . . WORKDIR "/src/DeviceArchiving.Api" RUN dotnet publish "DeviceArchiving.Api.csproj" -c Release -o /app/publish

============================================

🧱 Final Image

============================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final WORKDIR /app

Copy published .NET app

COPY --from=build /app/publish .

Copy Angular build to wwwroot

COPY --from=ng-builder /app/dist/device-archiving-ui /app/wwwroot

Set environment

ENV ASPNETCORE_URLS=http://+:80 EXPOSE 80

ENTRYPOINT ["dotnet", "DeviceArchiving.Api.dll"]