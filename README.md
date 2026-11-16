# CheefBook

Aplicativo mobile de receitas culinárias desenvolvido em **.NET MAUI (C#)** para o Projeto Integrador VI — Desenvolvimento Mobile.

**Dupla:** João Victor Vedovato e Luciano Eduardo Allebrandt

O usuário cria receitas (com foto, ingredientes, tempo, porções e dificuldade), pesquisa receitas em tempo real, favorita as que gostar e pode **capturar** a receita de outra pessoa para criar a sua própria versão, sem alterar a original.

---

## Como executar

### Pré-requisitos
- **Visual Studio 2026** (o usado no desenvolvimento), com a carga de trabalho **"Desenvolvimento de interface do usuário de aplicativo multiplataforma .NET"** (.NET MAUI).
- **SDK do .NET 10**.
- Para Android: um **celular com depuração USB ativada** ou um **emulador Android** (Android 5.0 / API 21 ou mais novo).

### Passo a passo
1. Clone o repositório:
   ```
   git clone <link do repositório>
   ```
2. Abra o arquivo **`ProjetoIntegradorMobile.slnx`** no Visual Studio.
3. Aguarde a restauração dos pacotes NuGet (é automática).
4. Na barra de ferramentas, escolha onde rodar:
   - **Android:** selecione o celular conectado ou o emulador;
   - **Windows:** selecione **Windows Machine**.
5. Pressione **F5**.

### Observação importante (Windows / Android)
O arquivo **`Directory.Build.props`** (na raiz) faz o Visual Studio gravar as pastas de compilação `bin` e `obj` em **`C:\CheefBookBuild\`**, um caminho curto. Isso evita o erro de caminho maior que 260 caracteres do Windows ao compilar para Android, já que a pasta do projeto tem um nome longo.

Se aparecerem erros estranhos de compilação depois de trocar de branch:
1. feche o Visual Studio;
2. apague as pastas `bin` e `obj` de dentro de `ProjetoIntegradorMobile` (se existirem) e a pasta `C:\CheefBookBuild`;
3. abra de novo e use **Compilar → Recompilar solução**.

### Primeiro uso
- Na primeira execução o banco SQLite é criado automaticamente, só com as **categorias** (Rápidos, Almoço, Jantar, Sobremesas). O app começa **sem contas e sem receitas**: tudo é criado pelos usuários.
- Para criar, favoritar ou capturar receitas, crie uma conta em **Entrar → Cadastre-se**.
- Para testar a captura, use **duas contas**: a conta B abre uma receita da conta A e toca em **Capturar**.

---

## Funcionalidades

| Tela | O que faz |
|---|---|
| **Início** | Saudação, busca em tempo real (por nome ou ingrediente), filtro por categoria, receita em destaque e lista de receitas. Menu inferior (Início, Favoritos, Perfil) e menu lateral. |
| **Entrar / Cadastro** | Login e criação de conta, com validação dos campos. A senha é guardada como hash (PBKDF2), nunca em texto. |
| **Nova receita / Editar receita** | Formulário com foto, nome, categoria, tempo, porções, dificuldade, vegetariana, ingredientes e modo de preparo, com validação. A mesma tela edita uma receita existente (só o autor pode editar). |
| **Detalhe da receita** | Abas Resumo, Ingredientes e Preparo. Botões Favoritar, Capturar (para quem não é o autor), Editar e Excluir (só para o autor). |
| **Excluir receita** | Tela de confirmação antes de apagar (avisa se outras pessoas capturaram a receita). |
| **Minha versão** | Edição da cópia capturada; as mudanças ficam só na cópia. Exclusão com confirmação. |
| **Favoritos / Criadas por mim / Cópias capturadas** | Listas do usuário logado. |
| **Meu perfil** | Foto do perfil (escolhida da galeria), atalhos para as listas e "Sair da conta". |

---

## Requisitos do trabalho e onde estão

| Requisito | Onde |
|---|---|
| Label, Entry/Editor (Textbox), Button | Todas as telas |
| CheckBox | "Receita vegetariana" — `NovaReceitaPage` |
| RadioButton | Dificuldade (Fácil / Média / Difícil) — `NovaReceitaPage` |
| Image (ImageView) | Fotos de receitas e do perfil |
| CollectionView (ListView) | `MainPage`, `FavoritosPage`, `ReceitasCriadasPage`, `CapturasPage`, aba Ingredientes |
| Toast | `Helpers/ToastHelper.cs` (Toast nativo do Android): receita salva, excluída, favoritada etc. |
| Grid | Todas as telas |
| Navegação **Modal** | Nova receita, Detalhe, Excluir, Minha versão (`Navigation.PushModalAsync`) |
| Navegação **Flyout** | Menu lateral (`AppShell.xaml`) |
| Navegação em **pilha** (Navigation) | Login → Cadastro (`Navigation.PushAsync` / `PopAsync`); Favoritos, Perfil, listas (`Shell.GoToAsync`) |
| Navegação **Tabbed** | `DetalheReceitaPage` (TabbedPage com 3 abas) |
| Outros componentes | Picker, TimePicker, ScrollView, Border, BindableLayout, seleção de foto da galeria (MediaPicker) |
| CRUD completo | `DAO/ReceitaDAO.cs`: `InserirAsync`, `ListarResumoAsync` / `BuscarPorIdAsync`, `AtualizarAsync`, `ExcluirAsync` |
| SQLite com 3+ tabelas e campos variados | `Data/Database.cs` — tabela `receita`: texto (`titulo`), número (`tempo_preparo_min`, `porcoes`), booleano (`vegetariana`) |
| DAO | Pasta `DAO/` |
| Formulários com validação | Cadastro, login, nova receita, minha versão |
| Filtro de pesquisa em tempo real | Barra de busca da tela inicial (`MainViewModel.Filtro`) |
| Tela de edição | `NovaReceitaPage` em modo edição, `EditarCapturaPage` |
| Confirmação de exclusão | `ExcluirReceitaPage`, cartão de confirmação em "Excluir minha versão" |
| MVVM | Pastas `Models/`, `Views/`, `ViewModels/` |

---

## Estrutura do projeto

```
ProjetoIntegradorMobile/
├── Models/          Classes das tabelas (Usuario, Receita, Categoria...) e resumos para as listas
├── Views/           Telas (XAML) e seus eventos (code-behind)
├── ViewModels/      Dados e regras de cada tela (MVVM); BaseViewModel com INotifyPropertyChanged
├── DAO/             Acesso ao banco: um DAO por assunto (Usuario, Receita, Categoria, Favorito, Adaptacao)
├── Data/            Database.cs (criação das tabelas), DatabaseConfig.cs (caminho do banco)
├── Helpers/         Sessão do usuário, Toast, hash de senha, foto da galeria, cores
├── Resources/
│   ├── Styles/      Colors.xaml (paleta), Styles.xaml (estilos), Icones.xaml (ícones em linha)
│   ├── Fonts/       Inter e Bricolage Grotesque (licença SIL OFL)
│   ├── AppIcon/     Ícone do app
│   └── Splash/      Tela de abertura
├── AppShell.xaml    Menu lateral (Flyout) e rotas de navegação
└── MauiProgram.cs   Configuração do app (fontes, ajustes dos campos de texto)
```

**Fluxo de uma ação:** a **View** (tela) recebe o toque → chama o **ViewModel** → o ViewModel valida e pede ao **DAO** → o DAO lê ou grava no **SQLite** → o ViewModel atualiza as propriedades e a tela se atualiza sozinha pelos *bindings*.

### Banco de dados (SQLite — `sqlite-net-pcl`)
- **usuario** — nome, e-mail, hash da senha, foto
- **categoria** — Rápidos, Almoço, Jantar, Sobremesas
- **ingrediente**
- **receita** — título, modo de preparo, tempo, porções, vegetariana, dificuldade, foto, autor, categoria
- **receita_ingrediente** — ingredientes de cada receita (quantidade e unidade)
- **favorito** — favoritos de cada usuário
- **receita_personalizada** e **receita_personalizada_ingrediente** — cópias capturadas ("Minha versão")
- **plano_alimentar**, **refeicao**, **sugestao** — previstas no modelo de dados para versões futuras (ainda sem telas)

---

## Tecnologias
- .NET 10 / .NET MAUI
- SQLite (`sqlite-net-pcl`)
- Arquitetura MVVM + camada DAO
