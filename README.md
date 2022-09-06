# Objetivo do Projeto
Realizar webscrapping na página da olx.com.br sobre novas publicações categorizadas em filtros previamente cadastrados, além de enviar notificações via e-mail aos interessados.

# Primeiros passos
* Na linha 03 do [Program.cs do projeto OlxFilterWatcher.Workers](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.Workers/Program.cs) e no [appsettings.json da OlxFilterWatcher.API](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.API/appsettings.json), definir a ConnectionString para conexão com MongoDB
* No arquivo [MailService](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.Services/Services/MailService.cs), realizar a integração com a plataforma de e-mail de sua preferência
* No [appsettings.json do OlxFilterWatcher.Web](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.Web/appsettings.json), inserir ConnectionString do MySQL para funcionamento do Identity (projeto independente e incompleto, não é obrigatório sua execução)

# Estrutura
Há uma API (OlxFilterWatcher.API) que fica responsável, também, por inserir na collection do MongoDB quais filtros serão trabalhados pelos robôs(OlxFilterWatcher.Workers)<br>
<p align="center">
<img src="https://i.imgur.com/V0KQNwf.png" />
</p>

Dentro do projeto dos robôs, estes se subdividem em dois: OlxScraperWorker e OlxNotifyWorker.<br>
* <b>[OlxScraperWorker](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.Workers/Workers/OlxScraperWorker.cs):</b> Realiza o GET nos filtros e fazer a extração dos dados, posteriormente subindo às collections do Mongo um registro de notificação aos e-mails previamente cadastrados naquele filtro como também registrado o post em si na collection
* <b>[OlxNotifyWorker](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.Workers/Workers/OlxNotifyWorker.cs):</b> Verifica na collection quais notificações ainda não foram enviadas a partir do esquema a seguir:

<p align="center">
<img src="https://i.imgur.com/udlHaHQ.png" />
</p>
uma vez enviado o e-mail, o documento é atualizado, definindo como true para todos os registros de e-mails em que a plataforma retorna positivo

# Débitos Técnicos
* Realizar integração com algum KeyVault para evitar exposição de connection strings e keys de plataformas de e-mail
* Alterar a função [CheckUserExists no serviço UserAuthService](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.Services/Services/UserAuthService.cs) para de fato checar em database se o usuário existe antes de retornar um Token
* Alterar o objetos de entidade que herdam do objeto pai [OlxGeneralPost](https://github.com/igor-henriques/OlxFilterWatcher/blob/master/OlxFilterWatcher.Domain/Models/OlxGeneralPost.cs) para uma composição, em que o objeto pai será composto pelas demais informações que os objetos filhos estendem, em vez de haver essa relação de herança, que adiciona complexidade ao código, fazendo-se necessário a criação de mais instâncias de MongoService para cada tipo (além de Mappers específicos, etc)

# O projeto te ajudou em algo?
No meu caso, consegui achar o apartamento dos meus sonhos e realizar a mudança que eu tanto esperava. Caso também tenha te ajudado, considere [me comprar um cafezinho <3](https://nubank.com.br/pagar/kcdug/sPKRSqKEWF)
