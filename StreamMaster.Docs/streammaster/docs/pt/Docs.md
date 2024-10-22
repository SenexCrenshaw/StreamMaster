# 📘 Documentação de streammaster

Este documento descreve as etapas para configurar e gerenciar a documentação para o projeto [Streammaster] (https://github.com/senexcrenshaw/streammaster) usando o MKDOCS.A configuração inclui suporte à internacionalização (I18N) e o uso do tema material MKDOCS para uma aparência moderna.

## Por que contribuir para a documentação?

A documentação é a primeira coisa que os usuários e desenvolvedores se referem ao usar ou contribuir para o Streammaster.Ao ajudar a melhorar e manter a documentação, você garante que o streammaster permaneça acessível e fácil de usar para a comunidade.

Suas contribuições para a documentação:
- Ajude os outros a aprender e a usar o streammaster com mais eficiência.
- Melhore a clareza para falantes de inglês não nativos por meio de um melhor suporte da I18N.
- Permitir que os desenvolvedores contribuam com mais facilidade para o projeto.

Mesmo pequenas atualizações, como corrigir erros de digitação ou esclarecimento, fazem uma grande diferença!

# 🛠 Pré -requisitos

Para gerar e servir documentação localmente, você precisará de Python instalado.Certifique -se de que o `Pip`, gerente de pacotes da Python, também esteja disponível.

### Instalando mkdocs e plugins

Para instalar MKDOCs e os plugins necessários para i18n e temas, execute o seguinte comando:

`` `BASH
python -m pip install mkdocs mkdocs-i18n mkdocs-mataterial mkdocs-static-i18n
`` `

Isso instala o seguinte:

- `mkdocs`: o gerador de sites estáticos.
- `mkdocs-i18n`: para lidar com a internacionalização.
- `Mkdocs-Material`: um tema popular com um design moderno.
-`mkdocs-static-i18n`: adiciona suporte de internacionalização estática.

## Desenvolvimento local

Para construir e servir a documentação localmente durante o desenvolvimento, siga estas etapas.

### Construindo os documentos

Para gerar a documentação estática:

`` `BASH
MKDOCS Build
`` `

### Servindo os documentos localmente

Para executar um servidor de desenvolvimento que observa as alterações e recarregue automaticamente:

`` `BASH
Mkdocs servem
`` `

Isso hospedará a documentação localmente em `http: // localhost: 8000`.

## Construção de produção

Quando estiver pronto para implantar a documentação para a produção, verifique a compilação anterior e defina o URL do site corretamente.Execute o seguinte comando:

`` `BASH
MKDOCS Build-Clean-Site-url https://senexcrenshaw.github.io/streammaster/
`` `

Isso constrói uma versão limpa da documentação e define o URL correto para o local de produção.

## 📝 Contribuindo para a documentação

Os arquivos de documentação vivem na pasta `streammaster.docs \ streammaster` no [streammaster repositório] (https://github.com/senexcrenshaw/streammaster).

Para contribuir para a documentação:

- ** Crie um novo ramo ** para suas alterações.
- ** Verifique se o inglês (`en`) é sempre atualizado **.O inglês serve como idioma principal e todo o conteúdo deve ser atualizado em inglês.
- ** Forneça as melhores traduções possíveis ** para outros idiomas suportados, como espanhol (`es`), francês (` fr`), alemão (`de`) e quaisquer outros idiomas suportados.Embora essas traduções não tenham que ser perfeitas, elas devem refletir com precisão o conteúdo em inglês.
- Os arquivos em inglês vivem em `docs/en`.
- As traduções vivem sob seus respectivos diretórios (por exemplo, `docs/es` para espanhol,` docs/fr` para francês, etc.).
- ** Teste ** Todas as alterações executando `mkdocs servem localmente para a versão em inglês e para qualquer tradução atualizada.
- ** Envie uma solicitação de tração (PR) ** para revisão.

### Introdução em 3 etapas fáceis!

1. Fork o repositório e clone -o para sua máquina local.
2. Crie uma nova filial para suas alterações.
3. Verifique se o inglês (`en`) é atualizado e forneça as melhores traduções possíveis para outros idiomas suportados e envie um PR.

É isso!🎉 Você contribuiu para o streammaster!

## Dicas para escrever boa documentação

- ** Seja claro e conciso: ** Concentre -se nos pontos principais e evite linguagem excessivamente técnica sempre que possível.
- ** Use exemplos: ** Flugets de código ou auxílios visuais ajudam a esclarecer conceitos complexos.
- ** Seja consistente: ** Mantenha o tom e a terminologia consistentes em todas as seções.
- ** Teste tudo: ** Verifique se todos os exemplos, comandos e instruções de código funcionam conforme o esperado.

## Reconhecimento colaborador 🌟

Agradecemos todas as contribuições, não importa quão pequeno!Todos os colaboradores serão adicionados ao Hall da Fama da Documentação do Streammaster, apresentado abaixo:

[Veja todos os colaboradores] (colaboradores.md)

## precisamos da sua ajuda!🤝

O streammaster está crescendo constantemente, e precisamos da ajuda da comunidade para manter a documentação de primeira linha.Nenhuma contribuição é muito pequena, seja consertando erros de digitação, adicionando exemplos ou traduzindo conteúdo.

Entre e vamos melhorar o streammaster juntos!✨

## precisa de ajuda ou tem perguntas?Junte -se a nós na discórdia!🎮

Para quaisquer perguntas, suporte ou discussões, você pode ingressar no servidor oficial ** streammaster discord **.

👉 [Junte -se a Streammaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Estamos aqui para ajudar e você encontrará uma comunidade ativa de desenvolvedores e usuários.Sinta -se à vontade para fazer perguntas, relatar questões ou discutir novas idéias para melhorar o streammaster!