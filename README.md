# Gerenciamento de Pedidos - Front-End

Front-end do sistema de **Gerenciamento de Pedidos**, desenvolvido para pequenas lojas, com interface **limpa, responsiva e interativa**, permitindo gerenciar clientes, produtos e pedidos de forma prática.

---

## 🛠 Tecnologias Utilizadas

- **HTML5** 
- **CSS3**
- **Javascript**
- **Bootstrap 5** – responsividade e componentes prontos.  
- **jQuery** – facilita a manipulação da interface, eventos e atualizações de conteúdo na página, permitindo implementar funcionalidades como filtros e atualizações do grid de forma mais prática e eficiente.


## 🎯 Funcionalidades Principais

### 1. Gerenciamento de Clientes
- CRUD completo: cadastro, edição, listagem e exclusão de clientes.  
- Listagem com filtros dinâmicos por **Nome** e **Email**.  
- Interface amigável e responsiva para dispositivos móveis e desktop.

### 2. Gerenciamento de Produtos
- CRUD completo: cadastro, edição, listagem e exclusão de produtos.  
- Listagem com filtro dinâmico por **Nome**.  
- Validação de campos (ex: preço e quantidade) antes de salvar.  

### 3. Registro e Gerenciamento de Pedidos
- Seleção de cliente existente ao criar um pedido.  
- Adição de múltiplos produtos ao pedido de forma dinâmica.  
- Cálculo automático do **valor total** do pedido.  
- Validação de **estoque disponível** antes de adicionar produtos.  
- Visualização detalhada de cada pedido, incluindo itens e preços unitários.  
- Alteração do **status do pedido** (ex: Novo → Processando → Finalizado).  
- Filtros de pedidos por **cliente** ou **status**.  

### 4. Experiência do Usuário
- Interações dinâmicas usando **jQuery**, sem necessidade de recarregar a página.  
- Interface limpa e intuitiva com **Bootstrap**, garantindo responsividade.  
- Feedback visual em ações importantes (ex: adicionar produto, atualizar pedido).  

## 🔗 Integração com Back-End

Este front-end foi desenvolvido para se conectar com o back-end do projeto de **Gerenciamento de Pedidos**, disponível em:

[Order Management API](https://github.com/DiogoLuiss/order-management-api)

Todas as operações de CRUD para clientes, produtos e pedidos são feitas via chamadas **HTTP usando Axios**.

## 🚀 Como Executar

Clone o repositório:
    <pre><code>git clone https://github.com/DiogoLuiss/order-management-frontend.git</code></pre>
    
  <li>Configure a URL da API no arquivo <code>src/js/service/api.js</code>, pois é o único lugar onde precisa ser alterada.</li>
  <li>Abra o arquivo <strong>index.html</strong> no navegador.</li>


