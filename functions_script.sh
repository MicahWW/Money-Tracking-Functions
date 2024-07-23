if [[ "$MASTER_KEY" = "<your-own-key>" || -z "$MASTER_KEY" ]]
then
    echo "\nERROR\nYou must set a MASTER_KEY ARG."
    exit 1
fi

if [[ "$MYSQL_HOST" = "<sql-db-url>" || -z "$MYSQL_HOST" ]]
then
	echo "\nERROR\nYou must set the MYSQL_HOST ARG"
	exit 1
fi

if [[ "$MYSQL_USER" = "<sql-username>" || -z "$MYSQL_USER" ]]
then
	echo "\nERROR\nYou must set the MYSQL_USER ARG"
	exit 1
fi

if [[ "$MYSQL_PASS" = "<sql-password>" || -z "$MYSQL_PASS" ]]
then
	echo "\nERROR\nYou must set the MYSQL_PASS ARG"
	exit 1
fi

if [[ "$MYSQL_DB" = "<sql-db-name>" || -z "$MYSQL_DB" ]]
then
	echo "\nERROR\nYou must set the MYSQL_DB ARG"
	exit 1
fi

if [[ "$TABLE_CATEGORIES" = "<categories-table-name>" || -z "$TABLE_CATEGORIES" ]]
then
	echo "\nERROR\nYou must set the TABLE_CATEGORIES ARG"
	exit 1
fi

if [[ "$TABLE_LOCATION_LONG_TO_SHORT" = "<LongToShortName-table-name>" || -z "$TABLE_LOCATION_LONG_TO_SHORT" ]]
then
	echo "\nERROR\nYou must set the TABLE_LOCATION_LONG_TO_SHORT ARG"
	exit 1
fi

if [[ "$TABLE_LOCATION_CATEGORY_DEFAULTS" = "<categoryDefaults-table-name>" || -z "$TABLE_LOCATION_CATEGORY_DEFAULTS" ]]
then
	echo "\nERROR\nYou must set the TABLE_LOCATION_CATEGORY_DEFAULTS ARG"
	exit 1
fi

if [[ "$TABLE_EXPENSE_ITEMS" = "<expenseItems-table-name>" || -z "$TABLE_EXPENSE_ITEMS" ]]
then
	echo "\nERROR\nYou must set the TABLE_EXPENSE_ITEMS ARG"
	exit 1
fi

echo "{'masterKey':{'name':'master','value':'$MASTER_KEY','encrypted':false},'functionKeys':[]}" > /azure-functions-host/Secrets/host.json