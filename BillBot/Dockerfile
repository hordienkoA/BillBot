#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /app

COPY BillBot.csproj .
RUN dotnet restore BillBot.csproj

COPY . .
RUN dotnet publish -c Release -o out

WORKDIR /app/out 

ENTRYPOINT ["dotnet", "BillBot.dll"]