import asyncio
import httpx

from core.godseyedata_loader import (
    load_godseye_data_from_api,
    GodsEyeLoadError
)
from core.initializer import initialize_monitoring


async def load_godseye_with_retry(app):
    retry = 0

    while True:
        try:
            print("🔄 Tentando buscar dados do GodsEye...")
            data = await load_godseye_data_from_api()

            print("✅ Dados carregados com sucesso")
            initialize_monitoring(app, data)
            return  # sucesso → sai do loop

        except (httpx.ConnectError, httpx.ReadTimeout):
            retry += 1
            wait = min(5 * retry, 30)
            print(f"⚠️ API .NET offline. Tentando novamente em {wait}s...")
            await asyncio.sleep(wait)

        except GodsEyeLoadError as e:
            print(f"❌ Erro lógico da API: {e}")
            await asyncio.sleep(10)

        except Exception as e:
            print("🔥 Erro inesperado no startup:", e)
            await asyncio.sleep(10)
