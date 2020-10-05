#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /
COPY ["server/Netptune.App/Netptune.App.csproj", "Netptune.App/"]
COPY ["server/Netptune.Services/Netptune.Services.csproj", "Netptune.Services/"]
COPY ["server/Netptune.Core/Netptune.Core.csproj", "Netptune.Core/"]
COPY ["server/Netptune.Repositories/Netptune.Repositories.csproj", "Netptune.Repositories/"]
COPY ["server/Netptune.Entities/Netptune.Entities.csproj", "Netptune.Entities/"]
COPY ["server/Netptune.Messaging/Netptune.Messaging.csproj", "Netptune.Messaging/"]
COPY ["server/Netptune.Storage/Netptune.Storage.csproj", "Netptune.Storage/"]
RUN dotnet restore "Netptune.App/Netptune.App.csproj"
COPY /server .
WORKDIR "/Netptune.App"
RUN dotnet build "Netptune.App.csproj" -c Release -o /app/build

FROM node:14 AS client-build
WORKDIR /client
COPY /client/package*.json ./
RUN npm install
COPY /client .
RUN npm run build


FROM build AS publish
RUN dotnet publish "Netptune.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=client-build /client/dist ./wwwroot/dist
ENTRYPOINT ["dotnet", "Netptune.App.dll"]