# Instalação — GTA 5 Single Player

## 1. Preparar o jogo

1. Abra a pasta do GTA V.
2. Instale **ScriptHookV** (copie `dinput8.dll` + `ScriptHookV.dll`).
3. Instale **ScriptHookVDotNet** v3 (copie as DLLs para a pasta do GTA V).
4. Crie a pasta `scripts` dentro da pasta do GTA V, se não existir.

## 2. Instalar o mod de script

### Opção A — Compilar (recomendado)

1. Abra `singleplayer/RealLifeAccessMod/RealLifeAccessMod.csproj` no Visual Studio 2022.
2. Ajuste as referências para as DLLs do ScriptHookVDotNet na pasta do GTA V:
   - `ScriptHookVDotNet3.dll`
3. Compile em **Release**.
4. Copie `RealLifeAccessMod.dll` para:

```
GTA V/scripts/RealLifeAccessMod.dll
```

### Opção B — Usar o código-fonte com SHVDN

Se sua instalação do SHVDN carregar scripts `.cs`:

1. Copie os arquivos `.cs` de `singleplayer/RealLifeAccessMod/` para `GTA V/scripts/`.
2. Mantenha `Config.cs`, `Main.cs` e os demais juntos.

## 3. (Opcional) Addons de carros no OpenIV

1. Ative o **mods folder** no OpenIV.
2. Copie `singleplayer/openiv/dlc_realife` para:

```
GTA V/mods/update/x64/dlcpacks/dlc_realife
```

3. Edite `mods/update/update.rpf/common/data/dlclist.xml` e adicione:

```xml
<Item>dlcpacks:/dlc_realife/</Item>
```

4. Coloque os arquivos `.yft/.ytd` dos carros em:

```
dlc_realife/x64/levels/gta5/vehicles/
```

5. Atualize `Config.cs` com o spawn name real do addon.

## 4. Menyoo (atalhos)

1. Instale Menyoo.
2. Importe os arquivos de `singleplayer/menyoo/`.
3. Use os teleports da secret room e dos edifícios.

## 5. Testar

1. Abra o **Modo História**.
2. Pressione `F8` e suba o acesso para nível 4.
3. Pressione `F7` e spawne um carro.
4. Pressione `F9` e vá até a Secret Room.
5. Pressione `F6` para iniciar a missão.

## Problemas comuns

| Problema | Solução |
|---|---|
| Jogo não abre | Atualize ScriptHookV para a versão do patch atual |
| Menu não aparece | Confirme a DLL em `scripts/` e o SHVDN instalado |
| Carro addon não spawna | Use o fallback ou confira o spawn name |
| Crash ao entrar em interior | Salve antes; use teleport do menu F9 |
