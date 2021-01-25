# build app
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source/Api

# copy source code and restore as distinct layers
COPY Todo.Shared/Todo.Shared.csproj ./Todo.Shared/
COPY Todo.Notifications/Todo.Notifications.csproj ./Todo.Notifications/
RUN dotnet new sln                      && \
    dotnet sln add Todo.Shared          && \
    dotnet sln add Todo.Notifications   && \
    dotnet restore -r linux-musl-x64

COPY Todo.Shared/. ./Todo.Shared/
COPY Todo.Notifications/. ./Todo.Notifications/
RUN dotnet publish                  \
        -c release                  \
        -o /app                     \
        -r linux-musl-x64           \
        --self-contained false      \
        --no-restore                \
        Todo.Notifications

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine-amd64

WORKDIR /app
COPY --from=build /app ./

ENV ConnectionStrings:MessageBroker="amqp://localhost"
    
ENTRYPOINT ["./Todo.Notifications"]

EXPOSE 80
