<h1>Gerenciamento de Pedidos - Back-End</h1>

<p>Back-end do sistema de <strong>Gerenciamento de Pedidos</strong>, desenvolvido para pequenas lojas, utilizando <strong>C# e ASP.NET Core MVC</strong>.<br>
Permite gerenciar clientes, produtos e pedidos, com acesso a dados via <strong>Dapper.NET</strong> e banco de dados <strong>SQL Server</strong>.</p>

<hr>

<h2>🛠 Tecnologias Utilizadas</h2>
<ul>
  <li><strong>C# / ASP.NET Core MVC</strong> – desenvolvimento do back-end.</li>
  <li><strong>SQL Server</strong> – banco de dados relacional.</li>
  <li><strong>Dapper.NET</strong> – ORM leve para acesso a dados.</li>
  <li><strong>Injeção de Dependência</strong> nativa do ASP.NET Core.</li>
</ul>

<hr>

<h2>🎯 Funcionalidades Principais</h2>

<h3>1. Gerenciamento de Clientes</h3>
<ul>
  <li>CRUD completo: cadastro, edição, listagem e exclusão.</li>
  <li>Campos: <strong>ID</strong>, <strong>Nome</strong>, <strong>Email</strong>, <strong>Telefone</strong>, <strong>Data de Cadastro</strong>.</li>
  <li>Endpoint para listar clientes com filtros por <strong>Nome</strong> e <strong>E-mail</strong>.</li>
</ul>

<h3>2. Gerenciamento de Produtos</h3>
<ul>
  <li>CRUD completo: cadastro, edição, listagem e exclusão.</li>
  <li>Campos: <strong>ID</strong>, <strong>Nome</strong>, <strong>Descrição</strong>, <strong>Preço</strong>, <strong>Quantidade em Estoque</strong>.</li>
  <li>Endpoint para listar produtos com filtro por <strong>Nome</strong>.</li>
  <li>Validação de campos antes de salvar.</li>
</ul>

<h3>3. Registro e Gerenciamento de Pedidos</h3>
<ul>
  <li>Criar pedidos associando <strong>Cliente</strong> e <strong>Produtos</strong>.</li>
  <li>Adicionar múltiplos produtos, informando a quantidade.</li>
  <li>Validação de <strong>estoque disponível</strong> antes de adicionar produtos.</li>
  <li>Cálculo automático do <strong>valor total</strong> do pedido.</li>
  <li>Alteração do <strong>status do pedido</strong> (ex: Novo → Processando → Finalizado).</li>
  <li>Listagem de pedidos com filtros por <strong>Cliente</strong> ou <strong>Status</strong>.</li>
  <li>Visualização detalhada de cada pedido, incluindo produtos, quantidades e preços unitários.</li>
</ul>

<h3>4. Estrutura e Boas Práticas</h3>
<ul>
  <li>Arquitetura organizada em camadas: <strong>Apresentação</strong>, <strong>Negócio/Domínio</strong>, <strong>Infraestrutura/Dados</strong>.</li>
  <li>Padrão Repository para abstração do acesso a dados via Dapper.NET.</li>
  <li>Princípios de <strong>Orientação a Objetos (SOLID)</strong> aplicados.</li>
  <li>Tratamento de exceções e validações básicas implementadas.</li>
</ul>

<hr>

<h2>🔗 Integração com Front-End</h2>
<p>O back-end é consumido pelo front-end do projeto, que realiza todas as operações de CRUD via <strong>HTTP usando Axios</strong>.<br>
Repositório front-end: <a href="https://github.com/DiogoLuiss/order-management-web">Order Management Front-End</a></p>

<hr>

<h2>🚀 Como Executar</h2>
<ol>
  <li>
    Clone o repositório:<br>
    <pre><code>git clone https://github.com/DiogoLuiss/order-management-api.git</code></pre>
  </li>
  <li>
    Configure os arquivos de configuração do projeto:
    <ul>
      <li><strong>appsettings.json</strong> – para ambiente de <strong>produção</strong>.</li>
      <li><strong>appsettings.Development.json</strong> – para ambiente de <strong>desenvolvimento/debug</strong>.</li>
    </ul>
    Ambos os arquivos devem conter a string de conexão com o banco de dados SQL Server:
    <pre><code>{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=order_management;User ID=SEU_USUARIO;Password=SUA_SENHA;Persist Security Info=True;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=SQLServerManagementStudio;Command Timeout=30"
  }
}</code></pre>
  </li>
  <li>
    Execute os scripts SQL disponíveis na pasta <strong>Scripts</strong> para criar tabelas e inserir dados iniciais.
  </li>
<li>
    Em seguida, verifique a URL em que a API está sendo executada e configure-a no front-end, no arquivo <code>src/js/services/api.js</code>.
</li>
    
</ol>
