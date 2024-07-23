ARG masterKey="<your-own-key>"
ARG mysqlHost="<sql-db-url>"
ARG mysqlUser="<sql-username>"
ARG mysqlPass="<sql-password>"
ARG mysqlDb="<sql-db-name>"
ARG tableCategories="<categories-table-name>"
ARG tableLocationLongToShortName="<LongToShortName-table-name>"
ARG tableLocationCategoryDefaults="<categoryDefaults-table-name>"
ARG tableExpenseItems="<expenseItems-table-name>"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS installer-env

COPY .. /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
mkdir -p /home/site/wwwroot && \
dotnet publish *.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

ARG MASTER_KEY
ARG MYSQL_HOST
ARG MYSQL_USER
ARG MYSQL_PASS
ARG MYSQL_DB
ARG TABLE_CATEGORIES
ARG TABLE_LOCATION_LONG_TO_SHORT
ARG TABLE_LOCATION_CATEGORY_DEFAULTS
ARG TABLE_EXPENSE_ITEMS
ENV mysql-host=$MYSQL_HOST
ENV mysql-user=$MYSQL_USER
ENV mysql-pass=$MYSQL_PASS
ENV mysql-db=$MYSQL_DB
ENV table-categories=$TABLE_CATEGORIES
ENV table-locationLongToShortName=$TABLE_LOCATION_LONG_TO_SHORT
ENV table-locationCategoryDefaults=$TABLE_LOCATION_CATEGORY_DEFAULTS
ENV table-expenseItems=$TABLE_EXPENSE_ITEMS

# https://github.com/Azure/azure-functions-host/issues/4147
ENV AzureWebJobsSecretStorageType=files
RUN mkdir -p /azure-functions-host/Secrets/

COPY functions_script.sh /home/.
# tests all of the arguments and sets the master key/password
RUN chmod 700 /home/functions_script.sh; /home/functions_script.sh

# calls the global ARG from the top of the file
# RUN echo "{'masterKey':{'name':'master','value':'$masterKey','encrypted':false},'functionKeys':[]}" > /azure-functions-host/Secrets/host.json

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]