[↩️](./../../README.md)

# Local docker-compose runbook - via Visual Studio
## Requirements
- WSL
- Docker, docker-compose
- Visual Studio 2022 or higher

## Before start docker-compose
### Developer SSL certificate generation - Windows
1. Go to repository root directory
2. Move to /src
3. Open terminal
4. Generate SSL developer certificate
```
dotnet dev-certs https -ep ./https-dev-cert.pfx -p yourPassword
```
5. Transform it to key.pem/cert.pem
6. key.pem and cert.pem copy to ./frontend/FlowJudge-webapp

### Transform .pfx to key.pem/cert.pem
1. Open linux terminal where certificate is located (default is [root]/src)
2. Use openssl to transform certificate:
```
openssl pkcs12 -in https-dev-cert.pfx -nocerts -out key-encrypted.key
openssl rsa -in key-encrypted.key -out key-decrypted.key
openssl pkcs12 -in https-dev-cert.pfx -clcerts -nokeys -out cert.pem
```
3. Rename key-decrypted.key to key.pem
4. Remove metadata from cert.pem, file should begin with `-----BEGIN CERTIFICATE-----`

### Configure local hosts
1. Open hosts file (C:\Windows\System32\drivers\etc) as administrator in any text editor.
2. Add 2 rows:
```
127.0.0.1	flowjudge
127.0.0.1   flowjudge.auth
```

## Run application
1. Open Visual Studio
2. Rebuild solution
3. Be sure that docker-compose is startup project
4. Click 'debug' or 'start without debugging'
5. Type in 'Realm name' textfield: flowjudge


## First time setup
### Import keycloak realm
1. Go to keycloak admin panel (default is http://localhost:8080)
2. Login (default is admin/admin)
3. Choose 'Manage realms' and click 'Create realm' button
4. Browse file with realm (\src\scripts\keycloak-init\flowjudge-realm-snapshot.json)
### Regenerate keyloak client secret
1. Go to keycloak admin panel (default is http://localhost:8080)
2. Login (default is admin/admin)
3. Choose 'Manage realms' and click in flowjudge -> flowjudge should be your current realm
4. Click 'Clients' in Manage section, then click in 'webapp'.
5. Go to 'Credentials' and click 'Regenerate' in Client Secret section.
6. Copy new client secret and paste it into AUTH_CLIENT_SECRET variable in .env file 