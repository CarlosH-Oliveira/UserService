# API de Sistema de Cadastro de Usuários

Esta API foi desenvolvida utilizando ASP.NET 8 e serve para gerenciar o cadastro de usuários,
incluindo funcionalidades como criação, login, atualização de senha e e-mail. O projeto utiliza
Entity Framework Core para gerenciamento de dados e Swagger para documentação. O banco de dados 
utilizado é MySQL.


## Tecnologias utilizadas  
1 - ASP.NET 8  
2 - Entity Framework Core - ORM  
3 - MySQL - Banco de dados  
4 - Swagger - Documentação da API 


## Funcionalidades

### Endpoints:  
#### [GET] /UserController/GetList  
##### Descrição:  
Obtém uma lista de usuários.  
##### Parâmetros:  
skip (opcional): Número de registros a pular. Default: 0.  
take (opcional): Número de registros a retornar. Default: 25.  
##### Resposta:  
Lista de usuários.  

#### [POST] /UserController/Create  
##### Descrição:  
Registra um novo usuário.  
##### Parâmetros:  
"dto" (CreateUserDTO): Dados necessários para criar uma nova conta.   
##### Resposta:  
Usuário recém registrado.  

#### [POST] /UserController/Login
##### Descrição:  
Realiza o login de um usuário.  
##### Parâmetros:  
"dto" (LoginDTO): Dados necessários para inicializar a sessão de um usuário.  
##### Resposta:  
Dados do usuário com token de acesso.  

#### [POST] /UserController/Update/Password   
##### Descrição:  
Permite a um usuário conectado alterar sua senha.  
##### Parâmetros:  
"changePasswordDTO" (ChangePasswordDTO): Dados necessários para troca de senha.  
##### Resposta:  
Mensagem de sucesso e e-mail de confirmação enviado ao usuário.    

#### [GET] /UserController/Update/Recover/Password  
##### Descrição:  
Permite que um usuário recupere uma conta da qual tenha esquecido a senha.  
##### Parâmetros:  
"recoverPasswordDTO" (RecoverPasswordDTO): Dados necessários para recuperação de senha.  
##### Resposta:  
Mensagem de sucesso e e-mail de confirmação enviado ao usuário.   

#### [GET] /UserController/Update/Password/Execute
##### Descrição:  
Recebe e executa um pedido de alteração de senha.  
##### Parâmetros:  
"tk" (string): Token de alteração de senha.    
##### Resposta:  
Mensagem de sucesso.   

#### [POST] /UserController/Update/Email  
##### Descrição:  
Permite a um usuário conectado alterar o e-mail em que sua conta está registrada.  
##### Parâmetros:  
"changeEmailDTO" (ChangeEmailDTO): Dados necessários para troca de e-mail.  
"tk" (string): Token de acesso gerado através do login.  
##### Resposta:  
Mensagem de sucesso e e-mail de confirmação enviado ao novo endereço escolhido.    

#### [GET] /UserController/Update/Email/Execute  
##### Descrição:  
Recebe e executa um pedido de alteração de e-mail.  
##### Parâmetros:  
"tk" (string): Token de alteração de e-mail.  
##### Resposta:  
Mensagem de sucesso.  


## Como executar o projeto  
### Pré-requisitos:  
1 - .NET SDK 8  
2 - MySQL  
3 - Node.js (para usar Swagger-UI)  

### Passos para Executar a API:  
1 - Clone este repositório:  
2 - Navegue até a pasta da API:  
3 - Configure a string de conexão do banco de dados no arquivo "appsettings.json"  
4 - Execute as migrações do Entity Framework para criar o banco de dados: "dotnet ef database update" (Terminal) || "Update-Database" (Console do gerenciador de pacotes Nuget)  
5 - Execute a aplicação: "dotnet run"
