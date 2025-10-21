PQT.com.br
=======================

Como rodar
------------
Por ser tratar de um projeto antigo, compatibilidade dom DotNet Framework 4.5, será necessário do  Visual Studio instalado, para rodar na IDE ou utilizar o comando abaixo, desde quê, tenha o MSBuild e IISExpress instalados e referenciados no PATH das variáveis de ambiente do Windows. Sim, só roda no Windows.

Executar o run-mvc.bat ou
No GitBash executar com o comando:>  cmd -c ./rrun-mvc.bat


Publicação
------------
Após publicação, tanto via Visual Studio ou por linha de comando, executar o deploy-views.bat para ajustar os arquivos de Views dos módulos incluído no projeto principal.


Módulos da aplicação
------------
Para facilitar a ciração de novos Módulos para a aplicação, utilizar o comando Criar-Modulo.bat, pois este pedirá o nome do módulo e já criarar o projeto com arquivos de exemplo inicial.


Introdução:
------------
Este é um projeto modelo, utilizado em nosso site. Ele possuí páginas estáticas em XML, para facilitar a manutenção de algumas páginas, sem a necessidade de alterar Controllers, Views e recompilar o projeto.
Foi construído a facilitar a modularização de partes do projeto, onde cada módulo tem seu projeto iniciando com o nome de Modulo.XXXX e um projeto Essenciais.MVC para conter dependências transversal aos projetos.



## Site:
[www.pqt.com.br](https://www.pqt.com.br)
---