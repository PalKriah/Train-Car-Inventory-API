FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5001
EXPOSE 443
ENV ASPNETCORE_URLS=http://*:5001

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Train_Car_Inventory_App.csproj", "./"]
RUN dotnet restore "Train_Car_Inventory_App.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Train_Car_Inventory_App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Train_Car_Inventory_App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Train_Car_Inventory_App.dll"]
