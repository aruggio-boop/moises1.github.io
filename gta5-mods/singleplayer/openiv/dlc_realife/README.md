# DLC Pack — Carros da vida real (OpenIV)

Esta pasta é a **estrutura** do DLC. Os modelos `.yft/.ytd` das marcas reais **não estão inclusos**.

## Como usar

1. Ative o Mods Folder no OpenIV.
2. Copie esta pasta para:

```
GTA V/mods/update/x64/dlcpacks/dlc_realife
```

3. Em `mods/update/update.rpf/common/data/dlclist.xml` adicione:

```xml
<Item>dlcpacks:/dlc_realife/</Item>
```

4. Coloque os arquivos do seu pack de carro em:

```
dlc_realife/x64/levels/gta5/vehicles/
```

5. Atualize o `vehicles.meta` / `carvariations.meta` / `handling.meta` do pack.
6. No script, em `Config.cs`, use o **spawn name** do addon no campo `Model`.

## Sem addons

O mod já funciona só com os **fallbacks vanilla** (Comet, Entity XF, Tempesta, etc.).
