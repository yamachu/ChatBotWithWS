FROM microsoft/aspnetcore:1.1.0

COPY ./out/ /app

WORKDIR /app

EXPOSE 80

ENTRYPOINT ["dotnet", "ChatBotWithWS.dll"]
