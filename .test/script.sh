# This ensures that the functions are running before continuing
wget --retry-connrefused http://functions/api/openapi.json?code=masterKey -O functions.json

portman -u http://functions/api/openapi.json?code=masterKey -b http://functions/api -o output.json
# portman has the ability to run newman but with running newman directly it gives the option to have the HTML report
newman run output.json --timeout-request 2500 -r html --reporter-html-export ./result.html

sleep infinity